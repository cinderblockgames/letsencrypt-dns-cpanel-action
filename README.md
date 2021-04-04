# Issue Certificates via Let's Encrypt DNS + cPanel
This action handles issuing a certificate through Let's Encrypt by managing the appropriate DNS entries via the cPanel API.  The resultant private key will be saved to the repository's secrets.

## Inputs
| Parameter               | Required      | Default               | Description                                                                                                                             |
| :---                    | :---          | :---                  | :---                                                                                                                                    |
| **host**                | **Yes**       |                       | Host portion of the cPanel server.                                                                                                      |
| port                    | No            | **2083**              | Port for the cPanel server.                                                                                                             |
| **cpanelUsername**      | **Yes**       |                       | Username for the cPanel server.                                                                                                         |
| **cpanelApiToken**      | **Yes**       |                       | API Token for the cPanel server.                                                                                                        |
| **domain**              | **Yes**       |                       | Domain under which to place the DNS verification on the cPanel server.                                                                  |
| acmeAccountEmailAddress | **Sometimes** |                       | The email address to associate with the account when communicating with Let's Encrypt.  REQUIRED if AcmeAccountKey is not provided.     |
| acmeAccountKey          | **Sometimes** |                       | The key associated with the account to use when communicating with Let's Encrypt.  REQUIRED if AcmeAccountEmailAddress is not provided. |
| certCN                  | **Yes**       |                       | The common name to be used for the issued certificate.                                                                                  |
| **certDomainList**      | **Yes**       |                       | The domains to be included in the issued certificate, separated by a pipe (\|) character.                                               |
| **certOrg**             | **Yes**       |                       | The organization to be included for the issued certificate.                                                                             |
| **certOU**              | **Yes**       |                       | The unit within the organization to be included for the issued certificate.                                                             |
| **certLocality**        | **Yes**       |                       | The locality in which the ogranization is located, to be included for the issued certificate.                                           |
| **certState**           | **Yes**       |                       | The state in which the ogranization is located, to be included for the issued certificate.                                              |
| **certCountry**         | **Yes**       |                       | The country in which the ogranization is located, to be included for the issued certificate.                                            |
| certPassword            | No            |                       | The password to apply to the issued PFX.  Leave blank for no password.                                                                  |
| certKeyAlgorithm        | No            | **ES256**             | Algorithm to use for private key.  See options at https://github.com/fszlin/certes/blob/master/src/Certes/KeyAlgorithm.cs.              |
| **githubAccessToken**   | **Yes**       |                       | Personal Access Token with repo access for GitHub secrets access.                                                                       |
| **secretsRepo**         | **Yes**       |                       | Repo in which to store outputs from this Action.                                                                                        |
| acmeAccountKeyName      | No            | **ACME_ACCOUNT_KEY**  | Name to use when saving the ACME account key as a secret in SecretsRepo.                                                                |
| publicChainName         | No            | **CERT_PUBLIC_CHAIN** | Name to use when saving the certificate's public chain as a secret in SecretsRepo.                                                      |
| privateKeyName          | No            | **CERT_PRIVATE_KEY**  | Name to use when saving the certificate's private key as a secret in SecretsRepo.                                                       |

## Example Workflow    DO THIS PART!!
```
# Workflow name
name: Deploy site to live

on:
  # Run automatically on push to main branch
  push:
    branches: [ main ]
    paths:
    - 'src/**'
  # Allow manual trigger
  workflow_dispatch:

jobs:
  web-deploy:
    name: Deploy
    runs-on: ubuntu-latest
    steps:
    - name: Issue Cert
      uses: cinderblockgames/letsencrypt-dns-cpanel-action@v1.0.0
      with:
        # required
        server: ftp.example.com
        username: example@example.com
        password: ${{ secrets.FTP_PASSWORD }}
        # optional
        port: 22
        source: src/path
        destination: target/path
        skipUnchanged: true
        skipDirectories: .github|.well-known|configs|private-keys
        test: true
```
