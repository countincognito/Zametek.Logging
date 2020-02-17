using System;

namespace Zametek.Utility.Logging.AspNetCore.TestApi
{
    [Serializable]
    public class ResponseDto
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}