
namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IEmployerInterestRepository
{
    Task<int> DeleteBefore(DateTime date);
}