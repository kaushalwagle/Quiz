using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Models;

namespace QuizApp.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public QuizController(ApplicationDbContext context, UserManager<IdentityUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        // GET: Quiz
        public IActionResult Index() {
            Random rnd = new Random();
            int rndId = rnd.Next(_context.Quizes.Min(q => q.Id), _context.Quizes.Max(q => q.Id));

            //Asked 10 questions
            int totalQuestionsServed = (int)HttpContext.Session.GetInt32("TotalQuestionServed");
            if (totalQuestionsServed <= 10) {
                Quiz rndQuiz = _context.Quizes.Find(rndId);
                totalQuestionsServed++;
                HttpContext.Session.SetInt32("TotalQuestionServed", totalQuestionsServed);
                return View(rndQuiz);
            }
            return RedirectToAction("Result");
        }

        public async Task<IActionResult> Result() {

            HttpContext.Session.SetInt32("TotalQuestionServed", 0);
            int points = (int)HttpContext.Session.GetInt32("Points");

            IdentityUser currentPlayer = await _userManager.GetUserAsync(User);
            Score score = new Score {
                Points = points,
                ScoredAt = DateTime.Now,
                QuizUser = currentPlayer
            };
            _context.ScoreBoard.Add(score);

            await _context.SaveChangesAsync();
            ViewData["currentScoreTime"] = _context.ScoreBoard
                .OrderByDescending(s => s.ScoredAt)
                .FirstOrDefault()
                .ScoredAt;

            IEnumerable<Score> myScores = _context.ScoreBoard
                .Include(u => u.QuizUser)
                .OrderByDescending(s => s.Points);
            

            return View(myScores);
        }


        //CheckAnswer?id=51&option=AmICorrect
        public IActionResult CheckAnswer(int id, string option) {
            Quiz quizFromDb = _context.Quizes.Find(id);
            string result = "Incorrect";
            if (quizFromDb.Answer.Equals(option, StringComparison.InvariantCultureIgnoreCase)) {
                result = "Correct";
                int currentScore = (int)HttpContext.Session.GetInt32("Points");
                currentScore++;
                HttpContext.Session.SetInt32("Points", currentScore);
            }
            return Json(result);
        }
    }
}