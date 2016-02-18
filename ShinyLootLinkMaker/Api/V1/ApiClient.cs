using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace ShinyLootLinkMaker.Api.V1
{
    public class ApiClient
    {
        static readonly string BaseUrl = "http://www.shinyloot.com/sl_ws/1.0";
        static readonly string MethodParamName = "rmethod";
        static readonly string TokenParamName = "rtoken";
        static readonly Version ClientVersion = new Version(0, 1, 1, 0); // Emulate last downloader version

        string token;
        IRestClient client;

        public ApiClient()
        {
            client = new RestClient(BaseUrl) { FollowRedirects = false };
            // Why do they return JSON with MIME type for HTML?
            client.AddHandler("text/html", new RestSharp.Deserializers.JsonDeserializer());
        }

        public void SetToken(string token)
        {
            this.token = token;
        }

        public ApiResponse<VersionCheckCode> CheckVersion()
        {
            IRestRequest req = new RestRequest(Method.GET);
            req.AddQueryParameter(MethodParamName, "client_version_check");
            req.AddQueryParameter("major", ClientVersion.Major.ToString());
            req.AddQueryParameter("minor", ClientVersion.Major.ToString());
            req.AddQueryParameter("build", ClientVersion.Major.ToString());
            req.AddQueryParameter("revision", ClientVersion.Major.ToString());
            var resp = client.Execute<ApiResponse<VersionCheckCode>>(req);
            if (resp.StatusCode != System.Net.HttpStatusCode.OK) return null;
            return resp.Data;
        }

        public ApiResponse<LoginCode, LoginResponse> LogIn(string username, string password)
        {
            IRestRequest req = new RestRequest(Method.GET);
            req.AddQueryParameter(MethodParamName, "login");
            req.AddQueryParameter("username", username);
            req.AddQueryParameter("password", HashPassword(password));
            var resp = client.Execute<ApiResponse<LoginCode, LoginResponse>>(req);
            if (resp.StatusCode != System.Net.HttpStatusCode.OK) return null;
            if (resp.Data.Code == LoginCode.Success) token = resp.Data.Data.Token;
            return resp.Data;
        }

        public ApiResponse<GamesCode, GamesResponse> GetMyGames()
        {
            ensureToken();
            IRestRequest req = new RestRequest(Method.GET);
            req.AddQueryParameter(MethodParamName, "my_games");
            req.AddQueryParameter(TokenParamName, token);
            var resp = client.Execute<ApiResponse<GamesCode, GamesResponse>>(req);
            if (resp.StatusCode != System.Net.HttpStatusCode.OK) return null;
            return resp.Data;
        }

        public string GetFileRedirect(int gameId, int fileId)
        {
            ensureToken();
            IRestRequest req = new RestRequest(Method.GET);
            req.AddQueryParameter(MethodParamName, "get_file");
            req.AddQueryParameter(TokenParamName, token);
            req.AddQueryParameter("game_id", gameId.ToString());
            req.AddQueryParameter("file_id", fileId.ToString());
            var resp = client.Execute(req);
            if (resp.StatusCode == System.Net.HttpStatusCode.Redirect)
            {
                return (string)resp.Headers.Single(h => h.Name == "Location").Value;
            }
            else
            {
                return null;
            }
        }

        public Uri BuildGetFileUri(int gameId, int fileId)
        {
            ensureToken();
            IRestRequest req = new RestRequest(Method.GET);
            req.AddQueryParameter(MethodParamName, "get_file");
            req.AddQueryParameter(TokenParamName, token);
            req.AddQueryParameter("game_id", gameId.ToString());
            req.AddQueryParameter("file_id", fileId.ToString());
            return client.BuildUri(req);
        }

        void ensureToken()
        {
            if (string.IsNullOrEmpty(token)) throw new InvalidOperationException("Client is not logged in.");
        }

        static string HashPassword(string password)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] data = Encoding.ASCII.GetBytes(password); // Downloader uses ASCII for some reason...
                byte[] hashBytes = md5.ComputeHash(data);
                StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes) sb.AppendFormat("{0:x2}", b);
                return sb.ToString();
            }
        }

    }
}
