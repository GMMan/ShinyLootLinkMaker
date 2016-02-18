using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using ShinyLootLinkMaker.Api.V1;
using SysFile = System.IO.File;
using SLFile = ShinyLootLinkMaker.Api.V1.File;

namespace ShinyLootLinkMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ShinyLoot Link Maker");
            Console.WriteLine("(C) cyanic");
            Console.WriteLine();

            ApiClient client = new ApiClient();
            bool addComments = !args.Any(a => a.ToLowerInvariant() == "/n");

            // Log in
            ApiResponse<LoginCode, LoginResponse> loginResp;
            do
            {
                Console.WriteLine("Log in");
                Console.Write("Username: ");
                string username = Console.ReadLine();
                Console.Write("Password: ");
                string pass = ReadPassword();

                loginResp = client.LogIn(username, pass);
                if (loginResp == null)
                {
                    Console.WriteLine("Unknown error while logging in.");
                    Environment.Exit(1);
                }
                else if (loginResp.Code != LoginCode.Success)
                {
                    Console.WriteLine("Error logging in: {0}", loginResp.Message);
                }
            } while (loginResp == null || loginResp.Code != LoginCode.Success);

            // Get games list
            Console.WriteLine("Getting games list...");
            ApiResponse<GamesCode, GamesResponse> gamesResp = client.GetMyGames();
            if (gamesResp == null)
            {
                Console.WriteLine("Failed to get games list.");
                Environment.Exit(2);
            }
            else if (gamesResp.Code != GamesCode.Success)
            {
                Console.WriteLine("Failed to get games list: {0}", gamesResp.Message);
                Environment.Exit(2);
            }
            List<Game> games = gamesResp.Data.Games;

            Console.WriteLine("Found games and files:");
            // Write info to file
            using (StreamWriter swLinks = SysFile.CreateText("links.txt"))
            using (StreamWriter swKeys = SysFile.CreateText("with_keys.txt"))
            {
                foreach (Game game in games)
                {
                    Console.WriteLine("Game: {0}", game.Name);
                    foreach (SLFile file in game.Files)
                    {
                        Console.WriteLine("\tFile: {0} ({1})", file.ShortDescription, file.OS);
                        if (addComments) swLinks.WriteLine("# {0} - {1} ({2})", game.Name, file.ShortDescription, file.OS);
                        // Note: using API URL instead of CloudFront URL because the latter is set to expire in 60 seconds.
                        swLinks.WriteLine(client.BuildGetFileUri(game.ID, file.ID));
                    }

                    // Write key status here
                    string keyStatus;
                    if (makeKeysStatus(game, out keyStatus)) swKeys.WriteLine(keyStatus);
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Done.");
        }

        static bool makeKeysStatus(Game game, out string message)
        {
            List<string> platNames = new List<string>();
            if (game.DevKeyStatus == KeyStatus.Available) platNames.Add("game");
            if (game.SteamKeyStatus == KeyStatus.Available) platNames.Add("Steam");
            if (game.DesuraKeyStatus == KeyStatus.Available) platNames.Add("Desura");

            if (platNames.Count == 0)
            {
                message = string.Empty;
                return false;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} has ", game.Name);
            sb.Append(string.Join(", ", platNames));
            sb.AppendFormat(" key{0}", platNames.Count != 1 ? "s" : string.Empty);
            message = sb.ToString();
            return true;
        }

        // https://stackoverflow.com/a/7049688/1180879

        /// <summary>
        /// Like System.Console.ReadLine(), only with a mask.
        /// </summary>
        /// <param name="mask">a <c>char</c> representing your choice of console mask</param>
        /// <returns>the string the user typed in </returns>
        static string ReadPassword(char mask)
        {
            // Modified for UNIX LF
            const int ENTER = 10, BACKSP = 8, CTRLBACKSP = 127;
            int[] FILTERED = { 0, 27, 9, 10, 13 /*, 32 space, if you care */ }; // const

            var pass = new Stack<char>();
            char chr = (char)0;

            while ((chr = System.Console.ReadKey(true).KeyChar) != ENTER && chr != 13)
            {
                if (chr == BACKSP || chr == 0)
                { // For some reason on my Ubuntu terminal backspace is a null char
                    if (pass.Count > 0)
                    {
                        System.Console.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (chr == CTRLBACKSP)
                {
                    while (pass.Count > 0)
                    {
                        System.Console.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (FILTERED.Count(x => chr == x) > 0)
                {
                }
                else
                {
                    pass.Push((char)chr);
                    System.Console.Write(mask);
                }
            }

            System.Console.WriteLine();

            return new string(pass.Reverse().ToArray());
        }

        /// <summary>
        /// Like System.Console.ReadLine(), only with a mask.
        /// </summary>
        /// <returns>the string the user typed in </returns>
        static string ReadPassword()
        {
            return ReadPassword('*');
        }
    }
}
