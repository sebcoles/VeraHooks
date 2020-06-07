using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace React.ViewModels
{
    public class NewHook
    {
        public MitigationWebhook Webhook { get; set; }
        public PropertyCondition[] Conditions { get; set; }
        public AppPoco[] Apps { get; set; }
    }
}
