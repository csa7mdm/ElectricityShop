using System.Threading;
using System.Threading.Tasks;
using ElectricityShop.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElectricityShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FailedMessagesController : ControllerBase
    {
        private readonly IFailedMessageLogger _failedMessageLogger;
        private readonly ILogger<FailedMessagesController> _logger;

        public FailedMessagesController(
            IFailedMessageLogger failedMessageLogger,
            ILogger<FailedMessagesController> logger)
        {
            _failedMessageLogger = failedMessageLogger;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FailedMessageDto>> GetFailedMessage(
            string id, 
            CancellationToken cancellationToken)
        {
            var failedMessage = await _failedMessageLogger.GetFailedMessageAsync(id, cancellationToken);
            
            if (failedMessage == null)
            {
                return NotFound();
            }
            
            return Ok(failedMessage);
        }

        [HttpPost("{id}/requeue")]
        public async Task<ActionResult> RequeueFailedMessage(
            string id, 
            CancellationToken cancellationToken)
        {
            var success = await _failedMessageLogger.RequeueFailedMessageAsync(id, cancellationToken);
            
            if (!success)
            {
                return NotFound();
            }
            
            return NoContent();
        }
    }
}