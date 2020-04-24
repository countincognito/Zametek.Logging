using System;

namespace Zametek.Utility.Logging.AspNetCore.TestApi
{
    [Serializable]
    public class RequestDto
    {
        public string Name { get; set; }

        public string Email { get; set; }

        [NoLogging]
        public string Password { get; set; }
    }
}