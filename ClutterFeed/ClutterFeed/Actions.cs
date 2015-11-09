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

namespace ClutterFeed
{
    class ActionValue
    {
        public bool AskForCommand { get; set; } = true;
        public bool OverrideCommand { get; set; } = false;
        public string OverrideCommandString { get; set; }

    }
    class Actions
    {
        //private TwitterService User.account = new TwitterService();
        private User getUser = new User();
        private StatusCommunication newTweet = new StatusCommunication();
        private GetUpdates showUpdates = new GetUpdates();
        private OAuthAccessToken key = new OAuthAccessToken();

        public static string SpecialCommandCase { get; set; }

        /// <summary>
        /// Initializes all the important objects and the API
        /// </summary>
        public void SetUpTwitter()
        {
            key = getUser.GetUser();
            User.Account = showUpdates.InitializeTwitter();

            Friend startFriend = new Friend();
            startFriend.ReadFriends();

            User.Account.IncludeRetweets = true;
        }

        public ActionValue NewTweet(string command)
        {
            ActionValue returnInfo = new ActionValue();

            if (command.Length > 140)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("      Tweet is too long.");
                Console.ForegroundColor = ConsoleColor.White;
                return new ActionValue();
            }

            newTweet.PostTweet(User.Account, command);
            Thread.Sleep(200);

            returnInfo.AskForCommand = false;
            return returnInfo;
        }

        public ActionValue AddFriend(string command)
        {
            ActionValue returnInfo = new ActionValue();

            string screenName = "";
            try
            {
                bool exceptionTest = command.Split(' ')[1].StartsWith("@");
            }
            catch (IndexOutOfRangeException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("      Wrong syntax. Use /friend [id]");
                Console.WriteLine("      Example: /friend 3f");
                Console.WriteLine("      If you meant to search by username,");
                Console.WriteLine("      use /friend @username");
                Console.ForegroundColor = ConsoleColor.White;
                return returnInfo;
            }
            if (command.Split(' ')[1].StartsWith("@"))
            {
                screenName = command.Split(' ')[1].Remove(0, 1);
            }
            else if (command.Split(' ')[1].Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("      Wrong syntax. Use /friend [id]");
                Console.WriteLine("      Example: /friend 3f");
                Console.WriteLine("      If you meant to search by username,");
                Console.WriteLine("      use /friend @username");
                Console.ForegroundColor = ConsoleColor.White;
                return returnInfo;
            }
            else
            {
                screenName = TweetIdentification.GetTweetID(command.Split(' ')[1]).Status;
            }
            /* This is where the magic happens */
            
            Friend luckyFriend = new Friend();
            luckyFriend.FriendToggle(screenName);
            returnInfo.AskForCommand = false;

            return returnInfo;
        }

        /// <summary>
        /// Posts a reply to a tweet
        /// </summary>
        /// <param name="command">the full command used</param>
        /// <returns>should the program ask for more input from the user?</returns>
        public ActionValue ReplyGeneric(string command)
        {
            bool askForCommand = true;
            if (User.IsMissingArgs(command) == false) /* It's just an exception catching method, don't mind it */
            {
                if (command.Split(' ')[1].Length != 2)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("      Wrong syntax. Use /r [id] [reply]");
                    Console.WriteLine("      Example: /r 3f Hey Adam, I'm doing fine!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    char[] splitter = new char[1];
                    splitter[0] = ' '; /* FUCK C# honestly */
                    string message = command.Split(splitter, 3)[2];
                    SendTweetOptions replyOpts = TweetIdentification.GetTweetID(command.Split(' ')[1]);

                    replyOpts.Status = replyOpts.Status + " ";

                    InteractiveTweet tweetReplyingTo = TweetIdentification.FindTweet(Convert.ToInt64(replyOpts.InReplyToStatusId));
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
                    askForCommand = false;
                    Thread.Sleep(200);
                }
            }
            ActionValue returnInfo = new ActionValue();
            returnInfo.AskForCommand = askForCommand;
            return returnInfo;
        }

