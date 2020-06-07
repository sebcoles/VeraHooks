using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using React.Helper;
using React.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace React.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AgentController : ControllerBase
    {
        private readonly ILogger<AgentController> _logger;
        private IMemoryCache _cache;
        private IAgentRepository _agentRepository;

        public AgentController(
            ILogger<AgentController> logger,
            IMemoryCache memoryCache,
            IAgentRepository agentRepository)
        {
            _logger = logger;
            _cache = memoryCache;
            _agentRepository = agentRepository;
        }

        [HttpGet]
        [Route("Get")]
        public ActionResult Get()
        {
            return Ok(_agentRepository.Agents);
        }

        [HttpDelete]
        [Route("Delete")]
        public ActionResult Delete(string name)
        {
            var myAgent = _agentRepository.Agents.SingleOrDefault(x => x.Name == name);

            if(myAgent != null)
                _agentRepository.Agents.Remove(myAgent);

            return Ok();
        }

        [HttpPost]
        [Route("Ping")]
        public ActionResult Ping(Agent agent)
        {
            lock (this)
            {
                var myAgent = _agentRepository.Agents.SingleOrDefault(x => x.Name == agent.Name);

                if (myAgent == null)
                {
                    agent.LastCalled = DateTime.Now;
                    _agentRepository.Agents.Add(agent);
                    return Ok();
                }

                myAgent.VeracodeOk = agent.VeracodeOk;
                myAgent.DatabaseOk = agent.DatabaseOk;
                myAgent.LastCalled = DateTime.Now;

                return Ok();
            }             
        }
    }
}
