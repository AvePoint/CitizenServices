using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvePoint.CitizenServices.WebhookSample.Models
{
    public class RequestWebhookMessage
    {
        [JsonProperty("service_request_id")]
        public string RequestId { get; set; }
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}