using System.Threading.Tasks;

namespace Sfa.Tl.Find.Provider.Api.Interfaces
{
    public interface ICourseDirectoryService
    {
        Task<(int Saved, int Deleted)> ImportQualifications();
        Task<(int Saved, int Deleted)> ImportProviders();
    }
}
