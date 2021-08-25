using System.Threading.Tasks;

namespace Sfa.Tl.Find.Provider.Api.Interfaces
{
    public interface ICourseDirectoryService
    {
        Task ImportProviders();

        Task ImportQualifications();
    }
}
