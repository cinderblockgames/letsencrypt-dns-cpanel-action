using CommandLine;

namespace CinderBlockGames.GitHub.Actions.LetsEncrypt
{
    public class Options
    {

        // Certificate



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

        [Option("apiToken",
                Required = true,
                HelpText = "API Token for the cPanel server.")]
        public string ApiToken { get; set; }

        [Option("domain",
                Required = true,
                HelpText = "Domain under which to place the DNS verification on the cPanel server.")]
        public string Domain { get; set; }

        // Let's Encrypt



        // GitHub



    }
}