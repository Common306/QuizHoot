using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizHoot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using QuizHoot.Areas.Identity.Data;
using QuizHoot.Models.ViewModels;

namespace QuizHoot.Controllers
{
    public class QuizController : Controller
    {
        private readonly QuizHootContext _context;
        private readonly UserManager<QuizHootUser> _userManager;
        public QuizController(QuizHootContext ctx, UserManager<QuizHootUser> userManager)
        {
            _context = ctx;
            _userManager = userManager;
        }

        public IActionResult List(int? id)
        {

            List<Quiz> quizzes = (id is null)
                ? _context.Quizzes.Include(q => q.Creator).Where(q => q.Publish).ToList()
                : _context.Quizzes.Include(q => q.Creator).Where(q => q.Publish && q.QuizId == id).ToList();


            var quizViewModels = new List<QuizViewModel>();

            foreach (var q in quizzes)
            {
                int amount = _context.TakeQuizzes.Where(tq => tq.QuizId == q.QuizId).Count();
                var qv = new QuizViewModel();
                qv.Quiz = q;
                qv.TotalQuestion = _context.Questions.Where(q => q.QuizId == qv.Quiz.QuizId).Count();
                qv.PlayQuizNumber = amount;
                quizViewModels.Add(qv);
            }

            return View(quizViewModels);
        }

        public IActionResult DoQuiz(int qid)
        {
            var user = HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }

            var uId = _userManager.GetUserId(user);
            var startTime = DateTime.Now;
            var quiz = _context.Quizzes.Find(qid);

            if (quiz is null) return NotFound();
            if (!quiz.Publish) return Content("Quiz is not avaiable.");
            if (quiz.StartAt is not null && startTime < quiz.StartAt)
            {
                ViewData["ErrorMessage"] = "This quiz is not started.";
                return View("ErrorPage");
            }

            if (quiz.EndAt is not null && quiz.EndAt < startTime)
            {
                ViewData["ErrorMessage"] = "The time to take this test has expired.";
                return View("ErrorPage");
            }

            TakeQuiz takeQuiz = new TakeQuiz
            {
                StartAt = DateTime.Now,
                FinishAt = null,
                Score = 0,
                QuizId = qid,
                UserId = uId
            };

            _context.TakeQuizzes.Add(takeQuiz);
            _context.SaveChanges();

            return RedirectToAction("DoQuizInProcess", new
            {
                takeQuizId = takeQuiz.TakeQuizId
            });
        }

        public IActionResult DoQuizInProcess(int takeQuizId)
        {

            var uId = _userManager.GetUserId(HttpContext.User);

            TakeQuiz takeQuiz = _context.TakeQuizzes
                .Where(tq => tq.TakeQuizId == takeQuizId && tq.UserId.Equals(uId))
                .FirstOrDefault();

            if (takeQuiz is null)
            {
                ViewData["ErrorCode"] = "404";
                ViewData["ErrorMessage"] = "PAGE NOT FOUND";
                ViewData["ErrorDescription"] = "The page you are looking for might have been removed had its name changed or is temporarily unavailable.";
                return View("ErrorPage");
            }

            var quiz = _context.Quizzes.Where(q => q.QuizId == takeQuiz.QuizId)
                .Include(q => q.Questions).ThenInclude(qs => qs.Answers).First();

            ViewBag.TakeQuiz = takeQuiz;
            return View(quiz);
        }

        [HttpPost]
        public IActionResult SubmitAnswer(List<string> selectAnswers, int takeQuizId)
        {
            var uId = _userManager.GetUserId(HttpContext.User);

            TakeQuiz takeQuiz = _context.TakeQuizzes
                .Where(tq => tq.TakeQuizId == takeQuizId && tq.UserId == uId)
                .FirstOrDefault();

            // user not start do this quiz
            if (takeQuiz is null)
            {
                ViewData["ErrorCode"] = "404";
                ViewData["ErrorMessage"] = "PAGE NOT FOUND";
                ViewData["ErrorDescription"] = "The page you are looking for might have been removed had its name changed or is temporarily unavailable.";
                return View("ErrorPage");
            }

            var quiz = _context.Quizzes.Find(takeQuiz.QuizId);

            if (quiz.EndAt is not null && quiz.EndAt < DateTime.Now)
            {
                ViewData["ErrorMessage"] = "Submit Answer Fail!";
                ViewData["ErrorDescription"] = "Time is expired";
                return View("ErrorPage");
            }

            if (takeQuiz.FinishAt is not null)
            {
                ViewData["ErrorMessage"] = "Submit Answer Fail!";
                ViewData["ErrorDescription"] = "This quiz was submited, can't resubmit.";
                return View("ErrorPage");

            }

            takeQuiz.FinishAt = DateTime.Now;
            _context.TakeQuizzes.Update(takeQuiz);

            double temp = 0; 
            foreach (var a in selectAnswers)
            {
                var tokens = a.Split("-");
                int questionId = int.Parse(tokens[0]);
                int answerId = int.Parse(tokens[1]);

                int questionScore = _context.Questions.Find(questionId).Score;
                int totalTrueAnswer = _context.Answers.Where(a => a.QuestionId == questionId && a.Correct).Count();
                double scorePerTrueAnswer = (totalTrueAnswer != 0) ? (1.0 * questionScore / totalTrueAnswer) : 0;
                bool isTrueAnswerChecked = _context.Answers.Where(a => a.AnswerId == answerId && a.Correct).Any();
                temp += (isTrueAnswerChecked) ? scorePerTrueAnswer : (scorePerTrueAnswer * -1);

                Console.WriteLine($"{questionId} - {answerId}");
                _context.Add(new TakeAnswer
                {
                    TakeQuizId = takeQuiz.TakeQuizId,
                    QuestionId = questionId,
                    AnswerId = answerId
                });
            }
            temp = (temp <= 0) ? 0 : temp;
            takeQuiz.Score = (int)temp;
            _context.Update(takeQuiz);
            _context.SaveChanges();

            ViewData["ErrorMessage"] = "Submit successfully!";
            return View("ErrorPage");
        }


