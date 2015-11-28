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
using TweetSharp;
using System.Threading;
using System.Diagnostics;
using CursesSharp;

namespace ClutterFeed
{
    class Actions
    {
        public static Thread StreamingThread { get; set; }
        private User getUser = new User();
        private ScreenDraw drawing = new ScreenDraw();
        private GetUpdates showUpdates = new GetUpdates();
        private OAuthAccessToken key = new OAuthAccessToken();

        /// <summary>
        /// Shows the timeline and parses commands as long as the user has not typed in /q
        /// </summary>
        public void TimelineConsole()
        {
            StreamingThread = new Thread(new ThreadStart(showUpdates.Stream));
            StreamingThread.Start();

            showUpdates.GetTweets();
            drawing.ShowTimeline(); /* Show the initial Timeline */

            string command = null;

            do
            {
                if (command != null)
                {
                    if (command.StartsWith("/"))
                    {
                        if (command.Command("/r"))
                        {
                            ReplyGeneric(command);
                        }

                        else if (command.Command("/rc"))
                        {
                            ReplyClean(command);
                        }

                        else if (command.Command("/rn"))
                        {
                            ReplyQuiet(command);
                        }

                        else if (command.Command("/rt"))
                        {
                            Retweet(command);
                        }

                        else if (command.Command("/fav") || command.Command("/f"))
                        {
                            FavoriteTweet(command);
                        }

                        else if (command.Command("/unfav") || command.Command("/uf"))
                        {
                            FavoriteTweet(command); /* This also unfavs tweets */
                        }

                        else if (command.Command("/h") || command.Command("/help"))
                        {
                            Help();
                        }

                        else if (command.Command("/api"))
                        {
                            /* Shouldn't use with streaming, but keeping it here for now */
                            /* for debugging purposes, better safe than sorry */
                            ApiInfo();
                        }

                        else if (command.Command("/profile"))
                        {
                            try
                            {
                                ShowProfile(command);
                            }
                            catch (NullReferenceException exceptionInfo)
                            {
                                ScreenDraw.ShowMessage(exceptionInfo.Message + "\n");
                            }
                        }

                        else if (command.Command("/me"))
                        {
                            Mentions();
                        }

                        else if (command.Command("/link"))
                        {
                            TweetLink(command);
                        }

                        else if (command.Command("/tweet"))
                        {
                            ShowTweet(command);
                        }

                        else if (command.Command("/friend"))
                        {
                            ToggleFriend(command);
                        }

                        else if (command.Command("/accounts"))
                        {
                            ProfileSelection();
                        }

                        else if (command.Command("/follow"))
                        {
                            FollowUser(command);
                        }

                        else if (command.Command("/block"))
                        {
                            BlockUser(command);
                        }

                        else if (command.Command("/del"))
                        {
                            RemoveTweet(command);
                        }

                        else if (command.Command("/afk"))
                        {
                            AfkToggle();
                        }

                        else
                        {
                            ScreenDraw.ShowMessage("Such a command does not exist");
                        }
                    }

                    else if (command.Length <= 140)
                    {
                        NewTweet(command);
                        if (User.Account.Response.Error != null)
                        {
                            ScreenDraw.ShowMessage(User.Account.Response.Error.Code + ": " + User.Account.Response.Error.Message);
                        }
                    }
                }

                command = User.CounterConsole();
            } while (command.Command("/q") == false);

        }

        /// <summary>
        /// Toggles AFK mode on and off
        /// </summary>
        private static void AfkToggle()
        {
            if (Settings.AFK)
            {
                ScreenDraw.ShowMessage("AFK Mode set to OFF.");
                Settings.AFK = false;
            }
            else
            {
                Settings.AFK = true;
                ScreenDraw.ShowMessage("AFK Mode set to ON.");
            }
        }
        public void MentionsConsole()
        {
            throw new NotImplementedException("Todo");
        }

