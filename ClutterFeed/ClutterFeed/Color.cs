﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ClutterFeed
{
    class Color
    {
        
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

        public static Color IdentifierColor { get; set; } = null;
        public static Color FriendColor { get; set; } = null;
        public static Color LinkColor { get; set; } = null;
        public static Color SelfColor { get; set; } = null;
        public static Color MentionColor { get; set; } = null;

        public enum Pairs
        {
            Identifier = 11,
            Link,
            Friend,
            Self,
            Mention
        }
    }
}
