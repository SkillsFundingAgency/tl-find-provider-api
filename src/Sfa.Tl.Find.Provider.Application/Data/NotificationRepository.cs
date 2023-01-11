using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.Data;
public  class NotificationRepository : INotificationRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;
    private readonly IDynamicParametersWrapper _dynamicParametersWrapper;
    private readonly ILogger<NotificationRepository> _logger;
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

    public NotificationRepository(
        IDbContextWrapper dbContextWrapper,
        IDynamicParametersWrapper dynamicParametersWrapper,
        IReadOnlyPolicyRegistry<string> policyRegistry,
        ILogger<NotificationRepository> logger)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        _dynamicParametersWrapper = dynamicParametersWrapper ?? throw new ArgumentNullException(nameof(dynamicParametersWrapper));
        _policyRegistry = policyRegistry ?? throw new ArgumentNullException(nameof(policyRegistry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
