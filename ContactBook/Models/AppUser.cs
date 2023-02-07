using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ContactBook.Models;

public class AppUser: IdentityUser
{
    [Required]
    [Display(Name = "First Name")]
    [StringLength(50, ErrorMessage = "{0} must be between {2} and {1} characters long!", MinimumLength = 2)]
    public string? FirstName { get; set; }
    
    [Required]
    [Display(Name = "Last Name")]
    [StringLength(50, ErrorMessage = "{0} must be between {2} and {1} characters long!", MinimumLength = 2)]
    public string? LastName { get; set; }
    
    [NotMapped]
    public string? FullName
    {
        get { return $"{FirstName} {LastName}"; }
    }
}