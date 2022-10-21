using Aserto.TodoApp.Domain.Models;

namespace Aserto.TodoApp.Domain.Services.Communication
{
  public class GetUserResponse : BaseResponse
  {
    public User User { get; private set; }

    private GetUserResponse(bool success, string message, User user) : base(success, message)
    {
      User = user;
    }

    /// <summary>
    /// Creates a success response.
    /// </summary>
    /// <param name="user">Got user.</param>
    /// <returns>Response.</returns>
    public GetUserResponse(bool success, User user) : this(true, string.Empty, user)
    { }

    /// <summary>
    /// Creates am error response.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <returns>Response.</returns>
    public GetUserResponse(string message) : this(false, message, null)
    { }
  }
}