using AutoMapper;
using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Domain.Services;
using Aserto.TodoApp.Resources;
using Aserto.TodoApp.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Aserto.AspNetCore.Middleware.Extensions;

namespace Aserto.TodoApp.Controllers
{
  [ApiController]
  [Route("/todos/{id}")]
  public class DeleteTodoController : ControllerBase
  {
    private readonly ITodoService todoService;
    private readonly IMapper mapper;

    public DeleteTodoController(ITodoService todoService, IMapper mapper)
    {
      this.todoService = todoService;
      this.mapper = mapper;
    }

    [HttpDelete]
    [Authorize("Aserto")]
    [Check("admin")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState.GetErrorMessages());

      var result = await todoService.DeleteAsync(id);

      if (!result.Success)
        return BadRequest(result.Message);

      var todoResource = mapper.Map<Todo, TodoResource>(result.Todo);
      return Ok(true);
    }
  }
}
