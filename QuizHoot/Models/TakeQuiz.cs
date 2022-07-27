using System;
using System.Collections.Generic;

#nullable disable

namespace QuizHoot.Models
{
    public partial class TakeQuiz
    {
        public TakeQuiz()
        {
            TakeAnswers = new HashSet<TakeAnswer>();
        }

        public int TakeQuizId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime? FinishAt { get; set; }
        public int Score { get; set; }
        public string UserId { get; set; }
        public int? QuizId { get; set; }

        public virtual Quiz Quiz { get; set; }
        public virtual AspNetUser User { get; set; }
        public virtual ICollection<TakeAnswer> TakeAnswers { get; set; }
    }
}
