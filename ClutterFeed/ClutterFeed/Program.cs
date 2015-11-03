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

namespace ClutterFeed
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode; /* Enables unicode */
            SetScreenColor.SetColor(ConsoleColor.Cyan, 0, 126, 199); /* Changes cyan to dark blue-ish */
            SetScreenColor.SetColor(ConsoleColor.DarkMagenta, 66, 140, 187); /* Makes the DarkMagenta color blue-ish */
            SetScreenColor.SetColor(ConsoleColor.DarkBlue, 249, 129, 245); /* Makes the DarkBlue color pink */

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

                    else if ((command.ToLower().CompareTo("/update") == 0) || (command.ToLower().CompareTo("/u") == 0))
                    {
                        commandMetadata = twitterDo.Update(command);
                    }

                    else if (command.Split(' ')[0].CompareTo("/r") == 0)
                    {
                        commandMetadata = twitterDo.ReplyGeneric(command);
                    }

                    else if (command.Split(' ')[0].CompareTo("/id") == 0)
                    {
                        commandMetadata = twitterDo.GetID(command);
                    }

                    else if (command.Split(' ')[0].CompareTo("/friend") == 0)
                    {
                        commandMetadata = twitterDo.AddFriend(command);
                    }

                    else if (command.Split(' ')[0].CompareTo("/link") == 0)
                    {
                        commandMetadata = twitterDo.TweetLink(command);
                    }

                    else if (command.Split(' ')[0].CompareTo("/rn") == 0)
                    {
                        commandMetadata = twitterDo.ReplyQuiet(command);
                    }

                    else if (command.Split(' ')[0].CompareTo("/rc") == 0)
                    {
                        commandMetadata = twitterDo.ReplyClean(command);
                    }

                    else if (command.Split(' ')[0].CompareTo("/rt") == 0)
                    {
                        commandMetadata = twitterDo.Retweet(command);
                    }

                    else if ((command.Split(' ')[0].CompareTo("/fav") == 0) || (command.Split(' ')[0].CompareTo("/f") == 0))
                    {
                        commandMetadata = twitterDo.FavoriteTweet(command);
                    }

                    else if ((command.Split(' ')[0].ToLower().CompareTo("/del") == 0) || (command.Split(' ')[0].ToLower().CompareTo("/d") == 0))
                    {
                        commandMetadata = twitterDo.RemoveTweet(command);
                    }

                    else if (command.Split(' ')[0].ToLower().CompareTo("/profile") == 0)
                    {
                        try
                        {
                            commandMetadata = twitterDo.ShowProfile(command);
                        }
                        catch (NullReferenceException exceptionInfo)
                        {
                            ScreenDraw.ShowError(exceptionInfo.Message + "\n");
                            commandMetadata = new ActionValue();
                        }
                    }

                    else if (command.Split(' ')[0].ToLower().CompareTo("/tweet") == 0)
                    {
                        commandMetadata = twitterDo.ShowTweet(command);
                    }

                    else if (command.Split(' ')[0].ToLower().CompareTo("/me") == 0)
                    {
                        commandMetadata = twitterDo.Mentions(command);
                    }

                    else if (command.Split(' ')[0].ToLower().CompareTo("/help") == 0 || command.Split(' ')[0].ToLower().CompareTo("/h") == 0)
                    {
                        commandMetadata = twitterDo.Help();
                    }

                    else if (command.ToLower().Contains("/api"))
                    {
                        commandMetadata = twitterDo.ApiInfo();
                    }

                    else
                    {
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        for (int index = 0; index < Console.WindowWidth; index++)
                        {
                            Console.Write(' ');
                        }
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                    }
                }
                /* End of commands */

                if (command.ToLower().StartsWith("/") == false) /* EXCEPT for this one */
                {
                    commandMetadata = twitterDo.NewTweet(command);
                }
                if (commandMetadata.AskForCommand)
                {
                    command = User.CounterConsole();
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
