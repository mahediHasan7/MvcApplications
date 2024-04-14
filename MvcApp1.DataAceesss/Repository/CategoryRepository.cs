using MahediBookStore.DataAccess.Data;
using MahediBookStore.DataAccess.Repository.IRepository;
using MahediBookStore.Models;
using System.Linq.Expressions;

namespace MahediBookStore.DataAccess.Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly ApplicationDbContext _db;

    // passing the db to the base class / parent class (Repository class)
    public CategoryRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Category category)
    {
        _db.Categories.Update(category);
    }
}
