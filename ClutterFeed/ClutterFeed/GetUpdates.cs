using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;


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
        public void GetTweets(TwitterService twitter, bool fullUpdate)
        {
            ListTweetsOnHomeTimelineOptions updateOpts = new ListTweetsOnHomeTimelineOptions();
            updateOpts.Count = 15;
            bool continueMethod = true;
            var numUpdates = twitter.ListTweetsOnHomeTimeline(updateOpts);
            List<TwitterStatus> unformattedTweets = new List<TwitterStatus>();
            try
            {
                unformattedTweets = numUpdates.ToList();
            }
            catch (ArgumentNullException)
            {
                Console.Clear();
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
            if(newTweetStartIndex == -1)
            {
                fullUpdate = true;
            }
            if(newTweetStartIndex == 0 && fullUpdate == false)
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
                    for(int index = 0; index < unformattedTweets.Count; index++)
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
        /// Gets new tweets that don't already exist in the list of tweets and removes old ones without changing the Identification
        /// strings for old, but not removed tweets
        /// </summary>
        /// <param name="twitter">An initiated and authenticated twitter API object</param>
        /// <returns></returns>
        //public List<InteractiveTweet> UpdateTweets(TwitterService twitter)
        //{
        //    List<InteractiveTweet> oldList = StatusCommunication.updateTweets;
        //    List<InteractiveTweet> intermediateList = new List<InteractiveTweet>();
        //    List<InteractiveTweet> newList = new List<InteractiveTweet>();
        //    ListTweetsOnHomeTimelineOptions gettingOpts = new ListTweetsOnHomeTimelineOptions();
        //    gettingOpts.Count = 15;
        //    //twitter.IncludeRetweets = true;
        //    var newUpdates = twitter.ListTweetsOnHomeTimeline(gettingOpts);


        //    List<TwitterStatus> unformattedTweets;

        //    try
        //    {
        //        unformattedLocalTweetList = newUpdates.ToList();
        //        unformattedTweets = unformattedLocalTweetList;
        //    }
        //    catch (ArgumentNullException) /* I don't know *exactly* why this exception is caused, */
        //    {                             /* but it's most likely the API being limited. */
        //        Console.Clear();
        //        Console.ForegroundColor = ConsoleColor.Red;
        //        string errorString = "ERROR: API Limited (I think)";
        //        Console.SetCursorPosition((Console.WindowWidth / 2) - (errorString.Length / 2), Console.WindowHeight / 2);
        //        Console.Write(errorString);
        //        Console.ReadKey(true);
        //        Console.Clear();
        //        Console.WriteLine("Please wait a bit before using the API again.");
        //        Console.WriteLine("However, you can use /api to see the status of the API limit.");
        //        Console.ForegroundColor = ConsoleColor.White;
        //        return null;
        //    }
        //    for (int index = 0; index < unformattedTweets.Count; index++)
        //    {
        //        InteractiveTweet formattedTweet = new InteractiveTweet();
        //        formattedTweet = ConvertTweet(unformattedTweets[index]);
        //        intermediateList.Add(formattedTweet);
        //        System.Threading.Thread.Sleep(25);
        //    }

        //    int tweetCount = 0; /* How many new tweets we get */
        //    for (int index = 0; index < intermediateList.Count; index++) /* Counts how many new tweets we have */
        //    {                                                           /* Since I don't trust the API */
        //        if (UpdateExists(oldList, intermediateList[index]))
        //        {
        //            tweetCount++;
        //        }
        //    }

        //    if (tweetCount == 0) /* Returns just the new list if all the tweets are new */
        //    {
        //        return intermediateList;
        //    }

        //    for (int index = 0; index < intermediateList.Count; index++) /* Michael Jackson */
        //    {
        //        newList.Add(intermediateList[index]);
        //    }
        //    int superIndex = oldList.Count - (1 + tweetCount); /* Two indexes for one loop? Nani?! */
        //    for (int newListIndex = tweetCount; newListIndex > 0; newListIndex--) /* This loop adds the old enough, but not too old */
        //    {                                                                     /* Entries from the old list into the new one */
        //        try
        //        {
        //            newList.Add(oldList[superIndex]);
        //        }
        //        catch (ArgumentOutOfRangeException)
        //        {
        //            return intermediateList;
        //        }
        //        superIndex++;
        //    }

        //    localTweetList = newList;
        //    LastTweetID = localTweetList[localTweetList.Count - 1].ID;

        //    return newList;
        //}


        /// <summary>
        /// Gets the list of Mention Objects from twitter
        /// </summary>
        /// <param name="twitter"></param>
        /// <returns></returns>
        public static List<TwitterStatus> GetMentions(TwitterService twitter)
        {
            ListTweetsMentioningMeOptions mentionOpts = new ListTweetsMentioningMeOptions();
            mentionOpts.IncludeEntities = true;
            mentionOpts.Count = 15;

            List<TwitterStatus> mentions = twitter.ListTweetsMentioningMe(mentionOpts).ToList();

            return mentions;
        }

        /// <summary>
        /// Returns the profile of a twitter user
        /// </summary>
        /// <param name="twitterAccess">An authorized twitter service object</param>
        /// <param name="author">@name of the author</param>
        public static TwitterUser GetUserProfile(TwitterService twitterAccess, string author)
        {
            return twitterAccess.GetUserProfile(new GetUserProfileOptions());
        }

        /// <summary>
        /// Checks if the ClutterFeed user is following the target
        /// </summary>
        /// <param name="twitter">An authorized twitter service object</param>
        /// <param name="targetScreenName">@name of the target</param>
        /// <returns></returns>
        public static bool IsFollowing(TwitterService twitter, string targetScreenName)
        {
            GetFriendshipInfoOptions friendOpts = new GetFriendshipInfoOptions();
            friendOpts.SourceScreenName = userScreenName;
            friendOpts.TargetScreenName = targetScreenName;
            TwitterFriendship friend = twitter.GetFriendshipInfo(friendOpts);
            return friend.Relationship.Source.Following;
        }

        public static bool IsBlocked(TwitterService twitter, string targetScreenName)
        {
            ListBlockedUsersOptions blockedListOpts = new ListBlockedUsersOptions();
            var blockedList = twitter.ListBlockedUsers(blockedListOpts);

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
        public TwitterService InitializeTwitter()
        {

            User files = new User();

            files.Run();

            OAuthAccessToken userToken = files.GetUser();
            OAuthAccessToken appToken = files.GetApp();

            TwitterService service = new TwitterService(appToken.Token, appToken.TokenSecret);
            service.AuthenticateWith(userToken.Token, userToken.TokenSecret);
            service.TraceEnabled = true;
            TwitterAccount user = service.GetAccountSettings();
            userScreenName = user.ScreenName;

            return service;
        }


        /// <summary>
        /// Converts the TwitterStatus object into a more readily usable InteractiveTweet object
        /// </summary>
        /// <param name="tweet">TwitterStatus object</param>
        /// <returns></returns>
        public InteractiveTweet ConvertTweet(TwitterStatus tweet)
        {
            InteractiveTweet formedTweet = new InteractiveTweet();
            formedTweet.AuthorName = "@" + tweet.Author.ScreenName;
            formedTweet.Contents = tweet.Text;
            formedTweet.ID = tweet.Id;
            TweetIdentification generateID = new TweetIdentification();
            formedTweet.TweetIdentification = generateID.GenerateIdentification();
            formedTweet.IsFavorited = tweet.IsFavorited;
            formedTweet.IsRetweeted = tweet.IsRetweeted;

            return formedTweet;
        }
    }
}
