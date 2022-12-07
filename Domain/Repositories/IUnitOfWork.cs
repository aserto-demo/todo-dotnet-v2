using System.Threading.Tasks;

namespace Aserto.TodoApp.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}