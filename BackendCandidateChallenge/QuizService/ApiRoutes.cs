using System;
namespace QuizService
{
    public static class ApiRoutes
    {
        public const string BaseRoute = "api/quizzes";
        public const string IdRoute = "{quizId}";

        public static class Questions
        {
            public const string BaseRoute = "{quizId}/questions";
            public const string IdRoute = "{quizId}/questions/{questionId}";
        }

        public static class Answers
        {
            public const string BaseRoute = "{quizId}/questions/{questionId}/answers";
            public const string IdRoute = "{quizId}/questions/{questionId}/answers/{answerId}";
        }

    }
}