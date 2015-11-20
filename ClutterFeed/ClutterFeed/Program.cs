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
using System.Threading;
using CursesSharp;

namespace ClutterFeed
{
    public static class ScreenInfo
    {
        public static int WindowHeight = 30;
        public static int WindowWidth = 120;
    }
    class Program
    {
        private static short[] color_table = {
            Colors.RED, Colors.BLUE, Colors.GREEN, Colors.CYAN,
            Colors.RED, Colors.MAGENTA, Colors.YELLOW, Colors.WHITE
        };

        public static int TimeLeft { get; set; }
        public static Timer UpdateTimer { get; set; }
        static void Main(string[] args)
        {
            Curses.InitScr();
            Curses.Newlines = true;
            Curses.ResizeTerm(ScreenInfo.WindowHeight, ScreenInfo.WindowWidth);
            if (User.ConfigExists() == false)
            {
                User.SetUnsetColorsToDefaults();
                Settings.RefreshSeconds = 300;
            }
            else
            {
                User config = new User();
                config.GetConsts();
                config.FindColors();
            }
            TimeLeft = Settings.RefreshSeconds;

            Curses.InitColor(101, Color.IdentifierColor.Red, Color.IdentifierColor.Green, Color.IdentifierColor.Blue);
            Curses.InitColor(102, Color.LinkColor.Red, Color.LinkColor.Green, Color.LinkColor.Blue);
            Curses.InitColor(103, Color.FriendColor.Red, Color.FriendColor.Green, Color.FriendColor.Blue);
            Curses.InitColor(104, Color.SelfColor.Red, Color.SelfColor.Green, Color.SelfColor.Blue);
            Curses.InitColor(105, Color.MentionColor.Red, Color.MentionColor.Green, Color.MentionColor.Blue);


            if (Curses.HasColors)
            {
                Curses.StartColor();
                for (short i = 1; i < 8; ++i)
                    Curses.InitPair(i, color_table[i], Colors.BLACK);
            }
            else
            {
                Curses.EndWin();
                Console.WriteLine("Color support not found");
                Environment.Exit(0);
            }

            Curses.InitPair(11, 101, Colors.BLACK);
            Curses.InitPair(12, 102, Colors.BLACK);
            Curses.InitPair(13, 103, Colors.BLACK);
            Curses.InitPair(14, 104, Colors.BLACK);
            Curses.InitPair(15, 105, Colors.BLACK);

            Curses.InitPair(21, Colors.BLACK, 101);

            Actions twitterDo = new Actions();
            twitterDo.SetUpTwitter();

            TimerCallback call = twitterDo.RefreshTweets;
            UpdateTimer = new Timer(call, null, 0, 1000);
            twitterDo.ActionStart();
            UpdateTimer.Dispose();

            ScreenDraw.HeadLine.Dispose();
            ScreenDraw.Tweets.Dispose();

            Curses.EndWin();

        }
    }
}
