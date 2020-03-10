using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuizApp.Models
{
    public class Score
    {
        public int Id { get; set; }
        public DateTime ScoredAt { get; set; }
        public int Points { get; set; }

        public IdentityUser QuizUser { get; set; }
    }
}
