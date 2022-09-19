﻿using System.Text.Json.Serialization;

namespace Sfa.Tl.Find.Provider.Application.Models;
public class EmailDeliveryReceipt
{
    public Guid Id { get; set; }

    public string Reference { get; set; }

    public string To { get; set; }

    public string Status { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [JsonPropertyName("sent_at")]
    public DateTime? SentAt { get; set; }

    [JsonPropertyName("notification_type")]
    public string NotificationType { get; set; }

    public string EmailDeliveryStatus => string.IsNullOrEmpty(Status) ? "unknown-failure" : Status;

}
