using Aserto.TodoApp.Domain.Models;

namespace Aserto.TodoApp.Domain.Services.Communication
{
  public class DeleteTodoResponse : BaseResponse
  {
    public Todo Todo { get; private set; }

    private DeleteTodoResponse(bool success, string message) : base(success, message)
    { }

    /// <summary>
    /// Creates a success response.
    /// </summary>
    /// <param name="todo">Deleted todo.</param>
    /// <returns>Response.</returns>
    public DeleteTodoResponse(bool success) : this(true, string.Empty)
    { }

    /// <summary>
    /// Creates am error response.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <returns>Response.</returns>
    public DeleteTodoResponse(string message) : this(false, message)
    { }
  }
}