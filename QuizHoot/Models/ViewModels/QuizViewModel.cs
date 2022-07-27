using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuizHoot.Models.ViewModels
{
    public class QuizViewModel
    {
        public Quiz Quiz { get; set; }
        public int PlayQuizNumber { get; set; }
        public int TotalQuestion { get; set; }
    }
}
