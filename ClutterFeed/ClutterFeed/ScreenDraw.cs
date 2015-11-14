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
using System.Drawing;
using CursesSharp;

namespace ClutterFeed
{

    class ScreenDraw
    {
        public static bool IsFollowing { get; set; } = false; /* DON'T LOOK! */
        public static bool IsBlocked { get; set; } = false;

        public static string Version = "1.4";
        public static Window HeadLine { get; set; }
        public static Window Tweets { get; set; }

        /// <summary>
        /// Turns a number of seconds into a digital clock string
        /// </summary>
        /// <param name="time">The time to change (in seconds)</param>
        private static string ClockifyTime(int time)
        {
            if (time < 60)
            {
                if (time < 10)
                {
                    return "0:0" + time;
                }
                else
                {
                    return "0:" + time;
                }
            }
            else
            {
                int minutes = time / 60;
                int seconds = time - (minutes * 60);
                if (seconds < 10)
                {
                    return minutes + ":0" + seconds;
                }
                else
                {
                    return minutes + ":" + seconds;
                }
            }
        }
        public static void UpdateHeader()
        {
            string clock = ClockifyTime(Program.TimeLeft);
            HeadLine.Clear();
            HeadLine.AttrOn(Attrs.BOLD);
            HeadLine.Add("ClutterFeed version " + Version);
            string signOn = "Next update in: " + clock +  ". Signed on as: @" + GetUpdates.userScreenName;
            HeadLine.Add(0, (ScreenInfo.WindowWidth - signOn.Length - 1), signOn);
            HeadLine.Refresh();
            if (User.CounterConsoleWin != null)
            {
                User.CounterConsoleWin.Refresh();
            }
        }
        public void StartScreen()
        {
            HeadLine = new Window(1, ScreenInfo.WindowWidth, 0, 0);
            UpdateHeader();

            Tweets = new Window(ScreenInfo.WindowHeight - 3, ScreenInfo.WindowWidth, 1, 0);
        }
        /// <summary>
        /// Removes mentions from the list
        /// </summary>
        private List<InteractiveTweet> RemoveMentions(List<InteractiveTweet> allTweets)
        {
            List<InteractiveTweet> returnList = new List<InteractiveTweet>();

            for (int index = 0; index < allTweets.Count; index++)
            {
                if (allTweets[index].IsMention == false)
                {
                    returnList.Add(allTweets[index]);
                }
            }

            return returnList;
        }

        private void ShowInteractions(InteractiveTweet update)
        {
            int curx, cury;
            Tweets.GetCursorYX(out cury, out curx);
            if (update.IsFavorited && update.IsRetweeted) /* Adds a neat little symbol for rts/favs */
            {
                Tweets.Color = Colors.YELLOW;
                Tweets.Add(cury, 3, "★");
                Tweets.Color = Colors.GREEN;
                Tweets.Add(cury, 4, "➥ ");
                Tweets.Color = Colors.WHITE;
            }
            else if (update.IsFavorited)
            {
                Tweets.Color = Colors.YELLOW;
                Tweets.Add(cury, 3, "★  ");
                Tweets.Color = Colors.WHITE;
            }
            else if (update.IsRetweeted)
            {
                Tweets.AttrOn(Attrs.BOLD);
                Tweets.Color = Colors.GREEN;
                Tweets.Add(cury, 3, "➥  ");
                Tweets.AttrOff(Attrs.BOLD);
                Tweets.Color = Colors.WHITE;
            }
        }
        private Window menu { get; set; }
        private void MenuDrawInMiddle(string message)
        {
            int line = 0;
            int notNessecary;
            menu.GetCursorYX(out line, out notNessecary);
            menu.Add(line, (ScreenInfo.WindowWidth / 2) - (message.Length / 2), message);
        }
        private void MenuDrawInMiddle(string message, int line)
        {
            menu.Add(line, (ScreenInfo.WindowWidth / 2) - (message.Length / 2), message);
        }
        public Profile SelectUser()
        {
            menu = new Window(User.profiles.Count + 1, ScreenInfo.WindowWidth, (ScreenInfo.WindowHeight / 2) - (User.profiles.Count + 1) / 2, 0);
            int selected = 0;
            menu.Keypad = true;
            int pressedKey = 0;
            Curses.Newlines = true;
            do
            {
                menu.Color = 11;
                menu.Clear();
                MenuDrawInMiddle("Select the user:", 0);

                for (int index = 0; index < User.profiles.Count; index++)
                {
                    if (index == selected)
                    {
                        menu.Color = 21;
                        MenuDrawInMiddle(User.profiles[index].Name, index + 1);
                        menu.Color = 11;
                    }
                    else
                    {
                        MenuDrawInMiddle(User.profiles[index].Name, index + 1);
                    }
                }

                menu.Refresh();
                pressedKey = menu.GetChar();
                if (pressedKey == Keys.UP)
                {
                    if (selected == 0)
                    {
                        Console.Write('\a');
                    }
                    else
                    {
                        selected--;
                    }
                }
                else if (pressedKey == Keys.DOWN)
                {
                    if (selected == User.profiles.Count)
                    {
                        Console.Write('\a');
                    }
                    else
                    {
                        selected++;
                    }
                }
            } while (pressedKey != 10);

            menu.Clear();
            menu.Refresh();
            menu.Dispose();
            return User.profiles[selected];
        }

