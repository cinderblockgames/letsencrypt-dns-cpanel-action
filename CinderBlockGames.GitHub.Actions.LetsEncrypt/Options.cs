using CommandLine;

namespace CinderBlockGames.GitHub.Actions.LetsEncrypt
{
    public class Options
    {

        // cPanel

        [Option("host",
                Required = true,
                HelpText = "Host portion of the cPanel server.")]
        public string Host { get; set; }

        [Option("port",
                Required = false, Default = 2083,
                HelpText = "Port for the cPanel server.")]
        public int Port { get; set; }

        [Option("username",
                Required = true,
                HelpText = "Username for the cPanel server.")]
        public string Username { get; set; }

        [Option("cpanelApiToken",
                Required = true,
                HelpText = "API Token for the cPanel server.")]
        public string CpanelApiToken { get; set; }

        [Option("domain",
                Required = true,
                HelpText = "Domain under which to place the DNS verification on the cPanel server.")]
        public string Domain { get; set; }

        // Let's Encrypt



        // GitHub

        [Option("githubAccessToken",
            Required = true,
            HelpText = "Personal Access Token with repo access for GitHub secrets access.")]
        public string GitHubAccessToken { get; set; }
        
        [Option("secretsRepo",
            Required = true,
            HelpText = "Repo in which to manage secrets related to this Action.")]
        public string SecretsRepo { get; set; }

        // Secrets



    }
}