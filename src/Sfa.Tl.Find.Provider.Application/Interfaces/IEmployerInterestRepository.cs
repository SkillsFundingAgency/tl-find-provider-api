
namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IEmployerInterestRepository
{
    Task<int> Delete(Guid uniqueId);

    Task<int> DeleteBefore(DateTime date);
}