using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Microsoft.Extensions.Logging;
using Polly.Registry;

namespace Sfa.Tl.Find.Provider.Application.Data;
public class EmailTemplateRepository : IEmailTemplateRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDynamicParametersWrapper _dynamicParametersWrapper;
    private readonly ILogger<EmailTemplateRepository> _logger;
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

    public EmailTemplateRepository(
        IDbContextWrapper dbContextWrapper,
        IDynamicParametersWrapper dynamicParametersWrapper,
        IReadOnlyPolicyRegistry<string> policyRegistry,
        ILogger<EmailTemplateRepository> logger)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        _dynamicParametersWrapper = dynamicParametersWrapper ?? throw new ArgumentNullException(nameof(dynamicParametersWrapper));
        _policyRegistry = policyRegistry ?? throw new ArgumentNullException(nameof(policyRegistry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
