
using System.Collections.Generic;

namespace Aserto.TodoApp.Domain.Models
{
  public class User
  {
    public string id { get; set; }
    public string display_name { get; set; }
    public string email { get; set; }

    public string picture { get; set; }
  }

  public class UserResponse
  {
    public User result { get; set; }
  }

  public class UserIdentity
  {
    public string id { get; set; }
  }
  public class UserIdentityResponse
  {

    public UserIdentity result { get; set; }
  }
}