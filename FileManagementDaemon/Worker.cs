using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileManagementDaemon
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private string path;
        private readonly string filter;

        public Worker(IConfiguration configuration, ILogger<Worker> logger)
        {
            path = configuration["path"];
            filter = configuration["filter"];
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var watcher = new FileSystemWatcher(path)
            {
                NotifyFilter = NotifyFilters.Attributes
                    | NotifyFilters.CreationTime
                    | NotifyFilters.DirectoryName
                    | NotifyFilters.FileName
                    | NotifyFilters.LastAccess
                    | NotifyFilters.LastWrite
                    | NotifyFilters.Security
                    | NotifyFilters.Size,
                Filter = filter
            };

            watcher.Created += OnCreated;
            watcher.EnableRaisingEvents = true;

            await Task.CompletedTask;
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            File.Delete(e.FullPath);
        }
    }
}
