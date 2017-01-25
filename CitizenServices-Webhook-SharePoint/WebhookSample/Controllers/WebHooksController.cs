using AvePoint.CitizenServices.WebhookSample.Models;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;
using System.Web.Http;

namespace AvePoint.CitizenServices.WebhookSample.Controllers
{
    public class WebHooksController : ApiController
    {
        [HttpPost]
        public async Task RequestAssigned()
        {
            //Get request ID from web hook message
            string webhookRequest = await this.Request.Content.ReadAsStringAsync();
            RequestWebhookMessage message = JsonConvert.DeserializeObject<RequestWebhookMessage>(webhookRequest);
            
            //Get request detail by API
            var serviceRequest = await GetServiceRequest(message.RequestId);

            //Add request to a SharePoint task list
            AddToTaskList(serviceRequest);
        }

        private static void AddToTaskList(ServiceRequest serviceRequest)
        {
            string siteUrl = ConfigurationManager.AppSettings["SharePointSite"];
            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                string username = ConfigurationManager.AppSettings["SharePointUser"];
                string password = ConfigurationManager.AppSettings["SharePointUserPassword"];

                var securePassword = new SecureString();
                foreach (var c in password) { securePassword.AppendChar(c); }
                clientContext.Credentials = new SharePointOnlineCredentials(username, securePassword);

                List taskList = clientContext.Web.Lists.GetByTitle("CitizenServiceRequests");
                clientContext.Load(taskList.Fields);
                clientContext.ExecuteQuery();

                ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                ListItem oListItem = taskList.AddItem(itemCreateInfo);

                oListItem["Title"] = serviceRequest.RequestId;
                oListItem["StartDate"] = serviceRequest.RequestedDate;
                oListItem["DueDate"] = serviceRequest.DueDate;

                oListItem.Update();
                clientContext.ExecuteQuery();
            }
        }

        private async Task<ServiceRequest> GetServiceRequest(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiAddress = ConfigurationManager.AppSettings["APIAddress"];

                string requestUrl = apiAddress + $"/beta/requests/{id}.json";

                string requestJson = await client.GetStringAsync(requestUrl);

                return JsonConvert.DeserializeObject<ServiceRequest>(requestJson);
            }
        }
    }
}
