﻿using System.Text.Json.Serialization;

namespace CRBClient.Models;

public class RoomViewModel
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("updateRateTime")]
    public DateTime UpdateRateTime { get; set; }

    [JsonPropertyName("currencyName")]
    public string СurrencyName { get; set; }

    [JsonPropertyName("currencyExchangeRate")]
    public decimal CurrencyExchangeRate { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("isClosed")]
    public bool IsClosed { get; set; }
}
