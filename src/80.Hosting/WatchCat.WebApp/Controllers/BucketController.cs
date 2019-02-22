using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WatchCat.Adapters;
using WatchCat.Adapters.Log;

namespace WatchCat.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BucketController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BucketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult> PostData([FromBody] JObject data)
        {
            if (data != null && data.HasValues)
            {
                await _mediator.Publish(new PayloadNotification(data));
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        [Consumes("text/plain")]
        public async Task<ActionResult> PostMessage([FromBody] string rawData)
        {
            if (!string.IsNullOrWhiteSpace(rawData))
            {
                await _mediator.Publish(new MessageNotification(rawData));
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("log")]
        [Consumes("text/plain")]
        public async Task<ActionResult> PostLog([FromBody] string rawData)
        {
            if (!string.IsNullOrWhiteSpace(rawData))
            {
                await _mediator.Publish(new LogNotification(Microsoft.Extensions.Logging.LogLevel.Warning, rawData));
                return Ok();
            }

            return BadRequest();
        }
    }
}