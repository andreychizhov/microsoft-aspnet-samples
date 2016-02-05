using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Jobs;
using PhluffyShuffyWebData;

namespace PhluffyShuffyCleanup
{
    public class Cleanup
    {
        // The number of days a shuffle is kept in the system before we delete it
        private const int RetentionPolicyDays = 2;

        private readonly IImageStorage storage;

        public Cleanup(IImageStorage storage)
        {
            if (storage == null)
            {
                throw new ArgumentNullException("storage");
            }

            this.storage = storage;
        }

        // This job will only triggered on demand
        [Description("Cleaup function")]
        public static void CleanupFunction(IBinder binder)
        {
            Cleanup cleanupClass = new Cleanup(new AzureImageStorage(binder.AccountConnectionString));
            cleanupClass.DoCleanup();
        }

        public void DoCleanup()
        {
            IEnumerable<string> oldShuffles = this.storage.GetShufflesOlderThan(DateTime.UtcNow.AddDays(-RetentionPolicyDays));
            foreach(string shuffle in oldShuffles)
            {
                Console.WriteLine("Deleting old container {0}", shuffle);
                this.storage.Delete(shuffle);
            }
        }
    }
}
