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
            var readProducts = await _readDb.Products.AsNoTracking().ToListAsync(token);

            var readDict = readProducts.ToDictionary(p => p.Id);



            foreach (var wp in writeProducts)
            {
                if (readDict.TryGetValue(wp.Id, out var existing))
                {
                    existing.Name = wp.Name;
                    existing.Description = wp.Description;
                    existing.Price = wp.Price;
                    existing.Stock = wp.Stock;
                    existing.CreatedAt = wp.CreatedAt ?? TimeSpan.MinValue;
                    existing.UpdatedAt = wp.UpdatedAt ?? TimeSpan.MinValue;
                    existing.IsActive = wp.IsActive;

                    _readDb.Products.Update(existing);
                }
                else
                {
                    _readDb.Products.Add(new Product
                    {
                        Id = wp.Id,
                        Name = wp.Name,
                        Description = wp.Description,
                        Price = wp.Price,
                        Stock = wp.Stock,
                        CreatedAt = wp.CreatedAt ?? TimeSpan.MinValue,
                        UpdatedAt = wp.UpdatedAt ?? TimeSpan.MinValue,
                        IsActive = wp.IsActive
                    });
                }
            }

            var writeIds = writeProducts.Select(p => p.Id).ToHashSet();
            var toRemove = readProducts.Where(p => !writeIds.Contains(p.Id)).ToList();
            _readDb.Products.RemoveRange(toRemove);

            await _readDb.SaveChangesAsync(token);
        }
    }
}
