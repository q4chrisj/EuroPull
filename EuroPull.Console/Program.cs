using System;
using System.IO;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace EuroPull
{
    class Program
    {
        private static string _repoUrl = "https://github.com/q4-euro/Solutions.git";
        private static string _localPath = "/users/jonezy/Projects/Solutions/";
        private static string _username = "";
        private static string _password = "";

        static void Main(string[] args)
        {
            Console.WriteLine("Please enter your github.com usename:");
            _username = Console.ReadLine();

            Console.WriteLine("Please enter your github.com password:");
            _password = Console.ReadLine();

            var isCloned = Directory.Exists(Path.GetFullPath(_localPath));
            if(!isCloned) 
            {
                Console.WriteLine("Checking out {0} to {1}. This may take a few minutes.", _repoUrl, _localPath);

                Clone();

                Console.WriteLine("Checkout complete");    
            } else {
                Console.WriteLine("Switch to branch: ");
                var branchName = Console.ReadLine();

                if (!string.IsNullOrEmpty(branchName))
                {
                    Checkout(branchName);
                }

                Pull();
                Console.WriteLine("Update complete");
            }
        }

        private static void Clone()
        {
            var cloneOptions = new CloneOptions
            {
                CredentialsProvider = (url, username, password) => new UsernamePasswordCredentials { Username = _username, Password = _password }
            };

            Repository.Clone(_repoUrl, _localPath, cloneOptions);
        }

        private static void Pull()
        {
            using(Repository repo = new Repository(_localPath))
            {
                PullOptions options = new PullOptions
                {
                    FetchOptions = new FetchOptions()
                };
                options.FetchOptions.CredentialsProvider = new CredentialsHandler(
                    (url, usernameFromUrl, types) =>
                    new UsernamePasswordCredentials()
                    {
                        Username = _username,
                        Password = _password
                    }
                );

                Commands.Pull(repo, new Signature("guest", "guest", new DateTimeOffset(DateTime.Now)), options);
            }
        }

        private static void Checkout(string branchName)
        {
            using (Repository repo = new Repository(_localPath))
            {
                var branch = repo.Branches[branchName];
                if (branch == null)
                    return;
                
                Commands.Checkout(repo, branch);
            }
        }
    }
}