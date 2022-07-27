using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuizHoot.Models.ViewModels
{
    public class ReviewQuizViewModel
    {
        public TakeQuiz TakeQuiz { get; set; }
        public Quiz Quiz { get; set; }
        
        public double TotalScore { get; set; }
        public List<Question> Questions { get; set; }
        public IDictionary<Answer, bool> ReviewAnswer { get; set; } = new Dictionary<Answer, bool>();
        public IDictionary<int, double> QuestionScore { get; set; } = new Dictionary<int, double>();
        
        public bool CheckAnswer(Answer trueAnswer, TakeAnswer checkAnswer)
        {
            if (checkAnswer is null) return false;

            return trueAnswer.Correct && trueAnswer.AnswerId == checkAnswer.AnswerId;
        }
    }
}
