using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Company.Function
{
    public static class DectalkApi
    {
        private static bool GenerateTTS(string phrase, string outPath, ILogger log, ExecutionContext context)
        {
            string directory = context.FunctionAppDirectory;
            string lib = Path.Combine(directory, "bin", "lib");
            string exePath = Path.Combine(lib, "say.exe");

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = exePath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.Arguments = $"-w out.wav \"{phrase}\"";
                process.StartInfo.WorkingDirectory = lib;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;

                process.EnableRaisingEvents = true;
                process.Refresh();
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                bool exited = process.WaitForExit(3000);

                if (!string.IsNullOrEmpty(output))
                {
                    log.LogInformation("received output: " + output);
                }
                if (!string.IsNullOrEmpty(error))
                {
                    log.LogError("received error: " + error);
                }
                return exited;
            }
            catch (Exception ex)
            {
                log.LogError($"Error running say.exe: {ex.ToString()}");
                return false;
            }
        }

        [FunctionName("Dectalk")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {

            string directory = context.FunctionAppDirectory;
            string lib = Path.Combine(directory, "bin", "lib");
            string phrase = req.Query["phrase"];

            log.LogInformation($"C# HTTP trigger function processed a request. (executionId: {context.InvocationId}, phrase: {phrase})");

            string filePath = Path.Combine(lib, "out.wav");
            bool success = GenerateTTS(phrase, filePath, log, context);

            if (!success)
            {
                log.LogError("failed to generate TTS after 3 seconds");
                return new BadRequestResult();
            }

            try
            {
                FileStream fs = File.OpenRead(filePath);
                return new FileStreamResult(fs, "audio/wav");
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
                return new BadRequestResult();
            }
        }
    }
}
