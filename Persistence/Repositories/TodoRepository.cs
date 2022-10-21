using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Domain.Repositories;
using Aserto.TodoApp.Persistence.Contexts;

namespace Aserto.TodoApp.Persistence.Repositories
{
  public class TodoRepository : BaseRepository, ITodoRepository
  {
    public TodoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Todo>> ListAsync()
    {
      return await _context.Todos.ToListAsync();
    }

    public async Task AddAsync(Todo todo)
    {
      await _context.Todos.AddAsync(todo);
    }

    public async Task<Todo> FindByIdAsync(string id)
    {
      return await _context.Todos.FindAsync(id);
    }

    public void Update(Todo todo)
    {
      _context.Todos.Update(todo);
    }

    public void Delete(Todo todo)
    {
      _context.Todos.Remove(todo);
    }
  }
}