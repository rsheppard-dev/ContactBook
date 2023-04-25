using System.ComponentModel.DataAnnotations;

namespace ContactBook.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string? AppUserId { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public string? Name { get; set; }

        // virtuals
        public virtual AppUser? AppUser { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; } = new HashSet<Contact>();
    }
}