using System.Globalization;
using CurrencyRateBattleServer.Data;
using CurrencyRateBattleServer.Dto;
using CurrencyRateBattleServer.Helpers;
using CurrencyRateBattleServer.Models;
using CurrencyRateBattleServer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CurrencyRateBattleServer.Services;

public class RoomService : IRoomService
{
    private readonly ILogger<RoomService> _logger;

    private readonly IServiceScopeFactory _scopeFactory;

    private readonly IRateCalculationService _rateCalculationService;

    private readonly SemaphoreSlim _semaphoreSlimRateHosted = new(1, 1);

    private readonly SemaphoreSlim _semaphoreSlimRoomHosted = new(1, 1);

    public RoomService(ILogger<RoomService> logger,
        IRateCalculationService rateCalculationService,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _rateCalculationService = rateCalculationService;
        _scopeFactory = scopeFactory;
    }

    public async Task GenerateRoomsByCurrencyCountAsync()
    {
        _logger.LogInformation($"{nameof(GenerateRoomsByCurrencyCountAsync)} was caused");
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CurrencyRateBattleContext>();

        await _semaphoreSlimRoomHosted.WaitAsync();
        try
        {
            foreach (var curr in dbContext.Currencies)
            {
                _ = await dbContext.CurrencyStates.AddAsync(await CreateRoomWithCurrencyStateAsync(curr));
            }

            _ = await dbContext.SaveChangesAsync();
        }
        finally
        {
            _ = _semaphoreSlimRoomHosted.Release();
        }
    }

    public Task<CurrencyState> CreateRoomWithCurrencyStateAsync(Currency curr)
    {
        _logger.LogInformation($"{nameof(CreateRoomWithCurrencyStateAsync)} was caused");
        var currentDate = DateTime.ParseExact(
            DateTime.UtcNow.ToString("MM.dd.yyyy HH:00:00", CultureInfo.InvariantCulture),
            "MM.dd.yyyy HH:mm:ss", null);

        return Task.FromResult(new CurrencyState
        {
            Date = currentDate,
            CurrencyExchangeRate = 0,
            Currency = curr,
            CurrencyId = curr.Id,
            Room = new Room { Date = currentDate.AddDays(1), IsClosed = false }
        });
    }

    public async Task UpdateRoomAsync(Guid id, Room updatedRoom)
    {
        _logger.LogInformation($"{nameof(UpdateRoomAsync)} was caused");
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CurrencyRateBattleContext>();

        await _semaphoreSlimRateHosted.WaitAsync();
        try
        {
            var roomExists = await db.Rooms.AnyAsync(r => r.Id == id);
            if (!roomExists)
                throw new GeneralException($"{nameof(Room)} with Id={id} is not found.");

            _ = db.Rooms.Update(updatedRoom);
            _ = await db.SaveChangesAsync();
        }
        finally
        {
            _ = _semaphoreSlimRateHosted.Release();
        }
    }

    public async Task CheckRoomsStateAsync()
    {
        _logger.LogInformation($"{nameof(CheckRoomsStateAsync)} was caused");
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CurrencyRateBattleContext>();


        foreach (var r in db.Rooms)
        {
            await RoomClosureCheckAsync(r);
            await CalculateRatesIfRoomClosed(r);
        }
    }

    private async Task RoomClosureCheckAsync(Room room)
    {
        _logger.LogInformation($"{nameof(RoomClosureCheckAsync)} was caused");
        if ((room.Date.Date == DateTime.Today
             && room.Date.Hour == DateTime.UtcNow.AddHours(1).Hour)
            || ((room.Date.Date == DateTime.Today.AddDays(1))
            && room.Date.Hour == 0 && DateTime.UtcNow.Hour == 23)
            || DateTime.UtcNow > room.Date)
        {
            room.IsClosed = true;
            await UpdateRoomAsync(room.Id, room);
        }
    }

    private async Task CalculateRatesIfRoomClosed(Room room)
    {
        _logger.LogInformation($"{nameof(CalculateRatesIfRoomClosed)} was caused");
        if ((room.Date.Date == DateTime.Today
             && room.Date.Hour == DateTime.UtcNow.Hour
             && room.IsClosed)
            || (DateTime.UtcNow > room.Date
                && room.IsClosed))
        {
            try
            {
                await _rateCalculationService.StartRateCalculationByRoomIdAsync(room.Id);
                await UpdateRoomAsync(room.Id, room);
            }
            catch (GeneralException)
            {
                await DeleteRoomByIdAsync(room.Id);
            }
        }
    }

