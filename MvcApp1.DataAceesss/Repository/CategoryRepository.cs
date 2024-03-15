using MvcApp1.DataAccess.Data;
using MvcApp1.DataAccess.Repository.IRepository;
using MvcApp1.Models;
using System.Linq.Expressions;

namespace MvcApp1.DataAccess.Repository;

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
