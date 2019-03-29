using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuStatePresenter
{
    internal static class Helpers
    {
        static Random _rnd = new Random();

        internal static int Rand(int from, int to)
        {
            return _rnd.Next(from, to + 1);
        }

        /// <summary>
        /// Returns a random string from the given strings.
        /// </summary>
        /// <param name="strings">The strings to randomly choose one from.</param>
        /// <returns></returns>
        internal static string RandomStringFrom(params string[] strings)
        {
            int n = strings.Length;

            int randIndex = Rand(0, n - 1);

            return strings[randIndex];
        }
    }
}
