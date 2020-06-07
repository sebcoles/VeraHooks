using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace React.ViewModels
{
    public class Agent
    {
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public bool VeracodeOk { get; set; }
        public bool DatabaseOk { get; set; }
        public DateTime LastCalled { get; set; }
        public bool Last30Seconds { get => Math.Abs((LastCalled - DateTime.Now).TotalSeconds) <= 30; }
    }
}
