
using System.Collections.Generic;

namespace Aserto.TodoApp.Domain.Models
{
  public class Todo
  {
    public string ID { get; set; }
    public string Title { get; set; }
    public bool Completed { get; set; }
    public string OwnerID { get; set; }
  }
}