        /// <summary>
        /// Deals with input shortcuts accordingly
        /// </summary>
        /// <returns>Returns true if the input was a shortcut</returns>
        public bool DealWithShortcuts(int ch)
        {
            if (ch == 3) /* ^C */
            {
                Curses.EndWin();
                Environment.Exit(0);
                return true;
            }
            if (ch == 4) /* Ctrl-D */
            {
                Mentions();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Initializes all the important objects and the API
        /// </summary>
        public void SetUpTwitter()
        {

            showUpdates.InitializeTwitter();
            key = getUser.GetUser();

            Friend startFriend = new Friend();
            startFriend.ReadFriends();

            User.Account.IncludeRetweets = true;
            drawing.StartScreen();
        }

        private Window menu { get; set; }

        /// <summary>
        /// Draws a message in the middle of the menu
        /// </summary>
        private void MenuDrawInMiddle(string message)
        {
            int line = 0;
            int notNessecary;
            menu.GetCursorYX(out line, out notNessecary);
            menu.Add(line, (ScreenInfo.WindowWidth / 2) - (message.Length / 2), message);
        }

        /// <summary>
        /// Draws a message in the middle of the menu, on a specific line
        /// </summary>
        private void MenuDrawInMiddle(string message, int line)
        {
            menu.Add(line, (ScreenInfo.WindowWidth / 2) - (message.Length / 2), message);
        }

        /// <summary>
        /// A menu and logic to select/add/remove profiles
        /// </summary>
        private void ProfileSelection()
        {
            int pressedKey;
            int selection = 0;
            menu = new Window(User.profiles.Count + 10, ScreenInfo.WindowWidth, (ScreenInfo.WindowHeight / 2) - 2, 0);
            do
            {
                menu.Clear();
                menu.Keypad = true;
                menu.Color = 11;
                MenuDrawInMiddle("What would you like to do?\n", 0);
                if (selection == 0)
                {
                    menu.Color = 21;
                }
                MenuDrawInMiddle("Add profile");
                menu.Color = 11;
                menu.Add("\n");

                if (selection == 1)
                {
                    menu.Color = 21;
                }
                MenuDrawInMiddle("Remove a profile");
                menu.Add("\n");
                menu.Color = 11;

                if (selection == 2)
                {
                    menu.Color = 21;
                }
                MenuDrawInMiddle("Switch profiles");
                menu.Add("\n");
                menu.Color = 11;

                if (selection == 3)
                {
                    menu.Color = 21;
                }
                menu.Refresh();
                MenuDrawInMiddle("Select default profile");
                menu.Add("\n");
                menu.Color = 11;

                if (selection == 4)
                {
                    menu.Color = 21;
                }
                MenuDrawInMiddle("Go back");
                menu.Color = 11;
                menu.Refresh();

                pressedKey = menu.GetChar();
                if (pressedKey == Keys.UP)
                {
                    if (selection == 0)
                    {
                        Curses.Beep();
                    }
                    else
                    {
                        selection--;
                    }
                }
                else if (pressedKey == Keys.DOWN)
                {
                    if (selection == 4)
                    {
                        Curses.Beep();
                    }
                    else
                    {
                        selection++;
                    }
                }
            } while (pressedKey != 10);

            menu.Clear();
            menu.Refresh();
            if (selection == 0)
            {
                getUser.AddProfile();
            }
            else if (selection == 1)
            {
                Profile deletingProfile = drawing.SelectUser();
                int minorSelection = 0;
                int pressedChar;
                do
                {
                    menu.Clear();
                    menu.Color = 11;
                    MenuDrawInMiddle("Are you sure you want to remove " + deletingProfile.Name + "?", 0);
                    if (minorSelection == 0)
                    {
                        menu.Color = 21;
                        MenuDrawInMiddle("Yes", 2);
                        menu.Color = 11;
                    }
                    else
                    {
                        MenuDrawInMiddle("Yes", 2);
                    }

                    if (minorSelection == 1)
                    {
                        menu.Color = 21;
                        MenuDrawInMiddle("No", 3);
                        menu.Color = 11;
                    }
                    else
                    {
                        MenuDrawInMiddle("No", 3);
                    }
                    menu.Refresh();

                    pressedChar = menu.GetChar();
                    if (pressedChar == Keys.UP)
                    {
                        if (minorSelection == 0)
                        {
                            Curses.Beep();
                        }
                        else
                        {
                            minorSelection--;
                        }
                    }
                    else if (pressedChar == Keys.DOWN)
                    {
                        if (minorSelection == 1)
                        {
                            Curses.Beep();
                        }
                        else
                        {
                            minorSelection++;
                        }
                    }
                } while (pressedChar != 10);

                if (minorSelection == 0)
                {
                    getUser.RemoveProfile(deletingProfile);
                }
            }
            else if (selection == 2)
            {
                Profile selectUser = drawing.SelectUser();
                getUser.SelectUser(selectUser.Name);

            }
            else if (selection == 3)
            {
                Profile selectUser = drawing.SelectUser();
                getUser.SetDefault(selectUser);
                GetUpdates.ReauthenticateTwitter();
            }
            else if (selection == 4)
            {
                return;
            }
        }

        /// <summary>
        /// Posts a new status to Twitter
        /// </summary>
        /// <param name="command"></param>
        private void NewTweet(string command)
        {
            SendTweetOptions opts = new SendTweetOptions();
            opts.Status = command;
            User.Account.SendTweet(opts);
        }


        /// <summary>
        /// Toggles the friend switch ( ͡° ͜ʖ ͡°)
        /// </summary>
        private void ToggleFriend(string command)
        {
            string screenName = "";
            try
            {
                bool exceptionTest = command.Split(' ')[1].StartsWith("@");
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
            if (command.Split(' ')[1].Length != 2)
            {
                return;
            }
            else
            {
                screenName = TweetIdentification.GetTweetID(command.Split(' ')[1]).Status;
            }
            /* This is where the magic happens */

            Friend luckyFriend = new Friend();
            luckyFriend.FriendToggle(screenName);
            ScreenDraw.ShowMessage(screenName + "'s friend switch has been toggled ( ͡° ͜ʖ ͡°)");
        }

        /// <summary>
        /// Posts a reply to a tweet
        /// </summary>
        /// <param name="command">the full command used</param>
        /// <returns>should the program ask for more input from the user?</returns>
        private void ReplyGeneric(string command)
        {
            if (User.IsMissingArgs(command) == false) /* It's just an exception catching method, don't mind it */
            {
                if (command.Split(' ')[1].Length != 2)
                {
                    ScreenDraw.ShowMessage("Wrong syntax. Use /r [id] [reply]");
                }
                else
                {
                    char[] splitter = { ' ' };
                    string message = "";
                    try
                    {
                        message = command.Split(splitter, 3)[2];
                    }
                    catch (Exception)
                    {
                        ScreenDraw.ShowMessage("The command was missing arguments");
                        return;
                    }
                    SendTweetOptions replyOpts = TweetIdentification.GetTweetID(command.Split(' ')[1]);

                    replyOpts.Status = replyOpts.Status + " ";

                    InteractiveTweet tweetReplyingTo;
                    try
                    {
                        tweetReplyingTo = TweetIdentification.FindTweet(Convert.ToInt64(replyOpts.InReplyToStatusId));
                    }
                    catch (KeyNotFoundException exIn)
                    {
                        ScreenDraw.ShowMessage(exIn.Message);
                        return;
                    }
                    string[] words = tweetReplyingTo.Contents.Split(' ');
                    string userScreenName = GetUpdates.userScreenName.ToLower();

                    for (int index = 0; index < words.Length; index++) /* This checks for extra people mentioned in the tweet */
                    {
                        if (words[index].StartsWith("@") && words[index].CompareTo("@") != 0)
                        {
                            if (words[index].ToLower().CompareTo("@" + userScreenName) != 0)
                            {
                                replyOpts.Status = replyOpts.Status + words[index] + " ";
                            }
                        }
                    }

                    replyOpts.Status = replyOpts.Status + message;
                    User.Account.BeginSendTweet(replyOpts);
                }
            }
        }

        /// <summary>
        /// Replies only to the tweet author, not mentioning everyone else the author mentioned
        /// </summary>
        private void ReplyClean(string command)
        {
            if (User.IsMissingArgs(command) == false) /* It's just an exception catching method, don't mind it */
            {
                if (command.Split(' ')[1].Length != 2)
                {
                    ScreenDraw.ShowMessage("Wrong syntax. Use /r [id] [reply]");
                }
                else
                {
                    char[] splitter = { ' ' }; /* Because why not? */
                    string message = command.Split(splitter, 3)[2];
                    SendTweetOptions replyOpts = TweetIdentification.GetTweetID(command.Split(' ')[1]);

                    replyOpts.Status = replyOpts.Status + " ";
                    replyOpts.Status = replyOpts.Status + message;
                    User.Account.BeginSendTweet(replyOpts);
                }
            }
        }

        /// <summary>
        /// Replies to a tweet without mentioning the other participants
        /// </summary>
        private void ReplyQuiet(string command)
        {
            if (User.IsMissingArgs(command) == false) /* It's just an exception catching method, don't mind it */
            {
                if (command.Split(' ')[1].Length != 2)
                {
                    ScreenDraw.ShowMessage("Wrong syntax. Use /rn [id] [reply]");
                }
                else
                {
                    char[] splitter = new char[1];
                    splitter[0] = ' '; /* FUCK C# honestly */
                    string message = command.Split(splitter, 3)[2];
                    SendTweetOptions replyOpts = TweetIdentification.GetTweetID(command.Split(' ')[1]);
                    replyOpts.Status = message;
                    User.Account.BeginSendTweet(replyOpts);
                }
            }
        }

        /// <summary>
        /// Gives the user a link to a tweet to copy and also opens it in browser
        /// </summary>
        private void TweetLink(string command)
        {
            if (command.Split(' ')[1].Length != 2)
            {
                ScreenDraw.ShowMessage("Wrong syntax. Use /link [id]");
            }
            else
            {
                try
                {
                    long tweetID = Convert.ToInt64(TweetIdentification.GetTweetID(command.Split(' ')[1]).InReplyToStatusId);
                    InteractiveTweet tweet = TweetIdentification.FindTweet(tweetID);
                    ScreenDraw.ShowMessage(tweet.LinkToTweet);
                    Process.Start(tweet.LinkToTweet);
                }
                catch (KeyNotFoundException exIn)
                {
                    ScreenDraw.ShowMessage(exIn.Message);
                }
            }
        }

        /// <summary>
        /// Retweets a specified tweet
        /// </summary>
        private void Retweet(string command)
        {
            if (User.IsMissingArgs(command) == false) /* It's just an exception catching method, don't mind it */
            {
                if (command.Split(' ')[1].Length != 2)
                {
                    ScreenDraw.ShowMessage("Wrong syntax. Use /rt [id]");
                }
                else
                {
                    SendTweetOptions replyOpts = TweetIdentification.GetTweetID(command.Split(' ')[1]);
                    RetweetOptions retweetOpts = new RetweetOptions();

                    long tweetID = Convert.ToInt64(replyOpts.InReplyToStatusId);
                    retweetOpts.Id = tweetID;
                    InteractiveTweet tweet = new InteractiveTweet();

                    try
                    {
                        tweet = TweetIdentification.FindTweet(tweetID);
                    }
                    catch (KeyNotFoundException exceptionInfo)
                    {
                        ScreenDraw.ShowMessage(exceptionInfo.Message);
                        return;
                    }
                    if (tweet.IsRetweeted)
                    {
                        User.Account.Retweet(retweetOpts);
                        if (User.Account.Response.Error != null)
                        {
                            ScreenDraw.ShowMessage(User.Account.Response.Error.Code + ": " + User.Account.Response.Error.Message);
                        }
                        return;
                    }

                    User.Account.Retweet(retweetOpts);
                    if (User.Account.Response.Error == null)
                    {
                        GetUpdates retweetInvert = new GetUpdates();
                        retweetInvert.InvertRetweetStatus(tweetID);

                        ScreenDraw.ShowMessage("Retweeted");

                        ScreenDraw redraw = new ScreenDraw();
                        redraw.ShowTimeline();
                    }
                    else
                    {
                        ScreenDraw.ShowMessage(User.Account.Response.Error.Code + ": " + User.Account.Response.Error.Message);
                    }

                }
            }
        }



        /// <summary>
        /// Favorites or unfavorites a tweet
        /// </summary>
        /// <param name="command">The entire command string</param>
        /// <returns></returns>
        private void FavoriteTweet(string command)
        {
            if (User.IsMissingArgs(command) == false) /* It's just an exception catching method, don't mind it */
            {
                if (command.Split(' ')[1].Length != 2)
                {
                    ScreenDraw.ShowMessage("Wrong syntax. Use /fav [id]");
                }
                else
                {
                    SendTweetOptions replyOpts = TweetIdentification.GetTweetID(command.Split(' ')[1]);
                    FavoriteTweetOptions favOpts = new FavoriteTweetOptions();
                    GetUpdates favoriteInvert = new GetUpdates();

                    long tweetID = Convert.ToInt64(replyOpts.InReplyToStatusId);
                    favOpts.Id = tweetID;
                    InteractiveTweet tweet = null;
                    try
                    {
                        tweet = TweetIdentification.FindTweet(tweetID);
                    }
                    catch (KeyNotFoundException exIn)
                    {
                        ScreenDraw.ShowMessage(exIn.Message);
                        return;
                    }

                    if (tweet.IsFavorited)
                    {
                        ScreenDraw.ShowMessage("Unfavoriting");
                        UnfavoriteTweetOptions unfavOpts = new UnfavoriteTweetOptions();
                        unfavOpts.Id = favOpts.Id;
                        User.Account.BeginUnfavoriteTweet(unfavOpts);

                        favoriteInvert.InvertFavoriteStatus(tweetID); /* Changes whether the tweet is counted as favorited */
                    }
                    else
                    {
                        User.Account.BeginFavoriteTweet(favOpts);
                        ScreenDraw.ShowMessage("Favoriting");

                        favoriteInvert.InvertFavoriteStatus(tweetID); /* Changes whether the tweet is counted as favorited */

                    }
                }
            }
        }

        /// <summary>
        /// Deletes a specified tweet
        /// </summary>
        private void RemoveTweet(string command)
        {
            if (command.Split(' ')[1].Length != 2)
            {
                ScreenDraw.ShowMessage("Wrong syntax. Use /del [id]");
            }
            else
            {
                long tweetID = Convert.ToInt64(TweetIdentification.GetTweetID(command.Split(' ')[1]).InReplyToStatusId);
                DeleteTweetOptions delOpts = new DeleteTweetOptions();
                delOpts.Id = tweetID;

                User.Account.DeleteTweet(delOpts);
                TwitterResponse result = User.Account.Response;
                if (result.Error == null)
                {
                    GetUpdates.localTweetList.Remove(TweetIdentification.FindTweet(tweetID));
                    ScreenDraw.ShowMessage("Deleted");
                    ScreenDraw draw = new ScreenDraw();
                    draw.ShowTimeline();
                }
                else
                {
                    ScreenDraw.ShowMessage(result.Error.Code + ": " + result.Error.Message);
                }

            }
        }

        /// <summary>
        /// Blocks/unblocks a twitter user
        /// </summary>
        private void BlockUser(string command)
        {
            if (command.Split(' ')[1].Length != 2 && command.Split(' ')[1].StartsWith("@") == false)
            {
                ScreenDraw.ShowMessage("Wrong syntax. Use /block [id] or /block @[name]");
            }
            else
            {
                string screenName = "";
                if (command.Split(' ')[1].StartsWith("@"))
                {
                    screenName = command.Split(' ')[1].Remove(0, 1);
                }
                else
                {
                    try
                    {
                        long tweetID = Convert.ToInt64(TweetIdentification.GetTweetID(command.Split(' ')[1]).InReplyToStatusId);
                        InteractiveTweet tweet = TweetIdentification.FindTweet(tweetID);
                        screenName = tweet.AuthorScreenName;
                    }
                    catch (KeyNotFoundException exIn)
                    {
                        ScreenDraw.ShowMessage(exIn.Message);
                    }
                }

                if (GetUpdates.IsBlocked(screenName))
                {
                    UnblockUserOptions unblockOpts = new UnblockUserOptions();
                    unblockOpts.ScreenName = screenName;
                    User.Account.UnblockUser(unblockOpts);
                    if (User.Account.Response.Error == null)
                    {
                        ScreenDraw.ShowMessage("Successfully unblocked @" + screenName);
                    }
                    else
                    {
                        ScreenDraw.ShowMessage(User.Account.Response.Error.Code + ": " + User.Account.Response.Error.Message);
                    }
                }
                else
                {
                    BlockUserOptions blockOpts = new BlockUserOptions();
                    blockOpts.ScreenName = screenName;
                    User.Account.BlockUser(blockOpts);
                    if (User.Account.Response.Error == null)
                    {
                        ScreenDraw.ShowMessage("Successfully blocked @" + screenName);
                    }
                    else
                    {
                        ScreenDraw.ShowMessage(User.Account.Response.Error.Code + ": " + User.Account.Response.Error.Message);
                    }
                }
            }
        }
        
        /// <summary>
        /// Follows/unfollows a twitter user
        /// </summary>
        private void FollowUser(string command)
        {
            if (command.Split(' ')[1].Length != 2 && command.Split(' ')[1].StartsWith("@") == false)
            {
                ScreenDraw.ShowMessage("Wrong syntax. Use /follow [id] or /follow @[name]");
            }
            else
            {
                string screenName = "";
                if (command.Split(' ')[1].StartsWith("@"))
                {
                    screenName = command.Split(' ')[1].Remove(0, 1);
                }
                else
                {
                    try
                    {
                        long tweetID = Convert.ToInt64(TweetIdentification.GetTweetID(command.Split(' ')[1]).InReplyToStatusId);
                        InteractiveTweet tweet = TweetIdentification.FindTweet(tweetID);
                        screenName = tweet.AuthorScreenName;
                    }
                    catch (KeyNotFoundException exIn)
                    {
                        ScreenDraw.ShowMessage(exIn.Message);
                    }
                }

                if (GetUpdates.IsFollowing(screenName))
                {
                    UnfollowUserOptions unfollowOpts = new UnfollowUserOptions();
                    unfollowOpts.ScreenName = screenName;
                    User.Account.UnfollowUser(unfollowOpts);
                    if (User.Account.Response.Error == null)
                    {
                        ScreenDraw.ShowMessage("Successfully unfollowed @" + screenName);
                    }
                    else
                    {
                        ScreenDraw.ShowMessage(User.Account.Response.Error.Code + ": " + User.Account.Response.Error.Message);
                    }
                }
                else
                {
                    FollowUserOptions followOpts = new FollowUserOptions();
                    followOpts.ScreenName = screenName;
                    User.Account.FollowUser(followOpts);
                    if (User.Account.Response.Error == null)
                    {
                        ScreenDraw.ShowMessage("Successfully followed @" + screenName);
                    }
                    else
                    {
                        ScreenDraw.ShowMessage(User.Account.Response.Error.Code + ": " + User.Account.Response.Error.Message);
                    }
                }
            }
        }
        /// <summary>
        /// Accesses the profile of a user
        /// </summary>
        /// <param name="command">The entire command string</param>
        /// <returns></returns>
        private void ShowProfile(string command)
        {

            GetUserProfileForOptions profileOpts = new GetUserProfileForOptions();
            string screenName = "";
            try
            {
                bool exceptionTest = command.Split(' ')[1].StartsWith("@");
            }
            catch (IndexOutOfRangeException)
            {
                ScreenDraw.ShowMessage("Wrong syntax. Use /profile [id] or /profile @[name]");
                return;
            }
            if (command.Split(' ')[1].StartsWith("@"))
            {
                screenName = command.Split(' ')[1].Remove(0, 1);
            }
            else if (command.Split(' ')[1].Length != 2)
            {
                ScreenDraw.ShowMessage("Wrong syntax. Use /profile [id] or /profile @[name]");
                return;
            }
            else
            {
                screenName = TweetIdentification.GetTweetID(command.Split(' ')[1]).Status;
            }

            profileOpts.ScreenName = screenName;
            TwitterUser profile = User.Account.GetUserProfileFor(profileOpts);
            ScreenDraw showProfile = new ScreenDraw();
            if (GetUpdates.IsFollowing(screenName)) /* Because the profile object doesn't say this */
            {
                ScreenDraw.IsFollowing = true;
            }
            else
            {
                ScreenDraw.IsFollowing = false;
            }
            if (GetUpdates.IsBlocked(screenName))
            {
                ScreenDraw.IsBlocked = true;
            }
            else
            {
                ScreenDraw.IsBlocked = false;
            }

            if (User.Account.Response.Error == null)
            {
                showProfile.ShowUserProfile(profile);
            }
            else
            {
                ScreenDraw.ShowMessage(User.Account.Response.Error.Code + ": " + User.Account.Response.Error.Message);
            }
            return;
        }

        /// <summary>
        /// Draws a tweet all fancy like in a box
        /// </summary>
        private void ShowTweet(string command)
        {
            /* This is still missing stuff, TODO! */

            try /* Checks if the command was valid */
            {
                bool exceptionTest = command.Split(' ')[1].StartsWith("@");
            }
            catch (IndexOutOfRangeException)
            {
                ScreenDraw.ShowMessage("Wrong syntax. Use /tweet [id]");
                return;
            }
            if (command.Split(' ')[1].Length != 2)
            {
                ScreenDraw.ShowMessage("Wrong syntax. Use /tweet [id]");
                return;
            }
            else
            {
                long tweetID = Convert.ToInt64(TweetIdentification.GetTweetID(command.Split(' ')[1]).InReplyToStatusId);
                InteractiveTweet tweet = null;
                try
                {
                    tweet = TweetIdentification.FindTweet(tweetID);
                }
                catch (KeyNotFoundException exIn)
                {
                    ScreenDraw.ShowMessage(exIn.Message);
                    return;
                }
                if (User.Account.Response.Error == null)
                {
                    ScreenDraw tweetDrawer = new ScreenDraw();
                    tweetDrawer.DrawTweet(tweet);
                }
                else
                {
                    ScreenDraw.ShowMessage(User.Account.Response.Error.Code + ": " + User.Account.Response.Error.Message);
                }
            }
        }

        public void GetID(string command)
        {
            if (User.IsMissingArgs(command) == false)
            {
                if (command.Split(' ')[1].Length != 2)
                {
                    ScreenDraw.ShowMessage("Wrong syntax. Use /id [id]");
                }
                else
                {
                    string id = Convert.ToString(TweetIdentification.GetTweetID(command.Split(' ')[1]).InReplyToStatusId);
                    ScreenDraw.ShowMessage("Tweet ID: " + id);
                }
            }
        }


        private void Mentions()
        {
            GetUpdates getMentions = new GetUpdates();
            ScreenDraw draw = new ScreenDraw();
            getMentions.GetMentions();
            TimerMan.Pause();

            draw.ShowMentions();
            Actions twitterMethods = new Actions();
            MentionsConsole();

            if (Settings.AFK == false)
            {
                TimerMan.Resume();
            }
            draw.ShowTimeline();
        }

        private void Help()
        {
            ScreenDraw drawHelp = new ScreenDraw();
            drawHelp.ShowHelp();
            drawHelp.ShowTimeline();
        }

        private void ApiInfo()
        {
            TwitterRateLimitStatus rate = User.Account.Response.RateLimitStatus;
            if (User.Account.Response.Error == null)
            {
                ScreenDraw.ShowMessage("You have " + rate.RemainingHits + " remaining calls out of your " + rate.HourlyLimit + " limit");
            }
            else
            {
                ScreenDraw.ShowMessage(User.Account.Response.Error.Code + ": " + User.Account.Response.Error.Message);
            }
        }

    }
}