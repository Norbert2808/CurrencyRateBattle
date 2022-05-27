﻿using CurrencyRateBattleServer.Services.Interfaces;

namespace CurrencyRateBattleServer.Services.HostedServices;

public class RateHostedService : IHostedService, IDisposable
{
    private Timer? _timer;

    private readonly ILogger<RateHostedService> _logger;

    private readonly IRoomService _roomService;

    public RateHostedService(ILogger<RateHostedService> logger,
        IRoomService roomService)
    {
        _logger = logger;
        _roomService = roomService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rate Hosted Service running.");

        _timer = new Timer(Callback, null, TimeSpan.FromMinutes(1),
            TimeSpan.FromHours(1));

        return Task.CompletedTask;
    }

    private async void Callback(object? state)
    {
        _logger.LogInformation("CheckRoomsStateAsync has been invoked.");
        await _roomService.CheckRoomsStateAsync();
        _logger.LogInformation("CheckRoomsStateAsync сompleted the execution.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_timer != null)
            _ = _timer.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_timer != null)
            _timer.Dispose();
        GC.SuppressFinalize(this);
    }
}
