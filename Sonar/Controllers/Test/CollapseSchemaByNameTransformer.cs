using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

public sealed class CollapseSchemaByNameTransformer : IOpenApiSchemaTransformer
{
    private readonly ILogger<CollapseSchemaByNameTransformer> _log;
    private readonly HashSet<string> _collapsedTypes = new();
    private static readonly int MaxCollapseDepth = 3;
    private static readonly AsyncLocal<Dictionary<string, int>> _typeDepthCounter = new();

    public CollapseSchemaByNameTransformer(ILogger<CollapseSchemaByNameTransformer> log)
    {
        _log = log;
    }

    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext ctx, CancellationToken ct)
    {
        var t = ctx.JsonTypeInfo?.Type;
        if (t is null || t.FullName is null) return Task.CompletedTask;

        // Initialize the depth counter for this async context
        _typeDepthCounter.Value ??= new Dictionary<string, int>(StringComparer.Ordinal);
        var depthCounter = _typeDepthCounter.Value;

        // Track how many times we've seen this type in the current processing path
        if (!depthCounter.TryGetValue(t.FullName, out var depth))
        {
            depth = 0;
        }

        depthCounter[t.FullName] = depth + 1;

        try
        {
            // If we've seen this type more than MaxCollapseDepth times, collapse it
            if (depth >= MaxCollapseDepth)
            {
                if (!_collapsedTypes.Contains(t.FullName))
                {
                    _collapsedTypes.Add(t.FullName);
                    _log.LogWarning("🔥 Collapsing schema for type {Type} at depth {Depth} to prevent cycles.", 
                        t.FullName, depth);
                }

                // Collapse the schema to a simple object reference
                schema.Type = "object";
                schema.Properties?.Clear();
                schema.AllOf?.Clear();
                schema.AnyOf?.Clear();
                schema.OneOf?.Clear();
                schema.AdditionalProperties = null;
                
                // Add a description indicating this was collapsed
                schema.Description = $"Schema collapsed to prevent circular reference. Original type: {t.Name}";
            }

            return Task.CompletedTask;
        }
        finally
        {
            // Decrement the counter when leaving this type
            if (depthCounter.ContainsKey(t.FullName))
            {
                depthCounter[t.FullName]--;
                if (depthCounter[t.FullName] <= 0)
                {
                    depthCounter.Remove(t.FullName);
                }
            }
        }
    }
}