using Destructurama.Attributed;
using System;

namespace Zametek.Utility.Logging.AspNetCore.Tests
{
    [Serializable]
    public class RequestDto
    {
        public string Name { get; set; }

        public string Email { get; set; }

        [NotLogged]
        public string Password { get; set; }
    }
}