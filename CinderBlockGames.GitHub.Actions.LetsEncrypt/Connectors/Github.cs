using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sodium;

namespace CinderBlockGames.GitHub.Actions.LetsEncrypt.Connectors
{
    internal class Github
    {

        private readonly ConnectionInfo _connection;
        private readonly HttpClient _client;

        public Github(ConnectionInfo connection)
        {
            _connection = connection;
            _client = HttpClientFactory.Create();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                ConnectionInfo.AUTHORIZATION_TYPE, _connection.AccessToken);
            _client.DefaultRequestHeaders.Add("User-Agent", "GitHub Action"); // GitHub returns 403 unless you have a user agent specified.
        }

        public async Task SetSecret(string name, string value)
        {
            var key = await GetPublicKey();
            var encrypted = Convert.ToBase64String(
                SealedPublicKeyBox.Create(value, key.Value));
            await _client.PutAsync(
                $"{_connection.BaseUri}/{name}",
                JsonContent.Create(new { encrypted_value = encrypted, key_id = key.Id }));
        }

        private async Task<Key> GetPublicKey()
        {
            var get = $"{_connection.BaseUri}{ConnectionInfo.PUBLIC_KEY_PATH}";
            Console.WriteLine("Getting public key from " + get);
            var response = await _client.GetStringAsync(get);
            return JsonConvert.DeserializeObject<Key>(response);
        }

        #region " Key "

        private class Key
        {

            [JsonProperty("key_id")]
            public string Id { get; set; }

            [JsonProperty("key")]
            public byte[] Value { get; set; }

        }

        #endregion

        #region " ConnectionInfo "

        internal class ConnectionInfo
        {

            private const string API_FORMAT = "https://api.github.com/repos/{0}/actions/secrets";

            public string AccessToken { get; set; }
            private string Repo { get; set; }

            public const string AUTHORIZATION_TYPE = "token";

            public const string PUBLIC_KEY_PATH = "/public-key";
            public const string PUBLIC_KEY_JPATH = "$.key";

            public string BaseUri
            {
                get
                {
                    return string.Format(API_FORMAT, Repo);
                }
            }

            public ConnectionInfo(
                string accessToken,
                string repo)
            {
                AccessToken = accessToken;
                Repo = repo;
            }

        }

        #endregion

    }
}