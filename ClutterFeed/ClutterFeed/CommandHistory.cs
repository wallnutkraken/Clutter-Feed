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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClutterFeed
{
    class CommandHistory
    {
        private static List<string> CommandBuffer = new List<string>();
        public static string HeldCommand { get; set; }

        /// <summary>
        /// Adds a string to the beggining of the Command Buffer
        /// </summary>
        public static void Add(string line) /* This is separate because it makes */
        {                                   /* the program easier to debug */
            try
            {
                if (CommandBuffer[0].CompareTo(line) != 0)
                {
                    CommandBuffer.Insert(0, line);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                CommandBuffer.Add(line);
            }
        }

        /// <summary>
        /// Gets the maximum CURRENT possible index of the Command Buffer.
        /// </summary>
        /// <returns></returns>
        public static int MaxIndex()
        {
            return CommandBuffer.Count - 1;
        }

        public static string GetCommand(int index)
        {
            return CommandBuffer[index];
        }

        public static void RemoveLatest()
        {
            CommandBuffer.RemoveAt(0);
        }

        public static void RemoveEmpties()
        {
            for (int index = 0; index < CommandBuffer.Count; index++)
            {
                if (CommandBuffer[index].CompareTo("") == 0)
                {
                    CommandBuffer.RemoveAt(index);
                }
            }
        }

    }
}
