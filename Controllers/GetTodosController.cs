using AutoMapper;
using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Domain.Services;
using Aserto.TodoApp.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aserto.AspNetCore.Middleware.Extensions;
namespace Aserto.TodoApp.Controllers
{
  [ApiController]
  [Route("/todos")]
  public class GetTodosController : ControllerBase
  {
    private readonly ITodoService todoService;
    private readonly IMapper mapper;

    public GetTodosController(ITodoService todoService, IMapper mapper)
    {
      this.todoService = todoService;
      this.mapper = mapper;
    }

    [HttpGet]
    [Aserto]
    public async Task<IEnumerable<TodoResource>> GetAllAsync()
    {
      var todos = await todoService.ListAsync();
      var resources = mapper.Map<IEnumerable<Todo>, IEnumerable<TodoResource>>(todos);
      return resources;
    }
  }
}
