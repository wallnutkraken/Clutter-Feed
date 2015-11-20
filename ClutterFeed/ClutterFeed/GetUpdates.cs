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
using CursesSharp;
using TweetSharp;
using System.Media;


namespace ClutterFeed
{
    class GetUpdates
    {
        public static string userScreenName;
        public static List<InteractiveTweet> localTweetList;

        private void CleanTweets()
        {
            try
            {
                if (localTweetList.Count >= 60)
                {
                    localTweetList.RemoveRange(localTweetList.Count - 21, 20);
                }
            }
            catch (NullReferenceException) { }
        }
        /// <summary>
        /// Updates the list of tweets one has
        /// </summary>
        public void GetTweets(bool fullUpdate)
        {
            CleanTweets();
            ListTweetsOnHomeTimelineOptions updateOpts = new ListTweetsOnHomeTimelineOptions();
            updateOpts.Count = 25;
            var numUpdates = User.Account.ListTweetsOnHomeTimeline(updateOpts);
            List<TwitterStatus> unformattedTweets = new List<TwitterStatus>();
            try
            {
                unformattedTweets = numUpdates.ToList();
            }
            catch (ArgumentNullException)
            {
                TwitterResponse resp = User.Account.Response;

                if (resp.RateLimitStatus.RemainingHits == 0)
                {
                    ScreenDraw.ShowMessage("You have hit the API Limit. Please try again in a few minutes");
                }
                else if (resp.RateLimitStatus.RemainingHits < 0)
                {
                    Program.UpdateTimer.Dispose();
                    ScreenDraw.ShowMessage("An error occured when retrieving tweets from Twitter. Please restart ClutterFeed");
                    System.Threading.Thread.Sleep(1000);
                    ScreenDraw.Tweets.Dispose();
                    ScreenDraw.HeadLine.Dispose();
                    Curses.EndWin();
                    Environment.Exit(1);
                }
                return;
            }
            int newTweetStartIndex = TweetIdentification.NewTweetStart(unformattedTweets);
            if (newTweetStartIndex == -1)
            {
                fullUpdate = true;
            }
            if (newTweetStartIndex == 0 && fullUpdate == false)
            {
                return;
            }

            if (fullUpdate == false)
            {
                for (int index = newTweetStartIndex - 1; index >= 0; index--)
                {
                    InteractiveTweet formattedTweet = new InteractiveTweet();
                    formattedTweet = ConvertTweet(unformattedTweets[index]);
                    localTweetList.Insert(0, formattedTweet);
                }
            }
            else
            {
                localTweetList = new List<InteractiveTweet>(); /* Resets the full list */
                for (int index = 0; index < unformattedTweets.Count; index++)
                {
                    InteractiveTweet formattedTweet = new InteractiveTweet();
                    formattedTweet = ConvertTweet(unformattedTweets[index]);
                    localTweetList.Add(formattedTweet);
                }

            }
        }




        /// <summary>
        /// Gets the list of Mention Objects from twitter
        /// </summary>
        /// <param name="twitter"></param>
        /// <returns></returns>
        public void GetMentions()
        {
            ListTweetsMentioningMeOptions mentionOpts = new ListTweetsMentioningMeOptions();
            mentionOpts.IncludeEntities = true;

            List<TwitterStatus> mentions = User.Account.ListTweetsMentioningMe(mentionOpts).ToList();
            for (int index = 0; index < mentions.Count; index++)
            {
                InteractiveTweet tempTweet = ConvertTweet(mentions[index], true);
                if (localTweetList.Contains(tempTweet) == false)
                {
                    localTweetList.Add(tempTweet);
                }
            }
        }

        /// <summary>
        /// Returns the profile of a twitter user
        /// </summary>
        /// <param name="author">@name of the author</param>
        public static TwitterUser GetUserProfile(string author)
        {
            return User.Account.GetUserProfile(new GetUserProfileOptions());
        }

        /// <summary>
        /// Checks if the ClutterFeed user is following the target
        /// </summary>
        /// <param name="twitter">An authorized twitter service object</param>
        /// <param name="targetScreenName">@name of the target</param>
        /// <returns></returns>
        public static bool IsFollowing(string targetScreenName)
        {
            GetFriendshipInfoOptions friendOpts = new GetFriendshipInfoOptions();
            friendOpts.SourceScreenName = userScreenName;
            friendOpts.TargetScreenName = targetScreenName;
            TwitterFriendship friend = User.Account.GetFriendshipInfo(friendOpts);
            try
            {
                return friend.Relationship.Source.Following;
            }
            catch (NullReferenceException)
            {
                return false; /* If twitter doesn't give me anything, they're probably not friends */
            } /* Fingers crossed */
        }

