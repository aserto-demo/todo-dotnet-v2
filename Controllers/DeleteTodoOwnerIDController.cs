using AutoMapper;
using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Domain.Services;
using Aserto.TodoApp.Resources;
using Aserto.TodoApp.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Aserto.TodoApp.Controllers
{
  [ApiController]
  [Route("/todos/{id}")]
  public class DeleteTodoOwnerIDController : ControllerBase
  {

    private readonly ITodoService _todoService;
    private readonly IMapper _mapper;

    public DeleteTodoOwnerIDController(ITodoService todoService, IMapper mapper)
    {
      _todoService = todoService;
      _mapper = mapper;
    }

    [HttpDelete]
    [Authorize("Aserto")]
    public async Task<IActionResult> DeleteAsync([FromBody] SaveTodoResource resource)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState.GetErrorMessages());

      var todo = _mapper.Map<SaveTodoResource, Todo>(resource);
      var result = await _todoService.DeleteAsync(todo);

      if (!result.Success)
        return BadRequest(result.Message);

      var todoResource = _mapper.Map<Todo, TodoResource>(result.Todo);
      return Ok(true);
    }
  }
}
