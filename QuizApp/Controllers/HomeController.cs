using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QuizApp.Data;
using QuizApp.Models;

namespace QuizApp.Controllers
{
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context) {
            _logger = logger;
            _context = context;
        }

        [Authorize]
        public IActionResult Index() {
            HttpContext.Session.SetInt32("State", 0);
            HttpContext.Session.SetInt32("Points", 0);
            HttpContext.Session.SetInt32("TotalQuestionServed", 0);
            return View();
        }

        
        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
