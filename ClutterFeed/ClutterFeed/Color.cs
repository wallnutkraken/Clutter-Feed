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

        public static Color IdentifierColor { get; set; }
        public static Color FriendColor { get; set; }
        public static Color LinkColor { get; set; }
        public static Color SelfColor { get; set; }
        public static Color MentionColor { get; set; }
        public static Color BackgroundColor { get; set; }

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
