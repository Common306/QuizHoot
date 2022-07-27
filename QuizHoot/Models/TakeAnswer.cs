using System;
using System.Collections.Generic;

#nullable disable

namespace QuizHoot.Models
{
    public partial class TakeAnswer
    {
        public int TakeQuizId { get; set; }
        public int? QuestionId { get; set; }
        public int AnswerId { get; set; }

        public virtual Answer Answer { get; set; }
        public virtual TakeQuiz TakeQuiz { get; set; }
    }
}
