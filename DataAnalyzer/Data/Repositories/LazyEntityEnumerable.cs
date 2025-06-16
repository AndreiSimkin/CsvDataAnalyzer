using System.Collections.Generic;
using System.Threading;
using SQLite;

namespace DataAnalyzer.Data.Repositories;

public class LazyEntityEnumerable<TEntity>(SQLiteAsyncConnection db, int batchSize = 100) : IAsyncEnumerable<TEntity>
    where TEntity : new()
{
    public async IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = new())
    {
        var offset = 0;
        while (true)
        {
            var batch = await db.Table<TEntity>()
                .Skip(offset)
                .Take(batchSize)
                .ToListAsync();

            if (batch.Count == 0) yield break;

            foreach (var item in batch)
                yield return item;

            offset += batchSize;
        }
    }
}