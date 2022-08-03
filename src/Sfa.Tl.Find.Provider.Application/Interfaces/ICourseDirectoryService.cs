namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface ICourseDirectoryService
{
    Task ImportProviders();

    Task ImportQualifications();
}