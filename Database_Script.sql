-- ============================================
-- DATABASE SCRIPT FOR ANANTVIDYA
-- GROUP: 25
-- ============================================

USE ElearningPlatformDB;
GO

-- TABLE: Users
CREATE TABLE dbo.Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL,
    Password NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    Role NVARCHAR(50) NOT NULL,
    IsApproved BIT NOT NULL,
    CreatedDate DATETIME NOT NULL
);
GO

-- TABLE: Courses
CREATE TABLE dbo.Courses (
    CourseId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    TeacherId INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    CreatedDate DATETIME NOT NULL,
    IsPublished BIT NOT NULL
);
GO

-- TABLE: Enrollments
CREATE TABLE dbo.Enrollments (
    EnrollmentId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL,
    CourseId INT NOT NULL,
    EnrollmentDate DATETIME NOT NULL
);
GO

-- TABLE: Quizzes
CREATE TABLE dbo.Quizzes (
    QuizId INT IDENTITY(1,1) PRIMARY KEY,
    CourseId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL
);
GO

-- TABLE: Questions
CREATE TABLE dbo.Questions (
    QuestionId INT IDENTITY(1,1) PRIMARY KEY,
    QuizId INT NOT NULL,
    QuestionText NVARCHAR(MAX) NOT NULL,
    OptionA NVARCHAR(500) NOT NULL,
    OptionB NVARCHAR(500) NOT NULL,
    OptionC NVARCHAR(500) NOT NULL,
    OptionD NVARCHAR(500) NOT NULL,
    CorrectAnswer NVARCHAR(1) NOT NULL
);
GO

-- TABLE: Results
CREATE TABLE dbo.Results (
    ResultId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    QuizId INT NOT NULL,
    Score INT NOT NULL,
    AttemptDate DATETIME NOT NULL,
    IsPassed BIT NOT NULL
);
GO

-- TABLE: Certificates
CREATE TABLE dbo.Certificates (
    CertificateId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    CourseId INT NOT NULL,
    IssueDate DATETIME NOT NULL,
    CertificateNumber NVARCHAR(100) NOT NULL
);
GO

-- TABLE: Payments
CREATE TABLE dbo.Payments (
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    CourseId INT NOT NULL,
    StudentId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    PaymentDate DATETIME NOT NULL
);
GO

-- TABLE: Videos
CREATE TABLE dbo.VideoCourseContents (
    VideoContentId INT IDENTITY(1,1) PRIMARY KEY,
    CourseId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    VideoFileName NVARCHAR(200),
    VideoFilePath NVARCHAR(500),
    ThumbnailPath NVARCHAR(500),
    Duration INT NULL,
    FileSize BIGINT NULL,
    UploadDate DATETIME NOT NULL,
    DisplayOrder INT NULL,
    IsProcessed BIT NOT NULL
);
GO

-- INSERT SAMPLE USERS
INSERT INTO dbo.Users (Username, Password, Email, Role, IsApproved, CreatedDate)
VALUES 
('admin', 'MTIzNDU2', 'admin@test.com', 'Admin', 1, GETDATE()),
('teacher1', 'MTIzNDU2', 'teacher1@test.com', 'Teacher', 1, GETDATE()),
('test', 'MTIzNDU2', 'test@test.com', 'Student', 1, GETDATE()),
('Avni', 'MTIzNDU2', 'avni@test.com', 'Student', 1, GETDATE());
GO

-- INSERT SAMPLE COURSES
INSERT INTO dbo.Courses (Title, Description, TeacherId, Price, CreatedDate, IsPublished)
VALUES 
('Data Science', 'Learn Data Science from basics', 2, 2500, GETDATE(), 1),
('Web Development', 'Learn Web Development', 2, 25000, GETDATE(), 1);
GO

-- INSERT SAMPLE QUIZ
INSERT INTO dbo.Quizzes (CourseId, Title)
VALUES (1, 'Data Science Basics Quiz');
GO

-- INSERT SAMPLE QUESTIONS
INSERT INTO dbo.Questions (QuizId, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectAnswer)
VALUES 
(1, 'What is Data Science?', 'Study of data', 'Study of computers', 'Study of rocks', 'Study of plants', 'A'),
(1, 'Which language is used in Data Science?', 'Java', 'C++', 'Python', 'C#', 'C'),
(1, 'What is Machine Learning?', 'Branch of AI', 'Database', 'Programming language', 'Framework', 'A');
GO

-- INSERT SAMPLE ENROLLMENTS
INSERT INTO dbo.Enrollments (StudentId, CourseId, EnrollmentDate)
VALUES (3, 1, GETDATE()), (4, 1, GETDATE());
GO

-- CHECK ALL TABLES
SELECT 'Users' as TableName, COUNT(*) as Count FROM dbo.Users
UNION
SELECT 'Courses', COUNT(*) FROM dbo.Courses
UNION
SELECT 'Enrollments', COUNT(*) FROM dbo.Enrollments
UNION
SELECT 'Quizzes', COUNT(*) FROM dbo.Quizzes
UNION
SELECT 'Questions', COUNT(*) FROM dbo.Questions
UNION
SELECT 'Results', COUNT(*) FROM dbo.Results
UNION
SELECT 'Certificates', COUNT(*) FROM dbo.Certificates
UNION
SELECT 'Payments', COUNT(*) FROM dbo.Payments
UNION
SELECT 'Videos', COUNT(*) FROM dbo.VideoCourseContents;
GO

PRINT '==========================================';
PRINT 'DATABASE CREATED SUCCESSFULLY!';
PRINT '==========================================';
GO