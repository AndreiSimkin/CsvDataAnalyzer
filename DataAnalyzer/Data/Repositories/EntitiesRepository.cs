using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace DataAnalyzer.Data.Repositories;

public class EntitiesRepository<TEntity> : IRepository<TEntity> where TEntity : new()
{
    private readonly SQLiteAsyncConnection _db;

    
    public EntitiesRepository(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);
        _db.CreateTableAsync<TEntity>().Wait();
    }
    
    /// <summary>
    /// Получение записей из БД.
    /// </summary>
    public Task<TEntity[]> GetAsync(int skip, int take)
    {
        return _db.Table<TEntity>()
            .Skip(skip)
            .Take(take)
            .ToArrayAsync();
    }
    
    
    /// <summary>
    /// Добавление записи в БД.
    /// </summary>
    public Task<int> AddAsync(TEntity item)
    {
        return _db.InsertAsync(item);
    }
    
    /// <summary>
    /// Обновление записи в БД.
    /// </summary>
    public Task<int> UpdateAsync(TEntity item)
    {
        return _db.UpdateAsync(item);
    }
    
    
    public IAsyncEnumerable<TEntity> AsLazyAsyncEnumerable()
    {
        return new LazyEntityEnumerable<TEntity>(_db);
    }
    

    public Task<int> TotalCount()
    {
        return _db.Table<TEntity>().CountAsync();
    }
}