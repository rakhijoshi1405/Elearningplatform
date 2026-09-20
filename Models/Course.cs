using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elearningplatform.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [StringLength(200)]  // Optional: add max length
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]  // Optional: improve view rendering
        public string Description { get; set; }

        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }

        [DataType(DataType.Currency)]  // Optional: helps in views
        public decimal Price { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Published")]
        public bool IsPublished { get; set; }

        // Navigation properties
        public virtual User Teacher { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<VideoCourseContent> VideoCourseContents { get; set; }

        public Course()
        {
            CreatedDate = DateTime.UtcNow;  // Use UTC for consistency
            IsPublished = false;
            Price = 0;
        }
    }
}