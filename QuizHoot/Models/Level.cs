using System;
using System.Collections.Generic;

#nullable disable

namespace QuizHoot.Models
{
    public partial class Level
    {
        public Level()
        {
            Questions = new HashSet<Question>();
        }

        public int LevelId { get; set; }
        public string LevelName { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}
