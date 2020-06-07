using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using VeracodeService.Repositories;

namespace React.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VeracodeController : ControllerBase
    {
        private readonly ILogger<VeracodeController> _logger;
        private readonly IVeracodeRepository _veracodeRepository;

        public VeracodeController(
            ILogger<VeracodeController> logger,
            IVeracodeRepository veracodeRepository)
        {
            _veracodeRepository = veracodeRepository;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetPropertyTypes")]
        public ActionResult GetPropertyTypes()
        {
            return Ok(new[] {
                new { name = "propertyType", label="Mitigations", value="1" } });
        }

        [HttpGet]
        [Route("GetFields")]
        public ActionResult GetFields(string propertyType)
        {
            switch (propertyType)
            {
                case "Mitigations":
                    return Ok(new[] {
                        new { name = "fieldType", label="Any", value="Any" },
                        new { name = "fieldType", label="Action", value="Action" }
                    });
            }
            return BadRequest();
        }

        [Route("GetSubPropertyTypes")]
        public ActionResult GetSubPropertyTypes(string propertyType)
        {
            if (propertyType == "1")
                return Ok(new[] { new { name = "propertyType", label = "Build", value = "2" } });

            return BadRequest();
        }

        [HttpGet]
        [Route("Applications")]
        public ActionResult Applications()
        {
            return Ok(
                _veracodeRepository.GetAllApps()
                .Select(x => new
                {
                    name = "application",
                    label = x.App_name,
                    value = x.App_id
                })
                .ToArray());
        }

        [HttpGet]
        [Route("Builds")]
        public ActionResult Builds(string appid)
        {
            return Ok(
                _veracodeRepository.GetAllBuildsForApp(appid)
                .Select(x => new
                {
                    name = "build",
                    label = $"{x.Version}",
                    value = x.Build_id
                })
                .ToArray());
        }

        [HttpGet]
        [Route("Flaws")]
        public ActionResult Flaws(string buildId)
        {
            var flaws = _veracodeRepository.GetFlaws(buildId);
            var mitigations = _veracodeRepository
                .GetAllMitigationsForBuildAndFlaws(buildId, flaws
                .Select(x => x.Issueid)
                .ToArray());

            return Ok(
                flaws.Select(x => new
                {
                    name = "build",
                    label = $"{x.Module} - {x.Sourcefile} - {x.Line} - Mitigation Comment Total: {mitigations.Issue.Single(mitigation => mitigation.Flaw_id == x.Issueid).MitigationActions.Count()}",
                    value = x.Issueid
                }).ToArray());
        }
    }
}
