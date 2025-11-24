using CommsSender.Core.Middleware;
using CommsSender.Domain.Database.Context;
using CommsSender.Domain.Enums;
using CommsSender.Domain.Helpers;
using CommsSender.Domain.Interfaces.Api;
using CommsSender.Domain.Interfaces.Helpers;
using CommsSender.Domain.Services;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Testcontainers.PostgreSql;

Log.Logger = new LoggerConfiguration().WriteTo.Async(x => x.File("/app/Logs/log.log", retainedFileCountLimit: 7, rollingInterval: RollingInterval.Day)).WriteTo.Console().CreateLogger();
Log.Information("Logger Setup");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

// Add in our own services
builder.Services.AddSingleton<IEnvironmentalSettingHelper, EnvironmentalSettingHelper>();
builder.Services.AddHttpClient<IExpoPushNotificationHelper, ExpoPushNotificationHelper>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IPushNotificationService, PushNotificationService>();

// Add in the auth
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JwtValidIssuer"),
            ValidAudience = Environment.GetEnvironmentVariable("JwtValidAudience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JwtKey")!))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


#if DEBUG
GlobalConfiguration.Configuration.UseMemoryStorage();

var postgresContainer = new PostgreSqlBuilder()
    .WithImage("postgres:15-alpine")
    .WithDatabase("commssender")
    .WithUsername("testuser")
    .WithPassword("testpass")
    .WithPortBinding(5432, true)
    .Build();

await postgresContainer.StartAsync();

var connectionString = postgresContainer.GetConnectionString();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseLazyLoadingProxies()
           .UseNpgsql(connectionString));

GlobalConfiguration.Configuration.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString));

builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString))
        );

#else
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseLazyLoadingProxies()
           .UseNpgsql(Environment.GetEnvironmentVariable("xxxConnStringLive")));

GlobalConfiguration.Configuration.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(Environment.GetEnvironmentVariable("xxxConnString")));

builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(Environment.GetEnvironmentVariable("xxxConnString")))
        );

#endif

// hangfire
builder.Services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(10));

var app = builder.Build();

app.UseCors("AllowAll");

// Add API Key middleware before other middleware
app.UseMiddleware<ApiKeyMiddleware>();

#if DEBUG
// Seed the database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetService<AppDbContext>()!;
    var settingsHelper = scope.ServiceProvider.GetRequiredService<IEnvironmentalSettingHelper>();

    if (dbContext.Database.GetPendingMigrations().Any())
    {
        await dbContext.Database.MigrateAsync();
        await DatabaseSeedHelper.SeedDatabase(dbContext, settingsHelper, scope.ServiceProvider);
    }
}
#endif

var environmentalSettingHelper = app.Services.GetService<IEnvironmentalSettingHelper>()!;
await environmentalSettingHelper.LoadEnvironmentalSettings();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard();

var auth = new[] { new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
{
    RequireSsl = false,
    SslRedirect = false,
    LoginCaseSensitive = true,
    Users = new []
    {
        new BasicAuthAuthorizationUser
        {
            Login = environmentalSettingHelper.TryGetEnviromentalSettingValue(EnvironmentalSettingEnum.HangfireUsername),
            PasswordClear = environmentalSettingHelper.TryGetEnviromentalSettingValue(EnvironmentalSettingEnum.HangfireUsername)
        }
    }
})};

app.MapHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = auth
}, JobStorage.Current);

app.Run();