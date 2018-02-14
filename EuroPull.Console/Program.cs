using GithubFacade;
using System;
using System.Configuration;
using System.IO;

namespace EuroPull.Console
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string gitBranch = ConfigurationManager.AppSettings["GitBranch"].ToString();
            string localPath = ConfigurationManager.AppSettings["LocalRepoPath"].ToString();
            GitRepository repo = new GitRepository(ConfigurationManager.AppSettings["GithubRepoUrl"].ToString(), localPath, ConfigurationManager.AppSettings["GithubToken"].ToString());

            if (!Directory.Exists(Path.GetFullPath(localPath)))
            {
                Directory.CreateDirectory(Path.GetFullPath(localPath));
            }

            try
            {
                repo.Clone();
            }
            catch (Exception ex)
            {
                //System.Console.WriteLine(ex);
            }

            if(!string.Equals(repo.CurrentBranch(), gitBranch))
            {
                repo.Checkout(gitBranch);
            }

            var result = repo.Pull();

            if(!result.Equals("UpToDate"))
            {
                System.Console.WriteLine("Invalidate cloudfront distribution.");
            }
        }
    }
}
