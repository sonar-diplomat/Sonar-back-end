using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using System.Reflection;

/// <summary>
/// Removes navigation properties from entity model schemas to prevent circular references in OpenAPI documentation.
/// This transformer identifies virtual properties (navigation properties) and removes them from the schema.
/// </summary>
public sealed class NavigationPropertyIgnoreTransformer : IOpenApiSchemaTransformer
{
    private readonly ILogger<NavigationPropertyIgnoreTransformer> _log;
    private readonly HashSet<string> _entityNamespaces;
    private static readonly AsyncLocal<int> _depth = new();

    public NavigationPropertyIgnoreTransformer(ILogger<NavigationPropertyIgnoreTransformer> log)
    {
        _log = log;
        _entityNamespaces = new HashSet<string>(StringComparer.Ordinal)
        {
            "Entities.Models.UserCore",
            "Entities.Models.UserExperience",
            "Entities.Models.Music",
            "Entities.Models.Chat",
            "Entities.Models.ClientSettings",
            "Entities.Models.Access",
            "Entities.Models.File",
            "Entities.Models.Distribution",
            "Entities.Models.Library",
            "Entities.Models.Report"
        };
    }

    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext ctx, CancellationToken ct)
    {
        try
        {
            var type = ctx.JsonTypeInfo?.Type;
            if (type is null || schema.Properties is null || schema.Properties.Count == 0)
                return Task.CompletedTask;

            // Track depth to prevent infinite recursion
            _depth.Value++;
            try
            {
                // If depth exceeds threshold, collapse the schema entirely
                if (_depth.Value > 10)
                {
                    _log.LogWarning("⚠️ Schema depth exceeded for {Type} at depth {Depth}. Collapsing schema.", 
                        type.FullName ?? "Unknown", _depth.Value);
                    schema.Properties?.Clear();
                    schema.Type = "object";
                    return Task.CompletedTask;
                }

                // Check if this is an entity model
                if (type.Namespace != null && _entityNamespaces.Contains(type.Namespace))
                {
                    var propertiesToRemove = new List<string>();

                    // Get all properties from the type
                    var typeProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var (propName, propSchema) in schema.Properties.ToList())
                    {
                        try
                        {
                            // Find the corresponding PropertyInfo
                            var propInfo = typeProperties.FirstOrDefault(p => 
                                string.Equals(p.Name, propName, StringComparison.OrdinalIgnoreCase));

                            if (propInfo != null)
                            {
                                // Check if it's a virtual property (navigation property)
                                var getMethod = propInfo.GetGetMethod();
                                if (getMethod != null && getMethod.IsVirtual && !getMethod.IsFinal)
                                {
                                    // Check if it's a collection or reference navigation property
                                    var propType = propInfo.PropertyType;
                                    var isCollection = propType.IsGenericType && 
                                        (propType.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                                         propType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                                         propType.GetInterfaces().Any(i => i.IsGenericType && 
                                            i.GetGenericTypeDefinition() == typeof(ICollection<>)));

                                    var isEntityReference = propType.Namespace != null && 
                                        _entityNamespaces.Contains(propType.Namespace);

                                    if (isCollection || isEntityReference)
                                    {
                                        propertiesToRemove.Add(propName);
                                        _log.LogDebug("🚫 Removing navigation property {Type}.{Property} from OpenAPI schema",
                                            type.Name, propName);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.LogWarning(ex, "Error processing property {Property} of type {Type}", propName, type.Name);
                        }
                    }

                    // Remove identified navigation properties
                    foreach (var prop in propertiesToRemove)
                    {
                        schema.Properties.Remove(prop);
                    }

                    if (propertiesToRemove.Count > 0)
                    {
                        _log.LogInformation("✂️ Removed {Count} navigation properties from {Type}",
                            propertiesToRemove.Count, type.Name);
                    }
                }

                return Task.CompletedTask;
            }
            finally
            {
                _depth.Value--;
            }
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Critical error in NavigationPropertyIgnoreTransformer for type {Type}", 
                ctx.JsonTypeInfo?.Type?.FullName ?? "Unknown");
            // Don't rethrow - allow OpenAPI generation to continue
            return Task.CompletedTask;
        }
    }
}

