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

        public const string Version = "1.6-devel";
        public const string VersionName = "Leaping Bomb";
        public static Window HeadLine { get; set; }
        public static Window Tweets { get; set; }

        public static void UpdateHeader()
        {
            HeadLine.Clear();
            HeadLine.AttrOn(Attrs.BOLD);
            HeadLine.Add("ClutterFeed | " + VersionName + " (" + Version + ")");
            string signOn = "Signed on as: @" + GetUpdates.userScreenName;
            if (Settings.AFK)
            {
                signOn = "(AFK) | " + signOn;
            }
            HeadLine.Add(0, (ScreenInfo.WindowWidth - signOn.Length - 1), signOn);
            HeadLine.Refresh();
            if (User.CounterConsoleWin != null)
            {
                User.CounterConsoleWin.Refresh();
            }
        }
        public void StartScreen()
        {
            /* Headline initalization moved to Program.cs because there is a null reference exception */
            /* that happens if you don't have internet access when starting ClutterFeed */
            UpdateHeader();

            Tweets = new Window(ScreenInfo.WindowHeight - 3, ScreenInfo.WindowWidth, 1, 0);
        }
        /// <summary>
        /// Removes mentions from the list
        /// </summary>
        private List<InteractiveTweet> GetOnlyTweets()
        {
            List<InteractiveTweet> allTweets = GetUpdates.localTweetList;
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

        private List<InteractiveTweet> GetOnlyMentions()
        {
            List<InteractiveTweet> mens = new List<InteractiveTweet>();
            foreach (InteractiveTweet tweet in GetUpdates.localTweetList)
            {
                if (tweet.IsMention)
                {
                    mens.Add(tweet);
                }
            }
            return mens;
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
            TimerMan.Pause();
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
            if (Settings.AFK == false)
            {
                TimerMan.Resume();
            }
            return User.profiles[selected];
        }

        public void ShowTimeline()
        {
            Tweets.Clear();
            Tweets.EnableScroll = true;
            List<InteractiveTweet> updates = GetOnlyTweets();
            for (int index = updates.Count - 1; index >= 0; index--)
            {
                string longUpdate = updates[index].AuthorScreenName + ": " + updates[index].Contents;
                int maxX, maxY;
                Tweets.GetMaxYX(out maxY, out maxX);

                int splitter = maxX - 8;

                List<string> shortenedUpdate = longUpdate.PartNewlineSplit(splitter).ToList();

                string cleanUserName = updates[index].AuthorScreenName.Remove(0, 1).ToLower();

                if (Friend.FriendsList != null && Friend.FriendsList.Contains(cleanUserName))
                {
                    Tweets.Color = (int)Color.Pairs.Friend; /* The color of friendship */
                }
                else
                {
                    Tweets.Color = (int)Color.Pairs.Identifier; /* Regular identifier color */
                }

                Tweets.Add(updates[index].TweetIdentification + "    ");
                Tweets.Color = Colors.WHITE;
                if (updates[index].IsDirectMessage)
                {
                    int x, y;
                    Tweets.GetCursorYX(out y, out x);
                    Tweets.AttrOn(Attrs.BOLD);
                    Tweets.Color = (int)Color.Pairs.Self;
                    Tweets.Add(y, 3, "DM ");
                    Tweets.AttrOff(Attrs.BOLD);
                }
                bool identificationWritten = true;

                ShowInteractions(updates[index]);

                if (updates[index].AuthorScreenName.CompareTo("@" + GetUpdates.userScreenName) == 0)
                {
                    Tweets.AttrOn(Attrs.BOLD);
                    Tweets.Color = (int)Color.Pairs.Self;
                }
                char[] splitInTwo = { ':' };
                if (longUpdate.Split(splitInTwo, 2)[1].Contains("@" + GetUpdates.userScreenName))
                {
                    Tweets.Color = (int)Color.Pairs.Mention;
                }

                for (int updateIndex = 0; updateIndex < shortenedUpdate.Count; updateIndex++)
                {
                    if (identificationWritten)
                    {
                        char[] splitChar = { ' ' };
                        string[] tweetParts = shortenedUpdate[updateIndex].Split(splitChar, 2);
                        Tweets.AttrOn(Attrs.BOLD);
                        Tweets.Add(tweetParts[0] + " ");
                        Tweets.AttrOff(Attrs.BOLD);
                        Tweets.Add(tweetParts[1] + "\n");
                        identificationWritten = false;
                    }
                    else
                    {
                        Tweets.Add("      " + shortenedUpdate[updateIndex] + "\n");
                    }
                }
                Tweets.Color = Colors.WHITE;
                Tweets.AttrOff(Attrs.BOLD);
                if (Settings.NoSquash == true)
                {
                    Tweets.Add("\n");
                }
            }
            Tweets.Refresh();
        }

        public void ShowUserProfile(TweetSharp.TwitterUser profile)
        {
            TimerMan.Pause();
            if (profile == null)
            {
                ScreenDraw.ShowMessage("Such a user does not exist");
                return;
            }
            Window showProfile = new Window(12, ScreenInfo.WindowWidth, (ScreenInfo.WindowHeight / 2) - 6, 0);
            showProfile.Box((int)'|', (int)'-');
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
                List<string> splitBio = profile.Description.SplitWords(splitter).ToList();
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
            if (Settings.AFK == false)
            {
                TimerMan.Resume();
            }

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
            TimerMan.Pause();
            Curses.Echo = false;
            Window tweetShow = new Window(8, ScreenInfo.WindowWidth, 11, 0);
            tweetShow.Box((int)'|', (int)'-');
            string longUpdate = tweet.Contents;
            int splitter = 120;
            int line = 1;

            tweetShow.Color = (int)Color.Pairs.Self;
            longUpdate = longUpdate.Replace("\n", " ");
            List<string> shortenedUpdate = longUpdate.SplitWords(splitter).ToList();

            tweetShow.Add(line, 2, tweet.AuthorScreenName);
            line++;
            string atName = "( " + tweet.AuthorDisplayName + " )";
            tweetShow.Color = (int)Color.Pairs.Identifier;
            tweetShow.Add(line, 2, atName);
            line++;
            tweetShow.Color = (int)Color.Pairs.Self;

            for (int index = 0; index < shortenedUpdate.Count; index++) /* Draws the tweet nicely */
            {
                tweetShow.Add(line, 2, shortenedUpdate[index]);
                line++;
            }

            if (tweet.IsFavorited)
            {
                tweetShow.Color = Colors.YELLOW;
            }
            tweetShow.Add(line, 2, "Favorites: " + tweet.FavoriteCount + " ");
            tweetShow.Color = (int)Color.Pairs.Self;

            if (tweet.IsRetweeted)
            {
                tweetShow.Color = Colors.GREEN;
            }
            tweetShow.Add("Retweets: " + tweet.RetweetCount + " ");
            line++;
            tweetShow.Color = (int)Color.Pairs.Self;

            tweetShow.Add("\t");
            if (DateTime.UtcNow.Subtract(tweet.TimePosted).Days > 1)
            {
                tweetShow.Add(tweet.TimePosted.ToShortDateString() + " | ");
            }
            tweetShow.Add(tweet.TimePosted.ToShortTimeString() + " | ");
            TimeSpan postedAgo = DateTime.UtcNow.Subtract(tweet.TimePosted);
            string ago = "";
            if (postedAgo.TotalSeconds < 60.0)
            {
                ago = postedAgo.Seconds + "s ago";
            }
            else if (postedAgo.TotalMinutes < 60.0)
            {
                ago = postedAgo.Minutes + "m ago";
            }
            else if (postedAgo.TotalHours < 24.0)
            {
                ago = postedAgo.Hours + "h ago";
            }
            tweetShow.Add(ago);

            tweetShow.Refresh();
            tweetShow.GetChar();
            tweetShow.Dispose();
            ShowTimeline();
            if (Settings.AFK == false)
            {
                TimerMan.Resume();
            }
        }

        private void DrawAtEnd(Window where, int line, string message)
        {
            where.Add(line, (ScreenInfo.WindowWidth - message.Length - 3), message);
        }
        public void ShowHelp()
        {
            TimerMan.Pause();
            Window help = new Window(23, ScreenInfo.WindowWidth, 4, 0);
            help.Color = (int)Color.Pairs.Self;

            int num = 2;
            help.Add(num, 2,"/h, /help");
            DrawAtEnd(help, num, "Shows this dialog\n");
            num++;

            help.Add(num, 2, ("/rt"));
            DrawAtEnd(help, num, "Retweets/undos a retweet on a selected tweet\n");
            num++;

            help.Add(num, 2,"/fav, /f");
            DrawAtEnd(help, num, "Favourites a selected tweet\n");
            num++;

            help.Add(num, 2,"/unfav, /uf");
            DrawAtEnd(help, num, "Unfavourites a selected tweet\n");
            num++;

            help.Add(num, 2,"/r");
            DrawAtEnd(help, num, "Replies to everyone in the selected tweet/DM\n");
            num++;

            help.Add(num, 2,"/rc");
            DrawAtEnd(help, num, "Replies only to the author of the tweet\n");
            num++;

            help.Add(num, 2,"/rn");
            DrawAtEnd(help, num, "Replies without using @ at all\n");
            num++;

            help.Add(num, 2,"/profile");
            DrawAtEnd(help, num, "Shows the profile of the selected user\n");
            num++;

            help.Add(num, 2,"/me");
            DrawAtEnd(help, num, "Shows your mentions\n");
            num++;

            help.Add(num, 2,"/link");
            DrawAtEnd(help, num, "Links you to a tweet\n");
            num++;

            help.Add(num, 2,"/tweet");
            DrawAtEnd(help, num, "Shows you details of a tweet\n");
            num++;

            help.Add(num, 2,"/friend");
            DrawAtEnd(help, num, "Adds/removes a friend\n");
            num++;

            help.Add(num, 2,"/accounts");
            DrawAtEnd(help, num, "Actions regarding twitter accounts\n");
            num++;

            help.Add(num, 2,"/follow");
            DrawAtEnd(help, num, "Follows or unfollows the selected user\n");
            num++;

            help.Add(num, 2,"/block");
            DrawAtEnd(help, num, "Blocks or unblocks the selected user\n");
            num++;

            help.Add(num, 2, "/afk");
            DrawAtEnd(help, num, "Toggles the auto-update timer on and off\n");
            num++;

            help.Add(num, 2, "/dm");
            DrawAtEnd(help, num, "Sends a Direct Message to someone\n");
            num++;

            help.Color = 11;
            string enter = "Press ENTER to close this dialog";
            help.Add(num, (ScreenInfo.WindowWidth / 2) - (enter.Length / 2), enter);

            help.Color = Colors.WHITE;
            help.Box((int)'|', (int)'-');

            Curses.Echo = false;

            int keypress;
            do
            {
                keypress = help.GetChar();
            } while (keypress != 10);
            if (Settings.AFK == false)
            {
                TimerMan.Resume();
            }
        }

        public void ShowMentions()
        {
            List<InteractiveTweet> mentions = GetOnlyMentions();
            Tweets.EnableScroll = true;
            Tweets.Clear();
            for (int index = mentions.Count - 1; index >= 0; index--)
            {
                string longUpdate = mentions[index].AuthorScreenName + ": " + mentions[index].Contents;

                int splitter = ScreenInfo.WindowWidth - 8;
                
                List<string> shortenedUpdate = longUpdate.PartNewlineSplit(splitter).ToList();

                string cleanUserName = mentions[index].AuthorScreenName.Remove(0, 1).ToLower();

                if (Friend.FriendsList != null && Friend.FriendsList.Contains(cleanUserName))
                {
                    Tweets.Color = 13; /* The color of friendship */
                }
                else
                {
                    Tweets.Color = 11; /* Regular identifier color */
                }

                Tweets.Add(mentions[index].TweetIdentification + "    ");
                Tweets.Color = Colors.WHITE;
                bool identificationWritten = true;

                ShowInteractions(mentions[index]);

                if (mentions[index].AuthorScreenName.CompareTo("@" + GetUpdates.userScreenName) == 0)
                {
                    Tweets.AttrOn(Attrs.BOLD);
                    Tweets.Color = 14;
                }
                if (mentions[index].Contents.Contains("@" + GetUpdates.userScreenName))
                {
                    Tweets.Color = 15;
                }

                for (int updateIndex = 0; updateIndex < shortenedUpdate.Count; updateIndex++)
                {
                    if (identificationWritten)
                    {
                        char[] splitChar = { ' ' };
                        string[] tweetParts = shortenedUpdate[updateIndex].Split(splitChar, 2);
                        Tweets.AttrOn(Attrs.BOLD);
                        Tweets.Add(tweetParts[0] + " ");
                        Tweets.AttrOff(Attrs.BOLD);
                        Tweets.Add(tweetParts[1] + "\n");
                        identificationWritten = false;
                    }
                    else
                    {
                        Tweets.Add("      " + shortenedUpdate[updateIndex] + "\n");
                    }
                }
                Tweets.Color = Colors.WHITE;
                Tweets.AttrOff(Attrs.BOLD);
                if (Settings.NoSquash == true)
                {
                    Tweets.Add("\n");
                }
            }
            Tweets.Refresh();
        }

        public static void ShowMessage(string message)
        {
            TimerMan.Pause();
            Window errorMessage = new Window(3, ScreenInfo.WindowWidth, (ScreenInfo.WindowHeight / 2) - 1, 0);
            errorMessage.Box((int)'|', (int)'-');
            Curses.Echo = false;
            errorMessage.Color = 11;
            errorMessage.Add(1, (ScreenInfo.WindowWidth / 2) - (message.Length / 2), message);
            errorMessage.Color = Colors.WHITE;
            errorMessage.GetChar();
            Curses.DoUpdate();
            if (Settings.AFK == false)
            {
                TimerMan.Resume();
            }
            ScreenDraw drawer = new ScreenDraw();
            drawer.ShowTimeline();
        }

        public static void ShowMessage(string message, bool noRefresh)
        {
            if (noRefresh)
            {
                TimerMan.Pause();
                Window errorMessage = new Window(3, ScreenInfo.WindowWidth, (ScreenInfo.WindowHeight / 2) - 1, 0);
                errorMessage.Box((int)'|', (int)'-');
                Curses.Echo = false;
                errorMessage.Color = 11;
                errorMessage.Add(1, (ScreenInfo.WindowWidth / 2) - (message.Length / 2), message);
                errorMessage.Color = Colors.WHITE;
                errorMessage.GetChar();
                errorMessage.Dispose();
                if (Settings.AFK == false)
                {
                    TimerMan.Resume();
                }
            }
            else
            {
                ShowMessage(message);
            }
        }

    }
}