        public static bool IsBlocked(string targetScreenName)
        {
            ListBlockedUsersOptions blockedListOpts = new ListBlockedUsersOptions();
            var blockedList = User.Account.ListBlockedUsers(blockedListOpts);

            for (int index = 0; index < blockedList.Count; index++)
            {
                if (blockedList[index].ScreenName.ToLower().CompareTo(targetScreenName) == 0)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Initializes the API quickly
        /// </summary>
        /// <returns></returns>
        public void InitializeTwitter()
        {

            User files = new User();

            files.Run();

            OAuthAccessToken userToken = files.GetUser();
            OAuthAccessToken appToken = User.appKey;

            User.Account = new TwitterService(appToken.Token, appToken.TokenSecret);
            User.Account.AuthenticateWith(userToken.Token, userToken.TokenSecret);
            User.Account.TraceEnabled = true; /* Forget what this does */
            TwitterAccount user = User.Account.GetAccountSettings();
            if (User.Account.Response.Error != null)
            {
                ScreenDraw.ShowMessage(User.Account.Response.Error.Code + ": " + User.Account.Response.Error.Message, true);
                Curses.EndWin();
                Environment.Exit(0);
            }
            try
            {
                userScreenName = user.ScreenName;
            }
            catch (NullReferenceException)
            {
                ScreenDraw.ShowMessage("Twitter is currently unavailable.", true);
                Curses.EndWin();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Reauthenticates twitter access when needed
        /// </summary>
        public static void ReauthenticateTwitter()
        {
            User files = new User();
            OAuthAccessToken userToken = files.GetUser();
            User.Account.AuthenticateWith(userToken.Token, userToken.TokenSecret);
            TwitterAccount user = User.Account.GetAccountSettings(); /* Update screen name for user switching */
            userScreenName = user.ScreenName;
            ScreenDraw.HeadLine.Clear();
            ScreenDraw.HeadLine.Add("ClutterFeed version " + ScreenDraw.Version);
            string signOn = "Signed on as: @" + GetUpdates.userScreenName;
            ScreenDraw.HeadLine.Add(0, (ScreenInfo.WindowWidth - signOn.Length - 1), signOn);
            ScreenDraw.HeadLine.Refresh();
        }

        private string EscapeChars(string message)
        {
            message = message.Replace("&lt;", "<");
            message = message.Replace("&gt;", ">");
            message = message.Replace("&quot;", "\"");
            message = message.Replace("&amp;", "&");
            message = message.Replace("&OElig;", "Œ");
            message = message.Replace("&oelig;", "œ");
            message = message.Replace("&Scaron;", "Š");
            message = message.Replace("&scaron;", "š");
            message = message.Replace("&Yuml;", "Ÿ");
            message = message.Replace("&circ;", "^");
            message = message.Replace("&tilde;", "~");
            message = message.Replace("&ndash;", "–");
            message = message.Replace("&mdash;", "—");
            message = message.Replace("&lsquo;", "‘");
            message = message.Replace("&rsquo;", "’");
            message = message.Replace("&sbquo;", "‚");
            message = message.Replace("&ldquo;", "“");
            message = message.Replace("&rdquo;", "”");
            message = message.Replace("&bdquo;", "„");
            message = message.Replace("&dagger;", "†");
            message = message.Replace("&Dagger;", "‡");
            message = message.Replace("&permil;", "‰");
            message = message.Replace("&lsaquo;", "‹");
            message = message.Replace("&rsaquo;", "›");
            message = message.Replace("&euro;", "€");
            return message;
        }

        /// <summary>
        /// Converts the TwitterStatus object into a more readily usable InteractiveTweet object
        /// </summary>
        /// <param name="tweet">TwitterStatus object</param>
        /// <returns></returns>
        public InteractiveTweet ConvertTweet(TwitterStatus tweet)
        {
            InteractiveTweet formedTweet = new InteractiveTweet();
            formedTweet.AuthorScreenName = "@" + tweet.Author.ScreenName;
            formedTweet.AuthorDisplayName = tweet.User.Name;
            if (tweet.Entities.Urls.Count == 0 || Settings.ShortLinks)
            {
                formedTweet.Contents = EscapeChars(tweet.Text);
            }
            else
            {
                formedTweet.Contents = tweet.Text;
                foreach (var link in tweet.Entities.Urls)
                {
                    formedTweet.Contents = formedTweet.Contents.Replace(link.Value, link.ExpandedValue);
                }
                formedTweet.Contents = EscapeChars(formedTweet.Contents);
            }
            formedTweet.ID = tweet.Id;
            TweetIdentification generateID = new TweetIdentification();
            formedTweet.TweetIdentification = generateID.GenerateIdentification();
            formedTweet.IsFavorited = tweet.IsFavorited;
            formedTweet.IsRetweeted = tweet.IsRetweeted;
            formedTweet.LinkToTweet = @"https://twitter.com/" + tweet.Author.ScreenName + @"/status/" + tweet.Id;
            formedTweet.FavoriteCount = tweet.FavoriteCount;
            formedTweet.RetweetCount = tweet.RetweetCount;
            formedTweet.TimePosted = tweet.CreatedDate;

            var shit = tweet.Entities.Urls;


            if (formedTweet.Contents.Contains("@" + userScreenName))
            {
                try
                {
                    SoundPlayer notification = new SoundPlayer();

                    notification.SoundLocation = Environment.CurrentDirectory + "/notification.wav";
                    notification.Play();
                }
                catch (Exception)
                {
                }
            }

            return formedTweet;
        }

        /// <summary>
        /// Override for mentions only
        /// </summary>
        public InteractiveTweet ConvertTweet(TwitterStatus tweet, bool isMention)
        {
            if (isMention)
            {
                InteractiveTweet formedTweet = new InteractiveTweet();
                /* Big block of filling the InteractiveTweet properties */
                formedTweet.AuthorScreenName = "@" + tweet.Author.ScreenName;
                formedTweet.AuthorDisplayName = tweet.User.Name;
                if (tweet.Entities.Urls.Count == 0 || Settings.ShortLinks)
                {
                    formedTweet.Contents = EscapeChars(tweet.Text);
                }
                else
                {
                    formedTweet.Contents = tweet.Text;
                    foreach (var link in tweet.Entities.Urls)
                    {
                        formedTweet.Contents = formedTweet.Contents.Replace(link.Value, link.ExpandedValue);
                    }
                    formedTweet.Contents = EscapeChars(formedTweet.Contents);
                }
                formedTweet.ID = tweet.Id;
                TweetIdentification generateID = new TweetIdentification();
                formedTweet.TweetIdentification = generateID.GenerateIdentification();
                formedTweet.IsFavorited = tweet.IsFavorited;
                formedTweet.IsRetweeted = tweet.IsRetweeted;
                formedTweet.IsMention = true;
                formedTweet.LinkToTweet = @"https://twitter.com/" + tweet.Author.ScreenName + @"/status/" + tweet.Id;
                formedTweet.FavoriteCount = tweet.FavoriteCount;
                formedTweet.RetweetCount = tweet.RetweetCount;
                formedTweet.TimePosted = tweet.CreatedDate;


                if (formedTweet.Contents.Contains("@" + userScreenName) && formedTweet.IsMention == false)
                {
                    try
                    {
                        SoundPlayer notification = new SoundPlayer();

                        notification.SoundLocation = Environment.CurrentDirectory + "/notification.wav";
                        notification.Play();
                    }
                    catch (Exception)
                    {
                    }
                }

                return formedTweet;
            }
            else
            {
                return ConvertTweet(tweet);
            }
        }

        /// <summary>
        /// Inverts the favorite status of a tweet (by ID)
        /// </summary>
        /// <param name="tweetID">the long tweet ID</param>
        public void InvertFavoriteStatus(long tweetID)
        {
            for (int index = 0; index < localTweetList.Count; index++)
            {
                if (localTweetList[index].ID == tweetID)
                {
                    localTweetList[index].IsFavorited = !localTweetList[index].IsFavorited;
                }
            }
        }

        /// <summary>
        /// Inverts the retweet status of a tweet (by ID)
        /// </summary>
        /// <param name="tweetID">the long tweet ID</param>
        public void InvertRetweetStatus(long tweetID)
        {
            for (int index = 0; index < localTweetList.Count; index++)
            {
                if (localTweetList[index].ID == tweetID)
                {
                    localTweetList[index].IsRetweeted = !localTweetList[index].IsRetweeted;
                }
            }
        }

    }
}
