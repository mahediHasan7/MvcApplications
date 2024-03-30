using MvcApp1.DataAccess.Data;
using MvcApp1.DataAccess.Repository.IRepository;

namespace MvcApp1.DataAccess.Repository;

public class UnitOfWork : IUnitOfWorks
{
    // ApplicationDbContext is a dependency of UnitOfWork. Instead of creating a new instance of ApplicationDbContext inside UnitOfWork, which would tightly couple these two classes, UnitOfWork declares that it needs an ApplicationDbContext and allows an external entity (the dependency injection container in ASP.NET Core) to provide that dependency.
    private readonly ApplicationDbContext _context;

    public ICategoryRepository CategoryRepository { get; private set; }
    public IProductRepository ProductRepository { get; private set; }
    public ICompanyRepository CompanyRepository { get; private set; }
    public IShoppingCartRepository ShoppingCartRepository { get; private set; }
    public IApplicationUserRepository ApplicationUserRepository { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        CategoryRepository = new CategoryRepository(_context);
        ProductRepository = new ProductRepository(_context);
        CompanyRepository = new CompanyRepository(_context);
        ShoppingCartRepository = new ShoppingCartRepository(_context);
        ApplicationUserRepository = new ApplicationUserRepository(_context);
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}