        public void ShowTimeline()
        {
            Tweets.Clear();
            Tweets.EnableScroll = true;
            List<InteractiveTweet> updates = RemoveMentions(GetUpdates.localTweetList);
            for (int index = updates.Count - 1; index >= 0; index--)
            {
                string longUpdate = updates[index].AuthorScreenName + ": " + updates[index].Contents;
                int maxX, maxY;
                Tweets.GetMaxYX(out maxY, out maxX);

                int splitter = maxX - 13;

                longUpdate = longUpdate.Replace("\n", " ");
                List<string> shortenedUpdate = longUpdate.SplitInParts(splitter).ToList();

                string cleanUserName = updates[index].AuthorScreenName.Remove(0, 1).ToLower();

                if (Friend.FriendsList != null && Friend.FriendsList.Contains(cleanUserName))
                {
                    Tweets.Color = 13; /* The color of friendship */
                }
                else
                {
                    Tweets.Color = 11; /* Regular identifier color */
                }

                Tweets.Add(updates[index].TweetIdentification + "    ");
                Tweets.Color = Colors.WHITE;
                bool identificationWritten = true;

                ShowInteractions(updates[index]);

                if (updates[index].AuthorScreenName.CompareTo("@" + GetUpdates.userScreenName) == 0)
                {
                    Tweets.AttrOn(Attrs.BOLD);
                    Tweets.Color = 14;
                }
                if (updates[index].Contents.Contains("@" + GetUpdates.userScreenName))
                {
                    Tweets.Color = 15;
                }

                for (int updateIndex = 0; updateIndex < shortenedUpdate.Count; updateIndex++)
                {
                    if (identificationWritten)
                    {
                        string[] tweetParts = shortenedUpdate[updateIndex].Split(':');
                        Tweets.AttrOn(Attrs.BOLD);
                        Tweets.Add(tweetParts[0] + ":");
                        Tweets.AttrOff(Attrs.BOLD);
                        Tweets.Add(tweetParts[1] + "\n");
                        identificationWritten = false;
                    }
                    else
                    {
                        Tweets.Add("      " + shortenedUpdate[updateIndex]);
                    }
                }
                Tweets.Color = Colors.WHITE;
                Tweets.AttrOff(Attrs.BOLD);
                Tweets.Add("\n");
            }
            Tweets.Refresh();
        }

