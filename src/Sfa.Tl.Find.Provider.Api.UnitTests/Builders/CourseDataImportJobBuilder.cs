using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Jobs;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
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
}
