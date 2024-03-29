using AutoMapper;
using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Domain.Services;
using Aserto.TodoApp.Resources;
using Aserto.TodoApp.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Aserto.AspNetCore.Middleware.Extensions;

namespace Aserto.TodoApp.Controllers
{
  [ApiController]
  [Route("/todos/{id}")]
  public class PutTodoController : ControllerBase
  {
    private readonly ITodoService todoService;
    private readonly IMapper mapper;

    public PutTodoController(ITodoService service, IMapper mapper)
    {
      todoService = service;
      this.mapper = mapper;
    }

    [HttpPut]
    [Aserto]
    public async Task<IActionResult> PutAsync(string id, [FromBody] SaveTodoResource resource)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState.GetErrorMessages());
      }

      var todo = await todoService.GetAsync(id);
      todo.Title = resource.Title;
      todo.Completed = resource.Completed;

      var result = await todoService.UpdateAsync(todo);
      if (!result.Success)
        return BadRequest(result.Message);

      var todoResource = mapper.Map<Todo, TodoResource>(result.Todo);
      return Ok(todoResource);
    }
  }
}
