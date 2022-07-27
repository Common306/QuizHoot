using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuizHoot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QuizHoot.Controllers
{
    public class QuestionController : Controller
    {
        private readonly QuizHootContext _context;

        public QuestionController(QuizHootContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("/api/question/{id}")]
        [HttpGet]
        public string GetQuestion(int id)
        {
            var question = _context.Questions
                .Include(q => q.Answers)
                .Where(q => q.QuestionId == id)
                .First();

            return JsonConvert.SerializeObject(question, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
        }


        [Route("/api/question")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string json = await reader.ReadToEndAsync();
                Question question = JsonConvert.DeserializeObject<Question>(json);
                await _context.Questions.AddAsync(question);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [Route("/api/question")]
        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] Question question)
        {
            _context.Remove(question);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("/api/question/{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var q = _context.Questions.Find(id);
            _context.Remove(q);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("/api/answer/{id}")]
        public async Task<IActionResult> DeleteAnswerAsync(int id)
        {
            var a = _context.Answers.Find(id);
            _context.Answers.Remove(a);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Route("/api/answer")]
        [Produces("application/json")]
        public IActionResult CreateAnswerAsync([FromBody] Answer answer)
        {
            _context.Answers.Add(answer);
            _context.SaveChanges();
            return Content(JsonConvert.SerializeObject(answer));
        }

        [HttpPut]
        [Route("/api/answer")]
        public async Task<IActionResult> UpdateAnswer([FromBody] Answer answer)
        {
            _context.Answers.Update(answer);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        public void UpdateQuestion(int quizId, int questionId, string questionContent, int questionLevel, int questionScore, string[] answersMark, string[] answers, int[] answersId)
        {
            Question question = _context.Questions.First(q => q.QuestionId == questionId);
            question.Content = questionContent;
            question.Active = true;
            question.Score = questionScore;
            question.LevelId = questionLevel;
            question.QuizId = quizId;
            _context.Questions.Update(question);

            // update answer in here
            List<Answer> listAnswerOfQuestion = _context.Answers.Where(q => q.QuestionId == questionId).ToList<Answer>();
            List<Answer> listAnswerCurrent = new List<Answer>();
            List<int> listIdAvaible = new List<int>();
            List<int> listIdDelete = new List<int>();
            foreach (Answer a in listAnswerOfQuestion)
            {
                if (answersId.Contains(a.AnswerId))
                {
                    listIdAvaible.Add(a.AnswerId);
                }
                else
                {
                    listIdDelete.Add(a.AnswerId);
                }
            }

            foreach (int i in listIdDelete)
            {
                Answer ansDelete = _context.Answers.First(s => s.AnswerId == i);
                _context.Answers.Remove(ansDelete);
            }
            for (int i = 0; i < listIdAvaible.Count; i++)
            {
                Answer ansUpdate = _context.Answers.First(s => s.AnswerId == listIdAvaible[i]);
                ansUpdate.Content = answers[i];
                ansUpdate.Correct = Convert.ToBoolean(answersMark[i]);
                ansUpdate.Active = true;
                _context.Answers.Update(ansUpdate);
            }
            // error in here 
            for (int i = 0; i < answersId.Length; i++)
            {
                if (answersId[i] == 0)
                {
                    Answer ansNew = new Answer
                    {
                        Content = answers[i],
                        Correct = Convert.ToBoolean(answersMark[i]),
                        Active = true,
                        QuestionId = questionId
                    };
                    _context.Answers.Add(ansNew);
                }
            }

            _context.SaveChanges();
        }

        [HttpPost]
        public void CreateQuestion(int quizId, string questionContent, int questionLevel, int questionScore, string[] answersMark, string[] answers)
        {
            List<Answer> listAnswer = new List<Answer>();
            for (int i = 0; i < answersMark.Length; i++)
            {
                Answer ans = new Answer
                {
                    Content = answers[i],
                    Correct = Convert.ToBoolean(answersMark[i]),
                    Active = true
                };
                listAnswer.Add(ans);
            }
            Question question = new Question
            {
                Content = questionContent,
                Active = true,
                Score = questionScore,
                LevelId = questionLevel,
                QuizId = quizId,
                Answers = listAnswer
            };
            _context.Questions.Add(question);
            _context.SaveChanges();
        }

        [HttpDelete]
        public void DeleteQuestion(int id)
        {
            Question question = _context.Questions.First(q => q.QuestionId == id);
            _context.Questions.Remove(question);
            _context.SaveChanges();
        }

    }
}
