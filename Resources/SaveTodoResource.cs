using System.ComponentModel.DataAnnotations;

namespace Aserto.TodoApp.Resources
{
    public class SaveTodoResource
    {
        [Required]
        public string Title { get; set; }
        public bool Completed { get; set; }
    }
}