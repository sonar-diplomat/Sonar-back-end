using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

public sealed class CollapseSchemaByNameTransformer : IOpenApiSchemaTransformer
{
    private readonly ILogger<CollapseSchemaByNameTransformer> _log;
    private readonly HashSet<string> _targets;

    // передай сюда имена типов-сущностей, которые подозреваешь
    public CollapseSchemaByNameTransformer(ILogger<CollapseSchemaByNameTransformer> log)
    {
        _log = log;
        _targets = new(StringComparer.Ordinal)
        {
            "Entities.Models.Access.AccessFeature",
            // добавляй при необходимости
            // "Entities.Models.UserCore.User"
        };
    }

    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext ctx, CancellationToken ct)
    {
        var t = ctx.JsonTypeInfo?.Type;
        if (t is null) return Task.CompletedTask;

        if (_targets.Contains(t.FullName!))
        {
            _log.LogError("🔥 Collapsing schema for suspected type: {Type}. Cutting properties to break cycles.", t.FullName);
            schema.Type = "object";
            schema.Properties?.Clear();
            schema.AllOf?.Clear();
            schema.AnyOf?.Clear();
            schema.OneOf?.Clear();
            schema.AdditionalProperties = null;
        }

        return Task.CompletedTask;
    }
}