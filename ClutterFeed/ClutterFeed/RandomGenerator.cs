using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClutterFeed
{
    static class RandomGenerator
    {
        private static object locker = new object();
        private static Random seedGenerator = new Random(Environment.TickCount);

        public static int GetRandomNumber(int minVal, int maxVal)
        {
            int seed;

            lock (locker)
            {
                seed = seedGenerator.Next(int.MinValue, int.MaxValue);
            }

            var random = new Random(seed);

            return random.Next(minVal, maxVal);
        }
    }
}
