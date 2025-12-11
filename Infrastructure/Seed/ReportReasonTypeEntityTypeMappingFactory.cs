using Entities.Models.Report;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seed;

/// <summary>
/// Factory for configuring many-to-many relationships between ReportReasonType and ReportableEntityType
/// This should be called after seed data is created
/// </summary>
public static class ReportReasonTypeEntityTypeMappingFactory
{
    /// <summary>
    /// Configures the relationships between report reason types and entity types
    /// Entity Types: User (1), Track (2), Album (3), Comment (4), Playlist (5), Artist (6)
    /// </summary>
    public static void ConfigureMappings(ModelBuilder builder)
    {
        // Configure the join table with seed data
        var joinTableData = new List<object>();

        // Old reasons (1-4) - applicable to multiple types
        // 1: Spam - all types
        AddMappings(joinTableData, 1, [1, 2, 3, 4, 5, 6]);
        
        // 2: Harassment - User only
        AddMappings(joinTableData, 2, [1]);
        
        // 3: Copyright Violation - Content types (Track, Album)
        AddMappings(joinTableData, 3, [2, 3]);
        
        // 4: Inappropriate Content - all types
        AddMappings(joinTableData, 4, [1, 2, 3, 4, 5, 6]);

        // Track-specific reasons (5-8)
        AddMappings(joinTableData, 5, [2]); // Copyright infringement
        AddMappings(joinTableData, 6, [2]); // Explicit content without marking
        AddMappings(joinTableData, 7, [2]); // Spam audio / noise
        AddMappings(joinTableData, 8, [2]); // Incorrect tags/metadata

        // Album-specific reasons (9)
        AddMappings(joinTableData, 9, [3]); // Copyrighted cover without permission

        // Playlist-specific reasons (10)
        AddMappings(joinTableData, 10, [5]); // Inappropriate playlist name

        // User-specific reasons (11-16)
        AddMappings(joinTableData, 11, [1]); // Abusive behavior
        AddMappings(joinTableData, 12, [1]); // Fake identity
        AddMappings(joinTableData, 13, [1]); // Bot account
        AddMappings(joinTableData, 14, [1]); // Inappropriate bio
        AddMappings(joinTableData, 15, [1]); // Scam attempts / phishing
        AddMappings(joinTableData, 16, [1]); // Spam messaging

        // Configure the join table
        builder.Entity("ReportReasonTypeReportableEntityType")
            .HasData(joinTableData);
    }

    private static void AddMappings(List<object> joinTableData, int reasonTypeId, int[] entityTypeIds)
    {
        foreach (var entityTypeId in entityTypeIds)
        {
            joinTableData.Add(new
            {
                ReportReasonTypeId = reasonTypeId,
                ApplicableEntityTypesId = entityTypeId
            });
        }
    }
}

