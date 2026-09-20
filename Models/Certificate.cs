using System;

namespace Elearningplatform.Models
{
    public class Certificate
    {
        public int CertificateId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime IssueDate { get; set; }
        public string CertificateNumber { get; set; }
    }
}