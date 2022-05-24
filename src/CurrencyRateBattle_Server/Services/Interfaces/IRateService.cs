﻿using CurrencyRateBattleServer.Dto;
using CurrencyRateBattleServer.Models;

namespace CurrencyRateBattleServer.Services.Interfaces;

public interface IRateService
{
    public Task<Rate> CreateRateAsync(Rate rate);

    public Task<List<Rate>> GetRateByRoomIdAsync(Guid roomId);

    public Task UpdateRateByRoomIdAsync(Guid id, Rate updatedRate);

    public Task DeleteRateAsync(Guid id);

    public Task<List<Rate>> GetRatesAsync(bool? isActive, string? currencyCode);

    public Task<List<BetDto>> GetRatesByAccountIdAsync(Guid accountId);

}
