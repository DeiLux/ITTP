using ITTP.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace ITTP.Server
{
    public static class MigrationManager
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using var appContext = scope.ServiceProvider.GetRequiredService<NpgSqlContext>();
                appContext.Database.Migrate();
            }

            return host;
        }
    }
}
