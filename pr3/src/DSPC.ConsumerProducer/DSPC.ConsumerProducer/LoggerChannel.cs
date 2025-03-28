using System.Collections.Concurrent;
using System.Text;
using System.Threading.Channels;

namespace DSPC.ConsumerProducer
{
    public class LoggerChannel : IDisposable
    {
        private StreamWriter _output;
        private ConcurrentQueue<ObjLog> _queue;
        private CancellationTokenSource _cancellationToken;
        private Semaphore semaphore;
        private Channel<ObjLog> _channel;
        private ChannelWriter<ObjLog> _writer;
        private ChannelReader<ObjLog> _reader;
        public string Filename { get; private set; }

        public LoggerChannel()
        {
            _channel = Channel.CreateUnbounded<ObjLog>();
            _writer = _channel.Writer;
            _reader = _channel.Reader;
            Filename = GenerateFileName();
            var logFile = new FileStream(Filename, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            semaphore = new Semaphore(1, 1);
            _output = new StreamWriter(logFile, Encoding.UTF8);
            _cancellationToken = new CancellationTokenSource();
            _queue = new ConcurrentQueue<ObjLog>();
        }

        public void LogMessage(string logLevel, string message)
        {
            ObjLog log = new ObjLog(logLevel, message);
            _writer.TryWrite(log);
            try
            {
                semaphore.Release();
            }
            catch
            {
            }
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
            _writer.Complete();
            _output.Flush();
            _output.Dispose();
        }
        public void Start()
        {
            Task.Run(() =>
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    _reader.TryRead(out var objLog);
                    if (objLog != null)
                    {
                        var logMessage = FromatMessage(DateTime.Now, objLog.LogLevel, objLog.Message);

                        _output.WriteLine(logMessage);
                        _output.Flush();
                    }
                    else
                    {
                        semaphore.WaitOne();
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
