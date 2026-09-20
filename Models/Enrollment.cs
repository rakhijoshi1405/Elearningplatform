using Elearningplatform.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elearningplatform.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Display(Name = "Enrollment Date")]
        [DataType(DataType.DateTime)]
        public DateTime EnrollmentDate { get; set; }

        public virtual User Student { get; set; }
        public virtual Course Course { get; set; }

        public Enrollment()
        {
            EnrollmentDate = DateTime.UtcNow;  // Use UTC
        }
    }
}