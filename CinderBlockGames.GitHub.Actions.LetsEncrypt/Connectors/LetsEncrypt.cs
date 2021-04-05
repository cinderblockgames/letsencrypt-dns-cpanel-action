using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Certes;
using Certes.Acme;
using Certes.Acme.Resource;

namespace CinderBlockGames.GitHub.Actions.LetsEncrypt.Connectors
{
    internal class LetsEncrypt
    {

        private readonly CertificateInfo _certInfo;
        private readonly SecretsInfo _names;
        private readonly Cpanel _cpanel;
        private readonly Github _secrets;

        public LetsEncrypt(
            CertificateInfo certInfo,
            SecretsInfo names,
            Cpanel cpanel,
            Github secrets)
        {
            _certInfo = certInfo;
            _names = names;
            _cpanel = cpanel;
            _secrets = secrets;
        }

        public async Task OrderCertificate()
        {
            // Get account context.
            var acme = await GetContext();

            // Submit the order.
            Console.WriteLine("Submitting certificate order...");
            var order = await acme.NewOrder(_certInfo.Identifiers);

            // Generate and apply the DNS record.
            Console.WriteLine("Getting domain ownership challenges...");
            var auth = await order.Authorizations();

            // Validate domain ownership.
            // Each challenge must be processed individually, so, if there are many of them, this will take some time.
            // (Probably, you could make it work in parallel, but it seems to work best if you don't do that.)
            Console.WriteLine("Validating domain ownership... (this may take a few minutes)");
            var count = auth.Count();
            for (var ii = 0; ii < count; ii++)
            {
                Console.WriteLine();
                Console.WriteLine($"Processing challenge ({ii + 1}/{count})...");
                if (!(await Validate(acme, auth.ElementAt(ii))))
                {
                    Console.WriteLine();
                    Console.WriteLine("At least one error was encountered.  Exiting.");
                    return;
                }
            }
            Console.WriteLine();
            Console.WriteLine("Validated domain ownership...");

            // Generate a CSR and private key for the certificate and get the certificate signed.
            var key = KeyFactory.NewKey(_certInfo.KeyAlgorithm);
            Console.WriteLine("Generating certificate...");
            var cert = await order.Generate(new CsrInfo
            {
                CommonName = _certInfo.CommonName,
                Organization = _certInfo.Organization,
                OrganizationUnit = _certInfo.OrganizationUnit,
                Locality = _certInfo.Locality,
                State = _certInfo.State,
                CountryName = _certInfo.Country
            }, key);
            
            // Save off results.
            Console.WriteLine("Saving results...");
            var chain = _secrets.SetSecret(
                _names.PublicChainName,
                cert.ToPem());
            var pfx = _secrets.SetSecret(
                _names.PrivateKeyName,
                Convert.ToBase64String(cert.ToPfx(key).Build(_certInfo.CommonName, _certInfo.Password)));
            await Task.WhenAll(chain, pfx);

            Console.WriteLine("Complete!");
        }

        private async Task<bool> Validate(AcmeContext acme, IAuthorizationContext auth)
        {
            var challengeTask = auth.Dns();
            var resourceTask = auth.Resource();
            await Task.WhenAll(challengeTask, resourceTask);
            var challenge = await challengeTask;
            var domain = (await resourceTask).Identifier.Value;

            var text = acme.AccountKey.DnsTxt(challenge.Token);
            Console.WriteLine($"Adding DNS record for {domain}...");
            await _cpanel.AddRecord(domain, text);

            Challenge result = null;
            try
            {
                do
                {
                    Console.WriteLine($"Validating {domain}...");
                    await Task.Delay(5000); // Wait between each attempt.
                    result = await challenge.Validate();
                } while (result.Status == ChallengeStatus.Pending || result.Status == ChallengeStatus.Processing) ;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to validate domain ownership: {ex.Message}");
            }

            // Remove DNS record.
            Console.WriteLine($"Removing DNS record for {domain}...");
            await _cpanel.RemoveRecord(domain);

            if (result?.Status == ChallengeStatus.Invalid)
            {
                Console.WriteLine($"Failed to validate domain ownership: {result.Error}");
            }
            return result?.Status == ChallengeStatus.Valid;
        }

        private async Task<AcmeContext> GetContext()
        {
            AcmeContext acme = null;
            var server = WellKnownServers.LetsEncryptV2;
#if DEBUG
            server = WellKnownServers.LetsEncryptStagingV2;
#endif
            if (string.IsNullOrWhiteSpace(_certInfo.AccountKey))
            {
                Console.WriteLine("Creating ACME account...");
                acme = new AcmeContext(server);
                var account = await acme.NewAccount(_certInfo.EmailAddress, true);

                Console.WriteLine("Saving ACME account key...");
                // Save the new account key.
                await _secrets.SetSecret(_names.AcmeAccountKeyName, acme.AccountKey.ToPem());
            }
            else
            {
                // Load the provided account key.
                Console.WriteLine("Loading ACME account key...");
                var key = KeyFactory.FromPem(_certInfo.AccountKey);
                acme = new AcmeContext(server, key);
                var account = await acme.Account();
            }
            return acme;
        }

        #region " CertificateInfo "

        internal class CertificateInfo
        {

            private const char IDENTIFIERS_SEPARATOR = '|';

            public string EmailAddress { get; }
            public string AccountKey { get; }
            public string CommonName { get; }
            public IList<string> Identifiers { get; }
            public string Organization { get; }
            public string OrganizationUnit { get; }
            public string Locality { get; }
            public string State { get; }
            public string Country { get; }
            public string Password { get; }
            public KeyAlgorithm KeyAlgorithm { get; }

            public CertificateInfo(
                string emailAddress,
                string accountKey,
                string commonName,
                string identifiers,
                string organization,
                string organizationUnit,
                string locality,
                string state,
                string country,
                string password,
                KeyAlgorithm keyAlgorithm)
            {
                EmailAddress = emailAddress;
                AccountKey = accountKey;
                CommonName = commonName;
                Identifiers = identifiers.Split(
                    IDENTIFIERS_SEPARATOR,
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                Organization = organization;
                OrganizationUnit = organizationUnit;
                Locality = locality;
                State = state;
                Country = country;
                Password = password;
                KeyAlgorithm = keyAlgorithm;
            }

        }

        #endregion

        #region " SecretsInfo "

        internal class SecretsInfo
        {

            public string AcmeAccountKeyName { get; }
            public string PublicChainName { get; }
            public string PrivateKeyName { get; }

            public SecretsInfo(
                string acmeAccountKeyName,
                string publicChainName,
                string privateKeyName)
            {
                AcmeAccountKeyName = acmeAccountKeyName;
                PublicChainName = publicChainName;
                PrivateKeyName = privateKeyName;
            }

        }

        #endregion

    }
}