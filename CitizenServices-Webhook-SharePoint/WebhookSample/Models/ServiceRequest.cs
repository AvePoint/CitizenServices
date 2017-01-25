using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvePoint.CitizenServices.WebhookSample.Models
{
    public class ServiceRequest
    {
        [JsonProperty("service_request_id")]
        public string RequestId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("service_name")]
        public string ServiceType { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("lat")]
        public double latitude { get; set; }
        [JsonProperty("long")]
        public double longitude { get; set; }
        [JsonProperty("requested_datetime")]
        public DateTime RequestedDate { get; set; }
        [JsonProperty("expected_datetime")]
        public DateTime DueDate { get; set; }
    }
}