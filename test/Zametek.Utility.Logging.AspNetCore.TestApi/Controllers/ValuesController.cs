using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Zametek.Utility.Logging.AspNetCore.TestApi
{
    [Route("api/[controller]")]
    public class ValuesController
        : Controller
    {
        private readonly IValueAccess m_ValueAccess;
        private readonly ILogger m_Logger;

        public ValuesController(
            IValueAccess valueAccess,
            ILogger logger)
        {
            m_ValueAccess = valueAccess ?? throw new ArgumentNullException(nameof(valueAccess));
            m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Debug.Assert(TrackingContext.Current != null);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]RequestDto requestDto)
        {
            m_Logger.Information($"{nameof(Post)} Invoked");

            // Ensure this actiuon is truly asyncronious.
            await Task.Yield();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                string result = await m_ValueAccess.AddAsync(requestDto).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(result))
                {

                    m_Logger.Information($"{nameof(Post)} Completed");
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                m_Logger.Error(ex, "Error caught in the controller class.");
            }
            return BadRequest(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            m_Logger.Information($"{nameof(Get)} Invoked");

            // Ensure this actiuon is truly asyncronious.
            await Task.Yield();

            try
            {
                IList<ResponseDto> responses = await m_ValueAccess.GetAsync(Guid.NewGuid().ToString(), "Password123!").ConfigureAwait(false);

                m_Logger.Information($"{nameof(Get)} Completed");
                return Ok(responses);
            }
            catch (Exception ex)
            {
                m_Logger.Error(ex, "Error caught in the controller class.");
            }
            return BadRequest(HttpStatusCode.BadRequest);
        }
    }
}
