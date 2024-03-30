namespace MvcApp1.DataAccess.Repository.IRepository;

public interface IUnitOfWorks
{
    //  bellow one is a field declaration, not a property. we can not use this for Interface
    //  public readonly ICategoryRepository categoryRepository; 

    // Instead we need to use a property Instead
    ICategoryRepository CategoryRepository { get; }
    IProductRepository ProductRepository { get; }
    ICompanyRepository CompanyRepository { get; }
    IShoppingCartRepository ShoppingCartRepository { get; }
    IApplicationUserRepository ApplicationUserRepository { get; }
    IOrderHeaderRepository OrderHeaderRepository { get; }
    IOrderDetailRepository OrderDetailRepository { get; }

    public void Save();
}
