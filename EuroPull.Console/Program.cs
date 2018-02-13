﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace EuroPull
{
    class Program
    {
        static void Main(string[] args)
        {
            var repoUrl = "https://github.com/q4-euro/Solutions.git";
            var localPath = "/users/jonezy/Projects/Solutions/";

            Console.WriteLine("Please enter your github.com usename:");
            var username = Console.ReadLine();

            Console.WriteLine("Please enter your github.com password:");
            var password = Console.ReadLine();

            var cloneOptions = new CloneOptions
            {
                CredentialsProvider = (_url, _username, _password) => new UsernamePasswordCredentials { Username = username, Password = password }
            };

            var isCloned = Directory.Exists(Path.GetFullPath(localPath));
            if(!isCloned) 
            {
                Console.WriteLine("Checking out {0} to {1}. This may take a few minutes.", repoUrl, localPath);

                Repository.Clone(repoUrl, localPath, cloneOptions);

                Console.WriteLine("Checkout complete");    
            } else {
                string logMessage = "";
                using (var repo = new Repository(localPath))
                {
                    FetchOptions options = new FetchOptions();
                    options.CredentialsProvider = new CredentialsHandler((url, usernameFromUrl, types) =>
                        new UsernamePasswordCredentials()
                        {
                            Username = username,
                            Password = password
                        });

                    foreach (Remote remote in repo.Network.Remotes)
                    {
                        Console.WriteLine("Fetching updates from {0}", remote.Name);
                        IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                        Commands.Fetch(repo, remote.Name, refSpecs, options, logMessage);
                    }
                }
                Console.WriteLine(logMessage);
            }
        }
    }
}