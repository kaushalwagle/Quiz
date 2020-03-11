using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuizApp.Data;
using QuizApp.Models;

namespace QuizApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context) {
            _logger = logger;
            _context = context;
        }

        [Authorize]
        public IActionResult Index() {
            return View();
        }

        [Authorize]
        public IActionResult Quiz() {
            List<Quiz> randomQuizes = new List<Quiz>();
            do {
                Random rnd = new Random();
                int rndId = rnd.Next(_context.Quizes.Min(q => q.Id), _context.Quizes.Max(q=>q.Id));

                Quiz rndQuiz = _context.Quizes.Find(rndId);

                if (!randomQuizes.Contains(rndQuiz))
                    randomQuizes.Add(rndQuiz);

            } while (randomQuizes.Count < 10);
            return View(randomQuizes);
        }

        [Authorize]
        [HttpGet]
        public IActionResult CheckAnswer(Quiz  quiz, string answer) {
            Quiz quizFromDb = _context.Quizes.Find(quiz.Id);
            string result = "Incorrect";
            if (quizFromDb.Answer.Equals(answer, StringComparison.InvariantCultureIgnoreCase))
                result = "Correct";
            return Ok(result);
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
