using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SimpleServer
{
    public static class RequestExtensions
    {
        public static async Task<string> GetPlainText(this HttpListenerRequest request)
        {
            using var reader = new StreamReader(request.InputStream);
            return await reader.ReadToEndAsync();
        }

        public static JsonObject GetJsonFromQuery(this HttpListenerRequest request)
        {
            var obj = new JsonObject();
            foreach (string key in request.QueryString.AllKeys)
                obj.Add(key, request.QueryString[key]);
            return obj;
        }

        public static async Task<JsonNode> GetJson(this HttpListenerRequest request) => JsonNode.Parse(await GetPlainText(request));
        public static async Task<T> Deserialize<T>(this HttpListenerRequest request) => await JsonSerializer.DeserializeAsync<T>(request.InputStream);
    }
}