        /// <summary>
        /// Replies only to the tweet author, not mentioning everyone else the author mentioned
        /// </summary>
        public ActionValue ReplyClean(string command)
        {
            bool askForCommand = true;
            if (User.IsMissingArgs(command) == false) /* It's just an exception catching method, don't mind it */
            {
                if (command.Split(' ')[1].Length != 2)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("      Wrong syntax. Use /r [id] [reply]");
                    Console.WriteLine("      Example: /r 3f Hey Adam, I'm doing fine!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    char[] splitter = new char[1];
                    splitter[0] = ' '; /* FUCK C# honestly */
                    string message = command.Split(splitter, 3)[2];
                    SendTweetOptions replyOpts = TweetIdentification.GetTweetID(command.Split(' ')[1]);

                    replyOpts.Status = replyOpts.Status + " ";
                    replyOpts.Status = replyOpts.Status + message;
                    User.Account.BeginSendTweet(replyOpts);
                    askForCommand = false;
                    Thread.Sleep(200);
                }
            }
            ActionValue returnInfo = new ActionValue();
            returnInfo.AskForCommand = askForCommand;
            return returnInfo;
        }


        public ActionValue ReplyQuiet(string command)
        {
            bool askForCommand = true;
            if (User.IsMissingArgs(command) == false) /* It's just an exception catching method, don't mind it */
            {
                if (command.Split(' ')[1].Length != 2)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("      Wrong syntax. Use /rn [id] [reply]");
                    Console.WriteLine("      Example: /rn 3f ...and then it just went out of control!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    char[] splitter = new char[1];
                    splitter[0] = ' '; /* FUCK C# honestly */
                    string message = command.Split(splitter, 3)[2];
                    SendTweetOptions replyOpts = TweetIdentification.GetTweetID(command.Split(' ')[1]);
                    replyOpts.Status = message;
                    User.Account.BeginSendTweet(replyOpts);
                    askForCommand = false;
                    Thread.Sleep(200);
                }
            }
            ActionValue returnInfo = new ActionValue();
            returnInfo.AskForCommand = askForCommand;
            return returnInfo;
        }


        public ActionValue TweetLink(string command)
        {
            ActionValue returnInfo = new ActionValue();

            if (command.Split(' ')[1].Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("      Wrong syntax. Use /link [id]");
                Console.WriteLine("      Example: /link 3f");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                long tweetID = Convert.ToInt64(TweetIdentification.GetTweetID(command.Split(' ')[1]).InReplyToStatusId);
                InteractiveTweet tweet = TweetIdentification.FindTweet(tweetID);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("      " + tweet.LinkToTweet);
                Console.ForegroundColor = ConsoleColor.White;
                Process.Start(tweet.LinkToTweet);
            }
            return returnInfo;
        }

