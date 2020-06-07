using DataAccess.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using VeracodeService.Models;

namespace WebhookLogic.Models
{
    public class WebhookMessage
    {
        public DateTime TimeSent { get; }

        public WebhookMessage()
        {
            TimeSent = DateTime.Now;
        }
        public MitigationAction Mitigation { get; set; }
        public App App { get; set; }
    }

    public class SlackHook
    {
        public string Text { get; set; }
    }
}
