using Certes;
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

        [Option("cpanelUsername",
                Required = true,
                HelpText = "Username for the cPanel server.")]
        public string CpanelUsername { get; set; }

        [Option("cpanelApiToken",
                Required = true,
                HelpText = "API Token for the cPanel server.")]
        public string CpanelApiToken { get; set; }

        [Option("domain",
                Required = true,
                HelpText = "Domain under which to place the DNS verification on the cPanel server.")]
        public string Domain { get; set; }

        // Let's Encrypt

        [Option("acmeAccountEmailAddress",
            Required = false,
            HelpText = "The email address to associate with the account when communicating with Let's Encrypt.  REQUIRED if AcmeAccountKey is not provided.")]
        public string AcmeAccountEmailAddress { get; set; }

        [Option("acmeAccountKeyPath",
            Required = false,
            HelpText = "The file holding the key associated with the account to use when communicating with Let's Encrypt.  REQUIRED if AcmeAccountEmailAddress is not provided.")]
        public string AcmeAccountKeyPath { get; set; }

        [Option("certCN",
            Required = true,
            HelpText = "The common name to be used for the issued certificate.")]
        public string CertificateCommonName { get; set; }

        [Option("certDomainList",
            Required = true,
            HelpText = "The domains to be included in the issued certificate, separated by a pipe (|) character.")]
        public string CertificateIdentifiers { get; set; }

        [Option("certOrg",
            Required = true,
            HelpText = "The organization to be included for the issued certificate.")]
        public string CertificateOrganization { get; set; }

        [Option("certOU",
            Required = true,
            HelpText = "The unit within the organization to be included for the issued certificate.")]
        public string CertificateOrganizationUnit { get; set; }

        [Option("certLocality",
            Required = true,
            HelpText = "The locality in which the ogranization is located, to be included for the issued certificate.")]
        public string CertificateLocality { get; set; }

        [Option("certState",
            Required = true,
            HelpText = "The state in which the ogranization is located, to be included for the issued certificate.")]
        public string CertificateState { get; set; }

        [Option("certCountry",
            Required = true,
            HelpText = "The country in which the ogranization is located, to be included for the issued certificate.")]
        public string CertificateCountry { get; set; }

        [Option("certPassword",
            Required = false, Default = "",
            HelpText = "The password to apply to the issued PFX.")]
        public string CertificatePassword { get; set; }

        [Option("certKeyAlgorithm",
            Required = false, Default = KeyAlgorithm.ES256,
            HelpText = "Algorithm to use for private key.  See options at https://github.com/fszlin/certes/blob/master/src/Certes/KeyAlgorithm.cs.")]
        public KeyAlgorithm CertificateKeyAlgorithm { get; set; }

        // GitHub

        [Option("githubAccessToken",
            Required = true,
            HelpText = "Personal Access Token with repo access for GitHub secrets access.")]
        public string GitHubAccessToken { get; set; }
        
        [Option("secretsRepo",
            Required = true,
            HelpText = "Repo in which to store outputs from this Action.")]
        public string SecretsRepo { get; set; }

        // Secrets

        [Option("acmeAccountKeyName",
            Required = false, Default = "ACME_ACCOUNT_KEY",
            HelpText = "Name to use when saving the ACME account key as a secret in SecretsRepo.")]
        public string AcmeAccountKeyName { get; set; }

        [Option("publicChainName",
            Required = false, Default = "CERT_PUBLIC_CHAIN",
            HelpText = "Name to use when saving the certificate's public chain as a secret in SecretsRepo.")]
        public string PublicChainName { get; set; }

        [Option("privateKeyName",
            Required = false, Default = "CERT_PRIVATE_KEY",
            HelpText = "Name to use when saving the certificate's private key as a secret in SecretsRepo.")]
        public string PrivateKeyName { get; set; }

    }
}