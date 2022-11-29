using System.Collections.Generic;
using System.Threading.Tasks;
using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Domain.Services.Communication;
namespace Aserto.TodoApp.Domain.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<Todo>> ListAsync();
        Task<SaveTodoResponse> SaveAsync(Todo todo);
        Task<SaveTodoResponse> UpdateAsync(Todo todo);
        Task<DeleteTodoResponse> DeleteAsync(Todo todo);
    }
}