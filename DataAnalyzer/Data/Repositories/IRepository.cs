using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAnalyzer.Data.Repositories;

public interface IRepository<TEntity>
{
    Task<TEntity[]> GetAsync(int skip, int take);

    Task<int> AddAsync(TEntity item);

    Task<int> UpdateAsync(TEntity item);

    IAsyncEnumerable<TEntity> AsLazyAsyncEnumerable();

    Task<int> TotalCount();
}