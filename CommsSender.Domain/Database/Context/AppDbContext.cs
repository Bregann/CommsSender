using CommsSender.Domain.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CommsSender.Domain.Database.Context
{
    public partial class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<EnvironmentalSetting> EnvironmentalSettings { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<PushToken> PushTokens { get; set; } = null!;
        public DbSet<MessageErrorLog> MessageErrorLogs { get; set; } = null!;
    }
}
