using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Domain.Services;
using Aserto.TodoApp.Domain.Services.Communication;
using Aserto.TodoApp.Persistence.Contexts;
using Aserto.TodoApp.Options;
using Aserto.Clients.Directory.V3;
using Aserto.AspNetCore.Middleware.Options;
using Aserto.Clients.Options;

namespace Aserto.TodoApp.Services
{
    public class TodoService : ITodoService
    {
        private readonly AppDbContext db;
        private readonly Aserto.Clients.Directory.V3.Directory directoryClient;
        private readonly DirectoryConfig opts;
        public TodoService(IOptions<DirectoryConfig> config, AppDbContext dbContext)
        {
            db = dbContext;
            this.opts = config.Value;
            if (!opts.IsValid())
            {
                throw new Exception("Invalid config. Service url can not be empty");
            }

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Debug)
                .AddConsole();
            });

            var options = new AsertoDirectoryOptions(opts.ServiceUrl, opts.APIKey, opts.TenantID,insecure: opts.Insecure);
            directoryClient = new Aserto.Clients.Directory.V3.Directory(options, loggerFactory);
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

                await directoryClient.SetObjectAsync("resource", todo.ID, todo.Title);
                await directoryClient.SetRelationAsync("resource", todo.ID, "owner", "user", todo.OwnerID, "");

                return new SaveTodoResponse(todo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException Caught insert todo!");
                Console.WriteLine("Message :{0} ", ex.Message);
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

                await directoryClient.DeleteObjectAsync("resource", id, true);

                return new DeleteTodoResponse(true);
            }
            catch (Exception ex)
            {
                return new DeleteTodoResponse($"An error occurred when deleting the todo: {ex.Message}");
            }
        }
    }
}
