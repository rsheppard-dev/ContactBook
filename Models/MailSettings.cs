namespace ContactBook.Models
{
    public class MailSettings
    {
        public string? SendGridApiKey { get; set; }
        public string? Email { get; set; }
        public string? MailPassword { get; set; }
        public string? DisplayName { get; set; }
        public string? MailHost { get; set; }
        public int MailPort { get; set; }
    }
}