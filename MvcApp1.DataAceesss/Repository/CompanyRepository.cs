using MahediBookStore.DataAccess.Data;
using MahediBookStore.DataAccess.Repository.IRepository;
using MahediBookStore.Models;

namespace MahediBookStore.DataAccess.Repository;

internal class CompanyRepository : Repository<Company>, ICompanyRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Company company)
    {
        _context.Companies.Update(company);
    }
}
