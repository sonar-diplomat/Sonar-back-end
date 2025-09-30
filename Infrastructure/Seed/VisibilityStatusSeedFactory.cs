using Entities.Models.Access;

namespace Infrastructure.Seed;
public static class VisibilityStatusSeedFactory
{
    public static VisibilityStatus[] CreateSeedData() => new[]
    {
        new VisibilityStatus
        {
            Id = 1,
            Name = "Visible",
            VisibilityStates = new List<VisibilityState>()
        },
        new VisibilityStatus
        {
            Id = 2,
            Name = "Unlisted",
            VisibilityStates = new List<VisibilityState>()
        },
        new VisibilityStatus
        {
            Id = 3,
            Name = "Restricted",
            VisibilityStates = new List<VisibilityState>()
        },
        new VisibilityStatus
        {
            Id = 4,
            Name = "Hidden",
            VisibilityStates = new List<VisibilityState>()
        }
    };
}