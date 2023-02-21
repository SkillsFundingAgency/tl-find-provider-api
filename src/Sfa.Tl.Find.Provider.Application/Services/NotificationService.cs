using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.Services;
public class NotificationService : INotificationService
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IGuidProvider _guidProvider;
    private readonly IEmailService _emailService;
    private readonly INotificationRepository _notificationRepository;
    private readonly ProviderSettings _providerSettings;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IDateTimeProvider dateTimeProvider,
        IGuidProvider guidProvider,
        IEmailService emailService,
        INotificationRepository notificationRepository,
        IOptions<ProviderSettings> providerOptions,
        IOptions<SearchSettings> searchOptions,
        ILogger<NotificationService> logger)
    {
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (providerOptions is null) throw new ArgumentNullException(nameof(providerOptions));
        if (searchOptions is null) throw new ArgumentNullException(nameof(searchOptions));

        _providerSettings = providerOptions?.Value
                            ?? throw new ArgumentNullException(nameof(providerOptions));
    }

    public async Task<int> CreateNotification(Notification notification, long ukPrn)
    {
        notification.EmailVerificationToken = _guidProvider.NewGuid();
        var id = await _notificationRepository.Create(notification, ukPrn);
        await SendProviderVerificationEmail(notification.Email, notification.EmailVerificationToken!.Value);
        return id;
    }

    public async Task CreateNotificationLocation(Notification notification, int? providerNotificationId = null)
    {
        await _notificationRepository.CreateLocation(notification, providerNotificationId.Value);
    }

    public async Task DeleteNotification(int notificationId)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Deleting notification {notificationId}", notificationId);
        }

        await _notificationRepository.Delete(notificationId);
    }

    public async Task DeleteNotificationLocation(int notificationLocationId)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Deleting notification location {notificationLocationId}", notificationLocationId);
        }

        await _notificationRepository.DeleteLocation(notificationLocationId);
    }

    public async Task<IEnumerable<NotificationSummary>> GetNotificationSummaryList(long ukPrn)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Getting notifications");
        }

        var notifications =
            (await _notificationRepository
            .GetNotificationSummaryList(ukPrn));

        return notifications;
    }

    public async Task<IEnumerable<NotificationLocationSummary>> GetNotificationLocationSummaryList(int notificationId)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Getting notification locations");
        }

        return await _notificationRepository
                .GetNotificationLocationSummaryList(notificationId);
    }

    public async Task<Notification> GetNotification(int notificationId)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Getting notification {notificationId}", notificationId);
        }

        return await _notificationRepository
            .GetNotification(notificationId);
    }

    public async Task<Notification> GetNotificationLocation(int notificationLocationId)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Getting notification location {notificationLocationId}", notificationLocationId);
        }

        return await _notificationRepository
            .GetNotificationLocation(notificationLocationId);
    }

    public async Task<IEnumerable<NotificationLocationName>> GetAvailableNotificationLocationPostcodes(int providerNotificationId)
    {
        return (await _notificationRepository
            .GetProviderNotificationLocations(providerNotificationId))
            .Where(p => p.Id is null && p.LocationId is not null);
    }
    
    public async Task SendProviderNotifications(NotificationFrequency frequency)
    {
        var currentDateTime = _dateTimeProvider.UtcNow;
        var pendingNotificationEmails = await _notificationRepository.GetPendingNotificationEmails(frequency);

        var groupedEmails = pendingNotificationEmails
            .GroupBy(p => p.Email, p => p.NotificationLocationId)
            .Select(g => new
            {
                Email = g.Key,
                IdList = g.ToList()
            });

        foreach (var notificationEmail in groupedEmails)
        {
            var lastNotificationSent = await _notificationRepository
                .GetLastNotificationSentDate(notificationEmail.IdList);

            if (lastNotificationSent >= currentDateTime)
            {
                _logger.LogWarning("Skipping emails because the notification sent time has already been updated. Ids: {ids}",
                    string.Join(',', notificationEmail.IdList));
                continue;
            }

            await SendProviderNotificationEmail(
                notificationEmail.Email);

            await _notificationRepository.UpdateNotificationSentDate(
                notificationEmail.IdList,
                currentDateTime);
        }
    }

    public async Task SendProviderNotificationEmail(string emailAddress)
    {
        var siteUri = new Uri(_providerSettings.ConnectSiteUri);
        var notificationsUri = new Uri(siteUri, "notifications");
        var employerListUri = new Uri(siteUri, "employer-list");
        var searchFiltersUri = new Uri(siteUri, "filters");

        var uniqueId = _guidProvider.NewGuid();

        await _emailService.SendEmail(
            emailAddress,
            EmailTemplateNames.ProviderNotification,
            new Dictionary<string, string>
            {
                { "employer_list_uri", employerListUri.ToString() },
                { "search_filters_uri", searchFiltersUri.ToString() },
                { "notifications_uri", notificationsUri.ToString() }
            },
            uniqueId.ToString());
    }

    public async Task SendProviderNotificationVerificationEmail(int notificationId, string emailAddress)
    {
        var verificationToken = _guidProvider.NewGuid();
        await SendProviderVerificationEmail(emailAddress, verificationToken);

        await _notificationRepository.SaveEmailVerificationToken(notificationId, emailAddress, verificationToken);
    }

    public async Task<(bool Success, string Email)> VerifyNotificationEmail(string token)
    {
        if (!Guid.TryParse(token, out var realToken))
        {
            _logger.LogError("Invalid token received in VerifyNotificationEmail");
        }

        return await _notificationRepository.VerifyEmailToken(realToken);
    }
    
    public async Task UpdateNotification(Notification notification)
    {
        await _notificationRepository.Update(notification);
    }

    public async Task UpdateNotificationLocation(Notification notification)
    {
        await _notificationRepository.UpdateLocation(notification);
    }

    private async Task SendProviderVerificationEmail(string emailAddress, Guid token)
    {
        var siteUri = new Uri(_providerSettings.ConnectSiteUri);
        var notificationsUri = new Uri(siteUri, "notifications");
        var verificationUri = new Uri(QueryHelpers.AddQueryString(
            notificationsUri.AbsoluteUri,
            "token",
            token.ToString("D").ToLower()));

        await _emailService.SendEmail(
            emailAddress,
            EmailTemplateNames.ProviderVerification,
            new Dictionary<string, string>
            {
                { "email_verification_link", verificationUri.ToString() },
                { "notifications_uri", notificationsUri.ToString() }
            },
            token.ToString());
    }
}
