using System;
using System.Collections.Generic;

#nullable disable

namespace QuizHoot.Models
{
    public partial class Quiz
    {
        public Quiz()
        {
            Questions = new HashSet<Question>();
            TakeQuizzes = new HashSet<TakeQuiz>();
        }

        public int QuizId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool Publish { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public int PassScore { get; set; }
        public string CreatorId { get; set; }

        public virtual AspNetUser Creator { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<TakeQuiz> TakeQuizzes { get; set; }
    }
}
