namespace DSPC.ConsumerProducer
{
    internal class Program
    {
        static void Main()
        {
            var logger = new LoggerChannel();
            logger.Start();

            Console.WriteLine($"Log file created: {Path.GetFullPath(logger.Filename)}");

            var a = new Producer(logger, "A");
            var b = new Producer(logger, "B");
            var c = new Producer(logger, "C");

            a.StartProduce();
            b.StartProduce();
            c.StartProduce();

            Console.WriteLine("Press <enter> to stop...");

            Console.ReadLine();

            //a.StopProduce();
            //b.StopProduce();
            //c.StopProduce();
            logger.Stop();
            logger.Dispose();

            Console.WriteLine("Done!");
        }
    }

    public class Producer
    {
        private readonly LoggerChannel _logger;
        private readonly string _name;
        private readonly Random _random;
        private bool _cancelled;

        public Producer(LoggerChannel logger, string name)
        {
            _random = new Random();

            _logger = logger;
            _name = name;
        }

        public void StartProduce()
        {
            var thread = new Thread(Produce);

            thread.Start();
        }

        public void StopProduce()
        {
            _cancelled = true;
        }

        private void Produce()
        {
            while (!_cancelled)
            {
                _logger.LogMessage("Debug", $"Message from the Producer {_name}");
                Thread.Sleep(_random.Next(50, 100));
            }
        }
    }
}
