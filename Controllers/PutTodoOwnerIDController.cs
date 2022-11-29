using AutoMapper;
using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Domain.Services;
using Aserto.TodoApp.Resources;
using Aserto.TodoApp.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aserto.TodoApp.Controllers
{
  [ApiController]
  [Route("/todo/{id}")]
  public class PutTodoOwnerIDController : ControllerBase
  {
    private readonly ITodoService _todoService;
    private readonly IMapper _mapper;

    public PutTodoOwnerIDController(ITodoService todoService, IMapper mapper)
    {
      _todoService = todoService;
      _mapper = mapper;
    }

    [HttpPut]
    [Authorize("Aserto")]
    public async Task<IActionResult> PutAsync([FromBody] SaveTodoResource resource)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState.GetErrorMessages());

      var todo = _mapper.Map<SaveTodoResource, Todo>(resource);
      var result = await _todoService.UpdateAsync(todo);

      if (!result.Success)
        return BadRequest(result.Message);

      var todoResource = _mapper.Map<Todo, TodoResource>(result.Todo);
      return Ok(todoResource);
    }
  }
}
