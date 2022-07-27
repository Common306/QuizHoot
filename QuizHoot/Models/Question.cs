using System;
using System.Collections.Generic;

#nullable disable

namespace QuizHoot.Models
{
    public partial class Question
    {
        public Question()
        {
            Answers = new HashSet<Answer>();
        }

        public int QuestionId { get; set; }
        public string Content { get; set; }
        public bool Active { get; set; }
        public int Score { get; set; }
        public int? LevelId { get; set; }
        public int? QuizId { get; set; }
        public bool MultipleChoice { get; set; }

        public virtual Level Level { get; set; }
        public virtual Quiz Quiz { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
    }
}
