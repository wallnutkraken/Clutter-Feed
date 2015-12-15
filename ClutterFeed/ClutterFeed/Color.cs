using CursesSharp;

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

        public static Color IdentifierColor { get; set; } = new Color();
        public static Color FriendColor { get; set; } = new Color();
        public static Color LinkColor { get; set; } = new Color();
        public static Color SelfColor { get; set; } = new Color();
        public static Color MentionColor { get; set; } = new Color();
        public static Color BackgroundColor { get; set; } = new Color();

        public static void SetColors()
        {
            IdentifierColor.Blue = Properties.Settings.Default.idBlue;
            IdentifierColor.Green = Properties.Settings.Default.idGreen;
            IdentifierColor.Red = Properties.Settings.Default.idRed;

            FriendColor.Red = Properties.Settings.Default.friendRed;
            FriendColor.Green = Properties.Settings.Default.friendGreen;
            FriendColor.Blue = Properties.Settings.Default.friendBlue;

            LinkColor.Blue = Properties.Settings.Default.linkBlue;
            LinkColor.Green = Properties.Settings.Default.linkGreen;
            LinkColor.Red = Properties.Settings.Default.linkRed;

            SelfColor.Red = Properties.Settings.Default.selfRed;
            SelfColor.Green = Properties.Settings.Default.selfGreen;
            SelfColor.Blue = Properties.Settings.Default.selfBlue;

            MentionColor.Blue = Properties.Settings.Default.mentionBlue;
            MentionColor.Green = Properties.Settings.Default.mentionGreen;
            MentionColor.Red = Properties.Settings.Default.mentionRed;

            BackgroundColor.Red = Properties.Settings.Default.bgRed;
            BackgroundColor.Green = Properties.Settings.Default.bgGreen;
            BackgroundColor.Blue = Properties.Settings.Default.bgBlue;

            Curses.InitColor(101, Color.IdentifierColor.Red, Color.IdentifierColor.Green, Color.IdentifierColor.Blue);
            Curses.InitColor(102, Color.LinkColor.Red, Color.LinkColor.Green, Color.LinkColor.Blue);
            Curses.InitColor(103, Color.FriendColor.Red, Color.FriendColor.Green, Color.FriendColor.Blue);
            Curses.InitColor(104, Color.SelfColor.Red, Color.SelfColor.Green, Color.SelfColor.Blue);
            Curses.InitColor(105, Color.MentionColor.Red, Color.MentionColor.Green, Color.MentionColor.Blue);
            Curses.InitColor(Colors.BLACK, Color.BackgroundColor.Red, Color.BackgroundColor.Green, Color.BackgroundColor.Blue);
        }
        
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
