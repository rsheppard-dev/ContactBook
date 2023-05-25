using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ContactBook.Models;
using ContactBook.Models.ViewModels;
using ContactBook.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContactBook.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        string appUserId = _userManager.GetUserId(User);

        var favouriteContacts = new List<Contact>();
        var favouriteCategories = new List<Category>();
        var recentContacts = new List<Contact>();
        var recentCategories= new List<Category>();

        if (appUserId != null)
        {
            // get user and their contacts
            AppUser appUser = _context.Users
                .Include(u => u.Contacts)
                .Include(u => u.Categories)
                .FirstOrDefault(u => u.Id == appUserId)!;

            favouriteContacts = appUser.Contacts
                .Where(c => c.Favourite == true)
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToList();

            favouriteCategories = appUser.Categories
                .Where(c => c.Favourite == true)
                .OrderBy(c => c.Name)
                .ToList();

            recentContacts = appUser.Contacts
                .Where(c => c.LastContact != null)
                .OrderByDescending(c => c.LastContact)
                .Take(5)
                .ToList();

            recentCategories = appUser.Categories
                .Where(c => c.LastContact != null)
                .OrderByDescending(c => c.LastContact)
                .Take(5)
                .ToList();
        }

        HomeViewModel model = new HomeViewModel()
        {
            FavouriteContacts = favouriteContacts,
            FavouriteCategories = favouriteCategories,
            RecentContacts = recentContacts,
            RecentCategories = recentCategories
        };

        return View(model);
    }

    [Route("/Home/HandleError/{status:int}")]
    public IActionResult HandleError(int status)
    {
        var customError = new CustomError() {
            Status = status
        };

        if (status == 404)
        {
            customError.Message = "Sorry, the page you are looking for might have been removed, had its name changed or is temporarily unavailable.";
        }
        else
        {
            customError.Message = "Sorry, something went wrong.";
        }

        return View("~/Views/Shared/CustomError.cshtml", customError);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
