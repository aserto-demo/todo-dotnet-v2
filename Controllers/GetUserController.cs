using AutoMapper;
using Aserto.TodoApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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
      var result = await userService.Get(userID);
      return Ok(result.User);
    }
  }
}
