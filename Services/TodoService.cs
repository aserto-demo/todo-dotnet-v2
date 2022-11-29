using System.Collections.Generic;
using System.Threading.Tasks;
using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Domain.Services;
using Aserto.TodoApp.Domain.Repositories;
using Aserto.TodoApp.Domain.Services.Communication;
using System;
using Google.Protobuf.WellKnownTypes;

namespace Aserto.TodoApp.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepository;
        private readonly IUnitOfWork _unitOfWork;


        public TodoService(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
        {
            _todoRepository = todoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Todo>> ListAsync()
        {
            return await _todoRepository.ListAsync();
        }

        public async Task<SaveTodoResponse> SaveAsync(Todo todo)
        {
            try
            {
                await _todoRepository.AddAsync(todo);
                await _unitOfWork.CompleteAsync();

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
            var existingTodo = await _todoRepository.FindByIdAsync(todo.ID);

            if (existingTodo == null)
                return new SaveTodoResponse("Todo not found.");

            existingTodo.ID = todo.ID;
            existingTodo.Completed = todo.Completed;
            existingTodo.Title = todo.Title;
            existingTodo.OwnerID = todo.OwnerID;

            try
            {
                _todoRepository.Update(existingTodo);
                await _unitOfWork.CompleteAsync();

                return new SaveTodoResponse(existingTodo);
            }
            catch (Exception ex)
            {
                return new SaveTodoResponse($"An error occurred when updating the todo: {ex.Message}");
            }
        }

        public async Task<DeleteTodoResponse> DeleteAsync(Todo todo)
        {
            try
            {
                _todoRepository.Delete(todo);
                await _unitOfWork.CompleteAsync();

                return new DeleteTodoResponse(true);
            }
            catch (Exception ex)
            {
                return new DeleteTodoResponse($"An error occurred when deleting the todo: {ex.Message}");
            }
        }

        public static Struct GetResource()
        {
            return null;
        }
    }
}