using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Avepoint.Open311.Samples
{
    class Program
    {

        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        static void Main(string[] args)
        {
            string baseAddress = "https://api.citizenservices.org";
            string jurisdictionId = "mycity02";
            string format = "xml";
            string id = "ave_buildingrequest";
            string accessToken = "MDA3YWQxZDM3MzI2YjQzMDJkZTU1YzcwNmE1ODFlOTE=+L";

            var t = Task.Run(async () =>
            {
                using (var client = new DataServiceClient(baseAddress, jurisdictionId, format, _cancellationTokenSource.Token))
                {
                    client.Timeout = new TimeSpan(0, 20, 0);

                    ConsoleColor fc = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("***********************************************************");
                    Console.WriteLine("*             Query Service Type Definitions              *");
                    Console.WriteLine("***********************************************************");
                    Console.ForegroundColor = fc;
                    NewLine();
                    Console.WriteLine($"{baseAddress}/{jurisdictionId}/api/beta/services.{format}");
                    string services = await client.GetServicesAsync();
                    Console.WriteLine(services);
                    NewLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("***********************************************************");
                    Console.WriteLine("*             Query Service Type Details                  *");
                    Console.WriteLine($"* Service TYpe ={id}");
                    Console.WriteLine("***********************************************************");

                    Console.WriteLine("Please press any key to continue.");
                    Console.ReadKey();
                    NewLine();
                    Console.ForegroundColor = fc;
                    Console.WriteLine($"{baseAddress}/{jurisdictionId}/api/beta/services/{id}.{format}");
                    NewLine();
                    string service = await client.GetServiceByIdAsync(id);
                    Console.WriteLine(service);
                    NewLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("***********************************************************");
                    Console.WriteLine("*             Query Existing Service Requests             *");
                    Console.WriteLine("***********************************************************");
                    Console.WriteLine("Please press any key to cntinue.");
                    Console.ReadKey();
                    NewLine();
                    Console.ForegroundColor = fc;

                    var startDate = DateTime.UtcNow.AddDays(-20).ToString("o");
                    var endDate = DateTime.UtcNow.ToString("o");
                    var status = "open";
                    string queryString = $"start_date={startDate}&end_date={endDate}&status={status}";
                    Console.WriteLine($"{baseAddress}/{jurisdictionId}/api/beta/requests.{format}?{queryString}");
                    NewLine();
                    string requests = await client.GetServiceRequestsAsync(queryString);
                    Console.WriteLine(requests);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("***********************************************************");
                    Console.WriteLine("*             Create a New Service Request                *");
                    Console.WriteLine("***********************************************************");
                    Console.WriteLine("Please press any key to cntinue.");
                    Console.ReadKey();
                    NewLine();
                    Console.ForegroundColor = fc;
                    var kvps = new Dictionary<string, string>();
                    kvps.Add("api_key", accessToken);
                    kvps.Add("service_code", id);
                    kvps.Add("lat", "47.640568390488625");
                    kvps.Add("long", "-122.1293731033802");
                    kvps.Add("address_string", "Microsoft Way, Redmond, WA 98052");
                    kvps.Add("email", "api.tester@outlook.com");
                    kvps.Add("first_name", "api");
                    kvps.Add("last_name", "tester");
                    kvps.Add("phone", "1234567890");
                    kvps.Add("description", "create a test requet to report");
                    kvps.Add("media_url", "https://e311production.blob.core.windows.net/mycity02/RequestIcons/Icon636203206875418480.jpg");
                    kvps.Add("attribute[ave_buildingrequestisgetnoticecode]", "true");
                    Console.WriteLine($"{baseAddress}/{jurisdictionId}/api/beta/requests.{format}");
                    NewLine();
                    string requestSummary = await client.PostServiceRequest(kvps);
                    Console.WriteLine(requestSummary);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Please press any key to Quit.");
                    Console.ForegroundColor = fc;
                    Console.ReadKey();

                }
            }, _cancellationTokenSource.Token);


              t.Wait();
        }

        private static void NewLine()
        {
            Console.WriteLine(Environment.NewLine);
        }
    }

    public class DataServiceClient : IDisposable
    {
        private readonly HttpClient client;
        private readonly string jurisdictionId;
        private readonly string format;
        private readonly CancellationToken token;
        public TimeSpan Timeout
        {
            get
            {
                return client.Timeout;
            }
            set
            {
                client.Timeout = value;
            }
        }

        public DataServiceClient(string baseAddress, string jurisdiction_id, string format, CancellationToken token)
        {
            client = new HttpClient { BaseAddress = new Uri(baseAddress) };
            jurisdictionId = jurisdiction_id;
            this.format = format;
            this.token = token;
        }

        public async Task<string> GetServicesAsync()
        {
            var response = await client.GetAsync($"/{jurisdictionId}/api/beta/services.{format}", token);

            if ("xml".Equals(format, StringComparison.OrdinalIgnoreCase))
            {
                return XDocument.Parse(await response.Content.ReadAsStringAsync()).ToString();
            }
            return JToken.Parse(await response.Content.ReadAsStringAsync()).ToString(Newtonsoft.Json.Formatting.Indented);
        }

        public async Task<string> GetServiceByIdAsync(string id)
        {
            var response = await client.GetAsync($"/{jurisdictionId}/api/beta/services/{id}.{format}", token);

            if ("xml".Equals(format, StringComparison.OrdinalIgnoreCase))
            {
                return XDocument.Parse(await response.Content.ReadAsStringAsync()).ToString();
            }
            return JToken.Parse(await response.Content.ReadAsStringAsync()).ToString(Newtonsoft.Json.Formatting.Indented);
        }

        public async Task<string> GetServiceRequestsAsync(string queryString)
        {
            var response = await client.GetAsync($"/{jurisdictionId}/api/beta/requests.{format}?{queryString}", token);

            if ("xml".Equals(format, StringComparison.OrdinalIgnoreCase))
            {
                return XDocument.Parse(await response.Content.ReadAsStringAsync()).ToString();
            }
            return JToken.Parse(await response.Content.ReadAsStringAsync()).ToString(Newtonsoft.Json.Formatting.Indented);
        }

        public async Task<string> GetServiceRequestByIdAstync(string id)
        {
            var response = await client.GetAsync($"/{jurisdictionId}/api/beta/requests/{id}.{format}", token);

            if ("xml".Equals(format, StringComparison.OrdinalIgnoreCase))
            {
                return XDocument.Parse(await response.Content.ReadAsStringAsync()).ToString();
            }
            return JToken.Parse(await response.Content.ReadAsStringAsync()).ToString(Newtonsoft.Json.Formatting.Indented);
        }

        public async Task<string> PostServiceRequest(IEnumerable<KeyValuePair<string, string>> request)
        {
            var response = await client.PostAsync($"/{jurisdictionId}/api/beta/requests.{format}", new FormUrlEncodedContent(request));

            if ("xml".Equals(format, StringComparison.OrdinalIgnoreCase))
            {
                return XDocument.Parse(await response.Content.ReadAsStringAsync()).ToString();
            }
            return JToken.Parse(await response.Content.ReadAsStringAsync()).ToString(Newtonsoft.Json.Formatting.Indented);
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
