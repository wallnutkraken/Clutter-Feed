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
using System.Media;


namespace ClutterFeed
{
    class GetUpdates
    {
        public static string userScreenName;
        public static List<InteractiveTweet> localTweetList;

        /// <summary>
        /// Checks if a specifc update exists in a list
        /// </summary>
        /// <param name="where">List to check</param>
        /// <param name="what">Update to find</param>
        /// <returns></returns>
        private bool UpdateExists(List<InteractiveTweet> where, InteractiveTweet what)
        {
            bool doesIt = false;
            for (int index = 0; index < where.Count; index++)
            {
                if (where[index].ID == what.ID)
                {
                    doesIt = true;
                }
            }

            return doesIt;
        }

        /// <summary>
        /// Updates the list of tweets one has
        /// </summary>
        public void GetTweets(bool fullUpdate)
        {
            ListTweetsOnHomeTimelineOptions updateOpts = new ListTweetsOnHomeTimelineOptions();
            bool continueMethod = true;
            var numUpdates = User.Account.ListTweetsOnHomeTimeline(updateOpts);
            List<TwitterStatus> unformattedTweets = new List<TwitterStatus>();
            try
            {
                unformattedTweets = numUpdates.ToList();
            }
            catch (ArgumentNullException)
            {
                TwitterResponse resp = User.Account.Response;
                
                Console.ForegroundColor = ConsoleColor.Red;
                string errorString = "ERROR: Empty list of tweets. Possibly API limited.";
                Console.SetCursorPosition((Console.WindowWidth / 2) - (errorString.Length / 2), Console.WindowHeight / 2);
                Console.Write(errorString);
                Console.ReadKey(true);
                Console.Clear();
                Console.WriteLine("Please wait a bit before using the API again.");
                Console.WriteLine("However, you can use /api to see the status of the API limit.");
                Console.ForegroundColor = ConsoleColor.White;
                continueMethod = false;
            }
            int newTweetStartIndex = TweetIdentification.NewTweetStart(unformattedTweets);
            if (newTweetStartIndex == -1)
            {
                fullUpdate = true;
            }
            if (newTweetStartIndex == 0 && fullUpdate == false)
            {
                continueMethod = false;
            }

            if (continueMethod)
            {
                if (fullUpdate == false)
                {
                    for (int index = newTweetStartIndex - 1; index >= 0; index--)
                    {
                        InteractiveTweet formattedTweet = new InteractiveTweet();
                        formattedTweet = ConvertTweet(unformattedTweets[index]);
                        localTweetList.Insert(0, formattedTweet);
                        System.Threading.Thread.Sleep(25);
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
                        System.Threading.Thread.Sleep(25);
                    }
                }
            }
        }




        /// <summary>
        /// Gets the list of Mention Objects from twitter
        /// </summary>
        /// <param name="twitter"></param>
        /// <returns></returns>
        public List<InteractiveTweet> GetMentions()
        {
            ListTweetsMentioningMeOptions mentionOpts = new ListTweetsMentioningMeOptions();
            mentionOpts.IncludeEntities = true;
            mentionOpts.Count = 15;

            List<TwitterStatus> mentions = User.Account.ListTweetsMentioningMe(mentionOpts).ToList();
            List<InteractiveTweet> taggedMentions = new List<InteractiveTweet>();
            for (int index = 0; index < mentions.Count; index++)
            {
                localTweetList.Add(ConvertTweet(mentions[index], true));
                System.Threading.Thread.Sleep(25);
            }

            return taggedMentions;
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
            userScreenName = user.ScreenName;
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
            formedTweet.Contents = tweet.Text;
            formedTweet.ID = tweet.Id;
            TweetIdentification generateID = new TweetIdentification();
            formedTweet.TweetIdentification = generateID.GenerateIdentification();
            formedTweet.IsFavorited = tweet.IsFavorited;
            formedTweet.IsRetweeted = tweet.IsRetweeted;
            formedTweet.LinkToTweet = @"https://twitter.com/" + tweet.Author.ScreenName + @"/status/" + tweet.Id;
            formedTweet.FavoriteCount = tweet.FavoriteCount;
            formedTweet.RetweetCount = tweet.RetweetCount;
            formedTweet.TimePosted = tweet.CreatedDate;


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
            InteractiveTweet formedTweet = new InteractiveTweet();
            /* Big block of filling the InteractiveTweet properties */
            formedTweet.AuthorScreenName = "@" + tweet.Author.ScreenName;
            formedTweet.AuthorDisplayName = tweet.User.Name;
            formedTweet.Contents = tweet.Text;
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