        public IActionResult MyQuiz()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Id = id;
            Quiz quiz = _context.Quizzes.First(s => s.QuizId == id);
            ViewBag.Quiz = quiz;
            List<Question> listQuestion = _context.Questions.Include(s => s.Level).Include(s => s.Quiz).Include(s => s.Answers).Where(s => s.QuizId == id).ToList<Question>();
            ViewBag.listQuestion = listQuestion;
            var listLevel = _context.Levels.ToList<Level>();
            ViewBag.listLevel = listLevel;
            return View();
        }
        // id is  TakeQuiz.TakeQuizID
        public IActionResult Review(int id)
        {
            TakeQuiz takeQuiz = _context.TakeQuizzes.Find(id);
            var takeAnswer = _context.TakeAnswers
                .Where(ta => ta.TakeQuizId == takeQuiz.TakeQuizId).ToList();

            var reviewModel = new ReviewQuizViewModel();
            reviewModel.TakeQuiz = takeQuiz;
            reviewModel.Quiz = _context.Quizzes.Find(takeQuiz.QuizId);
            reviewModel.Questions = _context.Questions.Include(q => q.Answers)
                .Where(q => q.QuizId == reviewModel.Quiz.QuizId)
                .ToList();


            foreach (var q in reviewModel.Questions)
            {
                var temp = 0.0;
                var amountTrue = 0;
                var amountFalse = 0;
                double scorePerAnswer = (q.Answers is null) ? 0
                    : (q.Score / q.Answers.Where(a => a.Correct).Count());

                foreach (var a in q.Answers)
                {
                    var ck = takeAnswer.Any(n => n.AnswerId == a.AnswerId);
                    if (ck && a.Correct) amountTrue++;
                    if (ck && !a.Correct) amountFalse++;
                    reviewModel.ReviewAnswer.Add(a, ck);
                }
                temp = (amountFalse >= amountTrue) ? 0 : ((amountTrue - amountFalse) * scorePerAnswer);
                reviewModel.TotalScore += temp;
                reviewModel.QuestionScore[q.QuestionId] = temp;
            }

            return View(reviewModel);
        }

        private double caculateScore(List<Question> questions, List<TakeAnswer> takeAnswers)
        {
            double score = 0;
            foreach (var q in questions)
            {
                var amountTrue = 0;
                var amountFalse = 0;
                var scorePerAnswer = (q.Answers is null)
                    ? 0 
                    : (q.Score / q.Answers.Where(a => a.Correct).Count());

                foreach (var a in q.Answers)
                {
                    var ck = takeAnswers.Any(n => n.AnswerId == a.AnswerId);
                    if (ck && a.Correct) amountTrue++;
                    if (ck && !a.Correct) amountFalse++;
                }
                var temp = (amountFalse >= amountTrue) ? 0 : (amountTrue * scorePerAnswer);
                score += temp;
            }
            return score;
        }

        // Crud part
        public IActionResult Detail(int id)
        {
            Quiz quiz = _context.Quizzes.Find(id);
            return View(quiz);
        }

        [HttpPost]
        public IActionResult Create(Quiz quiz, string returnUrl)
        {
            quiz.CreatorId = _userManager.GetUserId(HttpContext.User);
            _context.Add(quiz);
            _context.SaveChanges();
            return Redirect(returnUrl);
        }

        [HttpPost]
        public IActionResult Update(Quiz q, string returnUrl)
        {
            q.CreatorId = _userManager.GetUserId(HttpContext.User);
            _context.Update(q);
            _context.SaveChanges();
            return Redirect(returnUrl);
        }

        public IActionResult Delete(int id, string returnUrl)
        {
            var q = _context.Quizzes.Find(id);
            _context.Quizzes.Remove(q);
            _context.SaveChanges();
            return Redirect(returnUrl);
        }
        // End Crud part
        public IActionResult ViewQuizResult(int? id)
        {
            var uId = _userManager.GetUserId(HttpContext.User);

            var takeQuizzes = _context.TakeQuizzes
                .Include(tq => tq.Quiz)
                .Where(tq => tq.UserId.Equals(uId))
                .OrderByDescending(tq => tq.FinishAt)
                .ToList();

            return View(takeQuizzes);

        }

        public IActionResult Manage(int? id)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }

            var uId = _userManager.GetUserId(HttpContext.User);
            var quizzes = _context.Quizzes.Where(q => q.CreatorId.Equals(uId)).ToList();
            return View(quizzes);
        }

        public IActionResult AllResults(int id)
        {
            ViewBag.Id = id;
            var listTakeQuiz = _context.TakeQuizzes.Include(q => q.Quiz).Include(q => q.User).Where(s => s.QuizId == id).ToList<TakeQuiz>();
            ViewBag.listTakeQuiz = listTakeQuiz;
            return View();
        }
    }
}
