using System;

namespace Zametek.Utility.Logging.AspNetCore.Tests
{
    [Serializable]
    public class ResponseDto
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}