    public Task<List<RoomDto>> GetRoomsAsync(bool? isClosed)
    {
        _logger.LogInformation($"{nameof(GetRoomsAsync)} was caused");
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CurrencyRateBattleContext>();

        List<RoomDto> roomDtoStorage = new();
        var result = from curr in db.Currencies
                     join currState in db.CurrencyStates on curr.Id equals currState.CurrencyId
                     join room in db.Rooms on currState.RoomId equals room.Id
                     where room.IsClosed == isClosed
                     select new
                     {
                         room.Id,
                         curr.CurrencyName,
                         room.Date,
                         room.IsClosed,
                         currState.CurrencyExchangeRate,
                         RateDate = currState.Date,
                         RateCount = db.Rates.Count(r => r.RoomId == room.Id)
                     };

        foreach (var data in result)
        {
            roomDtoStorage.Add(new RoomDto
            {
                Id = data.Id,
                CurrencyExchangeRate = Math.Round(data.CurrencyExchangeRate, 2),
                СurrencyName = data.CurrencyName,
                Date = data.Date,
                IsClosed = data.IsClosed,
                UpdateRateTime = data.RateDate,
                CountRates = data.RateCount
            });
        }

        return Task.FromResult(roomDtoStorage);
    }

    public async Task<Room?> GetRoomByIdAsync(Guid id)
    {
        _logger.LogInformation($"{nameof(GetRoomByIdAsync)} was caused");
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CurrencyRateBattleContext>();

        var result = await db.Rooms.FirstOrDefaultAsync(r => r.Id == id);

        return result;
    }

    public Task<List<RoomDto>?> GetActiveRoomsWithFilterAsync(Filter filter)
    {
        _logger.LogInformation($"{nameof(GetActiveRoomsWithFilterAsync)} was caused");
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CurrencyRateBattleContext>();

        var result = new List<RoomDto>();

        var filteredRooms =
            from currencyState in db.CurrencyStates
            join room in db.Rooms on currencyState.RoomId equals room.Id
            join curr in db.Currencies on currencyState.CurrencyId equals curr.Id
            where room.IsClosed == false
            select new
            {
                room.Id,
                room.Date,
                currencyState.CurrencyExchangeRate,
                currencyState.CurrencyId,
                room.IsClosed,
                curr.CurrencyName,
                RateUpdateDate = currencyState.Date,
                RateCount = db.Rates.Count(r => r.RoomId == room.Id)
            };

        if (!string.IsNullOrWhiteSpace(filter.CurrencyName))
            filteredRooms =
                filteredRooms.Where(room => room.CurrencyName == filter.CurrencyName.ToUpperInvariant());
        if (filter.DateTryParse(filter.StartDate, out var startDate))
            filteredRooms = filteredRooms.Where(room => room.Date >= startDate);
        if (filter.DateTryParse(filter.EndDate, out var endDate))
            filteredRooms = filteredRooms.Where(room => room.Date <= endDate);

        foreach (var room in filteredRooms)
        {
            result.Add(
                new RoomDto
                {
                    Id = room.Id,
                    CurrencyExchangeRate = room.CurrencyExchangeRate,
                    Date = room.Date,
                    СurrencyName = room.CurrencyName,
                    UpdateRateTime = room.RateUpdateDate,
                    IsClosed = room.IsClosed,
                    CountRates = room.RateCount
                });
        }

        return Task.FromResult(result);
    }

    public async Task DeleteRoomByIdAsync(Guid roomId)
    {
        _logger.LogInformation($"{nameof(DeleteRoomByIdAsync)} was caused");
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CurrencyRateBattleContext>();

        await _semaphoreSlimRateHosted.WaitAsync();
        try
        {
            var room = await db.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);

            if (room is null)
                return;

            _ = db.Remove(room);
            _ = await db.SaveChangesAsync();
        }
        finally
        {
            _ = _semaphoreSlimRateHosted.Release();
        }
    }
}
