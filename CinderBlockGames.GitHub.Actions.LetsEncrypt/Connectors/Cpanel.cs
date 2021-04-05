using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CinderBlockGames.GitHub.Actions.LetsEncrypt.Connectors
{
    internal class Cpanel
    {

        private readonly ConnectionInfo _connection;
        private readonly HttpClient _client;

        public Cpanel(ConnectionInfo connection)
        {
            _connection = connection;
            _client = HttpClientFactory.Create();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                ConnectionInfo.AUTHORIZATION_TYPE, _connection.Authorization);
        }

        public async Task AddRecord(string domain, string data)
        {
            var add = string.Join(
                '&',
                _connection.BaseUri,
                $"{ConnectionInfo.FUNCTION_PARAMETER}={ConnectionInfo.ADD_FUNCTION}",
                ConnectionInfo.TYPE_PARAMETER_ARGUMENT,
                ConnectionInfo.TIME_TO_LIVE_PARAMETER_ARGUMENT,
                $"{ConnectionInfo.NAME_PARAMETER}={ConnectionInfo.SUBDOMAIN_BASE}.{domain}.",
                $"{ConnectionInfo.DATA_PARAMETER}={data}");
            await _client.GetAsync(add);
        }

        public async Task RemoveRecord(string domain)
        {
            var line = await GetLineNumber(domain);
            var remove = string.Join(
                '&',
                _connection.BaseUri,
                $"{ConnectionInfo.FUNCTION_PARAMETER}={ConnectionInfo.REMOVE_FUNCTION}",
                $"{ConnectionInfo.LINE_PARAMETER}={line}");
            await _client.GetAsync(remove);
        }

        private async Task<int> GetLineNumber(string domain)
        {
            var get = $"{_connection.BaseUri}&{ConnectionInfo.FUNCTION_PARAMETER}={ConnectionInfo.GET_FUNCTION}";
            var response = await _client.GetStringAsync(get);
            var json = (JObject)JsonConvert.DeserializeObject(response);
            var name = $"{ConnectionInfo.SUBDOMAIN_BASE}.{domain}.";
            var path = string.Format(ConnectionInfo.JPATH_FORMAT, name);
            // .Last() instead of .Single() because the user might have some already in there.
            // This will return the last one, which should be the one we added.
            return json.SelectTokens(path).Values<int>().Last();
        }

        #region " ConnectionInfo "

        internal class ConnectionInfo
        {

            private const string URL_SCHEME = "https";
            private const string BASE_PATH = "json-api/cpanel?cpanel_jsonapi_module=ZoneEdit&cpanel_jsonapi_version=2";
            private const string DOMAIN_PARAMETER = "domain";

            private string Host { get; }
            private int Port { get; }
            private string Username { get; }
            private string ApiToken { get; }
            private string Domain { get; }

            public const string AUTHORIZATION_TYPE = "cpanel";
            public const string FUNCTION_PARAMETER = "cpanel_jsonapi_func";

            // add
            public const string ADD_FUNCTION = "add_zone_record";
            public const string TYPE_PARAMETER_ARGUMENT = "type=TXT";
            public const string TIME_TO_LIVE_PARAMETER_ARGUMENT = "ttl=1";
            public const string NAME_PARAMETER = "name";
            public const string DATA_PARAMETER = "txtdata";
            public const string SUBDOMAIN_BASE = "_acme-challenge";

            // get
            public const string GET_FUNCTION = "get_zone_record";

            // remove
            public const string REMOVE_FUNCTION = "remove_zone_record";
            public const string LINE_PARAMETER = "line";
            public const string JPATH_FORMAT = "$.cpanelresult.data[?(@.name == '{0}')].line";

            public string Authorization
            {
                get
                {
                    return $"{Username}:{ApiToken}";
                }
            }

            public string BaseUri
            {
                get
                {
                    return $"{URL_SCHEME}://{Host}:{Port}/{BASE_PATH}&{DOMAIN_PARAMETER}={Domain}";
                }
            }

            public ConnectionInfo(
                string host,
                int port,
                string username,
                string apiToken,
                string domain)
            {
                Host = host;
                Port = port;
                Username = username;
                ApiToken = apiToken;
                Domain = domain;
            }

        }

        #endregion

    }
}