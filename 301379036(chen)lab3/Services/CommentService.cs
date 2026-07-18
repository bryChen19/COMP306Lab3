using Microsoft.AspNetCore.Mvc;

namespace _301379036_chen_lab3.Services
{
    public class CommentService : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
