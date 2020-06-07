using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using VeracodeService.Models;
using VeracodeService.Repositories;

namespace DataAccess.Models
{
    public class MitigationWebhook : BaseEntity
    {
        public MitigationWebhook()
        {
            LastFired = new DateTime(1970, 1, 1);
        }
        public string Name { get; set; }
        public List<App> Apps { get; } = new List<App>();
        public string SendAddress { get; set; }
        public List<PropertyCondition> PropertyConditions { get; } = new List<PropertyCondition>();
        public DateTime Created { get; set; }
        public DateTime LastFired { get; set; }
        public int TimesFired { get; set; }
        public int SecondsBetweenCheck { get; set; }
        public string Demand { get; set; }
    }
}
