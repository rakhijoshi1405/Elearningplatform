using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elearningplatform.Models
{
    public class Quiz
    {
        public int QuizId { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
    }
}