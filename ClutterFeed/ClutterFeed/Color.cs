using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClutterFeed
{
    class Color
    {
        public enum CustomColors
        {
            Identifier = 101,
            Link = 102,
            Friend = 103,
            Self = 104
        }
        public short Red { get; set; }
        public short Green { get; set; }
        public short Blue { get; set; }

        public Color(short r, short g, short b)
        {
            Red = r;
            Green = g;
            Blue = b;
        }

        public Color() : this(0, 0, 0)
        {

        }

        public static Color IdentifierColor { get; set; }
        public static Color FriendColor { get; set; }
        public static Color LinkColor { get; set; }
        public static Color SelfColor { get; set; }
    }
}
