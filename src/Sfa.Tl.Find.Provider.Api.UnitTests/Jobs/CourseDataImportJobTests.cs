using System.Threading.Tasks;
using NSubstitute;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Jobs;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Jobs
{
    public class CourseDataImportJobTests
    {
        [Fact]
        public void Constructor_Guards_Against_NullParameters()
        {
            typeof(CourseDataImportJob)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void Constructor_Guards_Against_BadParameters()
        {
            typeof(CourseDataImportJob)
                .ShouldNotAcceptNullOrBadConstructorArguments();
        }

        [Fact]
        public async Task Execute_Job_Works_As_Expected()
        {
            var jobContext = Substitute.For<IJobExecutionContext>();

            var job = new CourseDataImportJobBuilder()
                .Build();

            await job.Execute(jobContext);
        }
    }
}
