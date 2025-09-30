using Entities.Models.Report;

namespace Infrastructure.Seed;
public static class ReportableEntityTypeSeedFactory
{
    public static ReportableEntityType[] CreateSeedData() => new[]
    {
        new ReportableEntityType
        {
            Id = 1,
            Name = "User",
            Reports = new List<Report>()
        },
        new ReportableEntityType
        {
            Id = 2,
            Name = "Track",
            Reports = new List<Report>()
        },
        new ReportableEntityType
        {
            Id = 3,
            Name = "Album",
            Reports = new List<Report>()
        },
        new ReportableEntityType
        {
            Id = 4,
            Name = "Comment",
            Reports = new List<Report>()
        }
    };
}