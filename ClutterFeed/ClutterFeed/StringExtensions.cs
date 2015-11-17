/*   This file is part of ClutterFeed.
 *
 *    ClutterFeed is free software: you can redistribute it and/or modify
 *    it under the terms of the GNU General Public License as published by
 *    the Free Software Foundation, either version 3 of the License, or
 *    (at your option) any later version.
 *
 *    ClutterFeed is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 *    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *    GNU General Public License for more details.
 *
 *    You should have received a copy of the GNU General Public License
 *    along with ClutterFeed. If not, see <http://www.gnu.org/licenses/>.
 */

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
        public static bool Command(this String command, string wantedCommand)
        {
            return command.ToLower().Split(' ')[0].CompareTo(wantedCommand) == 0;
        }

        public static bool InsensitiveCompare(this String str, string comp)
        {
            if (str.ToLower().CompareTo(comp.ToLower()) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
