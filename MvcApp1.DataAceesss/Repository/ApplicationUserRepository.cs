using MvcApp1.DataAccess.Data;
using MvcApp1.DataAccess.Repository.IRepository;
using MvcApp1.Models;

namespace MvcApp1.DataAccess.Repository;

internal class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicationUserRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
