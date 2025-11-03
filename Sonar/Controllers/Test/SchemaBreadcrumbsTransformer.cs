using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

public sealed class SchemaBreadcrumbsTransformer : IOpenApiSchemaTransformer
{
    private static readonly AsyncLocal<Stack<Type>> _stack = new();
    private readonly ILogger<SchemaBreadcrumbsTransformer> _log;

    public SchemaBreadcrumbsTransformer(ILogger<SchemaBreadcrumbsTransformer> log) => _log = log;

    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext ctx, CancellationToken ct)
    {
        var type = ctx.JsonTypeInfo?.Type;
        if (type is null)
            return Task.CompletedTask;

        _stack.Value ??= new Stack<Type>();
        var stack = _stack.Value;

        // обнаружение повтора типа в текущем стеке
        if (stack.Contains(type))
        {
            var breadcrumb = string.Join(" -> ", stack.Reverse().Select(t => t.Name).Concat(new[] { type.Name }));
            _log.LogError("🔁 Potential cycle detected while building schema: {Breadcrumb}", breadcrumb);
        }

        stack.Push(type);
        try
        {
            // полезный диагностический лог
            _log.LogInformation("🧩 Schema for {Type} (Depth={Depth}) — props: {Props}",
                type.FullName, stack.Count, schema.Properties?.Count ?? 0);
        }
        finally
        {
            // ВАЖНО: выталкиваем тип, чтобы стек был корректен
            stack.Pop();
        }

        return Task.CompletedTask;
    }
}
