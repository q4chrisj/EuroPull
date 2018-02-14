using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using System;

namespace GithubFacade
{
    public class GitRepository
    {
        private readonly string _repoUrl;
        private readonly string _localPath;
        private readonly string _token;

        public GitRepository(string repoUrl, string localPath, string token)
        {
            _repoUrl = repoUrl;
            _localPath = localPath;
            _token = token;

        }

        public void Clone()
        {
            var cloneOptions = new CloneOptions
            {
                CredentialsProvider = (url, username, password) => new UsernamePasswordCredentials { Username = _token, Password = string.Empty }
            };

            string result = Repository.Clone(_repoUrl, _localPath, cloneOptions);
        }

        public void Checkout(string branchName)
        {
            using (Repository repo = new Repository(_localPath))
            {
                var branch = repo.Branches[branchName];
                if (branch == null)
                    return;

                Branch result = Commands.Checkout(repo, branch);
            }
        }

        public string Pull()
        {
            using (Repository repo = new Repository(_localPath))
            {
                PullOptions options = new PullOptions
                {
                    FetchOptions = new FetchOptions()
                };
                options.FetchOptions.CredentialsProvider = new CredentialsHandler(
                    (url, usernameFromUrl, types) =>
                    new UsernamePasswordCredentials()
                    {
                        Username = _token,
                        Password = string.Empty
                    }
                );

                return Commands.Pull(repo, new Signature("guest", "guest", new DateTimeOffset(DateTime.Now)), options).Status.ToString();
            }
        }

        public string CurrentBranch()
        {
            using (Repository repo = new Repository(_localPath))
            {
                return repo.Head.FriendlyName;
            }
        }

        public bool Status()
        {
            using (Repository repo = new Repository(_localPath))
            {
                var state = repo.RetrieveStatus(new StatusOptions());
                //foreach (var item in repo.RetrieveStatus(new StatusOptions()))
                //{
                //    var state = item.State;
                //}
            }

            return true;
        }
    }
}
