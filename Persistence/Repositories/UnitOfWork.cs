using System.Threading.Tasks;
using Aserto.TodoApp.Domain.Repositories;
using Aserto.TodoApp.Persistence.Contexts;

namespace Aserto.TodoApp.Persistence.Repositories
{
  public class UnitOfWork : IUnitOfWork
  {
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
      _context = context;
    }

    public async Task CompleteAsync()
    {
      await _context.SaveChangesAsync();
    }
  }
}