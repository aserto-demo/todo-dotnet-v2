
using AutoMapper;
using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Resources;

namespace Aserto.TodoApp.Mapping
{
  public class ResourceToModelProfile : Profile
  {
    public ResourceToModelProfile()
    {
      CreateMap<SaveTodoResource, Todo>();
    }
  }
}