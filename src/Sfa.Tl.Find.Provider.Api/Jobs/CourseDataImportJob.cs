using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Jobs
{
    public class CourseDataImportJob : IJob
    {
        private readonly IMemoryCache _cache;
        private readonly ICourseDirectoryService _courseDirectoryService;
        private readonly ILogger<CourseDataImportJob> _logger;

        public CourseDataImportJob(
            ICourseDirectoryService courseDirectoryService,
            IMemoryCache cache,
            ILogger<CourseDataImportJob> logger)
        {
            _courseDirectoryService = courseDirectoryService ?? throw new ArgumentNullException(nameof(courseDirectoryService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"{nameof(CourseDataImportJob)} job triggered. {context?.Trigger?.Description}");

            try
            {
                var qualifications = await _courseDirectoryService.ImportQualifications();
                var providers = await _courseDirectoryService.ImportProviders();

                if (context != null)
                {
                    context.Result = (qualifications, providers);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{nameof(CourseDataImportJob)} job failed.");
            }
        }
    }
}
