using E_Commerce.Context;
using EllipticCurve;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Database_Initializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ECommerceDbContext _context;
        public DBInitializer(ECommerceDbContext dbContext)
        {
            _context = dbContext;

        }

        public async Task initialise()
        {
            if (_context.Database.GetPendingMigrations().Any())
            {
                await _context.Database.MigrateAsync();
            }
        }
    }
}
