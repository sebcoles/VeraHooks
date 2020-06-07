using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using VeracodeService.Models;
using VeracodeService.Repositories;

namespace React.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConditionController : ControllerBase
    {
        private readonly ILogger<ConditionController> _logger;
        private readonly IVeracodeRepository _veracodeRepository;

        public ConditionController(
            ILogger<ConditionController> logger,
            IVeracodeRepository veracodeRepository)
        {
            _veracodeRepository = veracodeRepository;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetMitigationActionFields")]
        public ActionResult GetMitigationActionFields()
        {
            return Ok(new[] {
                new { name = "mitigationField", label = "Action", value = "Action" },
                new { name = "mitigationField", label = "Reviewer", value = "Reviewer" },
                new { name = "mitigationField", label = "Comment", value = "Comment" }
            });
        }

        [HttpGet]
        [Route("GetActionType")]
        public ActionResult GetActionType()
        {
            return Ok(new[] {
                new { name = "actionType", label = "Mitigate by Design", value = "appdesign" },
                new { name = "actionType", label = "Mitigate by Network Environment", value = "netenv" },
                new { name = "actionType", label = "Mitigate by OS Environment", value = "osenv" },
                new { name = "actionType", label = "Comment", value = "comment" },
                new { name = "actionType", label = "Reported by Library Maintainer", value = "library" },
                new { name = "actionType", label = "Accepted Mitigation", value = "accepted" },
                new { name = "actionType", label = "Rejected Mitigation", value = "rejected" },
                new { name = "actionType", label = "Potential False Positive", value = "fp" },
                new { name = "actionType", label = "Any", value = "Any" },
            });
        }
    }
}
