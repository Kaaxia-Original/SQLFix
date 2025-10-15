using Microsoft.EntityFrameworkCore;
using Quartz;
using SQLFix.Data;
using SQLFix.Entities;

namespace SQLFix.Jobs
{
    [DisallowConcurrentExecution]
    public class DataSyncJob : IJob
    {
        private readonly WriteDbContext _writeDb;
        private readonly ReadDbContext _readDb;

        public DataSyncJob(WriteDbContext writeDb, ReadDbContext readDb)
        {
            _writeDb = writeDb;
            _readDb = readDb;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var token = context.CancellationToken;

            var writeProducts = await _writeDb.Products.AsNoTracking().ToListAsync(token);

            await _readDb.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Products\" RESTART IDENTITY CASCADE;", token);

            _readDb.Products.AddRange(writeProducts);

            await _readDb.SaveChangesAsync(token);
        }
    }
}
