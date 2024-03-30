using MvcApp1.Models;

namespace MvcApp1.DataAccess.Repository.IRepository;

public interface ICompanyRepository : IRepository<Company>
{
    public void Update(Company company);
}
