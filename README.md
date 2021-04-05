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
| certPassword            | No            |                       | The password to apply to the issued PFX.                                                                                                |
| certKeyAlgorithm        | No            | **ES256**             | Algorithm to use for private key.  See options at https://github.com/fszlin/certes/blob/master/src/Certes/KeyAlgorithm.cs.              |
| **githubAccessToken**   | **Yes**       |                       | Personal Access Token with repo access for GitHub secrets access.                                                                       |
| **secretsRepo**         | **Yes**       |                       | Repo in which to store outputs from this Action.                                                                                        |
| acmeAccountKeyName      | No            | **ACME_ACCOUNT_KEY**  | Name to use when saving the ACME account key as a secret in SecretsRepo.                                                                |
| publicChainName         | No            | **CERT_PUBLIC_CHAIN** | Name to use when saving the certificate's public chain as a secret in SecretsRepo.                                                      |
| privateKeyName          | No            | **CERT_PRIVATE_KEY**  | Name to use when saving the certificate's private key as a secret in SecretsRepo.                                                       |

## Example Workflow
```
# Workflow name
name: Update Certificate

# Controls when the action will run.
on:
  schedule:
    # Runs at 16:00 UTC on the 15th in Jan, Mar, May, Jul, Sep, and Nov
    - cron: '0 16 15 1,3,5,7,9,11 *'

  # Allows you to run this workflow manually from the Actions tab
  workflow_dispatch:

jobs:
  update-cert:
    runs-on: ubuntu-latest
    steps:
      - name: Issue certificate
        uses: cinderblockgames/letsencrypt-dns-cpanel-action@v1.0.1
        with:
          # REQUIRED
          # cPanel
          host: example.com
          cpanelUsername: '${{ secrets.CPANEL_USERNAME }}'
          cpanelApiToken: '${{ secrets.CPANEL_API_KEY }}'
          domain: homelab.express
          # Let's Encrypt
          acmeAccountEmailAddress: youremail@example.com
          #acmeAccountKey: '${{ secrets.ACME_ACCOUNT_KEY }}'
          certCN: '*.homelab.express'
          certOrg: homelab.express
          certOU: private network
          certLocality: private
          certState: network
          certCountry: earth
          certDomainList: '*.homelab.express|*.red.homelab.express|*.orange.homelab.express|*.yellow.homelab.express'
          # GitHub
          githubAccessToken: '${{ secrets.GIT_HUB_ACCESS_TOKEN }}'
          secretsRepo: yourgithubuser/yourgithubrepo

          # OPTIONAL
          # cPanel
          port: 2084
          # Let's Encrypt
          certPassword: 'sUP3r--s3cuR3'
          certKeyAlgorithm: ES512
          # secrets
          acmeAccountKeyName: ACME_ACCOUNT_KEY_2
          publicChainName: CERT_PUBLIC_CHAIN_2
          privateKeyName: CERT_PRIVATE_KEY_2
```

## How to retrieve the PFX from secrets
The PFX is stored in a base64-encoded string, so you need to decode it on the way out.  For an example of how to do that, check out this workflow:  https://github.com/cinderblockgames/homelab.express/blob/main/.github/workflows/upload-cert.yml