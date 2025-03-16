using System.Text;

namespace DSPC.ConsumerProducer
{
    public class Logger : IAsyncDisposable
    {
        private StreamWriter _output;

        public string Filename { get; private set; }

        public Logger()
        {
            Filename = GenerateFileName();

            var logFile = new FileStream(Filename, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            
            _output = new StreamWriter(logFile, Encoding.UTF8);
        }

        public void LogMessage(string logLevel, string message)
        {
            var logMessage = FromatMessage(DateTime.Now, logLevel, message);

            _output.WriteLine(logMessage);
            _output.Flush();
        }

        private string FromatMessage(DateTime time, string logLevel, string message)
        {
            return $"{time:dd/MM/yyyy HH:mm:ss.fff} [{logLevel}] {message}";
        }

        private string GenerateFileName()
        {
            return $"logfile_{DateTime.Now:ddMM_HHmmss_fff}.log";
        }

        public async ValueTask DisposeAsync()
        {
            await _output.FlushAsync();
            await _output.DisposeAsync();
        }
    }
}
