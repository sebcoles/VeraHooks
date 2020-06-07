using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using VeracodeService.Models;

namespace DataAccess.Models
{
    public class PropertyCondition : BaseEntity
    {
        [JsonIgnore]
        public virtual int MitigationWebhookId { get; set; }
        [JsonIgnore]
        public virtual MitigationWebhook MitigationWebhook { get; set; }
        public string Field { get; set; }
        public string ExpectedValue { get; set; }
        public bool HaveIBeenMet(MitigationAction action) => action.GetType().GetProperties()
                .Where(prop => prop.Name.ToLower() == Field.ToLower())
                .Any(prop => 
                    prop.GetValue(action).ToString().Equals(ExpectedValue)
                    || ExpectedValue.Equals("Any") || ExpectedValue.Equals("*")
                );        
    }
}
