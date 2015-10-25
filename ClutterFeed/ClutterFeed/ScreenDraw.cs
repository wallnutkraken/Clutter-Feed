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

namespace ClutterFeed
{
    class ScreenDraw
    {
        public static bool IsFollowing { get; set; } = false; /* DON'T LOOK! */
        public static bool IsBlocked { get; set; } = false;

        public static void ShowTimeline()
        {
            List<InteractiveTweet> updates = GetUpdates.localTweetList;
            int index;
            if (updates.Count > 14)
            {
                index = 14;
            }
            else
            {
                index = updates.Count - 1;
            }
            for (; index >= 0; index--)
            {
                string longUpdate = updates[index].AuthorName + ": " + updates[index].Contents;


                int splitter = Console.WindowWidth - 13;

                longUpdate = longUpdate.Replace("\n", "\n      ");
                List<string> shortenedUpdate = longUpdate.SplitInParts(splitter).ToList();

                SetScreenColor.SetColor(ConsoleColor.DarkMagenta, 66, 140, 187); /* Makes the DarkMagenta color blue-ish */
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write(updates[index].TweetIdentification + "    ");
                Console.ForegroundColor = ConsoleColor.White;
                bool identificationWritten = true;

                if (updates[index].IsFavorited && updates[index].IsRetweeted) /* Adds a neat little symbol for rts/favs */
                {
                    Cursor cursorPosition = new Cursor();
                    cursorPosition = cursorPosition.GetCursorPosition();
                    Console.SetCursorPosition((cursorPosition.X - 3), cursorPosition.Y);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("▀");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("]");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (updates[index].IsFavorited)
                {
                    Cursor cursorPosition = new Cursor();
                    cursorPosition = cursorPosition.GetCursorPosition();
                    Console.SetCursorPosition((cursorPosition.X - 3), cursorPosition.Y);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[▀]");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (updates[index].IsRetweeted)
                {
                    Cursor cursorPosition = new Cursor();
                    cursorPosition = cursorPosition.GetCursorPosition();
                    Console.SetCursorPosition((cursorPosition.X - 3), cursorPosition.Y);

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("[▀]");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                if (updates[index].AuthorName.CompareTo("@" + GetUpdates.userScreenName) == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                if (updates[index].Contents.Contains("@" + GetUpdates.userScreenName))
                {
                    SetScreenColor.SetColor(ConsoleColor.DarkBlue, 255, 179, 64); /* Makes DarkBlue orange-ish */
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                }

                for (int updateIndex = 0; updateIndex < shortenedUpdate.Count; updateIndex++)
                {
                    if (identificationWritten)
                    {
                        Console.WriteLine(shortenedUpdate[updateIndex]);
                        identificationWritten = false;
                    }
                    else
                    {
                        Console.WriteLine("      " + shortenedUpdate[updateIndex]);
                    }
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();

            }
        }

        public void ShowUserProfile(TweetSharp.TwitterUser profile)
        {
            Console.Clear();

            ScreenSize screenInfo = new ScreenSize();
            screenInfo.Left = Console.WindowWidth;
            screenInfo.Top = Console.WindowHeight;

            if (IsFollowing)
            {
                string follow = "You are following";
                Console.SetCursorPosition((screenInfo.Left / 2) - (follow.Length / 2), 0);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(follow);
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (IsBlocked)
            {
                string blocked = "User is blocked.";
                Console.SetCursorPosition((screenInfo.Left / 2) - (blocked.Length / 2), 0);
                ScreenDraw.ShowError(blocked);
            }

            Console.SetCursorPosition((screenInfo.Left / 2) - (profile.Name.Length / 2), 2);
            Console.Write(profile.Name);
            if (Convert.ToBoolean(profile.IsVerified)) /* A verified symbol. Ish. */
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta; /* This is not Dark Magenta, see SetScreenColor.cs */
                Console.Write(" ¤");
                Console.ForegroundColor = ConsoleColor.White;
            }


            Console.SetCursorPosition((screenInfo.Left / 2) - ((profile.ScreenName.Length + 1) / 2), 3);
            Console.Write("@" + profile.ScreenName);
            if (Convert.ToBoolean(profile.IsProtected))
            {
                Console.ForegroundColor = ConsoleColor.DarkGray; /* This, however, is still Dark Gray */
                Console.Write(" ×");
                Console.ForegroundColor = ConsoleColor.White;
            }

            int bioStartLine = Console.CursorTop + 2;

            if (profile.Description.Length >= screenInfo.Left - 8)
            {
                Console.SetCursorPosition(0, bioStartLine); /* Makes sure the bio starts on the right line */
                int splitter = Console.WindowWidth - 8; /* Dictates how many characters to wait until splitting the bio */
                List<string> splitBio = profile.Description.SplitInParts(splitter).ToList();
                for (int index = 0; index < splitBio.Count; index++) /* Writes the bio of the user */
                {
                    Console.SetCursorPosition((screenInfo.Left / 2) - (splitBio[index].Length / 2), Console.CursorTop);
                    Console.Write(splitBio[index]);
                    Console.SetCursorPosition(0, Console.CursorTop + 1); /* Moves the cursor a line down basically */
                }
            }
            else
            {
                Console.SetCursorPosition((screenInfo.Left / 2) - (profile.Description.Length / 2), bioStartLine);
                Console.Write(profile.Description);
            }

            Console.SetCursorPosition(0, Console.CursorTop + 1);
            if (profile.Url != null)
            {
                Console.SetCursorPosition((screenInfo.Left / 2) - (profile.Url.Length / 2), Console.CursorTop);
                /* The line above doesn't change the "top" position because the last iteration of the above loop did just that */
                Console.ForegroundColor = ConsoleColor.DarkMagenta; /* Again, not magenta */
                Console.Write(profile.Url);
                Console.ForegroundColor = ConsoleColor.White;
            }

            int infoBeltNameLine = /*screenInfo.Top -*/ Console.CursorTop + 2; /* Tweets, following, followers. You know! */

            Console.SetCursorPosition(4, infoBeltNameLine); /* Okay, this might be a bit confusing */
            string tweets = "Tweets:\n    "; /* After the word tweets, there is a newline, so in the end, it'll look like */
            Console.Write(tweets + profile.StatusesCount); /* Tweets: */
                                                           /* 32521 */

            /* ...At least, I hope */
            string following = "Following:";
            Console.SetCursorPosition(((screenInfo.Left - 4) / 2) - (following.Length / 2), infoBeltNameLine);
            int followingLeft = Console.CursorLeft;
            Console.Write(following);
            Console.SetCursorPosition(followingLeft, infoBeltNameLine + 1);
            Console.Write(profile.FriendsCount);

            string followers = "Followers:";
            Console.SetCursorPosition((screenInfo.Left - followers.Length) - 4, infoBeltNameLine);
            int followersLeft = Console.CursorLeft;
            Console.Write(followers);
            Console.SetCursorPosition(followersLeft, infoBeltNameLine + 1);
            Console.Write(profile.FollowersCount);

            Console.SetCursorPosition(0, Console.CursorTop + 4);
        }

        private void CommandEplanation(string message, int top)
        {
            Console.SetCursorPosition((Console.WindowWidth - 6) - message.Length, top);
            Console.Write(message);
        }
        public void ShowHelp()
        {
            Console.Clear();
            int linestart = 6;

            Console.SetCursorPosition(linestart, 3);
            Console.Write("/h, /help");
            CommandEplanation("Shows this dialog.", 3);

            Console.SetCursorPosition(linestart, 4);
            Console.Write("/n, /new, no command");
            CommandEplanation("Posts a new tweet", 4);

            Console.SetCursorPosition(linestart, 5);
            Console.Write("/rt");
            CommandEplanation("Retweets/undos a retweet on a selected tweet.", 5);

            Console.SetCursorPosition(linestart, 6);
            Console.Write("/fav, /f");
            CommandEplanation("Favourites a selected tweet", 6);

            Console.SetCursorPosition(linestart, 7);
            Console.Write("/unfav, /uf");
            CommandEplanation("Unfavourites a selected tweet", 7);

            Console.SetCursorPosition(linestart, 8);
            Console.Write("/api");
            CommandEplanation("Shows the remaining API hits", 8);

            Console.SetCursorPosition(linestart, 9);
            Console.Write("/r");
            CommandEplanation("Replies to the selected tweet", 9);

            Console.SetCursorPosition(linestart, 10);
            Console.Write("/rn");
            CommandEplanation("Replies without using @", 10);

            Console.SetCursorPosition(linestart, 11);
            Console.Write("/id");
            CommandEplanation("Shows the ID of the tweet", 11);

            Console.SetCursorPosition(linestart, 12);
            Console.Write("/profile");
            CommandEplanation("Shows the profile of the selected user", 12);

            Console.SetCursorPosition(linestart, 13);
            Console.Write("/me");
            CommandEplanation("Shows your mentions", 13);

            string enter = "Press ENTER to close this dialog";
            Console.SetCursorPosition((Console.WindowWidth / 2) - (enter.Length / 2), Console.WindowHeight - 1);

            Console.CursorVisible = false;

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(enter);
            Console.ForegroundColor = ConsoleColor.White;

            char keypress = ' ';
            do
            {
                keypress = Console.ReadKey(true).KeyChar;
            } while (keypress != '\r');

            Console.CursorVisible = true;
        }

        public void DrawMentions(List<TweetSharp.TwitterStatus> mentions)
        {
            Console.Clear();

            int numberToDisplay = 14;
            if (mentions.Count < 15)
            {
                numberToDisplay = mentions.Count - 1;
            }
            int splitter = Console.WindowWidth - 6;

            

            for (int index = numberToDisplay; index >=0; index--)
            {
                string tweetText = "@" + mentions[index].Author.ScreenName + ": " + mentions[index].Text;
                tweetText = tweetText.Replace("\n", "\n   ");
                List<string> shortenedUpdate = tweetText.SplitInParts(splitter).ToList();

                for(int shorterTweetIndex = 0; shorterTweetIndex < shortenedUpdate.Count; shorterTweetIndex++)
                {
                    Console.WriteLine("   " + shortenedUpdate[shorterTweetIndex]);
                }
                Console.WriteLine();
            }
        }
        
        public static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("      " + message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void ShowSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("      " + message);
            Console.ForegroundColor = ConsoleColor.White;
        }

    }
}
