using System;
using System.Collections.Generic;

namespace ClutterFeed
{
    static class StringExtensions
    {
        /// <summary>
        /// Splits a string into parts every partLength number of characters
        /// </summary>
        /// <param name="s">the string to split</param>
        /// <param name="partLength">Number of how many characters between splits</param>
        /// <returns></returns>
        public static IEnumerable<String> SplitInParts(this String s, Int32 partLength)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", "partLength");

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

    }
}
