using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function
{
    public static class HttpTriggerCSharp1
    {
        [FunctionName("HttpTriggerCSharp1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            // string path = @""
            // new FileStream()
            Console.WriteLine("The current directory is {0}", Directory.GetCurrentDirectory());
            // var bytes = await GetDecryptedByteArrayAsync(blobStream);
            var bytes = new byte[10] {1, 0, 1, 1, 1, 1, 1, 0, 1, 1};
            return new FileContentResult(bytes, "application/octet-stream");
        }
    }
}
