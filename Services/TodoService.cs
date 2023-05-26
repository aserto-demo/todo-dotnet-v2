using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;

using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Domain.Services;
using Aserto.TodoApp.Domain.Services.Communication;
using Aserto.TodoApp.Persistence.Contexts;

namespace Aserto.TodoApp.Services
{
    public class TodoService : ITodoService
    {
        private readonly AppDbContext db;

        public TodoService(AppDbContext dbContext)
        {
            db = dbContext;
        }

        public async Task<IEnumerable<Todo>> ListAsync()
        {
            return await db.Todos.ToListAsync();
        }

        public async Task<Todo> GetAsync(string id)
        {
            return await db.Todos.FindAsync(id);
        }

        public async Task<SaveTodoResponse> InsertAsync(Todo todo)
        {
            try
            {
                await db.Todos.AddAsync(todo);
                await db.SaveChangesAsync();

                return new SaveTodoResponse(todo);
            }
            catch (Exception ex)
            {
                // Do some logging stuff
                return new SaveTodoResponse($"An error occurred when saving the todo: {ex.Message}");
            }
        }

        public async Task<SaveTodoResponse> UpdateAsync(Todo todo)
        {
            var existingTodo = await db.Todos.FindAsync(todo.ID);

            if (existingTodo == null)
            {
                return new SaveTodoResponse("Todo not found.");
            }

            existingTodo.ID = todo.ID;
            existingTodo.Completed = todo.Completed;
            existingTodo.Title = todo.Title;
            existingTodo.OwnerID = todo.OwnerID;

            try
            {
                db.Todos.Update(existingTodo);
                await db.SaveChangesAsync();

                return new SaveTodoResponse(existingTodo);
            }
            catch (Exception ex)
            {
                return new SaveTodoResponse($"An error occurred when updating the todo: {ex.Message}");
            }
        }

        public async Task<DeleteTodoResponse> DeleteAsync(string id)
        {
            try
            {
                var todo = db.Todos.Find(id);
                db.Todos.Remove(todo);
                await db.SaveChangesAsync();

                return new DeleteTodoResponse(true);
            }
            catch (Exception ex)
            {
                return new DeleteTodoResponse($"An error occurred when deleting the todo: {ex.Message}");
            }
        }
    }
}
