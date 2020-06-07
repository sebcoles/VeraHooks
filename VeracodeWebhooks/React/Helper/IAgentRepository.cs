using React.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace React.Helper
{
    public interface IAgentRepository
    {
        List<Agent> Agents { get; set; }
    }

    public class AgentRepository : IAgentRepository
    {
        public List<Agent> Agents { get; set; }

        public AgentRepository()
        {
            Agents = new List<Agent>();
        }
    }
}
