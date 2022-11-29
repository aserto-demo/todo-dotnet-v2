using AutoMapper;
using Aserto.TodoApp.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace Aserto.TodoApp.Controllers
{
  [ApiController]
  [Route("/users/{userID}")]
  public class GetUserUserIDController : ControllerBase
  {
    private readonly IUserService _userService;
    private readonly IMapper _mapper;


    public GetUserUserIDController(IUserService userService, IMapper mapper)
    {
      _userService = userService;
      _mapper = mapper;
    }

    [HttpGet]
    [Authorize("Aserto")]
    public async Task<IActionResult> GetAllAsync(string userID)
    {
      var result = await _userService.Get(userID);
      return Ok(result.User);
    }
  }
}
