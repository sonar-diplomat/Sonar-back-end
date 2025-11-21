using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

public sealed class OperationTraceTransformer : IOpenApiOperationTransformer
{
    private readonly ILogger<OperationTraceTransformer> _log;
    public OperationTraceTransformer(ILogger<OperationTraceTransformer> log) => _log = log;

    public Task TransformAsync(OpenApiOperation op, OpenApiOperationTransformerContext ctx, CancellationToken ct)
    {
        var route = ctx.Description.RelativePath;
        var controller = ctx.Description.ActionDescriptor.RouteValues.TryGetValue("controller", out var c) ? c : "?";
        var action = ctx.Description.ActionDescriptor.RouteValues.TryGetValue("action", out var a) ? a : "?";

        _log.LogInformation("🛠 Building operation {Controller}.{Action} [{Route}]",
            controller, action, route);

        return Task.CompletedTask;
    }
}
