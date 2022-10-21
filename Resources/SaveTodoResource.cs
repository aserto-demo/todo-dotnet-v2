using System.ComponentModel.DataAnnotations;

namespace Aserto.TodoApp.Resources
{
  public class SaveTodoResource
  {
    public string ID { get; set; }
    [Required]
    public string Title { get; set; }
    public string OwnerID { get; set; }
    public bool Completed { get; set; }
  }
}