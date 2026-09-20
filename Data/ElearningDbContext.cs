using Elearningplatform.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Elearningplatform.Data
{
    public class ElearningDbContext : DbContext
    {
        public ElearningDbContext()
            : base("name=ElearningDbContext")
        {
            // Disable initializer since we're managing DB manually
            Database.SetInitializer<ElearningDbContext>(null);

            // Disable lazy loading
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<VideoCourseContent> VideoCourseContents { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Certificate> Certificates { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Don't use conventions since we have existing database
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Map to existing tables
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Course>().ToTable("Courses");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollments");
            modelBuilder.Entity<VideoCourseContent>().ToTable("VideoCourseContents");

        }
    }
}