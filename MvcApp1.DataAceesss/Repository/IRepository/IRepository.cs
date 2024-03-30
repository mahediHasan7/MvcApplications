using System.Linq.Expressions;

namespace MvcApp1.DataAccess.Repository.IRepository;


// It specifies that T must be a reference type, not a value type. In other words, T must be a class, not a struct or an enum.
public interface IRepository<T> where T : class
{
    // T - Category
    IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, string? includeProperties = null);

    // this is a delegate that points to a method which takes a parameter of type T and returns a bool.
    //T Get(Func<T,bool> filter);


    // this is an expression tree. Its not executed immediately rather its convert into a query syntax for Entity Framework.
    T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracker = false);

    void Add(T entity);

    void Remove(T entity);

    void RemoveRange(IEnumerable<T> entity);
}
