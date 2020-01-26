using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NetCorePluginArchitecture.PluginCore;
using NetCorePluginArchitecture.WebHost.Models;
using System.Diagnostics;
using System.Linq;

namespace NetCorePluginArchitecture.WebHost.Controllers
{
    public class HomeController : Controller
    {
        private readonly PluginContext _pluginContext;

        public HomeController(PluginContext pluginContext)
        {
            _pluginContext = pluginContext;
        }

        public IActionResult Index()
        {
            var plugins = _pluginContext.Plugins;
            if (plugins.Any())
            {
                var pluginType = plugins.FirstOrDefault().Assembly.GetTypes()
                         .FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
                var plugin = (IPlugin)ActivatorUtilities.GetServiceOrCreateInstance(HttpContext.RequestServices, pluginType);

                ViewBag.PluginName = plugin.GetPluginName();
            }

            ViewBag.Plugins = plugins;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
