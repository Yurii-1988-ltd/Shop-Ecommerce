
using Ecommerce.Currency.Modules.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Currency.Modules.Infrastructure.Configurations;

internal sealed class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
{
    public void Configure(EntityTypeBuilder<ExchangeRate> builder)
    {
        builder.ToTable("ExchangeRates");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CurrencyCode)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(x => x.Rate)
            .HasPrecision(18, 8)
            .IsRequired();

        builder.Property(x => x.Units)
            .IsRequired();

        builder.Property(x => x.RateDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => new { x.CurrencyCode, x.RateDate })
            .IsUnique();
    }
}

