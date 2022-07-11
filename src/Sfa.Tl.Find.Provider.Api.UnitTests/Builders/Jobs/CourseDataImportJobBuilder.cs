using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Jobs;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Jobs;

public class CourseDataImportJobBuilder
{
    public CourseDataImportJob Build(
        ICourseDirectoryService courseDirectoryService = null,
        ILogger<CourseDataImportJob> logger = null)
    {
        courseDirectoryService ??= Substitute.For<ICourseDirectoryService>();
        logger ??= Substitute.For<ILogger<CourseDataImportJob>>();

        return new CourseDataImportJob(
            courseDirectoryService,
            logger);
    }
}