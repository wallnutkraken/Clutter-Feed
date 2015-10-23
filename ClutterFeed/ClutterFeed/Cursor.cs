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
