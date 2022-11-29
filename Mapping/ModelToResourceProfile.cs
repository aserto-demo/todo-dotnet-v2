using AutoMapper;
using Aserto.TodoApp.Domain.Models;
using Aserto.TodoApp.Resources;

namespace Aserto.TodoApp.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<Todo, TodoResource>();
        }
    }
}