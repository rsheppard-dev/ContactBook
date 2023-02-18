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

    public IActionResult Privacy()
    {
        return View();
    }

    [Route("/Home/HandleError/{code:int}")]
    public IActionResult HandleError(int code)
    {
        var customError = new CustomError();

        customError.Code = code;

        if (code == 404)
        {
            customError.Message = "The page you are looking for might have been removed, had its name changed or is temporarily unavailable.";
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