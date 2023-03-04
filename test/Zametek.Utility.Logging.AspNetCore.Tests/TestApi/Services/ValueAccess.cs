using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zametek.Utility.Logging.AspNetCore.Tests
{
    [DiagnosticLogging(LogActive.On)]
    public class ValueAccess
        : IValueAccess
    {
        private readonly IDictionary<string, ResponseDto> m_Cache;
        private readonly ILogger m_Logger;

        public ValueAccess(ILogger logger)
        {
            m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            m_Cache = new Dictionary<string, ResponseDto>();
        }

        [return: DiagnosticLogging(LogActive.Off)]
        public async Task<string> AddAsync(RequestDto requestDto)
        {
            m_Logger.Information($"{nameof(AddAsync)} Invoked");
            m_Logger.Information($"{nameof(AddAsync)} {requestDto.Name}");

            var response = new ResponseDto
            {
                Name = requestDto.Name,
                Email = requestDto.Email,
                Password = requestDto.Password,
            };

            if (string.CompareOrdinal(response.Name, "ThrowException") == 0)
            {
                throw new Exception("Throw Exception just to make things interesting.");
            }

            if (!m_Cache.TryAdd(requestDto.Name, response))
            {
                throw new InvalidOperationException("Cannot add a key that already exists.");
            }

            await Task.Delay(new Random().Next(100, 200)).ConfigureAwait(false);

            return response.Name;
        }

        public async Task<IList<ResponseDto>> GetAsync(
            [DiagnosticLogging(LogActive.Off)] string random,
            string password)
        {
            m_Logger.Information($"{nameof(GetAsync)} Invoked");

            await Task.Delay(new Random().Next(100, 200)).ConfigureAwait(false);

            m_Logger.Information($"{nameof(GetAsync)} Finished");

            return m_Cache.Values.ToList();
        }
    }
}