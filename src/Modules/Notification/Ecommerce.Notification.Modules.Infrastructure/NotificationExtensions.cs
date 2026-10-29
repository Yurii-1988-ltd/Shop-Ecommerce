using Ecommerce.Notification.Modules.Infrastructure.Adapters;
using Ecommerce.Notification.Modules.Infrastructure.Configuration;
using Ecommerce.Notification.Modules.Infrastructure.Templates;

namespace Ecommerce.Notification.Modules.Infrastructure;

public static class NotificationsModuleExtensions
{
    public static IServiceCollection AddNotificationsModule(
        this IServiceCollection services,
        IConfiguration config)
    {
        // 1. MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(
                typeof(INotificationService).Assembly));

        // 2. PostgreSQL
        var connectionString =
            config.GetConnectionString("notifications")
            ?? throw new InvalidOperationException(
                "Connection string 'notifications' is not configured.");

        services.AddNpgsqlDataSource(
            connectionString,
            serviceKey: "notification");

        services.AddDbContext<NotificationsContext>((sp, options) =>
        {
            var dataSource =
                sp.GetRequiredKeyedService<NpgsqlDataSource>(
                    "notification");

            options.UseNpgsql(dataSource);
        });

        // 3. MassTransit
        services.AddCustomTransit(
            config,
            typeof(NotificationsModuleExtensions).Assembly);

        // 4. SMTP configuration
        var smtpOptions = config
            .GetSection("SmtpOptions")
            .Get<SmtpOptions>()
            ?? throw new InvalidOperationException(
                "SmtpOptions configuration is not configured.");

        if (string.IsNullOrWhiteSpace(smtpOptions.Host))
        {
            throw new InvalidOperationException(
                "SmtpOptions:Host is not configured.");
        }

        if (string.IsNullOrWhiteSpace(smtpOptions.Username))
        {
            throw new InvalidOperationException(
                "SmtpOptions:Username is not configured.");
        }

        if (string.IsNullOrWhiteSpace(smtpOptions.Password))
        {
            throw new InvalidOperationException(
                "SmtpOptions:Password is not configured.");
        }

        // TEMPORARY SMTP diagnostics — do not log credentials.
        Console.WriteLine(
            $"[SMTP DEBUG] Host={smtpOptions.Host}; " +
            $"Port={smtpOptions.Port}; " +
            $"UseSsl={smtpOptions.UseSsl}; " +
            $"UsernameConfigured={!string.IsNullOrWhiteSpace(smtpOptions.Username)}; " +
            $"PasswordConfigured={!string.IsNullOrWhiteSpace(smtpOptions.Password)}");

        // 5. FluentEmail + SMTP
        services
            .AddFluentEmail(
                smtpOptions.FromEmail,
                smtpOptions.FromName)
            .AddSmtpSender(() => new System.Net.Mail.SmtpClient
            {
                Host = smtpOptions.Host,
                Port = smtpOptions.Port,
                EnableSsl = smtpOptions.UseSsl,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(
                    smtpOptions.Username,
                    smtpOptions.Password)
            });

        // 6. Application services
        services.AddScoped<
            INotificationUnitOfWork,
            NotificationsUnitOfWork>();

        services.AddScoped<
            INotificationRepository,
            NotificationRepository>();

        services.AddScoped<
            IEmailSender,
            FluentEmailSender>();

        services.AddScoped<
            IEmailTemplateRenderer,
            LiquidEmailTemplateRenderer>();

        services.AddScoped<
            INotificationService,
            NotificationService>();

        return services;
    }

    public static async Task ApplyNotificationsMigrationsAsync(
        this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<NotificationsContext>();

        await context.Database.MigrateAsync();
    }
}