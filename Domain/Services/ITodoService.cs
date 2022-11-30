using System.Collections.Generic;
using System.Threading.Tasks;
using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Domain.Services.Communication;
namespace Aserto.TodoApp.Domain.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<Todo>> ListAsync();
        Task<Todo> GetAsync(string id);
        Task<SaveTodoResponse> InsertAsync(Todo todo);
        Task<SaveTodoResponse> UpdateAsync(Todo todo);
        Task<DeleteTodoResponse> DeleteAsync(string id);
    }
}
