using Aserto.TodoApp.Domain.Models;

namespace Aserto.TodoApp.Domain.Services.Communication
{
  public class SaveTodoResponse : BaseResponse
  {
    public Todo Todo { get; private set; }

    private SaveTodoResponse(bool success, string message, Todo todo) : base(success, message)
    {
      Todo = todo;
    }

    /// <summary>
    /// Creates a success response.
    /// </summary>
    /// <param name="todo">Saved todo.</param>
    /// <returns>Response.</returns>
    public SaveTodoResponse(Todo todo) : this(true, string.Empty, todo)
    { }

    /// <summary>
    /// Creates am error response.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <returns>Response.</returns>
    public SaveTodoResponse(string message) : this(false, message, null)
    { }
  }
}