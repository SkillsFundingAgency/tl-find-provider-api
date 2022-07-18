using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Models;

public class QualificationBuilder
{
    public IEnumerable<Qualification> BuildList() =>
        new List<Qualification>
        {
            new() { Id = 45, Name = "Building Services Engineering for Construction" },
            new() { Id = 36, Name = "Design, Surveying and Planning for Construction" },
            new() { Id = 39, Name = "Digital Business Services" },
            new() { Id = 37, Name = "Digital Production, Design and Development" },
            new() { Id = 40, Name = "Digital Support Services" },
            new() { Id = 38, Name = "Education and Childcare" },
            new() { Id = 41, Name = "Health" },
            new() { Id = 42, Name = "Healthcare Science" },
            new() { Id = 44, Name = "Onsite Construction" },
            new() { Id = 43, Name = "Science" },
            new() { Id = 46, Name = "Finance" },
            new() { Id = 47, Name = "Accounting" },
            new() { Id = 48, Name = "Design and development for engineering and manufacturing" },
            new() { Id = 49, Name = "Maintenance, installation and repair for engineering and manufacturing" },
            new() { Id = 50, Name = "Engineering, manufacturing, processing and control" },
            new() { Id = 51, Name = "Management and administration" }
        };

    public IList<Qualification> BuildList(int numberToTake) =>
        BuildList().Take(numberToTake).ToList();
}