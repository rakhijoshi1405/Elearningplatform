USE ElearningPlatformDB;
GO

-- Step 1: Delete old data
DELETE FROM dbo.Questions;
DELETE FROM dbo.Quizzes;
GO

-- Step 2: Add course
INSERT INTO dbo.Courses (Title, Description, TeacherId, Price, CreatedDate, IsPublished)
VALUES ('Introduction to Python', 'Learn Python', 4, 0, GETDATE(), 1);
GO

-- Step 3: Add quiz
INSERT INTO dbo.Quizzes (CourseId, Title)
VALUES (1, 'Python Basics Quiz');
GO

-- Step 4: Add questions
INSERT INTO dbo.Questions (QuizId, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectAnswer)
VALUES 
(1, 'Who created Python?', 'Dennis Ritchie', 'Guido van Rossum', 'James Gosling', 'Bjarne Stroustrup', 'B'),
(1, 'What is file extension for Python?', '.py', '.java', '.c', '.cpp', 'A'),
(1, 'Which keyword defines a function?', 'function', 'def', 'define', 'func', 'B');
GO

-- Step 5: Check
SELECT * FROM dbo.Courses;
SELECT * FROM dbo.Quizzes;
SELECT * FROM dbo.Questions;
GO