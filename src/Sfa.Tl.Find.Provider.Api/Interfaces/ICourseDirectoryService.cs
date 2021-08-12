using System.Threading.Tasks;

namespace Sfa.Tl.Find.Provider.Api.Interfaces
{
    public interface ICourseDirectoryService
    {
        Task<(int Saved, int Updated, int Deleted)> ImportProviders();

        Task<(int Saved, int Updated, int Deleted)> ImportQualifications();
    }
}
