using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Jobs;

namespace PhluffyShuffyImageProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            JobHost jobHost = new JobHost();
            jobHost.RunAndBlock();
        }
    }
}
