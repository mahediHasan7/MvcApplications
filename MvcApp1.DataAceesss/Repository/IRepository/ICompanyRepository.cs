using MahediBookStore.Models;

namespace MahediBookStore.DataAccess.Repository.IRepository;

public interface ICompanyRepository : IRepository<Company>
{
    public void Update(Company company);
}
