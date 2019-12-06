using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AfyaHMIS.Models;
using Microsoft.AspNetCore.Authorization;
using AfyaHMIS.ViewModel;
using AfyaHMIS.Service;

namespace AfyaHMIS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserService IService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUserService service) {
            _logger = logger;
            IService = service;
        }

        public IActionResult Index(HomeIndexViewModel model) {
            return View(model);
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
