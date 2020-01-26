using Microsoft.AspNetCore.Mvc;

namespace WebPlugin1
{
    public class WebPluginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}