using System;
using System.Threading.Tasks;
using CinderBlockGames.GitHub.Actions.LetsEncrypt.Connectors;
using CommandLine;

namespace CinderBlockGames.GitHub.Actions.LetsEncrypt
{
    class Program
    {

        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(Run);
#if DEBUG
            Console.ReadLine(); // Pause for review.
#endif
        }

        private static async Task Run(Options options)
        {
            // Set up connectors.
            var cpanel = new Cpanel(
                new Cpanel.ConnectionInfo(
                    options.Host,
                    options.Port,
                    options.Username,
                    options.ApiToken,
                    options.Domain));

            // Start the Let's Encrypt DNS verification.
            var value = "info";

            // Add the DNS record to verify the domain.
            await cpanel.AddRecord(value);

            // Complete the Let's Encrypt DNS verification.


            // Remove the DNS record, now that it's no longer needed.
            var remove = cpanel.RemoveRecord();

            // Save the certificate to the repository's secrets.


            // Complete processing.
            await Task.WhenAll(remove);
        }

    }
}
