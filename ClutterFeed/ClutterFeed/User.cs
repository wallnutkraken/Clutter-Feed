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
using System.IO;
using TweetSharp;
using System.Diagnostics;

namespace ClutterFeed
{
    class User
    {
        OAuthAccessToken userKey = new OAuthAccessToken();
        OAuthAccessToken appKey = new OAuthAccessToken();
        /// <summary>
        /// Checks if all the files are in order
        /// </summary>
        private void ReadKeys()
        {
            string userDir = Environment.CurrentDirectory;
            if (!File.Exists(userDir + "/keys.conf"))
            {
                throw new FileNotFoundException("keys.conf does not exist.");
            }
            List<string> readAPIKeys = File.ReadAllLines(userDir + "/keys.conf").ToList();

            int tempIndex = 0;
            while (tempIndex < readAPIKeys.Count) /* Removes empty lines */
            {
                if (readAPIKeys[tempIndex] == "")
                {
                    readAPIKeys.RemoveAt(tempIndex);
                }
                else
                {
                    tempIndex++;
                }
            }

            if (readAPIKeys.Count != 4)
            {
                Console.WriteLine("Bad keys.conf file.");
                Environment.Exit(0);
            }

            bool appTokenExists = false;
            bool appSecretExists = false;
            bool userTokenExists = false;
            bool userSecretExists = false;

            for (int index = 0; index < 4; index++)
            {
                string[] splitter = new string[1];
                splitter = readAPIKeys[index].Split('=');
                if (splitter[0].ToLower().CompareTo("apptoken") == 0)
                {
                    appTokenExists = true;
                    appKey.Token = splitter[1];
                }
                if (splitter[0].ToLower().CompareTo("appsecret") == 0)
                {
                    appSecretExists = true;
                    appKey.TokenSecret = splitter[1];
                }
                if (splitter[0].ToLower().CompareTo("usertoken") == 0)
                {
                    userTokenExists = true;
                    userKey.Token = splitter[1];
                }
                if (splitter[0].ToLower().CompareTo("usersecret") == 0)
                {
                    userSecretExists = true;
                    userKey.TokenSecret = splitter[1];
                }
            }

            if ((userKey.Token == "") || (userKey.TokenSecret == ""))
            {
                CreateUser();
            }

            if ((appTokenExists == false) || (appSecretExists == false) || (userTokenExists == false) || (userSecretExists == false))
            {
                Console.WriteLine("Error: keys.conf is set up improperly.");
                Environment.Exit(0);
            }

        }
        /// <summary>
        /// Creates a user using OAuth
        /// </summary>
        private void CreateUser()
        {
            TwitterService service = new TwitterService(appKey.Token, appKey.TokenSecret);
            OAuthRequestToken requestToken = service.GetRequestToken();
            Uri uri = service.GetAuthorizationUri(requestToken);
            Process.Start(uri.ToString());
            service.AuthenticateWith(appKey.Token, appKey.TokenSecret);

            Console.Write("Please input the PIN: ");
            string verifier = Console.ReadLine();
            userKey = service.GetAccessToken(requestToken, verifier);

            WriteFile();
        }
        private void WriteFile()
        {
            List<string> newFile = new List<string>();
            newFile.Add("appToken=" + appKey.Token);
            newFile.Add("appSecret=" + appKey.TokenSecret);
            newFile.Add("userToken=" + userKey.Token);
            newFile.Add("userSecret=" + userKey.TokenSecret);
            File.WriteAllLines(Environment.CurrentDirectory + "/keys.conf", newFile);
        }
        public void Run()
        {
            ReadKeys();
        }

        public OAuthAccessToken GetUser()
        {
            return userKey;
        }

        public OAuthAccessToken GetApp()
        {
            return appKey;
        }
        public static string GetCommand()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("      > ");
            Console.ForegroundColor = ConsoleColor.White;
            return Console.ReadLine();
        }

        public static bool IsMissingArgs(string command)
        {
            try
            {
                bool checker = command.Split(' ')[1].Length != 2;
            }
            catch (IndexOutOfRangeException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("      Error: input was not complete.");
                Console.WriteLine("      You probaby didn't use enough args");
                Console.ForegroundColor = ConsoleColor.White;
                return true;
            }
            return false;
        }

