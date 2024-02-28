using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SpawnProcessOnUbuntu.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;

        public MainController(ILogger<MainController> logger)
        {
            _logger = logger;
        }

        [HttpGet("SpwanProcess")]
        public IActionResult SpawnProcess()
        {
            using (var process = Process.Start(new ProcessStartInfo("dotnet")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            })) 
            {


                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        Console.WriteLine(args.Data);
                    }
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        Console.WriteLine(args.Data);
                    }

                    Console.WriteLine(args.Data);
                };


                process.Exited += (sender, args) =>
                {

                };
            }

            return Ok();
        }

        [HttpPost("Convert")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> ConvertFile([FromForm] UploadFileModel input)
        {
            var guid = Guid.NewGuid();
            var dir = $"{Environment.CurrentDirectory}/videos";
            var outputDir = $"{Environment.CurrentDirectory}/output-videos/{guid}";

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // TODO: use file id instead of guid
            var filePath = $"{dir}/{guid}.mp4";

            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                input.File.CopyTo(fileStream);
                await fileStream.FlushAsync();
            }

            using (var process = Process.Start(new ProcessStartInfo("ffmpeg")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                // TODO: use file id in output naming
                Arguments = $"-i {filePath} -b:v 1M -g 60 -hls_time 2 -hls_list_size 0 -hls_segment_size 500000 {outputDir}/output.m3u8",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            }))
            {
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        Console.WriteLine(args.Data);
                    }
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        Console.WriteLine(args.Data);
                    }

                    Console.WriteLine(args.Data);
                };

                process.Exited += (sender, args) =>
                {
                    if (process.ExitCode == 0)
                    {

                    }
                };
            }

                


            return Ok();
        }
    }

    public class UploadFileModel
    {
        // TODO: get playlist id file id etc.

        public IFormFile File  { get; set; }
    }
}
