using System;
using System.Threading.Tasks;
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
            var cpanel = new Connectors.Cpanel(
                new Connectors.Cpanel.ConnectionInfo(
                    options.Host,
                    options.Port,
                    options.Username,
                    options.CpanelApiToken,
                    options.Domain));
            var github = new Connectors.Github(
                new Connectors.Github.ConnectionInfo(
                    options.GitHubAccessToken,
                    options.SecretsRepo));
            var letsEncrypt = new Connectors.LetsEncrypt(
                new Connectors.LetsEncrypt.CertificateInfo(
                    options.AcmeAccountEmailAddress,
                    options.AcmeAccountKey,
                    options.CertificateCommonName,
                    options.CertificateIdentifiers,
                    options.CertificateOrganization,
                    options.CertificateOrganizationUnit,
                    options.CertificateLocality,
                    options.CertificateState,
                    options.CertificateCountry,
                    options.CertificatePassword,
                    options.CertificateKeyAlgorithm),
                new Connectors.LetsEncrypt.SecretsInfo(
                    options.AcmeAccountKeyName,
                    options.PublicChainName,
                    options.PrivateKeyName),
                cpanel,
                github);

            // Process the certificate order.
            await letsEncrypt.OrderCertificate();
        }

    }
}