        public ActionValue Retweet(string command)
        {
            ActionValue returnInfo = new ActionValue();

            if (User.IsMissingArgs(command) == false) /* It's just an exception catching method, don't mind it */
            {
                if (command.Split(' ')[1].Length != 2)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("      Wrong syntax. Use /rt [id]");
                    Console.WriteLine("      Example: /rt 3f");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    SendTweetOptions replyOpts = TweetIdentification.GetTweetID(command.Split(' ')[1]);
                    RetweetOptions retweetOpts = new RetweetOptions();

                    long tweetID = Convert.ToInt64(replyOpts.InReplyToStatusId);
                    retweetOpts.Id = tweetID;
                    InteractiveTweet tweet = new InteractiveTweet();
                    bool finishBlock = true;

                    try
                    {
                        tweet = TweetIdentification.FindTweet(tweetID);
                    }
                    catch (KeyNotFoundException exceptionInfo)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("      " + exceptionInfo.Message);
                        Console.ForegroundColor = ConsoleColor.White;
                        finishBlock = false;
                    }
                    if (finishBlock)
                    {
                        if (tweet.IsRetweeted)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("      Unretweeting currently is not supported.");
                            Console.WriteLine("      Might be something with the API.");
                            Console.ForegroundColor = ConsoleColor.White;
                            return new ActionValue();
                        }

                        User.Account.Retweet(retweetOpts);

                        GetUpdates retweetInvert = new GetUpdates();
                        retweetInvert.InvertRetweetStatus(tweetID);

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("      Retweeted.");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.ReadKey(true);

                        returnInfo.OverrideCommand = true;
                        returnInfo.OverrideCommandString = "/fu";
                        returnInfo.AskForCommand = false;
                    }
                }
            }
            return returnInfo;

        }



        /// <summary>
        /// Favorites or unfavorites a tweet
        /// </summary>
        /// <param name="command">The entire command string</param>
        /// <returns></returns>
        public ActionValue FavoriteTweet(string command)
        {
            ActionValue returnInfo = new ActionValue();

            if (User.IsMissingArgs(command) == false) /* It's just an exception catching method, don't mind it */
            {
                if (command.Split(' ')[1].Length != 2)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("      Wrong syntax. Use /fav [id]");
                    Console.WriteLine("      Example: /fav 3f");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    SendTweetOptions replyOpts = TweetIdentification.GetTweetID(command.Split(' ')[1]);
                    FavoriteTweetOptions favOpts = new FavoriteTweetOptions();
                    GetUpdates favoriteInvert = new GetUpdates();

                    long tweetID = Convert.ToInt64(replyOpts.InReplyToStatusId);
                    favOpts.Id = tweetID;
                    InteractiveTweet tweet = TweetIdentification.FindTweet(tweetID);

                    if (tweet.IsFavorited)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("      Unfavoriting.");
                        Console.ForegroundColor = ConsoleColor.White;
                        UnfavoriteTweetOptions unfavOpts = new UnfavoriteTweetOptions();
                        unfavOpts.Id = favOpts.Id;
                        User.Account.BeginUnfavoriteTweet(unfavOpts);

                        favoriteInvert.InvertFavoriteStatus(tweetID); /* Changes whether the tweet is counted as favorited */
                    }
                    else
                    {
                        User.Account.BeginFavoriteTweet(favOpts);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("      Favoriting.");
                        Console.ForegroundColor = ConsoleColor.White;

                        favoriteInvert.InvertFavoriteStatus(tweetID); /* Changes whether the tweet is counted as favorited */

                    }
                    Console.ReadKey(true);
                    returnInfo.OverrideCommand = true;
                    returnInfo.OverrideCommandString = "/fu";
                    returnInfo.AskForCommand = false;
                }
            }
            return returnInfo;
        }


        public ActionValue RemoveTweet(string command)
        {
            ActionValue returnInfo = new ActionValue();

            if (command.Split(' ')[1].Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("      Wrong syntax. Use /del [id]");
                Console.WriteLine("      Example: /del 3f");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                long tweetID = Convert.ToInt64(TweetIdentification.GetTweetID(command.Split(' ')[1]).InReplyToStatusId);
                DeleteTweetOptions delOpts = new DeleteTweetOptions();
                delOpts.Id = tweetID;

                User.Account.DeleteTweet(delOpts);

                returnInfo.OverrideCommandString = "/fu";
                returnInfo.OverrideCommand = true;
            }
            return returnInfo;
        }

        public ActionValue BlockUser(string command)
        {
            ActionValue returnInfo = new ActionValue();

            if (command.Split(' ')[1].Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("      Wrong syntax. Use /block [id]");
                Console.WriteLine("      Example: /block 3f");
                Console.ForegroundColor = ConsoleColor.White;
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
                    long tweetID = Convert.ToInt64(TweetIdentification.GetTweetID(command.Split(' ')[1]).InReplyToStatusId);
                    InteractiveTweet tweet = TweetIdentification.FindTweet(tweetID);
                    screenName = tweet.AuthorScreenName;
                }

                if (GetUpdates.IsBlocked(screenName))
                {
                    UnblockUserOptions unblockOpts = new UnblockUserOptions();
                    unblockOpts.ScreenName = screenName;
                    User.Account.UnblockUser(unblockOpts);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("      Successfully unblocked @" + screenName);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    BlockUserOptions blockOpts = new BlockUserOptions();
                    blockOpts.ScreenName = screenName;
                    User.Account.BlockUser(blockOpts);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("      Successfully blocked @" + screenName);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            return returnInfo;
        }

        public ActionValue BlockUser(string command, bool profileCommand, string screenName)
        {
            if (profileCommand)
            {
                if (GetUpdates.IsBlocked(screenName))
                {
                    UnblockUserOptions unblockOpts = new UnblockUserOptions();
                    unblockOpts.ScreenName = screenName;
                    User.Account.UnblockUser(unblockOpts);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("      Successfully unblocked @" + screenName);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    BlockUserOptions blockOpts = new BlockUserOptions();
                    blockOpts.ScreenName = screenName;
                    User.Account.BlockUser(blockOpts);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("      Successfully blocked @" + screenName);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.ReadKey(true);
                return new ActionValue();
            }
            else
            {
                return BlockUser(command);
            }
        }

        public ActionValue FollowUser(string command)
        {
            ActionValue returnInfo = new ActionValue();

            if (command.Split(' ')[1].Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("      Wrong syntax. Use /follow [id]");
                Console.WriteLine("      Example: /follow 3f");
                Console.ForegroundColor = ConsoleColor.White;
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
                    long tweetID = Convert.ToInt64(TweetIdentification.GetTweetID(command.Split(' ')[1]).InReplyToStatusId);
                    InteractiveTweet tweet = TweetIdentification.FindTweet(tweetID);
                    screenName = tweet.AuthorScreenName;
                }

                if (GetUpdates.IsFollowing(screenName))
                {
                    UnfollowUserOptions unfollowOpts = new UnfollowUserOptions();
                    unfollowOpts.ScreenName = screenName;
                    User.Account.UnfollowUser(unfollowOpts);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("      Successfully unfollowed @" + screenName);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    FollowUserOptions followOpts = new FollowUserOptions();
                    followOpts.ScreenName = screenName;
                    User.Account.FollowUser(followOpts);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("      Successfully followed @" + screenName);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            return returnInfo;
        }
        public ActionValue FollowUser(string command, bool profileCommand, string screenName)
        {
            if (profileCommand)
            {
                if (GetUpdates.IsFollowing(screenName))
                {
                    UnfollowUserOptions unfollowOpts = new UnfollowUserOptions();
                    unfollowOpts.ScreenName = screenName;
                    User.Account.UnfollowUser(unfollowOpts);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("      Successfully unfollowed @" + screenName);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    FollowUserOptions followOpts = new FollowUserOptions();
                    followOpts.ScreenName = screenName;
                    User.Account.FollowUser(followOpts);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("      Successfully followed @" + screenName);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.ReadKey(true);
                return new ActionValue();
            }
            else
            {
                return FollowUser(command);
            }
        }

        /// <summary>
        /// Accesses the profile of a user
        /// </summary>
        /// <param name="command">The entire command string</param>
        /// <returns></returns>
        public ActionValue ShowProfile(string command)
        {
            ActionValue returnInfo = new ActionValue();

            GetUserProfileForOptions profileOpts = new GetUserProfileForOptions();
            string screenName = "";
            try
            {
                bool exceptionTest = command.Split(' ')[1].StartsWith("@");
            }
            catch (IndexOutOfRangeException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("      Wrong syntax. Use /profile [id]");
                Console.WriteLine("      Example: /profile 3f");
                Console.WriteLine("      If you meant to search by username,");
                Console.WriteLine("      use /profile @username");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey(true);
                returnInfo.OverrideCommandString = "/u";
                returnInfo.OverrideCommand = true;
                returnInfo.AskForCommand = false;
                return returnInfo;
            }
            if (command.Split(' ')[1].StartsWith("@"))
            {
                screenName = command.Split(' ')[1].Remove(0, 1);
            }
            else if (command.Split(' ')[1].Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("      Wrong syntax. Use /profile [id]");
                Console.WriteLine("      Example: /profile 3f");
                Console.WriteLine("      If you meant to search by username,");
                Console.WriteLine("      use /profile @username");
                Console.ForegroundColor = ConsoleColor.White;
                returnInfo.OverrideCommandString = "/u";
                returnInfo.OverrideCommand = true;
                returnInfo.AskForCommand = false;
                return returnInfo;
            }
            else
            {
                screenName = TweetIdentification.GetTweetID(command.Split(' ')[1]).Status;
            }

            profileOpts.ScreenName = screenName;
            TwitterUser profile = User.Account.GetUserProfileFor(profileOpts);
            ScreenDraw showProfile = new ScreenDraw();
            string profileCommand = ""; /* The command from inside the profile screen */

            do
            {
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

                showProfile.ShowUserProfile(profile);
                Cursor returnPosition = new Cursor();
                returnPosition.X = Console.CursorLeft;
                returnPosition.Y = Console.CursorTop;

                profileCommand = User.GetCommand();

                string[] splitCommand = new string[2];
                splitCommand = profileCommand.Split(' ');

                if (splitCommand[0].ToLower().CompareTo("/follow") == 0 || splitCommand[0].ToLower().CompareTo("/f") == 0)
                {
                    FollowUser(profileCommand, true, screenName);
                }

                if (splitCommand[0].ToLower().CompareTo("/block") == 0)
                {
                    BlockUser(profileCommand, true, screenName);
                }

            } while (profileCommand.ToLower().CompareTo("/b") != 0);


            returnInfo.AskForCommand = false;
            return returnInfo;
        }

        public ActionValue ShowTweet(string command)
        {
            ActionValue returnInfo = new ActionValue();

            try /* Checks if the command was valid */
            {
                bool exceptionTest = command.Split(' ')[1].StartsWith("@");
            }
            catch (IndexOutOfRangeException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("      Wrong syntax. Use /tweet [id]");
                Console.WriteLine("      Example: /tweet 3f");
                Console.ForegroundColor = ConsoleColor.White;
                return new ActionValue(); /* Exits method with default opts */
            }
            if (command.Split(' ')[1].Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("      Wrong syntax. Use /tweet [id]");
                Console.WriteLine("      Example: /tweet 3f");
                Console.ForegroundColor = ConsoleColor.White;
                return new ActionValue(); /* Exits method with default opts */
            }
            else
            {
                long tweetID = Convert.ToInt64(TweetIdentification.GetTweetID(command.Split(' ')[1]).InReplyToStatusId);
                InteractiveTweet tweet = TweetIdentification.FindTweet(tweetID);
                ScreenDraw tweetDrawer = new ScreenDraw();
                string tweetCommand = "";
                do
                {
                    tweetDrawer.DrawTweet(tweet);

                    tweetCommand = User.CounterConsole();

                    if (tweetCommand.ToLower().Split(' ')[0].CompareTo("/open") == 0)
                    {
                        Process.Start(tweet.LinkToTweet);
                        tweetCommand = "/b";
                    }

                } while (tweetCommand.ToLower().CompareTo("/b") != 0);
                returnInfo.AskForCommand = false;
            }

            return returnInfo;
        }

        public ActionValue Update(string command)
        {
            newTweet.ShowUpdates(User.Account, showUpdates, false);
            return new ActionValue();
        }
        public ActionValue Update(string command, bool fullUpdate)
        {
            newTweet.ShowUpdates(User.Account, showUpdates, fullUpdate);
            return new ActionValue();
        }

        public ActionValue GetID(string command)
        {
            if (User.IsMissingArgs(command) == false)
            {
                if (command.Split(' ')[1].Length != 2)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("      Wrong syntax. Use /id [id]");
                    Console.WriteLine("      Example: /id 3f");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write("     Tweet ID: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    var id = TweetIdentification.GetTweetID(command.Split(' ')[1]);
                    Console.WriteLine(id.InReplyToStatusId);
                }
            }
            return new ActionValue();
        }


        public ActionValue Mentions(string command)
        {
            ActionValue returnInfo = new ActionValue();

            string mentionCommand = "";
            GetUpdates mentionGet = new GetUpdates();
            List<InteractiveTweet> mentions = mentionGet.GetMentions();

            do
            {
                ScreenDraw drawMentions = new ScreenDraw();
                drawMentions.ShowMentions();
                Console.SetCursorPosition(3, Console.CursorTop);
                mentionCommand = User.CounterConsole();
                /* Here the commands begin */

                if (mentionCommand.ToLower().StartsWith("/rt"))
                {
                    Retweet(mentionCommand);
                }
                else if (mentionCommand.ToLower().StartsWith("/r"))
                {
                    ReplyGeneric(mentionCommand);
                }
                else if (mentionCommand.ToLower().StartsWith("/f"))
                {
                    FavoriteTweet(mentionCommand);
                }

            } while (mentionCommand.ToLower().CompareTo("/b") != 0);
            returnInfo.AskForCommand = false;
            return returnInfo;
        }

        public ActionValue Help()
        {
            ActionValue returnInfo = new ActionValue();
            ScreenDraw drawHelp = new ScreenDraw();
            drawHelp.ShowHelp();
            returnInfo.AskForCommand = false;

            return returnInfo;
        }

        public ActionValue ApiInfo()
        {
            TwitterRateLimitStatus rate = User.Account.Response.RateLimitStatus;
            if (rate.RemainingHits >= 5)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (rate.RemainingHits >= 1)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.WriteLine("      You have " + rate.RemainingHits + " remaining calls out of your " + rate.HourlyLimit + " limit");

            Console.ForegroundColor = ConsoleColor.White;

            return new ActionValue(); /* Returns default values */
        }

    }
}
