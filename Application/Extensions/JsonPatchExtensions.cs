using System.Reflection;
using System.Text.Json;
using Entities.Models.ClientSettings;

namespace Application.Extensions;

public static class JsonPatchExtensions
{
    public static void ApplyJsonPatch(this Settings settings, JsonElement updates)
    {
        Dictionary<string, JsonElement>? dict = updates.Deserialize<Dictionary<string, JsonElement>>();
        if (dict == null)
            return;
        foreach (KeyValuePair<string, JsonElement> kv in dict)
        {
            PropertyInfo? prop = typeof(Settings).GetProperty(kv.Key,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
                continue;
            if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
            {
                object? val = JsonSerializer.Deserialize(kv.Value.GetRawText(), prop.PropertyType);
                prop.SetValue(settings, val);
                continue;
            }

            if (kv.Value.ValueKind != JsonValueKind.Object)
                continue;

            if (kv.Value.TryGetProperty("id", out JsonElement idJson))
            {
                string fkName = prop.Name + "Id";
                PropertyInfo? fkProp = typeof(Settings).GetProperty(fkName);
                if (fkProp != null)
                {
                    fkProp.SetValue(settings, idJson.GetInt32());
                    continue;
                }
            }

            if (prop.GetValue(settings) is { } nestedObject)
                ApplyNestedPatch(nestedObject, kv.Value);
        }
    }

    private static void ApplyNestedPatch(object target, JsonElement updates)
    {
        Dictionary<string, JsonElement>? dict = updates.Deserialize<Dictionary<string, JsonElement>>();
        if (dict == null)
            return;
        foreach (KeyValuePair<string, JsonElement> kv in dict)
        {
            if (string.Equals(kv.Key, "id", StringComparison.OrdinalIgnoreCase))
                continue;
            PropertyInfo? prop = target.GetType().GetProperty(kv.Key,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
                continue;
            if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
            {
                object? val = JsonSerializer.Deserialize(kv.Value.GetRawText(), prop.PropertyType);
                prop.SetValue(target, val);
            }
            else if (kv.Value.ValueKind == JsonValueKind.Object)
            {
                if (prop.GetValue(target) is { } nested)
                    ApplyNestedPatch(nested, kv.Value);
            }
        }
    }
}