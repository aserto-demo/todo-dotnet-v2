using AutoMapper;
using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Domain.Services;
using Aserto.TodoApp.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Aserto.TodoApp.Controllers
{
  [ApiController]
  [Route("/todos")]
  public class GetTodosController : ControllerBase
  {
    private readonly ITodoService _todoService;
    private readonly IMapper _mapper;

    public GetTodosController(ITodoService todoService, IMapper mapper)
    {
      _todoService = todoService;
      _mapper = mapper;
    }

    [HttpGet]
    [Authorize("Aserto")]

    public async Task<IEnumerable<TodoResource>> GetAllAsync()
    {
      var todos = await _todoService.ListAsync();
      var resources = _mapper.Map<IEnumerable<Todo>, IEnumerable<TodoResource>>(todos);
      return resources;
    }
  }
}
