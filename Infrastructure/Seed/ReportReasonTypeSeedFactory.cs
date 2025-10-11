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
            }
        };
    }
}
