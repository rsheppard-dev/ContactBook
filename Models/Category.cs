using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [DefaultValue(false)]
        public bool Favourite { get; set; }

        [DataType(DataType.Date)]
        public DateTime LastContact { get; set; }

        // image properties
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; } 

        // virtuals
        [NotMapped]
        [Display(Name = "Category Image")]
        public IFormFile? ImageFile { get; set; }

        public virtual AppUser? AppUser { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; } = new HashSet<Contact>();
    }
}