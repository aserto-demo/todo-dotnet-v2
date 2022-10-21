namespace Aserto.TodoApp.Options
{
    public class DirectoryConfig
    {
        public bool Insecure { get; set; } = false;

        public string ServiceUrl { get; set; } = "https://localhost:9292";

        public string TenantID { get; set; } = string.Empty;

        public string APIKey { get; set; } = string.Empty;

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(ServiceUrl))
            {
                return false;
            }

            return true;
        }
    }
}
