﻿/*   This file is part of ClutterFeed.
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
            if (update.IsFavorited && update.IsRetweeted) /* Adds a neat little symbol for rts/favs */
            {
                Cursor cursorPosition = new Cursor();
                cursorPosition = cursorPosition.GetPosition();
                Console.SetCursorPosition((cursorPosition.X - 3), cursorPosition.Y);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("▀");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("]");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (update.IsFavorited)
            {
                Cursor cursorPosition = new Cursor();
                cursorPosition = cursorPosition.GetPosition();
                Console.SetCursorPosition((cursorPosition.X - 3), cursorPosition.Y);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[▀]");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (update.IsRetweeted)
            {
                Cursor cursorPosition = new Cursor();
                cursorPosition = cursorPosition.GetPosition();
                Console.SetCursorPosition((cursorPosition.X - 3), cursorPosition.Y);

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("[▀]");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public Profile SelectUser()
        {
            int selected = 0;
            ConsoleKeyInfo pressedKey;
            do
            {
                Console.Clear();
                Actions.CenterWriteLine("Select the user:");

                for (int index = 0; index < User.profiles.Count; index++)
                {
                    if (index == selected)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Actions.CenterWrite(User.profiles[index].Name);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.WriteLine();
                    }
                    else
                    {
                        Actions.CenterWriteLine(User.profiles[index].Name);
                    }
                }

                pressedKey = Console.ReadKey(true);
                if (pressedKey.Key == ConsoleKey.UpArrow)
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
                else if (pressedKey.Key == ConsoleKey.DownArrow)
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
            } while (pressedKey.KeyChar.CompareTo('\r') != 0);

            return User.profiles[selected];
        }

        public void ShowTimeline()
        {
            List<InteractiveTweet> updates = RemoveMentions(GetUpdates.localTweetList);
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
                string longUpdate = updates[index].AuthorScreenName + ": " + updates[index].Contents;


                int splitter = Console.WindowWidth - 13;

                longUpdate = longUpdate.Replace("\n", "\n      ");
                List<string> shortenedUpdate = longUpdate.SplitInParts(splitter).ToList();

                string cleanUserName = updates[index].AuthorScreenName.Remove(0, 1).ToLower();

                if (Friend.FriendsList != null && Friend.FriendsList.Contains(cleanUserName))
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                }

                Console.Write(updates[index].TweetIdentification + "    ");
                Console.ForegroundColor = ConsoleColor.White;
                bool identificationWritten = true;

                ShowInteractions(updates[index]);

                if (updates[index].AuthorScreenName.CompareTo("@" + GetUpdates.userScreenName) == 0)
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
            if (profile == null)
            {
                throw new NullReferenceException("User does not exist");
            }

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
                Console.ForegroundColor = ConsoleColor.Cyan; /* Again, not magenta */
                Console.Write(profile.Url);
                Console.ForegroundColor = ConsoleColor.White;
            }

            int infoBeltNameLine = /*screenInfo.Top -*/ Console.CursorTop + 2; /* Tweets, following, followers. You know! */

            /* Tweets */

            Console.SetCursorPosition(4, infoBeltNameLine); /* Okay, this might be a bit confusing */
            string tweets = "Tweets:\n    "; /* After the word tweets, there is a newline, so in the end, it'll look like */
            Console.ForegroundColor = ConsoleColor.DarkMagenta;   /* Tweets: */
            Console.Write(tweets);                         /* 32521 */
            Console.ForegroundColor = ConsoleColor.White; /* ...At least, I hope */
            Console.Write(profile.StatusesCount);

            /* Following */

            string following = "Following:";
            Console.SetCursorPosition(((screenInfo.Left - 4) / 2) - (following.Length / 2), infoBeltNameLine);
            int followingLeft = Console.CursorLeft;
            Console.ForegroundColor = ConsoleColor.DarkMagenta; /* Color coding, or is it coating? Can't remember */
            Console.Write(following);
            Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(followingLeft, infoBeltNameLine + 1);
            Console.Write(profile.FriendsCount);

            /* Followers */

            string followers = "Followers:";
            Console.SetCursorPosition((screenInfo.Left - followers.Length) - 4, infoBeltNameLine);

            int followersLeft = Console.CursorLeft;
            Console.ForegroundColor = ConsoleColor.DarkMagenta; /* This is not cyan btw [SetScreenColor.cs] */
            Console.Write(followers);
            Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(followersLeft, infoBeltNameLine + 1);
            Console.Write(profile.FollowersCount);

            Console.SetCursorPosition(0, Console.CursorTop + 4);
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
            Console.Clear();

            string longUpdate = tweet.Contents;
            int splitter = Console.WindowWidth - 10;

            longUpdate = longUpdate.Replace("\n", " ");
            List<string> shortenedUpdate = longUpdate.SplitInParts(splitter).ToList();

            int startingLine = /* Math time! */
                (Console.WindowHeight / 2) -  /* Middle of screen */
                (shortenedUpdate.Count / 2) -/* Lines of the tweet */
                1; /* Two lines for screen and display name divided by two, you get one */

            Cursor position = new Cursor();
            position.X = (Console.WindowWidth / 2) - (tweet.AuthorScreenName.Length / 2);
            position.Y = startingLine;
            position.SetPosition(position); /* Moves the cursor to the set X & Y co-ordinates */

            Console.Write(tweet.AuthorScreenName);
            string atName = "( " + tweet.AuthorDisplayName + " )";
            position.X = (Console.WindowWidth / 2) - (atName.Length / 2);
            position.Y = startingLine + 1;
            position.SetPosition(position);
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(atName); /* WriteLine here because I want an extra empty line because pretty */
            Console.ForegroundColor = ConsoleColor.White;

            for (int index = 0; index < shortenedUpdate.Count; index++) /* Draws the tweet nicely */
            {
                position.X = (Console.WindowWidth / 2) - (shortenedUpdate[index].Length / 2);
                position.Y = position.GetPosition().Y + 1;
                position.SetPosition(position);
                Console.Write(shortenedUpdate[index]);
            }

            int infoBeltLength = "Favorites: ".Length + /* Counts how long the info belt should be */
                (tweet.FavoriteCount + " ").Length +
                "Retweets: ".Length +
                (tweet.RetweetCount + " ").Length;

            position = position.GetPosition();
            position.X = (Console.WindowWidth / 2) - (infoBeltLength / 2);
            position.Y = position.GetPosition().Y + 1;
            position.SetPosition(position);

            if (tweet.IsFavorited)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            Console.Write("Favorites: " + tweet.FavoriteCount + " ");
            Console.ForegroundColor = ConsoleColor.White;

            if (tweet.IsRetweeted)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
            Console.Write("Retweets: " + tweet.RetweetCount + " ");
            Console.ForegroundColor = ConsoleColor.White;

            position = position.GetPosition();
            position.X = (Console.WindowWidth / 2) - (infoBeltLength / 2);
            position.MoveDown(3);
        }

        public void ShowHelp()
        {
            Console.Clear();
            int linestart = 6;

            Console.SetCursorPosition(linestart, 3);
            Console.Write("/h, /help");
            CommandEplanation("Shows this dialog.", 3);

            Console.SetCursorPosition(linestart, Console.CursorTop + 1);
            Console.Write("/rt");
            CommandEplanation("Retweets/undos a retweet on a selected tweet.", Console.CursorTop);

            Console.SetCursorPosition(linestart, Console.CursorTop + 1);
            Console.Write("/fav, /f");
            CommandEplanation("Favourites a selected tweet", Console.CursorTop);

            Console.SetCursorPosition(linestart, Console.CursorTop + 1);
            Console.Write("/unfav, /uf");
            CommandEplanation("Unfavourites a selected tweet", Console.CursorTop);

            Console.SetCursorPosition(linestart, Console.CursorTop + 1);
            Console.Write("/api");
            CommandEplanation("Shows the remaining API hits", Console.CursorTop);

            Console.SetCursorPosition(linestart, Console.CursorTop + 1);
            Console.Write("/r");
            CommandEplanation("Replies to everyone in the selected tweet", Console.CursorTop);

            Console.SetCursorPosition(linestart, Console.CursorTop + 1);
            Console.Write("/rc");
            CommandEplanation("Replies only to the author of the tweet", Console.CursorTop);

            Console.SetCursorPosition(linestart, Console.CursorTop + 1);
            Console.Write("/rn");
            CommandEplanation("Replies without using @ at all", Console.CursorTop);

            Console.SetCursorPosition(linestart, Console.CursorTop + 1);
            Console.Write("/id");
            CommandEplanation("Shows the ID of the tweet", Console.CursorTop);

            Console.SetCursorPosition(linestart, Console.CursorTop + 1);
            Console.Write("/profile");
            CommandEplanation("Shows the profile of the selected user", Console.CursorTop);

            Console.SetCursorPosition(linestart, Console.CursorTop + 1);
            Console.Write("/me");
            CommandEplanation("Shows your mentions", Console.CursorTop);

            Console.SetCursorPosition(linestart, Console.CursorTop + 1);
            Console.Write("/link");
            CommandEplanation("Links you to a tweet", Console.CursorTop);

            Console.SetCursorPosition(linestart, Console.CursorTop + 1);
            Console.Write("/tweet");
            CommandEplanation("Shows you details of a tweet", Console.CursorTop);

            Console.SetCursorPosition(linestart, Console.CursorTop + 1);
            Console.Write("/open");
            CommandEplanation("Opens the tweet in browser (only from /tweet)", Console.CursorTop);

            Console.SetCursorPosition(linestart, Console.CursorTop + 1);
            Console.Write("/friend");
            CommandEplanation("Adds/removes a friend", Console.CursorTop);

            Console.SetCursorPosition(linestart, Console.CursorTop + 1);
            Console.Write("/accounts");
            CommandEplanation("Actions regarding twitter accounts", Console.CursorTop);

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

            Console.Clear();

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
