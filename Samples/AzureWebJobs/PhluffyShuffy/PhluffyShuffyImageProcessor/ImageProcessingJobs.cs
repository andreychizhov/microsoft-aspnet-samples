using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using Microsoft.WindowsAzure.Jobs;
using Microsoft.WindowsAzure.Storage.Blob;
using PhluffyShuffyWebData;

namespace PhluffyShuffyImageProcessor
{
    public class ImageProcessingJobs
    {
        private static Random r = new Random();

        private readonly IImageStorage storage;

        public ImageProcessingJobs()
            : this(new AzureImageStorage(ConfigurationManager.ConnectionStrings["AzureJobsData"].ConnectionString))
        {
        }

        public ImageProcessingJobs(IImageStorage storage)
        {
            if (storage == null)
            {
                throw new ArgumentNullException("storage");
            }

            this.storage = storage;
        }


        /// <summary>
        /// Waits for message from the queue and creates shuffles
        /// </summary>
        /// <param name="shufflerequests">The message received from queue</param>
        public static void CreateShuffle(
            [QueueInput] ShuffleRequestMessage shufflerequests,
            IBinder binder)
        {
            string shuffleId = shufflerequests.ShuffleId;

            ImageProcessingJobs processor = new ImageProcessingJobs();
            string shuffle = processor.CreateShuffle(shuffleId);

            Stream shuffleBlob = binder.Bind<Stream>(new BlobOutputAttribute("shuffle" + shuffleId + @"/shuffle.jpg"));

            using (Stream localShuffle = File.OpenRead(shuffle))
            {
                localShuffle.CopyTo(shuffleBlob);
            }
        }

        public string CreateShuffle(string shuffleId)
        {
            ImageProcessingJobs processor = new ImageProcessingJobs();

            IEnumerable<Uri> shuffleParts = this.storage.GetAllShuffleParts(shuffleId);
            Tuple<string, IEnumerable<string>> localImages = LoadImages(shuffleParts);
            return CreateThumbanils(localImages.Item1, localImages.Item2);
        }

        private string CreateThumbanils(string p, IEnumerable<string> enumerable)
        {
            using (MultiThumbnailGenerator generator = new MultiThumbnailGenerator())
            {
                foreach (string imagePath in enumerable)
                {
                    Image img = Image.FromFile(imagePath);
                    generator.AddImage(img);
                }

                string resultName = p + "_result.jpg";
                using (FileStream fs = File.OpenWrite(resultName))
                {
                    generator.WriteJpgToStream(fs);
                }

                return resultName;
            }
        }

        private Tuple<string, IEnumerable<string>> LoadImages(IEnumerable<Uri> partLinks)
        {
            string tempName = Guid.NewGuid().ToString("N");

            List<string> parts = new List<string>();

            foreach (Uri partUri in partLinks)
            {
                WebClient client = new WebClient();

                string fileName = tempName + Path.GetFileName(partUri.ToString());
                client.DownloadFile(partUri, fileName);

                parts.Add(fileName);
            }

            return new Tuple<string, IEnumerable<string>>(tempName, parts);
        }
    }
}
