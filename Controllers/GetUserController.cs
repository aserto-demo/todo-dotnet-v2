using System.IdentityModel.Tokens.Jwt;
using Aserto.TodoApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Aserto.TodoApp.Domain.Services.Communication;

namespace Aserto.TodoApp.Controllers
{
  [ApiController]
  [Route("/users/{userID}")]
  public class GetUserController : ControllerBase
  {
    private readonly IUserService userService;

    public GetUserController(IUserService userService)
    {
      this.userService = userService;
    }

    [HttpGet]
    [Authorize("Aserto")]
    public async Task<IActionResult> GetUserAsync(string userID)
    {
      var auth = Request.Headers["Authorization"].ToString().Split(' ');
      string subject = "";

      if (auth.Length > 1)
      {
        var handler = new JwtSecurityTokenHandler();
        subject = handler.ReadJwtToken(auth[1]).Subject;
      }

      GetUserResponse userResponse;
      if (userID == subject)
      {
        userResponse = await userService.Get(userID);
      }
      else
      {
        userResponse =await userService.GetByUserId(userID);
      }
      
      return Ok(userResponse.User);
    }
  }
}
