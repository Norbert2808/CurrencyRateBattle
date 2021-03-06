using CurrencyRateBattleServer.Dto;
using CurrencyRateBattleServer.Models;

namespace CurrencyRateBattleServer.Services.Interfaces;

public interface IRoomService
{
    Task GenerateRoomsByCurrencyCountAsync();

    Task<CurrencyState> CreateRoomWithCurrencyStateAsync(Currency curr);

    Task UpdateRoomAsync(Guid id, Room updatedRoom);

    Task CheckRoomsStateAsync();

    Task<List<RoomDto>> GetRoomsAsync(bool? isClosed);

    Task<Room?> GetRoomByIdAsync(Guid id);

    Task<List<RoomDto>?> GetActiveRoomsWithFilterAsync(Filter filter);

    Task DeleteRoomByIdAsync(Guid roomId);
}
