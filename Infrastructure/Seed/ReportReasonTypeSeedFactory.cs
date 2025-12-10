using Entities.Models.Report;

namespace Infrastructure.Seed;

public static class ReportReasonTypeSeedFactory
{
    public static ReportReasonType[] CreateSeedData()
    {
        return new[]
        {
            new ReportReasonType
            {
                Id = 1,
                Name = "Spam",
                RecommendedSuspensionDuration = TimeSpan.FromDays(1),
                Reports = new List<Report>()
            },
            new ReportReasonType
            {
                Id = 2,
                Name = "Harassment",
                RecommendedSuspensionDuration = TimeSpan.FromDays(7),
                Reports = new List<Report>()
            },
            new ReportReasonType
            {
                Id = 3,
                Name = "Copyright Violation",
                RecommendedSuspensionDuration = TimeSpan.FromDays(30),
                Reports = new List<Report>()
            },
            new ReportReasonType
            {
                Id = 4,
                Name = "Inappropriate Content",
                RecommendedSuspensionDuration = TimeSpan.FromDays(14),
                Reports = new List<Report>()
            },
            // Track-specific reasons
            new ReportReasonType
            {
                Id = 5,
                Name = "Copyright infringement",
                RecommendedSuspensionDuration = TimeSpan.FromDays(30),
                Reports = new List<Report>()
            },
            new ReportReasonType
            {
                Id = 6,
                Name = "Explicit content without marking",
                RecommendedSuspensionDuration = TimeSpan.FromDays(7),
                Reports = new List<Report>()
            },
            new ReportReasonType
            {
                Id = 7,
                Name = "Spam audio / noise",
                RecommendedSuspensionDuration = TimeSpan.FromDays(3),
                Reports = new List<Report>()
            },
            new ReportReasonType
            {
                Id = 8,
                Name = "Incorrect tags/metadata",
                RecommendedSuspensionDuration = TimeSpan.FromDays(1),
                Reports = new List<Report>()
            },
            // Album-specific reasons
            new ReportReasonType
            {
                Id = 9,
                Name = "Copyrighted cover without permission",
                RecommendedSuspensionDuration = TimeSpan.FromDays(30),
                Reports = new List<Report>()
            },
            // Playlist-specific reasons
            new ReportReasonType
            {
                Id = 10,
                Name = "Inappropriate playlist name",
                RecommendedSuspensionDuration = TimeSpan.FromDays(7),
                Reports = new List<Report>()
            },
            // User-specific reasons
            new ReportReasonType
            {
                Id = 11,
                Name = "Abusive behavior",
                RecommendedSuspensionDuration = TimeSpan.FromDays(14),
                Reports = new List<Report>()
            },
            new ReportReasonType
            {
                Id = 12,
                Name = "Fake identity",
                RecommendedSuspensionDuration = TimeSpan.FromDays(30),
                Reports = new List<Report>()
            },
            new ReportReasonType
            {
                Id = 13,
                Name = "Bot account",
                RecommendedSuspensionDuration = TimeSpan.FromDays(90),
                Reports = new List<Report>()
            },
            new ReportReasonType
            {
                Id = 14,
                Name = "Inappropriate bio",
                RecommendedSuspensionDuration = TimeSpan.FromDays(7),
                Reports = new List<Report>()
            },
            new ReportReasonType
            {
                Id = 15,
                Name = "Scam attempts / phishing",
                RecommendedSuspensionDuration = TimeSpan.FromDays(90),
                Reports = new List<Report>()
            },
            new ReportReasonType
            {
                Id = 16,
                Name = "Spam messaging",
                RecommendedSuspensionDuration = TimeSpan.FromDays(7),
                Reports = new List<Report>()
            }
        };
    }
}
