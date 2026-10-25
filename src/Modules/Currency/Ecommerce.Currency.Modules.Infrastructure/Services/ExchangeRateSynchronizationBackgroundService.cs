
using Ecommerce.Currency.Modules.Application.Features.SynchronizeExchangeRates;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Currency.Modules.Infrastructure.Services;

internal sealed class ExchangeRateSynchronizationBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<ExchangeRateSynchronizationBackgroundService> logger)
    : BackgroundService
{
    private static readonly TimeSpan SynchronizationTime =
        new(10, 0, 0);

    private static readonly TimeZoneInfo KyivTimeZone =
        FindKyivTimeZone();

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await SynchronizeAsync(stoppingToken);

            var delay = GetDelayUntilNextRun();

            logger.LogInformation(
                "Next exchange rate synchronization is scheduled in {Delay}.",
                delay);

            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task SynchronizeAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();

            var sender = scope.ServiceProvider
                .GetRequiredService<ISender>();

            var result = await sender.Send(
                new SynchronizeExchangeRatesCommand(),
                cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation(
                    "Exchange rates synchronized successfully. Count: {Count}.",
                    result.Value);
            }
            else
            {
                logger.LogError(
                    "Exchange rate synchronization failed: {Error}",
                    result.Error);
            }
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "An error occurred while synchronizing exchange rates.");
        }
    }

    private static TimeSpan GetDelayUntilNextRun()
    {
        var now = TimeZoneInfo.ConvertTime(
            DateTimeOffset.UtcNow,
            KyivTimeZone);

        var nextRun = now.Date.Add(SynchronizationTime);

        if (now.DateTime >= nextRun)
        {
            nextRun = nextRun.AddDays(1);
        }

        var nextRunUtc = TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(nextRun, DateTimeKind.Unspecified),
            KyivTimeZone);

        var delay = nextRunUtc - DateTime.UtcNow;

        return delay > TimeSpan.Zero
            ? delay
            : TimeSpan.FromSeconds(1);
    }

    private static TimeZoneInfo FindKyivTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(
                "FLE Standard Time");
        }
    }
}
