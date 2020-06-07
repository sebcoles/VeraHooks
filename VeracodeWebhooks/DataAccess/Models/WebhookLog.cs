using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Models
{
    public class WebhookLog : BaseEntity
    {
        public string WebHookName { get; set; }
        public string WebHookSendAddress { get; set; }
        public DateTime TimeFired { get; set; }
        public int StatusReturned { get; set; }
        public string MessageSent { get; set; }
        public string Response { get; set; }
        public string AppName { get; set; }

        public override string ToString()
        {
            return $"{TimeFired} : {WebHookName} was fired at {WebHookSendAddress} and recieved as HTTP {StatusReturned}.";
        }
    }
}
