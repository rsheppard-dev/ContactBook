using System.ComponentModel.DataAnnotations;

namespace ContactBook.Models
{
    public class EmailData
    {
        [Required]
        [Display(Name = "To")]
        public string EmailAddress { get; set; } = "";
        
        [Required]
        public string Subject { get; set; } = "";

        [Required]
        public string Body { get; set; } = "";

        public int? Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? GroupName { get; set; }
    }
}