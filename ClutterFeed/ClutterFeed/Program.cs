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
        public static int ConfigSetTimeout { get; set; }
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
                ConfigSetTimeout = 301;
            }
            else
            {
                User config = new User();
                ConfigSetTimeout = config.GetTimeout() + 1;
                config.FindColors();
            }
            TimeLeft = ConfigSetTimeout;

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

            string command = "/fullupdate";
            AutoResetEvent autoEvent = new AutoResetEvent(false);
            TimerCallback call = twitterDo.RefreshTweets;
            
            UpdateTimer = new Timer(call, null, 0, 1000);
            do
            {
                ActionValue commandMetadata = new ActionValue();

                if (command.StartsWith("/"))
                {

                    StatusCommunication newTweet = new StatusCommunication();
                    if ((command.ToLower().CompareTo("/fullupdate") == 0) || (command.ToLower().CompareTo("/fu") == 0))
                    {
                        commandMetadata = twitterDo.Update(command, true);
                    }

                    else if ((command.ToLower().CompareTo("/update") == 0) || (command.ToLower().CompareTo("/u") == 0))
                    {
                        commandMetadata = twitterDo.Update(command);
                    }

                    else if (command.InsensitiveCompare("/accounts"))
                    {
                        commandMetadata = twitterDo.ProfileSelection();
                    }

                    else if (command.Split(' ')[0].CompareTo("/r") == 0)
                    {
                        commandMetadata = twitterDo.ReplyGeneric(command);
                    }

                    else if (command.Split(' ')[0].CompareTo("/id") == 0)
                    {
                        commandMetadata = twitterDo.GetID(command);
                    }

                    else if (command.Split(' ')[0].CompareTo("/friend") == 0)
                    {
                        commandMetadata = twitterDo.AddFriend(command);
                    }

                    else if (command.Split(' ')[0].CompareTo("/link") == 0)
                    {
                        commandMetadata = twitterDo.TweetLink(command);
                    }

                    else if (command.Split(' ')[0].CompareTo("/rn") == 0)
                    {
                        commandMetadata = twitterDo.ReplyQuiet(command);
                    }

                    else if (command.Split(' ')[0].CompareTo("/rc") == 0)
                    {
                        commandMetadata = twitterDo.ReplyClean(command);
                    }

                    else if (command.Split(' ')[0].CompareTo("/rt") == 0)
                    {
                        commandMetadata = twitterDo.Retweet(command);
                    }

                    else if ((command.Split(' ')[0].CompareTo("/fav") == 0) || (command.Split(' ')[0].CompareTo("/f") == 0))
                    {
                        commandMetadata = twitterDo.FavoriteTweet(command);
                    }

                    else if ((command.Split(' ')[0].ToLower().CompareTo("/del") == 0) || (command.Split(' ')[0].ToLower().CompareTo("/d") == 0))
                    {
                        commandMetadata = twitterDo.RemoveTweet(command);
                    }

                    else if (command.Split(' ')[0].ToLower().CompareTo("/profile") == 0)
                    {
                        try
                        {
                            commandMetadata = twitterDo.ShowProfile(command);
                        }
                        catch (NullReferenceException exceptionInfo)
                        {
                            ScreenDraw.ShowMessage(exceptionInfo.Message + "\n");
                            commandMetadata = new ActionValue();
                        }
                    }

                    else if (command.Split(' ')[0].ToLower().CompareTo("/tweet") == 0)
                    {
                        commandMetadata = twitterDo.ShowTweet(command);
                    }

                    else if (command.Split(' ')[0].ToLower().CompareTo("/me") == 0)
                    {
                        commandMetadata = twitterDo.Mentions(command);
                    }

                    else if (command.Split(' ')[0].ToLower().CompareTo("/help") == 0 || command.Split(' ')[0].ToLower().CompareTo("/h") == 0)
                    {
                        commandMetadata = twitterDo.Help();
                    }

                    else if (command.ToLower().Contains("/api"))
                    {
                        commandMetadata = twitterDo.ApiInfo();
                    }

                }
                /* End of commands */

                if (command.ToLower().StartsWith("/") == false) /* EXCEPT for this one */
                {
                    commandMetadata = twitterDo.NewTweet(command);
                }
                if (commandMetadata.AskForCommand)
                {
                    command = User.CounterConsole();
                }
                else
                {
                    if (commandMetadata.OverrideCommand)
                    {
                        command = commandMetadata.OverrideCommandString;
                    }
                    else
                    {
                        command = "/u";
                    }
                    commandMetadata.AskForCommand = true;
                    Thread.Sleep(200);
                }
            } while ((!command.ToLower().StartsWith("/q")));

            UpdateTimer.Dispose();
            ScreenDraw.HeadLine.Dispose();
            ScreenDraw.Tweets.Dispose();

            Curses.EndWin();

        }
    }
}
