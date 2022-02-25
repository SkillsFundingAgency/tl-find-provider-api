using System;
using System.ServiceModel;

namespace Sfa.Tl.Find.Provider.Api.Connected_Services.Sfa.Tl.Find.Provider.Api.UkRlp.Api.Client;

public interface IProviderQueryPortTypeClient : ICommunicationObject, ProviderQueryPortType, IDisposable { }
public partial class ProviderQueryPortTypeClient : IProviderQueryPortTypeClient { }