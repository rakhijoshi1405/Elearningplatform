using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elearningplatform.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int CourseId { get; set; }

        public int StudentId { get; set; } // 👈 change this

        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}