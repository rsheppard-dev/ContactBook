using ContactBook.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactBook.Helpers
{
    public static class DataHelper
    {
        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();

            await dbContextSvc.Database.MigrateAsync();
        }
    }
}