using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PROG7311_POE.Models;

namespace PROG7311_POE.Controllers
{
    public class HomeController : Controller
    {
        // shows main home page
        public IActionResult Index()
        {
            return View();
        }

        // shows privacy policy
        public IActionResult Privacy()
        {
            return View();
        }

        // shows errors, uses error view model
        public IActionResult Error()
        {
            return View();
        }
    }
}