        public void ShowUserProfile(TweetSharp.TwitterUser profile)
        {
            if (profile == null)
            {
                ScreenDraw.ShowMessage("Such a user does not exist");
                return;
            }
            Window showProfile = new Window(12, ScreenInfo.WindowWidth, (ScreenInfo.WindowHeight / 2) - 6, 0);

            if (IsFollowing)
            {
                string follow = "You are following";
                showProfile.Color = Colors.GREEN;
                showProfile.Add(0, (ScreenInfo.WindowWidth / 2) - (follow.Length / 2), follow);
                showProfile.Color = Colors.WHITE;
            }
            else if (IsBlocked)
            {
                string blocked = "User is blocked.";
                showProfile.Color = Colors.RED;
                showProfile.Add(0, (ScreenInfo.WindowWidth / 2) - (blocked.Length / 2), blocked);
                showProfile.Color = Colors.WHITE;
            }

            showProfile.Add(2, (ScreenInfo.WindowWidth / 2) - (profile.Name.Length / 2), profile.Name);
            if (Convert.ToBoolean(profile.IsVerified)) /* A verified symbol. Ish. */
            {
                showProfile.Color = 102;
                showProfile.Add(" ✓");
                showProfile.Color = Colors.WHITE;
            }

            string atName = "@" + profile.ScreenName;
            showProfile.Add(3, (ScreenInfo.WindowWidth / 2) - (atName.Length / 2), atName);

            int bioStartLine = 5;

            if (profile.Description.Length >= ScreenInfo.WindowWidth - 8)
            {
                int splitter = ScreenInfo.WindowWidth - 8; /* Dictates how many characters to wait until splitting the bio */
                List<string> splitBio = profile.Description.SplitInParts(splitter).ToList();
                for (int index = 0; index < splitBio.Count; index++) /* Writes the bio of the user */
                {
                    showProfile.Add(bioStartLine, 4, splitBio[index]);
                    bioStartLine++;
                }
            }
            else
            {
                showProfile.Add(bioStartLine, 4, profile.Description);
            }

            int urlLine = bioStartLine + 1;
            if (profile.Url != null)
            {
                showProfile.Color = 102;
                showProfile.Add(urlLine, (ScreenInfo.WindowWidth / 2) - (profile.Url.Length / 2), profile.Url);
                showProfile.Color = Colors.WHITE;
            }

            int infoBeltNameLine = urlLine + 2; /* Tweets, following, followers. You know! */

            /* Tweets */

            showProfile.Color = 11;
            showProfile.Add(infoBeltNameLine, 0, "Tweets:\n");
            showProfile.Color = Colors.WHITE;
            showProfile.Add(profile.StatusesCount + "");

            /* Following */

            string following = "Following:";
            showProfile.Color = 11;
            showProfile.Add(infoBeltNameLine, (ScreenInfo.WindowWidth / 2) - (following.Length / 2), following);
            showProfile.Color = Colors.WHITE;
            showProfile.Add(infoBeltNameLine + 1, (ScreenInfo.WindowWidth / 2) - (following.Length / 2), profile.FriendsCount + "");

            /* Followers */

            string followers = "Followers:";
            showProfile.Color = 11;
            showProfile.Add(infoBeltNameLine, (ScreenInfo.WindowWidth - followers.Length - 1), followers);
            showProfile.Color = Colors.WHITE;
            showProfile.Add(infoBeltNameLine + 1, (ScreenInfo.WindowWidth - followers.Length - 1), profile.FollowersCount + "");

            showProfile.Refresh();
            showProfile.GetChar();
            showProfile.Dispose();

            ScreenDraw draw = new ScreenDraw();
            draw.ShowTimeline();
        }

        private void CommandEplanation(string message, int top)
        {
            Console.SetCursorPosition((Console.WindowWidth - 6) - message.Length, top);
            Console.Write(message);
        }

        /// <summary>
        /// Draws the tweet on the screen
        /// </summary>
        /// <param name="tweet">tweet to draw</param>
        public void DrawTweet(InteractiveTweet tweet)
        {
            Curses.Echo = false;
            Window tweetShow = new Window(8, ScreenInfo.WindowWidth, 11, 0);
            string longUpdate = tweet.Contents;
            int splitter = 120;

            tweetShow.Color = 14;
            longUpdate = longUpdate.Replace("\n", " ");
            List<string> shortenedUpdate = longUpdate.SplitInParts(splitter).ToList();

            tweetShow.Add(tweet.AuthorScreenName + "\n");
            string atName = "( " + tweet.AuthorDisplayName + " )\n";
            tweetShow.Color = 11;
            tweetShow.Add(atName);
            tweetShow.Color = 14;

            for (int index = 0; index < shortenedUpdate.Count; index++) /* Draws the tweet nicely */
            {
                tweetShow.Add(shortenedUpdate[index] + "\n");
            }

            if (tweet.IsFavorited)
            {
                tweetShow.Color = Colors.YELLOW;
            }
            tweetShow.Add("Favorites: " + tweet.FavoriteCount + " ");
            tweetShow.Color = 14;

            if (tweet.IsRetweeted)
            {
                tweetShow.Color = Colors.GREEN;
            }
            tweetShow.Add("Retweets: " + tweet.RetweetCount + " ");
            tweetShow.Color = 14;

            tweetShow.Refresh();
            tweetShow.GetChar();
            tweetShow.Dispose();
            ShowTimeline();
        }

