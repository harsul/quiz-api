using System;
using System.Collections.Generic;
using System.Net;

namespace QuizService.Contracts.Common
{
    public class ErrorResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }
}