using System;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace NHibernateQueryableSample
{
    class Program
    {
        static readonly Uri _baseAddress = new Uri("http://localhost:50231/");

        static void Main(string[] args)
        {
            HttpSelfHostServer server = null;
            try
            {
                // Set up server configuration
                HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(_baseAddress);
                config.HostNameComparisonMode = HostNameComparisonMode.Exact;
                config.Routes.MapHttpRoute("default", "{controller}");
                config.Formatters.JsonFormatter.Indent = true;

                // Create server
                server = new HttpSelfHostServer(config);

                // Start listening
                server.OpenAsync().Wait();
                Console.WriteLine("Listening on " + _baseAddress);

                RunClient();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not start server: {0}", e.GetBaseException().Message);
            }
            finally
            {
                Console.WriteLine("Hit ENTER to exit...");
                Console.ReadLine();

                if (server != null)
                {
                    // Stop listening
                    server.CloseAsync().Wait();
                }
            }
        }

        /// <summary>
        /// This client issues requests against the CustomerController
        /// </summary>
        static void RunClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = _baseAddress;

            Console.WriteLine("GET top 2 customers from Redmond and having all orders with amount greater than or equal to 10...\n");
            HttpResponseMessage response = client.GetAsync("/Customers?$top=2&$skip=0&$orderby=Name desc, City asc&$filter=City eq 'Redmond' and Orders/all(o: o/Amount ge 10)").Result;
            response.EnsureSuccessStatusCode();
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        }
    }
}
