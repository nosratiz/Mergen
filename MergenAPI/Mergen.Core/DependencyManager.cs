using Mergen.Core.Data;
using Mergen.Core.Managers;
using Mergen.Core.Options;
using Mergen.Core.QueryProcessing;
using Mergen.Core.Services;
using Mergen.Core.Services.EmailSenders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mergen.Core
{
    public static class DependencyManager
    {
        public static void RegisterMergenServices(this IServiceCollection services, IConfiguration configuration,
            bool isDevelopment)
        {
            services.RegisterData(configuration);
            services.RegisterManagers();
            services.RegisterQueryProcessing();

            services.ConfigOptions(configuration);

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailSender, GmailEmailSender>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IImageProcessingService, ImageProcessingService>();
        }

        private static void ConfigureLocalization(this IServiceCollection services)
        {
        }

        private static void RegisterData(this IServiceCollection services, IConfiguration configuration)
        {
            if (bool.TryParse(configuration["Data:InMemory"], out var inMemory) && inMemory)
                services.AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<DataContext>(options => options.UseInMemoryDatabase("Mergen"));
            else
            {
                var provider = configuration["Data:Provider"]?.ToLower();
                switch (provider)
                {
                    case "sqlite":
                        services.AddEntityFrameworkSqlite().AddDbContext<DataContext>(options =>
                            options.UseSqlite(configuration.GetConnectionString("Mergen")));
                        break;
                    case "postgresql":
                        services.AddEntityFrameworkNpgsql().AddDbContext<DataContext>(options =>
                            options.UseNpgsql(configuration.GetConnectionString("Mergen")));
                        break;
                    case "sqlserver":
                    default:
                        services.AddEntityFrameworkSqlServer().AddDbContext<DataContext>(options =>
                            options.UseSqlServer(configuration.GetConnectionString("Mergen")));
                        break;
                }
            }

            services.AddSingleton<DbContextFactory>();
        }
            
        private static void RegisterManagers(this IServiceCollection services)
        {
            services.AddSingleton<AccountInvitationManager>();
            services.AddSingleton<AccountManager>();
            services.AddSingleton<AchievementManager>();
            services.AddSingleton<CategoryManager>();
            services.AddSingleton<FileManager>();
            services.AddSingleton<QuestionManager>();
            services.AddSingleton<SessionManager>();
            services.AddSingleton<ShopItemManager>();
            services.AddSingleton<StatsManager>();
            services.AddSingleton<AccountItemManager>();
            services.AddSingleton<AccountFriendManager>();
            services.AddSingleton<FriendRequestManager>();
            services.AddSingleton<LevelManager>();

            services.AddScoped<GamingService>();
            services.AddScoped<AchievementService>();
        }

        private static void ConfigOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GmailEmailSenderOptions>(configuration.GetSection("MailSenderOptions")
                .GetSection("Gmail"));
            services.Configure<EmailVerificationOptions>(configuration.GetSection("EmailVerificationOptions"));
            services.Configure<ResetPasswordOptions>(configuration.GetSection("ResetPasswordOptions"));
            services.Configure<OnlinePaymentOptions>(configuration.GetSection("OnlinePayment"));
            services.Configure<FinancialOptions>(configuration.GetSection("Financial"));
            services.Configure<FileOptions>(configuration.GetSection("File"));
            services.Configure<BaseUrlsOptions>(configuration.GetSection("BaseUrls"));
            services.Configure<GameSettings>(configuration.GetSection("GameSettings"));
        }

        private static void RegisterQueryProcessing(this IServiceCollection services)
        {
            services.AddSingleton<QueryProcessor>();
            services.AddSingleton<PropertyCache>();
            services.AddSingleton<InputProcessor>();
        }
    }
}