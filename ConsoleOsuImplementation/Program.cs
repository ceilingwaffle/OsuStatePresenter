using DVPF.Core;
using OsuStatePresenter;
using OsuStatePresenter.Nodes;
using System;

namespace ConsoleOsuImplementation
{
    class Program
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static long _lastTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        static void Main(string[] args)
        {
            OsuPresenter presenter = new OsuPresenter(StateCreatedHandler);

            // handle value changed on a specific node
            if (presenter.TryGetNode(typeof(BpmNode), out var bpmNode))
            {
                bpmNode.OnValueChange += (sender, e) => Console.WriteLine($"BPM: {bpmNode.GetValue()}");
            }

            // start the Presenter
            presenter.Start();

            // keep the console open
            while (true)
            {
                Console.ReadKey();
                Console.WriteLine("Key pressed. Stopping Presenter...");
                presenter.Stop();
            }
        }

        static void StateCreatedHandler(State state)
        {
            var now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            _logger.Info("ΔT = {0} ms", (now - _lastTime));
            _lastTime = now;

            Console.WriteLine("State created:\n{0}", state);
            _logger.Info("State created:\n{0}", state);
        }
    }
}
