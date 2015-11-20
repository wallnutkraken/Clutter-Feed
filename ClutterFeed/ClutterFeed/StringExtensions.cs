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

        /// <summary>
        /// Splits a string into parts so that seperate words are not longer than partLength
        /// </summary>
        /// <param name="partLength">length of the split</param>
        public static List<string> SplitWords(this String origin, int partLength)
        {
            if (origin == null)
            {
                throw new ArgumentNullException("The string used is null");
            }
            if (partLength <= 0)
            {
                throw new ArgumentException("partLength has to be a positive integer");
            }
            char[] splitters = { ' ' };
            string[] words = origin.Split(' ');
            List<string> lines = new List<string>();
            int splitCount = 0;
            for (int index = 0; index < words.Length; index++)
            {
                if (index == 0)
                {
                    lines.Add(words[index]);
                }
                else if (lines[splitCount].Length + words[index].Length >= partLength)
                {
                    splitCount++;
                    lines.Add(words[index]);
                }
                else
                {
                    lines[splitCount] = lines[splitCount] + " " + words[index];
                }
            }

            return lines;
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

        /// <summary>
        /// Splits the string into where it has newlines, and then splits them more if the lines are longer than split
        /// </summary>
        /// <param name="split">number of characters to split at</param>
        /// <returns></returns>
        public static string[] PartNewlineSplit(this String s, int split)
        {
            string[] lines = s.Split('\n');
            List<string> splittedList = new List<string>();
            for (int index = 0; index < lines.Length; index++)
            {
                if (lines[index].Length > split)
                {
                    List<string> temp = lines[index].SplitWords(split);
                    foreach (string line in temp)
                    {
                        splittedList.Add(line);
                    }
                }
                else
                {
                    splittedList.Add(lines[index]);
                }
            }
            return splittedList.ToArray();
        }

    }
}
