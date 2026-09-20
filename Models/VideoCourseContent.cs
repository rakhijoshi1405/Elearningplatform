using Elearningplatform.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elearningplatform.Models
{
    public class VideoCourseContent
    {
        [Key]
        public int VideoContentId { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
        public string VideoFileName { get; set; }
        public string VideoFilePath { get; set; }
        public string ThumbnailPath { get; set; }
        public int? Duration { get; set; }  // Changed to nullable
        public long? FileSize { get; set; }  // Changed to nullable
        public DateTime UploadDate { get; set; }
        public int? DisplayOrder { get; set; }  // Changed to nullable
        public bool IsProcessed { get; set; }

        public virtual Course Course { get; set; }

        public VideoCourseContent()
        {
            UploadDate = DateTime.Now;
            DisplayOrder = 0;
            IsProcessed = false;
            Duration = 0;  // Default value
            FileSize = 0;  // Default value
        }
    }
}