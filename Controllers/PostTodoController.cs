using AutoMapper;
using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Domain.Services;
using Aserto.TodoApp.Resources;
using Aserto.TodoApp.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Aserto.TodoApp.Controllers
{
    [ApiController]
    [Route("/todos")]
    public class PostTodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly IMapper _mapper;

        public PostTodoController(ITodoService todoService, IMapper mapper)
        {
            _todoService = todoService;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize("Aserto")]
        public async Task<IActionResult> PostAsync([FromBody] SaveTodoResource resource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            var todo = new Todo();
            todo.Title = resource.Title;
            todo.Completed = resource.Completed;

            var authorizationHeader = HttpContext.Request.Headers.Authorization;
            var userIdentitiesEnumerator = HttpContext.User.Identities.GetEnumerator();


            while (userIdentitiesEnumerator.MoveNext())
            {
                todo.OwnerID = GetNameIdentifierValue(userIdentitiesEnumerator.Current);
            }

            var result = await _todoService.SaveAsync(todo);

            if (!result.Success)
                return BadRequest(result.Message);

            var todoResource = _mapper.Map<Todo, TodoResource>(result.Todo);
            return Ok(todoResource);
        }

        private static string GetNameIdentifierValue(ClaimsIdentity claimsIdentity)
        {
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            return claim;
        }
    }
}
