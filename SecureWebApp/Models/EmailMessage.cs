namespace SecureWebApp.Models
{
    public class EmailMessage
    {
        public string To { get; set; }

        public string Subject { get; set; }

        public string HtmlMessage { get; set; }
    }
}