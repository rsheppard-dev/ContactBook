using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ContactBook.Models;

namespace ContactBook.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
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
