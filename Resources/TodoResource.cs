using System.Text.Json.Serialization;
namespace Aserto.TodoApp.Resources
{
  public class TodoResource
  {
    [JsonPropertyName("ID")]
    public string ID { get; set; }
    [JsonPropertyName("Title")]
    public string Title { get; set; }
    [JsonPropertyName("Completed")]
    public bool Completed { get; set; }
    [JsonPropertyName("OwnerID")]
    public string OwnerID { get; set; }
  }
}