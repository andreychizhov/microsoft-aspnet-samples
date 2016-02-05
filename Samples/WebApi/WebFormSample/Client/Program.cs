using System;
using System.Net.Http;
using System.Text;

namespace Client
{
    /// <summary>
    /// This sample client tries out the ContentController exposed by the ContentController project in this solution.
    /// It first issues a PUT request and then a GET request.
    /// </summary>
    class Program
    {
        static readonly Uri _addres = new Uri("http://localhost:65291/api/values");

        static void RunClient()
        {
            HttpClient client = new HttpClient();

            // Issue GET request against Web API
            client.GetAsync(_addres).ContinueWith(
                (getTask) =>
                {
                    if (getTask.IsCanceled)
                    {
                        return;
                    }
                    if (getTask.IsFaulted)
                    {
                        throw getTask.Exception;
                    }
                    HttpResponseMessage getResponse = getTask.Result;

                    getResponse.EnsureSuccessStatusCode();

                    string result = getResponse.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("Received response: {0}", result);
                });
        }

        static void Main(string[] args)
        {
            RunClient();

            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }
    }
}
