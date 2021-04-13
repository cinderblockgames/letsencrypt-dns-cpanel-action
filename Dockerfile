FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-env

COPY . ./
RUN dotnet publish ./CinderBlockGames.GitHub.Actions.LetsEncrypt/CinderBlockGames.GitHub.Actions.LetsEncrypt.csproj -c Release -o out --no-self-contained

LABEL maintainer="cinder block games <hello@cinderblockgames.com>"
LABEL repository="https://github.com/cinderblockgames/letsencrypt-dns-cpanel-action"
LABEL homepage="https://cinderblockgames.com/"

LABEL com.github.actions.name="Issue Certificates via Let's Encrypt DNS + cPanel"
LABEL com.github.actions.description="This action handles issuing a certificate through Let's Encrypt by managing the appropriate DNS entries via the cPanel API.  The resultant private key will be saved to the repository's secrets."
# https://docs.github.com/actions/creating-actions/metadata-syntax-for-github-actions#branding
LABEL com.github.actions.icon="lock"
LABEL com.github.actions.color="green"

FROM mcr.microsoft.com/dotnet/runtime:5.0
COPY --from=build-env /out .
ENTRYPOINT [ "dotnet", "/CinderBlockGames.GitHub.Actions.LetsEncrypt.dll" ]