
using Microsoft.EntityFrameworkCore;
using MahediBookStore.DataAccess.Data;
using MahediBookStore.DataAccess.Repository.IRepository;
using System.Linq.Expressions;

namespace MahediBookStore.DataAccess.Repository;


// It specifies that T must be a reference type, not a value type. In other words, T must be a class, not a struct or an enum.
public class Repository<T> : IRepository<T> where T : class
{

    // Dependency Injection
    private readonly ApplicationDbContext _db;

    // DbSet represents collection of Entities that are retrieved from database
    internal DbSet<T> dbSet;

    public Repository(ApplicationDbContext db)
    {
        _db = db;

        // Set<T> method returns DbSet<T> - a collection of entities of given Type (T)
        dbSet = _db.Set<T>();
    }


    public void Add(T entity)
    {
        dbSet.Add(entity);
    }

    public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracker = false)
    {
        IQueryable<T> query;
        if (tracker == false)
        {
            query = dbSet.AsNoTracking();
        }
        else
        {
            query = dbSet;
        }


        // deferred execution: The actual database query doesn't happen until you enumerate the IQueryable<T> (for example, by calling ToList, ToArray, FirstOrDefault, etc.). At that point, Entity Framework translates the IQueryable<T> into a SQL query, sends it to the database, and materializes the results into .NET objects. This is known as "deferred execution".
        query = query.Where(filter);

        // iterate through includeProp and add them with the query
        if (!string.IsNullOrEmpty(includeProperties))
        {
            string[] props = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string prop in props)
            {
                query = query.Include(prop);
            }

        }

        // this will send the database query
        return query.FirstOrDefault();

    }

    public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter, string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        // iterate through includeProp and add them with the query
        if (!string.IsNullOrEmpty(includeProperties))
        {
            string[] props = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string prop in props)
            {
                query = query.Include(prop);
            }

        }
        return query.ToList();
    }

    public void Remove(T entity)
    {
        dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entity)
    {
        dbSet.RemoveRange(entity);
    }
}
