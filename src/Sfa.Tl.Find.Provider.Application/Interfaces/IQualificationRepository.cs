using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IQualificationRepository
{
    Task<bool> HasAny();

    Task<IEnumerable<Qualification>> GetAll();

    Task Save(IEnumerable<Qualification> qualifications);
}