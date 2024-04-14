using MahediBookStore.Models;

namespace MahediBookStore.DataAccess.Repository.IRepository;

public interface ICategoryRepository : IRepository<Category>
{
    void Update(Category category);
}
