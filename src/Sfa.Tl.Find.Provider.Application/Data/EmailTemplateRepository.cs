using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Data;
public class EmailTemplateRepository : IEmailTemplateRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDynamicParametersWrapper _dynamicParametersWrapper;

    public EmailTemplateRepository(
        IDbContextWrapper dbContextWrapper,
        IDynamicParametersWrapper dynamicParametersWrapper)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        _dynamicParametersWrapper = dynamicParametersWrapper ?? throw new ArgumentNullException(nameof(dynamicParametersWrapper));
    }

    public async Task<EmailTemplate> GetEmailTemplate(string templateName)
    {
        using var connection = _dbContextWrapper.CreateConnection();

        _dynamicParametersWrapper.CreateParameters(new
        {
            templateName
        });

        return (await _dbContextWrapper
            .QueryAsync<EmailTemplate>(
                connection,
                "SELECT TOP(1) TemplateId, Name " +
                "FROM dbo.EmailTemplate " +
                "WHERE Name = @templateName",
                _dynamicParametersWrapper.DynamicParameters))
            .SingleOrDefault();
    }
}
