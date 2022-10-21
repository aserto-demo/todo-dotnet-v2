using Aserto.TodoApp.Domain.Models;

namespace Aserto.TodoApp.Domain.Services.Communication
{
  public class GetUserIdentityResponse : BaseResponse
  {
    public string UserId { get; private set; }

    private GetUserIdentityResponse(bool success, string message, string userId) : base(success, message)
    {
      UserId = userId;
    }

    /// <summary>
    /// Creates a success response.
    /// </summary>
    /// <param name="user">Got user.</param>
    /// <returns>Response.</returns>
    public GetUserIdentityResponse(bool success, string userId) : this(true, string.Empty, userId)
    { }

    /// <summary>
    /// Creates am error response.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <returns>Response.</returns>
    public GetUserIdentityResponse(string message) : this(false, message, null)
    { }
  }
}