using MahediBookStore.DataAccess.Data;
using MahediBookStore.DataAccess.Repository.IRepository;
using MahediBookStore.Models;

namespace MahediBookStore.DataAccess.Repository;

internal class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicationUserRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
