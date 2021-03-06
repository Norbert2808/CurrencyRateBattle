using CurrencyRateBattleServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CurrencyRateBattleServer.Data.ModelConfigurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        _ = builder.ToTable("Room")
            .HasKey(room => room.Id);
        _ = builder.ToTable("Room")
            .Property(r => r.IsClosed)
            .HasDefaultValue(0);
    }
}


