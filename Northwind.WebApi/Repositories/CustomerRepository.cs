using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Memory;
using Northwind.EntityModels;

namespace Northwind.WebApi.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _cacheEntryOptions = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(30)
    };
    private readonly ILogger<CustomerRepository> _logger;

    private NorthwindContext _db;

    public CustomerRepository(
        NorthwindContext db,
        IMemoryCache memoryCache,
        ILogger<CustomerRepository> logger
        )
    {
        _db = db;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<Customer?> CreateAsync(Customer c)
    {
        try
        {

            c.CustomerId = c.CustomerId.ToUpper();

            EntityEntry<Customer> added = await _db.Customers.AddAsync(c);

            int affected = await _db.SaveChangesAsync();

            if (affected == 1)
            {
                // if stored in db then add to cache
                _memoryCache.Set(c.CustomerId, c, _cacheEntryOptions);
                return c;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error trying to persist to db: {ex}");
            return null;
        }
    }

    public async Task<bool?> DeleteAsync(string id)
    {
        id = id.ToUpper();
        Customer? c = await _db.Customers.FindAsync(id);
        if (c is null) return null;
        _db.Customers.Remove(c);
        int affected = await _db.SaveChangesAsync();
        if (affected == 1)
        {
            _memoryCache.Remove(c.CustomerId);
            return true;
        }
        return null;
    }

    public Task<Customer[]> RetrieveAllAsync()
    {
        return _db.Customers.ToArrayAsync();
    }

    public Task<Customer?> RetrieveAsync(string id)
    {
        id = id.ToUpper();

        // search cache first
        if (_memoryCache.TryGetValue(id, out Customer? fromCache))
            return Task.FromResult(fromCache);

        Customer? fromDb = _db.Customers.FirstOrDefault(c => c.CustomerId == id);

        // if not in db return null result
        if (fromDb is null) return Task.FromResult(fromDb);

        // if in db store in cache
        _memoryCache.Set(fromDb.CustomerId, fromDb, _cacheEntryOptions);

        return Task.FromResult(fromDb)!;

    }

    public async Task<Customer?> UpdateAsync(Customer c)
    {
        c.CustomerId = c.CustomerId.ToUpper();

        _db.Customers.Update(c);
        int affected = await _db.SaveChangesAsync();
        if (affected == 1)
        {
            _memoryCache.Set(c.CustomerId, c, _cacheEntryOptions);
            return c;
        }
        return null;
    }
}
