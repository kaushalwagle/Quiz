using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuizApp.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Incorrect1 { get; set; }
        public string Incorrect2 { get; set; }
        public string Incorrect3 { get; set; }

    }
}
