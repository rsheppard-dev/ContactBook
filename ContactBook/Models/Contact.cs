using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactBook.Models;

public class Contact
{
    public int Id { get; set; }
    
    [Required]
    public string? AppUserID { get; set; }
    
    [Required]
    [Display(Name = "First Name")]
    [StringLength(50, ErrorMessage = "The {0) must be between {2} and {1} characters long.", MinimumLength = 2)]
    public string? FirstName { get; set; }
    
    [Required]
    [Display(Name = "Last Name")]
    [StringLength(50, ErrorMessage = "The {0) must be between {2} and {1} characters long.", MinimumLength = 2)]
    public string? LastName { get; set; }
    
    [NotMapped]
    public string? FullName
    {
        get { return $"{FirstName} {LastName}"; }
    }
    
    [Display(Name = "Birthday")]
    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }
    
    [Required]
    public string? Address1 { get; set; }
    
    public string? Address2 { get; set; }
    
    [Required]
    public string? City { get; set; }
    
    [Required]
    public string? County { get; set; }
    
    [Required]
    [Display(Name = "Post Code")]
    [DataType(DataType.PostalCode)]
    public string? PostCode { get; set; }
    
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    
    [Required]
    [Display(Name = "Phone Number")]
    public string? Phone { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime Created { get; set; }
    
    // image properties
    public byte[]? ImageData { get; set; }
    public string? ImageType { get; set; }
    
    [NotMapped]
    public IFormFile? ImageFile { get; set; }
    
    // virtuals
    public virtual AppUser? AppUser { get; set; }
    public virtual ICollection<Category> Catorgories { get; set; } = new HashSet<Category>();
}