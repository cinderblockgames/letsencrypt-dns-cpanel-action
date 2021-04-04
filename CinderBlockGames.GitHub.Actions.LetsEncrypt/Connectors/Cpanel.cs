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

        private readonly ConnectionInfo Connection;
        private readonly HttpClient Client;

        public Cpanel(ConnectionInfo connection)
        {
            Connection = connection;
            Client = HttpClientFactory.Create();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                ConnectionInfo.AUTHORIZATION_TYPE, Connection.Authorization);
        }

        public async Task AddRecord(string data)
        {
            var add = string.Join(
                '&',
                Connection.BaseUri,
                $"{ConnectionInfo.FUNCTION_PARAMETER}={ConnectionInfo.ADD_FUNCTION}",
                ConnectionInfo.TYPE_PARAMETER_ARGUMENT,
                $"{ConnectionInfo.NAME_PARAMETER}={ConnectionInfo.SUBDOMAIN_BASE}.{Connection.Domain}.",
                $"{ConnectionInfo.DATA_PARAMETER}={data}");
            await Client.GetAsync(add);
        }

        public async Task RemoveRecord()
        {
            var line = await GetLineNumber();
            var remove = string.Join(
                '&',
                Connection.BaseUri,
                $"{ConnectionInfo.FUNCTION_PARAMETER}={ConnectionInfo.REMOVE_FUNCTION}",
                $"{ConnectionInfo.LINE_PARAMETER}={line}");
            await Client.GetAsync(remove);
        }

        private async Task<int> GetLineNumber()
        {
            var get = $"{Connection.BaseUri}&{ConnectionInfo.FUNCTION_PARAMETER}={ConnectionInfo.GET_FUNCTION}";
            var response = await Client.GetStringAsync(get);
            var json = (JObject)JsonConvert.DeserializeObject(response);
            var name = $"{ConnectionInfo.SUBDOMAIN_BASE}.{Connection.Domain}.";
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
            public string Domain { get; }

            public const string AUTHORIZATION_TYPE = "cpanel";
            public const string FUNCTION_PARAMETER = "cpanel_jsonapi_func";

            // add
            public const string ADD_FUNCTION = "add_zone_record";
            public const string TYPE_PARAMETER_ARGUMENT = "type=TXT";
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