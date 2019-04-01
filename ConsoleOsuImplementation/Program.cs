using DVPF.Core;
using OsuStatePresenter;
using System;

namespace ConsoleOsuImplementation
{
    class Program
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Presenter presenter = new Presenter(StateCreatedHandler);

            presenter.Start();

            // keep the console open
            while (true)
            {
                Console.ReadKey();
            }
        }

        static void StateCreatedHandler(State state)
        {
            // TODO: Do something other than log the state
            _logger.Info("State created:\n{0}", state);
        }
    }
}
