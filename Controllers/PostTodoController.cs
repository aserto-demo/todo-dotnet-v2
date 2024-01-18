using System;
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
using Aserto.AspNetCore.Middleware.Extensions;

namespace Aserto.TodoApp.Controllers
{
    [ApiController]
    [Route("/todos")]
    public class PostTodoController : ControllerBase
    {
        private readonly ITodoService todoService;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public PostTodoController(ITodoService todoService, IUserService userService, IMapper mapper)
        {
            this.todoService = todoService;
            this.userService = userService;
            this.mapper = mapper;
        }

        [HttpPost]        
        [Check("member")]
        public async Task<IActionResult> PostAsync([FromBody] SaveTodoResource resource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            var pid = GetCallerPID();
            if (pid == "")
                return Unauthorized();

            var userResponse = await userService.Get(pid);

            var todo = new Todo();
            todo.ID = Guid.NewGuid().ToString();
            todo.OwnerID = userResponse.User.id;
            todo.Title = resource.Title;
            todo.Completed = resource.Completed;

            var result = await todoService.InsertAsync(todo);

            if (!result.Success)
                return BadRequest(result.Message);

            var todoResource = mapper.Map<Todo, TodoResource>(result.Todo);
            return Ok(todoResource);
        }

        private static string GetNameIdentifierValue(ClaimsIdentity claimsIdentity)
        {
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            return claim;
        }

        private string GetCallerPID() {
            var userIdentitiesEnumerator = HttpContext.User.Identities.GetEnumerator();
            while (userIdentitiesEnumerator.MoveNext())
            {
                return GetNameIdentifierValue(userIdentitiesEnumerator.Current);
            }

            return "";
        }
    }
}
