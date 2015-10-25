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

        public Cursor GetCursorPosition()
        {
            Cursor currentPos = new Cursor();
            currentPos.X = Console.CursorLeft;
            currentPos.Y = Console.CursorTop;

            return currentPos;
        }
    }
    class ScreenSize
    {
        public int Left { get; set; }
        public int Top { get; set; }
    }
}
