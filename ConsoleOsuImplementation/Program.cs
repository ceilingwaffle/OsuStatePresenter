namespace ConsoleOsuImplementation
{
    using System;

    using DVPF.Core;

    using OsuStatePresenter;
    using OsuStatePresenter.Nodes;

    /// <summary>
    /// The console program.
    /// </summary>
    public class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static long lastTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        /// <summary>
        /// The main console method.
        /// </summary>
        public static void Main()
        {
            var presenter = new OsuPresenter(StateCreatedHandler);

            // handle value changed on a specific node
            if (presenter.TryGetNode(typeof(BpmNode), out Node bpmNode))
            {
                bpmNode.OnValueChange += (sender, e) => Console.WriteLine($@"BPM: {bpmNode.GetValue()}");
            }

            // start the Presenter
            presenter.Start();

            // keep the console open
            while (true)
            {
                Console.ReadKey();
                Console.WriteLine(@"Key pressed. Stopping Presenter...");
                presenter.Stop();
            }

            // ReSharper disable once FunctionNeverReturns
        }

        private static void StateCreatedHandler(State state)
        {
            long now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            Logger.Info("ΔT = {0} ms", now - lastTime);
            lastTime = now;

            // ReSharper disable once LocalizableElement
            Console.WriteLine("State created:\n{0}", state);
            Logger.Info("State created:\n{0}", state);
        }
    }
}
