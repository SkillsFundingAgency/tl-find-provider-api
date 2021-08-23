using Microsoft.Extensions.Caching.Memory;
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
            IMemoryCache cache = null,
            ILogger<CourseDataImportJob> logger = null)
        {
            courseDirectoryService ??= Substitute.For<ICourseDirectoryService>();
            cache ??= Substitute.For<IMemoryCache>();
            logger ??= Substitute.For<ILogger<CourseDataImportJob>>();

            return new CourseDataImportJob(
                courseDirectoryService,
                cache,
                logger);
        }
    }
}
