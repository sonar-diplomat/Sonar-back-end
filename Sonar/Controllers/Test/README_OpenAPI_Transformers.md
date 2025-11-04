# OpenAPI Schema Transformers - Circular Reference Solution

## Problem

When generating OpenAPI documentation for Entity Framework models with navigation properties, the schema generator encounters circular references. This causes the error:

```
System.InvalidOperationException: The depth of the generated JSON schema exceeds the JsonSerializerOptions.MaxDepth setting.
```

## Solution Architecture

We've implemented a multi-layered defense strategy with several transformers that work together:

### 1. **NavigationPropertyIgnoreTransformer** (Primary Defense)
**Order: FIRST**

This transformer proactively identifies and removes navigation properties from entity model schemas BEFORE they can cause circular references.

**How it works:**
- Detects types in entity namespaces (UserCore, Music, Chat, etc.)
- Uses reflection to identify virtual properties (EF navigation properties)
- Removes both collection and reference navigation properties from the OpenAPI schema
- Tracks depth to prevent infinite recursion (max depth: 10)

**Benefits:**
- Prevents the circular reference problem at the source
- Clean OpenAPI documentation without confusing navigation property references
- Works with `[JsonIgnore]` attributes for runtime JSON serialization

### 2. **CollapseSchemaByNameTransformer** (Secondary Defense)
**Order: THIRD**

This transformer tracks how many times a type appears in the schema generation path and collapses schemas that appear too frequently.

**How it works:**
- Maintains a depth counter for each type in the current async context
- When a type is encountered more than 3 times in the same path, it collapses the schema
- Collapsed schemas become simple objects with a description noting the collapse

**Benefits:**
- Catches any circular references that slip through the first line of defense
- Provides clear documentation when schemas are collapsed

### 3. **SchemaBreadcrumbsTransformer** (Monitoring)
**Order: SECOND**

This transformer monitors the schema generation process and logs diagnostic information.

**How it works:**
- Maintains a stack of types currently being processed
- Detects when the same type appears multiple times in the stack (cycle detection)
- Logs detailed information about deep schemas (depth > 5)

**Benefits:**
- Helps identify problematic types during development
- Provides debugging information when circular references occur
- Reduced noise by only logging warnings for deep schemas

### 4. **RefGraphCycleDetectorDocumentTransformer** (Post-Processing)
**Order: LAST (Document-level)**

This transformer analyzes the final OpenAPI document for circular `$ref` references.

**How it works:**
- Builds a graph of all schema references
- Performs depth-first search to detect cycles
- Logs any cycles found in the final document

**Benefits:**
- Final validation that no circular references made it through
- Useful for catching issues with manually defined schemas

## Configuration

### Program.cs Changes

```csharp
// Increased MaxDepth to allow schema generation to complete
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.MaxDepth = 64;
        options.JsonSerializerOptions.DefaultIgnoreCondition = 
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Transformer registration order is critical
builder.Services.AddOpenApi(o =>
{
    o.AddOperationTransformer<OperationTraceTransformer>();
    o.AddSchemaTransformer<NavigationPropertyIgnoreTransformer>();        // FIRST - Remove nav properties
    o.AddSchemaTransformer<SchemaBreadcrumbsTransformer>();               // SECOND - Monitor depth
    o.AddSchemaTransformer<CollapseSchemaByNameTransformer>();            // THIRD - Collapse repeated types
    o.AddDocumentTransformer<RefGraphCycleDetectorDocumentTransformer>(); // LAST - Validate final document
});
```

### Model Changes

All navigation properties have been annotated with `[JsonIgnore]`:

```csharp
[JsonIgnore]
[ForeignKey("UserId")]
[DeleteBehavior(DeleteBehavior.Cascade)]
public virtual User User { get; set; }

[JsonIgnore]
public virtual ICollection<Track> Tracks { get; set; }
```

This ensures runtime JSON serialization respects the exclusion of navigation properties.

## How They Work Together

1. **NavigationPropertyIgnoreTransformer** runs first and removes most navigation properties
2. **SchemaBreadcrumbsTransformer** monitors the generation process and logs issues
3. **CollapseSchemaByNameTransformer** catches any types that still create deep nesting
4. **RefGraphCycleDetectorDocumentTransformer** validates the final OpenAPI document

## Testing

To verify the solution is working:

1. Start the application in Development mode
2. Navigate to the OpenAPI endpoint
3. Check the logs for:
   - `✂️ Removed X navigation properties from Type` (successful removal)
   - `🔥 Collapsing schema for type` (secondary defense triggered - rare)
   - `🔁 Potential cycle detected` (should not appear if working correctly)
   - `🔄 OpenAPI $ref cycle` (should not appear if working correctly)

## Troubleshooting

### If you still see MaxDepth errors:

1. Check that `NavigationPropertyIgnoreTransformer` is registered FIRST
2. Verify all entity namespaces are in the `_entityNamespaces` set
3. Increase `MaxCollapseDepth` in `CollapseSchemaByNameTransformer` (currently 3)
4. Increase `MaxDepth` in `JsonSerializerOptions` (currently 64)

### If OpenAPI documentation is missing properties:

1. Check logs for which properties were removed
2. Verify the properties are truly navigation properties (virtual)
3. Adjust the logic in `NavigationPropertyIgnoreTransformer` if needed

## Performance Considerations

- The transformers use `AsyncLocal` for thread-safe state management
- Reflection is used minimally and cached where possible
- Logging is set to Debug level for normal schemas, Warning for deep ones
- The transformers add minimal overhead to OpenAPI generation

## Maintenance

When adding new entity namespaces, update the `_entityNamespaces` HashSet in `NavigationPropertyIgnoreTransformer`:

```csharp
_entityNamespaces = new HashSet<string>(StringComparer.Ordinal)
{
    "Entities.Models.YourNewNamespace"
};
```

## Credits

This solution combines multiple defensive layers to handle Entity Framework's circular navigation properties in OpenAPI schema generation while maintaining clean, accurate API documentation.

