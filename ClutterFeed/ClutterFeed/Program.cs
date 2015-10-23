using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;
using System.Threading;

namespace ClutterFeed
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode; /* Enables unicode */
            SetScreenColor.SetColor(ConsoleColor.Cyan, 0, 126, 199); /* Changes cyan to dark blue-ish */


            Console.ForegroundColor = ConsoleColor.White;
            GetUpdates showUpdates = new GetUpdates();
            User getUser = new User();
            OAuthAccessToken key = getUser.GetUser();
            TwitterService twitterAccess = showUpdates.InitializeTwitter();

            twitterAccess.IncludeRetweets = true;

            string command = "/fullupdate";

            bool askForCommand = true;
            do
            {
                if (command.StartsWith("/"))
                {
                    bool validCommand = false;

                    StatusCommunication newTweet = new StatusCommunication();
                    if ((command.ToLower().CompareTo("/fullupdate") == 0) || (command.ToLower().CompareTo("/fu") == 0))
                    {
                        validCommand = true;
                        newTweet.ShowUpdates(twitterAccess, showUpdates, true);
                    }
                    if ((command.ToLower().CompareTo("/update") == 0) || (command.ToLower().CompareTo("/u") == 0))
                    {
                        validCommand = true;
                        newTweet.ShowUpdates(twitterAccess, showUpdates, false);
                    }
                    if (command.ToLower().CompareTo("/new") == 0)
                    {
                        validCommand = true;
                        command = command.Remove(0, 4);
                        newTweet.PostTweet(twitterAccess, command);
                        askForCommand = false;
                        Thread.Sleep(500);
                    }
                    else if (command.ToLower().CompareTo("/n") == 0)
                    {
                        validCommand = true;
                        command = command.Remove(0, 2);
                        newTweet.PostTweet(twitterAccess, command);
                        askForCommand = false;
                        Thread.Sleep(500);
                    }
                    string[] getIdentifier = new string[1];
                    getIdentifier = command.ToLower().Split(' ');


                    if (getIdentifier[0].CompareTo("/r") == 0)
                    {

                        validCommand = true;
                        if (User.IsMissingArgs(getIdentifier) == false) /* It's just an exception catching method, don't mind it */
                        {
                            if (getIdentifier[1].Length != 2)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("      Wrong syntax. Use /r [id] [reply]");
                                Console.WriteLine("      Example: /r 3f Hey Adam, I'm doing fine!");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else
                            {
                                command = command.Remove(0, 5);
                                SendTweetOptions replyOpts = TweetIdentification.GetTweetID(getIdentifier[1]);
                                replyOpts.Status = replyOpts.Status + " " + command;
                                twitterAccess.BeginSendTweet(replyOpts);
                                askForCommand = false;
                                Thread.Sleep(500);
                            }
                        }
                    }
                    if (getIdentifier[0].CompareTo("/id") == 0)
                    {
                        validCommand = true;
                        if (User.IsMissingArgs(getIdentifier) == false)
                        {
                            if (getIdentifier[1].Length != 2)
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
                                var id = TweetIdentification.GetTweetID(getIdentifier[1]);
                                Console.WriteLine(id.InReplyToStatusId);
                            }
                        }
                    }

                    if (getIdentifier[0].CompareTo("/rn") == 0)
                    {
                        validCommand = true;
                        if (User.IsMissingArgs(getIdentifier) == false)
                        {

                            if (getIdentifier[1].Length != 2)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("      Wrong syntax. Use /rn [id] [reply]");
                                Console.WriteLine("      Example: /rn 3f @UserName I know him too!");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else
                            {
                                command = command.Remove(0, 6);
                                SendTweetOptions replyOpts = TweetIdentification.GetTweetID(getIdentifier[1]);
                                replyOpts.Status = command;
                                twitterAccess.BeginSendTweet(replyOpts);
                                askForCommand = false;
                                Thread.Sleep(500);
                            }
                        }

                    }
                    if (getIdentifier[0].CompareTo("/rt") == 0)
                    {
                        validCommand = true;
                        if (User.IsMissingArgs(getIdentifier) == false) /* It's just an exception catching method, don't mind it */
                        {
                            if (getIdentifier[1].Length != 2)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("      Wrong syntax. Use /rt [id]");
                                Console.WriteLine("      Example: /rt 3f");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else
                            {
                                command = command.Remove(0, 6);
                                SendTweetOptions replyOpts = TweetIdentification.GetTweetID(getIdentifier[1]);
                                RetweetOptions retweetOpts = new RetweetOptions();

                                long tweetID = Convert.ToInt64(replyOpts.InReplyToStatusId);
                                retweetOpts.Id = tweetID;
                                TwitterStatus tweet = new TwitterStatus();
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
                                    if (tweet.IsRetweeted == false)
                                    {
                                        twitterAccess.Retweet(retweetOpts);
                                        command = "/fu";
                                        continue; /* You can't stop me */
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("      This tweet was already retweeted.");
                                        Console.WriteLine("      Unretweeting.");
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                }
                            }
                        }
                    }

                    if ((getIdentifier[0].CompareTo("/fav") == 0) || (getIdentifier[0].CompareTo("/f") == 0))
                    {
                        validCommand = true;
                        if (User.IsMissingArgs(getIdentifier) == false) /* It's just an exception catching method, don't mind it */
                        {
                            if (getIdentifier[1].Length != 2)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("      Wrong syntax. Use /fav [id]");
                                Console.WriteLine("      Example: /fav 3f");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else
                            {
                                if (getIdentifier[0].CompareTo("/f") == 0)
                                {
                                    command = command.Remove(0, 5);
                                }
                                else
                                {
                                    command = command.Remove(0, 7);
                                }

                                SendTweetOptions replyOpts = TweetIdentification.GetTweetID(getIdentifier[1]);
                                FavoriteTweetOptions favOpts = new FavoriteTweetOptions();

                                long tweetID = Convert.ToInt64(replyOpts.InReplyToStatusId);
                                favOpts.Id = tweetID;
                                TwitterStatus tweet = TweetIdentification.FindTweet(tweetID);

                                if (tweet.IsFavorited)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("      Favourite failed.");
                                    Console.WriteLine("      You already favorited this tweet.");
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                                else
                                {
                                    twitterAccess.BeginFavoriteTweet(favOpts);
                                    command = "/fu";
                                    continue; /* Used a continue here because I am too afraid to change the askForCommand bit */
                                }

                            }
                        }
                    }
                    if ((getIdentifier[0].CompareTo("/unfav") == 0) || (getIdentifier[0].CompareTo("/uf") == 0)) /* Fix unfav */ /* Fix'd */
                    {
                        validCommand = true;
                        if (User.IsMissingArgs(getIdentifier) == false) /* It's just an exception catching method, don't mind it */
                        {
                            if (getIdentifier[1].Length != 2)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("      Wrong syntax. Use /unfav [id]");
                                Console.WriteLine("      Example: /unfav 3f");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else
                            {
                                if (getIdentifier[0].CompareTo("/uf") == 0)
                                {
                                    command = command.Remove(0, 6);
                                }
                                else
                                {
                                    command = command.Remove(0, 8);
                                }
                                SendTweetOptions replyOpts = TweetIdentification.GetTweetID(getIdentifier[1]);
                                UnfavoriteTweetOptions unfavOpts = new UnfavoriteTweetOptions();

                                long tweetID = Convert.ToInt64(replyOpts.InReplyToStatusId);
                                unfavOpts.Id = tweetID;
                                TwitterStatus tweet = TweetIdentification.FindTweet(tweetID);

                                if (tweet.IsFavorited)
                                {
                                    twitterAccess.BeginUnfavoriteTweet(unfavOpts);
                                    command = "/fu";
                                    continue; /* I'll just use continue when I need a custom forced command now */
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("      Error: this tweet isn't favorited.");
                                    Console.ForegroundColor = ConsoleColor.White;
                                }

                            }
                        }
                    }

                    if ((getIdentifier[0].ToLower().CompareTo("/del") == 0) || (getIdentifier[0].ToLower().CompareTo("/d") == 0))
                    {
                        validCommand = true;
                        if (getIdentifier[1].Length != 2)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("      Wrong syntax. Use /del [id]");
                            Console.WriteLine("      Example: /del 3f");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            long tweetID = Convert.ToInt64(TweetIdentification.GetTweetID(getIdentifier[1]).InReplyToStatusId);
                            DeleteTweetOptions delOpts = new DeleteTweetOptions();
                            delOpts.Id = tweetID;

                            twitterAccess.DeleteTweet(delOpts);

                            command = "/fu";
                            continue;
                        }
                    }

                    if (getIdentifier[0].ToLower().CompareTo("/profile") == 0)
                    {
                        validCommand = true;
                        GetUserProfileForOptions profileOpts = new GetUserProfileForOptions();
                        string screenName = "";
                        try
                        {
                            bool exceptionTest = getIdentifier[1].StartsWith("@");
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
                            command = "/u";
                            continue;
                        }
                        if (getIdentifier[1].StartsWith("@"))
                        {
                            screenName = getIdentifier[1].Remove(0, 1);
                        }
                        else if (getIdentifier[1].Length != 2)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("      Wrong syntax. Use /profile [id]");
                            Console.WriteLine("      Example: /profile 3f");
                            Console.WriteLine("      If you meant to search by username,");
                            Console.WriteLine("      use /profile @username");
                            Console.ForegroundColor = ConsoleColor.White;
                            command = "/u";
                            continue;
                        }
                        else
                        {
                            screenName = TweetIdentification.GetTweetID(getIdentifier[1]).Status;
                        }

                        profileOpts.ScreenName = screenName;
                        TwitterUser profile = twitterAccess.GetUserProfileFor(profileOpts);
                        ScreenDraw showProfile = new ScreenDraw();
                        string profileCommand = ""; /* The command from inside the profile screen */

                        do
                        {
                            if (GetUpdates.IsFollowing(twitterAccess, screenName)) /* Because the profile object doesn't say this */
                            {
                                ScreenDraw.IsFollowing = true;
                            }
                            else
                            {
                                ScreenDraw.IsFollowing = false;
                            }
                            if (GetUpdates.IsBlocked(twitterAccess, screenName))
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
                                if (GetUpdates.IsFollowing(twitterAccess, profile.ScreenName))
                                {
                                    UnfollowUserOptions unfollowOpts = new UnfollowUserOptions();
                                    unfollowOpts.ScreenName = profile.ScreenName;

                                    twitterAccess.UnfollowUser(unfollowOpts);

                                    ScreenDraw.ShowSuccess("You unfollowed @" + profile.ScreenName);
                                    Console.ReadKey(true);
                                    profileCommand = "";
                                }
                                else
                                {
                                    FollowUserOptions followOpts = new FollowUserOptions();
                                    followOpts.Follow = true;
                                    followOpts.ScreenName = profile.ScreenName;

                                    twitterAccess.FollowUser(followOpts);

                                    ScreenDraw.ShowSuccess("Congratulations! You now follow @" + profile.ScreenName);
                                    Console.ReadKey(true);
                                    profileCommand = "";
                                }
                            }

                            if (splitCommand[0].ToLower().CompareTo("/block") == 0)
                            {
                                if (ScreenDraw.IsBlocked)
                                {
                                    UnblockUserOptions unblockOpts = new UnblockUserOptions();
                                    unblockOpts.ScreenName = profile.ScreenName;

                                    twitterAccess.UnblockUser(unblockOpts);

                                    ScreenDraw.ShowSuccess("Successfully unblocked @" + profile.ScreenName);
                                    Console.ReadKey(true);
                                    profileCommand = "";
                                }
                                else
                                {
                                    BlockUserOptions blockOpts = new BlockUserOptions();
                                    blockOpts.ScreenName = profile.ScreenName;

                                    twitterAccess.BlockUser(blockOpts);

                                    ScreenDraw.ShowSuccess("Successfully blocked @" + profile.ScreenName);
                                    Console.ReadKey(true);
                                    profileCommand = "";
                                }
                            }

                        } while (profileCommand.ToLower().CompareTo("/b") != 0);


                        askForCommand = false;
                    }
                    if (getIdentifier[0].ToLower().CompareTo("/me") == 0)
                    {
                        validCommand = true;
                        string mentionCommand = "";
                        List<TwitterStatus> mentions = GetUpdates.GetMentions(twitterAccess);

                        do
                        {
                            ScreenDraw drawMentions = new ScreenDraw();
                            drawMentions.DrawMentions(mentions);
                            Console.SetCursorPosition(3, Console.WindowHeight - 1);
                            mentionCommand = User.GetCommand();
                        } while (mentionCommand.ToLower().CompareTo("/b") != 0);
                        askForCommand = false;
                    }

                    if (getIdentifier[0].ToLower().CompareTo("/help") == 0 || getIdentifier[0].ToLower().CompareTo("/h") == 0)
                    {
                        validCommand = true;
                        ScreenDraw drawHelp = new ScreenDraw();
                        drawHelp.ShowHelp();
                        askForCommand = false;
                    }


                    if (command.ToLower().Contains("/api"))
                    {
                        validCommand = true;
                        TwitterRateLimitStatus rate = twitterAccess.Response.RateLimitStatus;
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
                    }

                    /////
                    if (validCommand == false && command.ToLower() != "/q")
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("      Command invalid. Try again.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                }
                else
                {
                    StatusCommunication newTweet = new StatusCommunication();
                    newTweet.PostTweet(twitterAccess, command);
                    askForCommand = false;
                }
                if (askForCommand)
                {
                    command = User.GetCommand();
                }
                else
                {
                    command = "/u";
                    askForCommand = true;
                    Thread.Sleep(500);
                }
            } while ((!command.ToLower().StartsWith("/q")));
            Console.Clear();
        }
    }
}
