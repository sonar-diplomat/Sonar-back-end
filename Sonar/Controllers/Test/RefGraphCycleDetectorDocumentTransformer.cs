using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

public sealed class RefGraphCycleDetectorDocumentTransformer : IOpenApiDocumentTransformer
{
    private readonly ILogger<RefGraphCycleDetectorDocumentTransformer> _log;
    public RefGraphCycleDetectorDocumentTransformer(ILogger<RefGraphCycleDetectorDocumentTransformer> log) => _log = log;

    public Task TransformAsync(OpenApiDocument doc, OpenApiDocumentTransformerContext ctx, CancellationToken ct)
    {
        if (doc.Components?.Schemas is null || doc.Components.Schemas.Count == 0)
            return Task.CompletedTask;

        var schemas = doc.Components.Schemas;
        var graph = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

        // собрать граф: schemaName -> { referencedSchemaNames }
        foreach (var (name, schema) in schemas)
        {
            var refs = new HashSet<string>(StringComparer.Ordinal);
            CollectRefs(schema, refs);
            graph[name] = refs;
        }

        var visited = new HashSet<string>(StringComparer.Ordinal);
        var onStack = new HashSet<string>(StringComparer.Ordinal);
        var path = new Stack<string>();

        void Dfs(string v)
        {
            visited.Add(v);
            onStack.Add(v);
            path.Push(v);

            if (graph.TryGetValue(v, out var edges))
            {
                foreach (var w in edges)
                {
                    if (!schemas.ContainsKey(w)) continue;

                    if (!visited.Contains(w))
                    {
                        Dfs(w);
                    }
                    else if (onStack.Contains(w))
                    {
                        // найден цикл — печатаем путь
                        var cycle = path.Reverse().TakeWhile(x => x != w).Concat(new[] { w }).ToArray();
                        _log.LogError("🔄 OpenAPI $ref cycle: {Cycle}", string.Join(" -> ", cycle));
                    }
                }
            }

            path.Pop();
            onStack.Remove(v);
        }

        foreach (var s in schemas.Keys)
            if (!visited.Contains(s))
                Dfs(s);

        return Task.CompletedTask;

        // рекурсивный сбор всех $ref из схемы
        static void CollectRefs(OpenApiSchema s, HashSet<string> acc)
        {
            if (s.Reference?.Id is string id)
                acc.Add(id);

            if (s.Properties != null)
                foreach (var p in s.Properties.Values)
                    CollectRefs(p, acc);

            if (s.Items != null)
                CollectRefs(s.Items, acc);

            if (s.AllOf != null)
                foreach (var x in s.AllOf) CollectRefs(x, acc);

            if (s.OneOf != null)
                foreach (var x in s.OneOf) CollectRefs(x, acc);

            if (s.AnyOf != null)
                foreach (var x in s.AnyOf) CollectRefs(x, acc);

            if (s.AdditionalProperties is OpenApiSchema addl)
                CollectRefs(addl, acc);
        }
    }
}