        private void DrawAtEnd(Window where, int line, string message)
        {
            where.Add(line, (ScreenInfo.WindowWidth - message.Length - 1), message);
        }
        public void ShowHelp()
        {

            Window help = new Window(20, ScreenInfo.WindowWidth, 5, 0);
            help.Color = 14;

            help.Add("/h, /help");
            DrawAtEnd(help, 0, "Shows this dialog\n");

            help.Add("/rt");
            DrawAtEnd(help, 1, "Retweets/undos a retweet on a selected tweet\n");

            help.Add("/fav, /f");
            DrawAtEnd(help, 2, "Favourites a selected tweet\n");

            help.Add("/unfav, /uf");
            DrawAtEnd(help, 3, "Unfavourites a selected tweet\n");

            help.Add("/api");
            DrawAtEnd(help, 4, "Shows the remaining API hits\n");

            help.Add("/r");
            DrawAtEnd(help, 5, "Replies to everyone in the selected tweet\n");

            help.Add("/rc");
            DrawAtEnd(help, 6, "Replies only to the author of the tweet\n");

            help.Add("/rn");
            DrawAtEnd(help, 7, "Replies without using @ at all\n");

            help.Add("/id");
            DrawAtEnd(help, 8, "Shows the ID of the tweet\n");

            help.Add("/profile");
            DrawAtEnd(help, 9, "Shows the profile of the selected user\n");

            help.Add("/me");
            DrawAtEnd(help, 10, "Shows your mentions\n");

            help.Add("/link");
            DrawAtEnd(help, 11, "Links you to a tweet\n");

            help.Add("/tweet");
            DrawAtEnd(help, 12, "Shows you details of a tweet\n");

            help.Add("/open");
            DrawAtEnd(help, 13, "Opens the tweet in browser (only from /tweet)\n");

            help.Add("/friend");
            DrawAtEnd(help, 14, "Adds/removes a friend\n");

            help.Add("/accounts");
            DrawAtEnd(help, 15, "Actions regarding twitter accounts\n");

            string enter = "Press ENTER to close this dialog";
            help.Add(18, (ScreenInfo.WindowWidth / 2) - (enter.Length / 2), enter);

            Curses.Echo = false;

            int keypress;
            do
            {
                keypress = help.GetChar();
            } while (keypress != 10);
        }

        public void ShowMentions()
        {
            List<InteractiveTweet> allTweets = GetUpdates.localTweetList;
            List<InteractiveTweet> mentions = new List<InteractiveTweet>();
            for (int index = 0; index < allTweets.Count; index++) /* Creates a mention-only tweet */
            {
                if (allTweets[index].IsMention)
                {
                    mentions.Add(allTweets[index]);
                }
            }

            int numberToDisplay = 14;
            if (mentions.Count < 15)
            {
                numberToDisplay = mentions.Count - 1;
            }
            int splitter = Console.WindowWidth - 12;



            for (int index = numberToDisplay; index >= 0; index--)
            {
                string tweetText = mentions[index].AuthorScreenName + ": " + mentions[index].Contents;
                tweetText = tweetText.Replace("\n", "\n      ");
                List<string> shortenedUpdate = tweetText.SplitInParts(splitter).ToList();

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write(mentions[index].TweetIdentification + "    ");
                Console.ForegroundColor = ConsoleColor.White;

                ShowInteractions(mentions[index]);

                for (int shorterTweetIndex = 0; shorterTweetIndex < shortenedUpdate.Count; shorterTweetIndex++)
                {
                    if (shorterTweetIndex > 0)
                    {
                        Console.Write("      ");
                    }
                    Console.WriteLine(shortenedUpdate[shorterTweetIndex]);
                }
                Console.WriteLine();
            }
        }

        public static void ShowMessage(string message)
        {
            Window errorMessage = new Window(3, ScreenInfo.WindowWidth, (ScreenInfo.WindowHeight / 2) - 1, 0);
            Curses.Echo = false;
            errorMessage.Color = 11;
            errorMessage.Add(1, (ScreenInfo.WindowWidth / 2) - (message.Length / 2), message);
            errorMessage.Color = Colors.WHITE;
            errorMessage.GetChar();
            errorMessage.Dispose();
            ScreenDraw drawer = new ScreenDraw();
            drawer.ShowTimeline();
        }

    }
}
