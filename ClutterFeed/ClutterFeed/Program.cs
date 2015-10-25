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
            Actions twitterDo = new Actions();
            twitterDo.SetUpTwitter();

            string command = "/fullupdate";
            do
            {
                ActionValue commandMetadata = new ActionValue();

                if (command.StartsWith("/"))
                {

                    StatusCommunication newTweet = new StatusCommunication();
                    if ((command.ToLower().CompareTo("/fullupdate") == 0) || (command.ToLower().CompareTo("/fu") == 0))
                    {
                        commandMetadata = twitterDo.Update(command, true);
                    }
                    if ((command.ToLower().CompareTo("/update") == 0) || (command.ToLower().CompareTo("/u") == 0))
                    {
                        commandMetadata = twitterDo.Update(command);
                    }


                    if (command.Split(' ')[0].CompareTo("/r") == 0)
                    {
                        commandMetadata = twitterDo.GenericReply(command);
                    }

                    if (command.Split(' ')[0].CompareTo("/id") == 0)
                    {
                        commandMetadata = twitterDo.GetID(command);
                    }

                    if (command.Split(' ')[0].CompareTo("/rn") == 0)
                    {
                        commandMetadata = twitterDo.NoAddedMentionReply(command);
                    }
                    if (command.Split(' ')[0].CompareTo("/rt") == 0)
                    {
                        commandMetadata = twitterDo.Retweet(command);
                    }

                    if ((command.Split(' ')[0].CompareTo("/fav") == 0) || (command.Split(' ')[0].CompareTo("/f") == 0))
                    {
                        commandMetadata = twitterDo.FavoriteTweet(command);
                    }

                    if ((command.Split(' ')[0].ToLower().CompareTo("/del") == 0) || (command.Split(' ')[0].ToLower().CompareTo("/d") == 0))
                    {
                        commandMetadata = twitterDo.RemoveTweet(command);
                    }

                    if (command.Split(' ')[0].ToLower().CompareTo("/profile") == 0)
                    {
                        commandMetadata = twitterDo.AccessProfile(command);
                    }

                    if (command.Split(' ')[0].ToLower().CompareTo("/me") == 0)
                    {
                        commandMetadata = twitterDo.Mentions(command);
                    }

                    if (command.Split(' ')[0].ToLower().CompareTo("/help") == 0 || command.Split(' ')[0].ToLower().CompareTo("/h") == 0)
                    {
                        commandMetadata = twitterDo.Help(command);
                    }


                    if (command.ToLower().Contains("/api"))
                    {
                        commandMetadata = twitterDo.ApiInfo();
                    }
                }
                /* End of commands */

                if(command.ToLower().StartsWith("/") == false) /* EXCEPT for this one */
                {
                    commandMetadata = twitterDo.NewTweet(command);
                }
                if (commandMetadata.AskForCommand)
                {
                    command = User.GetCommand();
                }
                else
                {
                    if (commandMetadata.OverrideCommand)
                    {
                        command = commandMetadata.OverrideCommandString;
                    }
                    else
                    {
                        command = "/u";
                    }
                    commandMetadata.AskForCommand = true;
                    Thread.Sleep(200);
                }
            } while ((!command.ToLower().StartsWith("/q")));
            Console.Clear();
        }
    }
}
