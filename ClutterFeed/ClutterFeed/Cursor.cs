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
    class Cursor
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Cursor GetPosition()
        {
            Cursor currentPos = new Cursor();
            currentPos.X = Console.CursorLeft;
            currentPos.Y = Console.CursorTop;

            return currentPos;
        }
        public void SetPosition(Cursor cursorPosition)
        {
            Console.SetCursorPosition(cursorPosition.X, cursorPosition.Y);
        }

        /// <summary>
        ///  Moves the cursor down by one character
        /// </summary>
        public void MoveDown()
        {
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop + 1);
        }

        /// <summary>
        /// Moves the cursor down by a number of characters
        /// </summary>
        /// <param name="times">Number of times to move down</param>
        public void MoveDown(int times)
        {
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop + times);
        }

        /// <summary>
        ///  Moves the cursor up by one character
        /// </summary>
        public void MoveUp()
        {
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
        }

        /// <summary>
        /// Moves the cursor up by a number of characters
        /// </summary>
        /// <param name="times">Number of times to move up</param>
        public void MoveUp(int times)
        {
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - times);
        }

        /// <summary>
        /// Moves the cursor left by one character
        /// </summary>
        public void MoveLeft()
        {
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }

        /// <summary>
        /// Moves the cursor left by a number of characters
        /// </summary>
        /// <param name="times">Number of times to move left</param>
        public void MoveLeft(int times)
        {
            Console.SetCursorPosition(Console.CursorLeft - times, Console.CursorTop);
        }

        /// <summary>
        /// Moves the cursor right by one character
        /// </summary>
        public void MoveRight()
        {
            Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
        }

        /// <summary>
        /// Moves the cursor right by a number of characters
        /// </summary>
        /// <param name="times">Number of times to move left</param>
        public void MoveRight(int times)
        {
            Console.SetCursorPosition(Console.CursorLeft + times, Console.CursorTop);
        }
    }
    class ScreenSize
    {
        public int Left { get; set; }
        public int Top { get; set; }
    }
}