        private static int bufferPosition = 0;
        public static string CounterConsole()
        {
            int splitCount = 3;
            string command = "";
            string message = "";
            char writeChar = '\0';
            int charCount = 0;
            char[] splitter = new char[1];
            splitter[0] = ' '; /* FUCK C# honestly */

            int cursorPosX = Console.CursorTop;
            do
            {
                Console.SetCursorPosition(0, cursorPosX);

                if (charCount > 140)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                if (command.StartsWith("/"))
                {
                    try
                    {
                        message = command.Split(splitter, splitCount)[2];
                        Console.Write("[{0:000}] ", message.Length);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.Write("[{0:000}] ", charCount);
                    }
                }
                else
                {
                    Console.Write("[{0:000}] ", charCount);
                }


                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("> ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(command);
                ConsoleKeyInfo characterInfo = Console.ReadKey(true);
                writeChar = characterInfo.KeyChar;

                if (writeChar == '\b')
                {
                    if (charCount != 0)
                    {
                        if (Console.CursorLeft == 0)
                        {
                            Console.SetCursorPosition(Console.WindowWidth - 1, Console.CursorTop - 1);
                            Console.Write(' ');
                            Console.SetCursorPosition(Console.WindowWidth - 1, Console.CursorTop);
                        }
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        command = command.Remove(command.Length - 1, 1);
                        charCount--;
                    }
                    else
                    {
                        Console.Beep();
                    }
                }
                else
                {
                    if (characterInfo.Key == ConsoleKey.DownArrow)
                    {
                        if (bufferPosition == 0) /* Nothing happens if you're already at the latest command possible */
                        {
                            Console.Beep();
                        }
                        else
                        {
                            try
                            {
                                bufferPosition--;
                                command = CommandHistory.GetCommand(bufferPosition);
                                charCount = command.Length;
                                ClearLine();
                                Console.SetCursorPosition(8 + charCount, Console.CursorTop);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                Console.Beep();
                            }
                        }
                    }
                    else if (characterInfo.Key == ConsoleKey.UpArrow) /* Handles going to earlier points in the history */
                    {
                        if (bufferPosition == 0)
                        {
                            if (CommandHistory.MaxIndex() != bufferPosition || CommandHistory.MaxIndex() == 0)
                            {
                                try
                                {
                                    CommandHistory.Add(command);
                                    bufferPosition++;
                                    command = CommandHistory.GetCommand(bufferPosition);
                                    charCount = command.Length;
                                    ClearLine();
                                    Console.SetCursorPosition(8 + charCount, Console.CursorTop);
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    Console.Beep();
                                }
                            }
                            else
                            {
                                Console.Beep();
                            }
                        }
                        else if (bufferPosition == CommandHistory.MaxIndex())
                        {
                            Console.Beep();
                        }
                        else
                        {
                            try
                            {
                                bufferPosition++;
                                command = CommandHistory.GetCommand(bufferPosition);
                                charCount = command.Length;
                                ClearLine();
                                Console.SetCursorPosition(8 + charCount, Console.CursorTop);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                Console.Beep();
                            }
                        }
                    }
                    else if (characterInfo.Key == ConsoleKey.RightArrow
                        || characterInfo.Key == ConsoleKey.LeftArrow)
                    {
                        /* Ignores left and right arrow key currently */
                        /* but one day I hope you could move in the command */
                    }
                    else
                    {
                        command = command + writeChar;
                        try
                        {
                            message = command.Split(splitter, splitCount)[2];
                        }
                        catch (IndexOutOfRangeException) { }
                        charCount++;
                    }
                }

            } while (writeChar != '\r' && message.Length <= 140);

            command = command.Replace("\r", "");
            CommandHistory.Add(command);
            bufferPosition = 0; /* resets the buffer position every time you finish typing a command */


            Console.WriteLine();
            CommandHistory.RemoveEmpties();

            return command;
        }


        private static void ClearLine()
        {
            System.Threading.Thread.Sleep(25);
            for (int index = 1; index <= Console.WindowWidth; index++)
            {
                Console.SetCursorPosition(index - 1, Console.CursorTop);
                Console.Write(" ");
            }
        }
    }
}
