using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Connected_Services.Sfa.Tl.Find.Provider.Api.UkRlp.Api.Client;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Services;

public class ProviderReferenceDataService : IProviderReferenceDataService
{
    private readonly ILogger<ProviderReferenceDataService> _logger;
    private readonly IProviderQueryPortTypeClient _providerQueryPortTypeClient;

    public ProviderReferenceDataService(
        IProviderQueryPortTypeClient providerQueryPortTypeClient,
        ILogger<ProviderReferenceDataService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _providerQueryPortTypeClient = providerQueryPortTypeClient ?? throw new ArgumentNullException(nameof(providerQueryPortTypeClient));
    }

    public async Task<List<ProviderReference>> GetAllAsync(DateTime lastUpdateDate)
    {
        var query = CreateQuery(lastUpdateDate);

        var response = await RetrieveAllAsync(query);

        var providerRecords = response.ProviderQueryResponse.MatchingProviderRecords;

        var providerReferences = providerRecords
            .Select(p => new ProviderReference
            {
                Name = p.ProviderName,
                UkPrn = long.Parse(p.UnitedKingdomProviderReferenceNumber),
                Urn = ExtractUniqueReferenceNumber(p.VerificationDetails)
            })
            .ToList();

        return providerReferences;
    }

    private static long ExtractUniqueReferenceNumber(IEnumerable<VerificationDetailsStructure> verificationDetails)
    {
        var urnString = verificationDetails?
            .FirstOrDefault(x =>
                x.VerificationAuthority == "DfE (Schools Unique Reference Number)")
            ?.VerificationID;

        return urnString != null && long.TryParse(urnString, out var urn) 
            ? urn 
            : 0;
    }
    private static ProviderQueryParam CreateQuery(DateTime lastUpdateDate)
    {
        //var activeStatus = UkRlpRecordStatus.Active.Humanize();

        var query = new ProviderQueryParam(new ProviderQueryStructure
        {
            QueryId = "1",
            SelectionCriteria = new SelectionCriteriaStructure
            {
                //TODO: Should be able to use a unitedKingdomProviderReferenceNumberListField to limit data
                StakeholderId = "1",
                ProviderUpdatedSince = lastUpdateDate,
                ProviderUpdatedSinceSpecified = true,
                ApprovedProvidersOnly = YesNoType.No,
                ApprovedProvidersOnlySpecified = true,
                CriteriaCondition = QueryCriteriaConditionType.OR,
                CriteriaConditionSpecified = true,
                ProviderStatus = UkRlpRecordStatus.Active.Humanize()
            }
        });

        return query;
    }

    private async Task<response> RetrieveAllAsync(ProviderQueryParam query)
    {
        try
        {
            var response = await _providerQueryPortTypeClient.retrieveAllProvidersV3V4Async(query);

            return response;
        }
        catch (Exception ex)
        {
            if (_providerQueryPortTypeClient.State == CommunicationState.Faulted)
            {
                _providerQueryPortTypeClient.Abort();
            }

            _logger.LogError(ex, "Error Executing ProviderQueryPortTypeClient Internal Exception");
        }

        return null;
    }

    public enum UkRlpRecordStatus
    {
        [Description("A")]
        Active,
        [Description("V")]
        Verified,
        [Description("PD1")]
        DeactivationInProcess,
        [Description("PD2")]
        DeactivateComplete
    }

}

