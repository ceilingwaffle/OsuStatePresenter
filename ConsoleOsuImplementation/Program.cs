using DVPF.Core;
using OsuStatePresenter;
using System;

namespace ConsoleOsuImplementation
{
    class Program
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static long _lastTime = 0;

        static void Main(string[] args)
        {
            _lastTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            Presenter presenter = new Presenter(StateCreatedHandler);

            presenter.Start();

            // keep the console open
            while (true)
            {
                Console.ReadKey();
                presenter.Stop();
            }
        }

        static void StateCreatedHandler(State state)
        {
            var now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            _logger.Info("ΔT = {0} ms", (now - _lastTime));
            _lastTime = now;

            // TODO: Do something other than log the state
            _logger.Info("State created:\n{0}", state);

        }
    }
}
