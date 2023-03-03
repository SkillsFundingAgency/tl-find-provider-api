using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class QualificationBuilder
{
    public IEnumerable<Qualification> BuildList() =>
        new List<Qualification>
        {
            new() { Id = 45, Name = "Building Services Engineering for Construction", NumberOfQualificationsOffered = 0 },
            new() { Id = 36, Name = "Design, Surveying and Planning for Construction", NumberOfQualificationsOffered = 1 },
            new() { Id = 39, Name = "Digital Business Services", NumberOfQualificationsOffered = 2 },
            new() { Id = 37, Name = "Digital Production, Design and Development", NumberOfQualificationsOffered = 3 },
            new() { Id = 40, Name = "Digital Support Services", NumberOfQualificationsOffered = 4 },
            new() { Id = 38, Name = "Education and Early Years", NumberOfQualificationsOffered = 5 },
            new() { Id = 41, Name = "Health", NumberOfQualificationsOffered = 6 },
            new() { Id = 42, Name = "Healthcare Science", NumberOfQualificationsOffered = 7 },
            new() { Id = 44, Name = "Onsite Construction", NumberOfQualificationsOffered = 8 },
            new() { Id = 43, Name = "Science", NumberOfQualificationsOffered = 9 },
            new() { Id = 46, Name = "Finance", NumberOfQualificationsOffered = 10 },
            new() { Id = 47, Name = "Accounting", NumberOfQualificationsOffered = 11 },
            new() { Id = 48, Name = "Design and development for engineering and manufacturing", NumberOfQualificationsOffered = 12 },
            new() { Id = 49, Name = "Maintenance, installation and repair for engineering and manufacturing", NumberOfQualificationsOffered = 13 },
            new() { Id = 50, Name = "Engineering, manufacturing, processing and control", NumberOfQualificationsOffered = 14 },
            new() { Id = 51, Name = "Management and administration", NumberOfQualificationsOffered = 15 }
        };

    public IList<Qualification> BuildList(int numberToTake) =>
        BuildList().Take(numberToTake).ToList();
}