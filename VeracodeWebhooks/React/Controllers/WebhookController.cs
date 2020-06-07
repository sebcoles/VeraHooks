using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using React.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace React.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        private IGenericRepository<MitigationWebhook> _webhooksRepository;
        private IGenericRepository<WebhookLog> _logRepository;
        private ApplicationDbContext _context;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(ILogger<WebhookController> logger, 
            IGenericRepository<MitigationWebhook> webhooksRepository,
            IGenericRepository<WebhookLog> logRepository,
            ApplicationDbContext context
            )
        {
            _logger = logger;
            _webhooksRepository = webhooksRepository;
            _logRepository = logRepository;
            _context = context;
        }

        [HttpGet]
        public ActionResult Get()
        {
            var webhooks = _webhooksRepository.GetAll().ToList();
            return Ok(webhooks);
        }

        [Route("Logs")]
        [HttpGet]
        public ActionResult<IEnumerable<WebhookLog>> Logs()
        {
            return Ok(_logRepository.GetAll()
                .OrderByDescending(x => x.TimeFired));
        }

        [Route("Add")]
        [HttpPost]
        public async Task<IActionResult> Add(NewHook newhook)
        {
            newhook.Webhook.Created = DateTime.Now;
            newhook.Webhook.PropertyConditions.AddRange(newhook.Conditions);
            newhook.Webhook.Apps.AddRange(newhook.Apps
                .Select(x => new App { AppId = x.AppId, AppName = x.AppName }).ToArray());
            var created = await _webhooksRepository.Create(newhook.Webhook);
            return Ok();
        }


        [Route("Delete")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _webhooksRepository.Delete(id);
            return Ok();
        }
    }
}
