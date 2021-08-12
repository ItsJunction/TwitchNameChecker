using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;

namespace Twitch_Checker
{
    class Program
    {
        static void Main()
        {
            int count = 0;

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.Title = "Quicc Twitch Checker";
            string name;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            StreamReader reader = new StreamReader("names.txt");
            while((name = reader.ReadLine()) != null)
            {
                if(!(name.Length < 4 || name.Length > 25))
                {
                    var url = "https://gql.twitch.tv/gql";

                    var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpRequest.Method = "POST";

                    httpRequest.Headers["Client-Id"] = "kimne78kx3ncx6brgo4mv6wki5h1ko";
                    httpRequest.ContentType = "application/json";

                    var data = "[{\"operationName\":\"UsernameValidator_User\",\"variables\":{\"username\":\"" + name + "\"},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"fd1085cf8350e309b725cf8ca91cd90cac03909a3edeeedbd0872ac912f3d660\"}}}]";

                    using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                    {
                        streamWriter.Write(data);
                    }

                    var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        if (result.Contains("\"isUsernameAvailable\":false"))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"{name} is taken!");
                        }
                        else if (result.Contains("\"isUsernameAvailable\":null"))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"{name} is taken!");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{name} is untaken!");
                            File.AppendAllText("untaken.txt", $"{name}\n");
                            //Console.WriteLine(result);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"{name} is too short or too long, skipped. ({name.Length} chars)");
                }

                count++;
                Console.Title = $"Quicc Twitch Checker | {count}/{File.ReadLines("names.txt").Count()} checked!";
            }
            
            stopwatch.Stop();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Finished checking {File.ReadLines("names.txt").Count()} names in {stopwatch.ElapsedMilliseconds}ms ({stopwatch.Elapsed.TotalSeconds}s or {stopwatch.Elapsed.TotalMinutes}m).");
            Console.ReadLine();
        }
    }
}
