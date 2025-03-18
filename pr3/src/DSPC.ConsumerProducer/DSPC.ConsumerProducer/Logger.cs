using System.Collections.Concurrent;
using System.Text;

namespace DSPC.ConsumerProducer
{
    public class Logger : IDisposable
    {
        private StreamWriter _output;
        private ConcurrentQueue<ObjLog> _queue;
        private CancellationTokenSource _cancellationToken;
        public string Filename { get; private set; }

        public Logger()
        {
            Filename = GenerateFileName();

            var logFile = new FileStream(Filename, FileMode.CreateNew, FileAccess.Write, FileShare.Read);

            _output = new StreamWriter(logFile, Encoding.UTF8);
            _cancellationToken = new CancellationTokenSource();
            _queue = new ConcurrentQueue<ObjLog>();
        }

        public void LogMessage(string logLevel, string message)
        {
            ObjLog log = new ObjLog(logLevel, message);
            _queue.Enqueue(log);
        }
        private string FromatMessage(DateTime time, string logLevel, string message)
        {
            return $"{time:dd/MM/yyyy HH:mm:ss.fff} [{logLevel}] {message}";
        }

        private string GenerateFileName()
        {
            return $"logfile_{DateTime.Now:ddMM_HHmmss_fff}.log";
        }

        public void Dispose()
        {
            _output.Flush();
            _output.Dispose();
        }
        public void Start()
        {
            Task.Run(() =>
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    _queue.TryDequeue(out var objLog);
                    if (objLog != null)
                    {
                        var logMessage = FromatMessage(DateTime.Now, objLog.LogLevel, objLog.Message);

                        _output.WriteLine(logMessage);
                        _output.Flush();
                    }
                }
                Console.WriteLine("end");
            });
        }
        public void Stop()
        {
            _cancellationToken.Cancel();
        }
    }
}
