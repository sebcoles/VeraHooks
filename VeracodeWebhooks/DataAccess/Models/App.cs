using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using VeracodeService.Models;

namespace DataAccess.Models
{
    public class App : BaseEntity
    {

        [JsonIgnore]
        public int MitigationWebhookId { get; set; }
        [JsonIgnore]
        public MitigationWebhook MitigationWebhook { get; set; }
        
        public int AppId { get; set; }
        public string AppName { get; set; }
    
        [JsonIgnore]
        public string LastBuild { get; set; }
    
        [JsonIgnore]
        public string FlawString { get; set; }

    }
}
