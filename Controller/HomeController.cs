using Microsoft.AspNetCore.Mvc;

namespace Task_System.Controller;
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
