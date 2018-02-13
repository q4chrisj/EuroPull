using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace EuroPull
{
    class Program
    {
        private static string _repoUrl = "https://github.com/q4-euro/Solutions.git";
        private static string _localPath = "/users/jonezy/Projects/Solutions/";

        static void Main(string[] args)
        {
            Console.WriteLine("Please enter your github.com usename:");
            var username = Console.ReadLine();

            Console.WriteLine("Please enter your github.com password:");
            var password = Console.ReadLine();

            var cloneOptions = new CloneOptions
            {
                CredentialsProvider = (_url, _username, _password) => new UsernamePasswordCredentials { Username = username, Password = password }
            };

            var isCloned = Directory.Exists(Path.GetFullPath(_localPath));
            if(!isCloned) 
            {
                Console.WriteLine("Checking out {0} to {1}. This may take a few minutes.", _repoUrl, _localPath);

                Repository.Clone(_repoUrl, _localPath, cloneOptions);

                Console.WriteLine("Checkout complete");    
            } else {
                string logMessage = "";

                Console.WriteLine("Switch to branch: ");

                var newBranch = Console.ReadLine();

                ChangeBranch(newBranch ?? "master");

                using (var repo = new Repository(_localPath))
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

        static void ChangeBranch(string newBranch)
        {
            // the master branch friendly name is origin/master
            // all other branches exclude origin/
            if (newBranch == "master")
                newBranch = "origin/master";
            
            using (var repo = new Repository(_localPath))
            {
                var branch = repo.Branches[newBranch];
                if (branch == null)
                    return;
                
                Commands.Checkout(repo, branch);
            }
        }
    }
}