using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SimpleServer
{
    #region Input
    public abstract class ResponseInput
    {
        internal ResponseInput() { }
        internal abstract Task Init(HttpListenerRequest request);
    }

    public class NullInput : ResponseInput
    {
        internal override Task Init(HttpListenerRequest _) => Task.CompletedTask;
    }

    public class PlainTextInput : ResponseInput
    {
        public string Text { get; private set; }

        internal override async Task Init(HttpListenerRequest request)
        {
            using var reader = new StreamReader(request.InputStream);
            Text = await reader.ReadToEndAsync();
        }
    }

    public class JsonInput : ResponseInput
    {
        public JsonNode Node { get; private set; }

        internal override async Task Init(HttpListenerRequest request)
        {
            using var reader = new StreamReader(request.InputStream);
            Node = JsonNode.Parse(await reader.ReadToEndAsync());
        }
    }

    public class SerializedInput<T> : ResponseInput
    {
        public T Content { get; private set; }

        internal override async Task Init(HttpListenerRequest request)
        {
            Content = await JsonSerializer.DeserializeAsync<T>(request.InputStream);
        }
    }

    public class QueryJsonInput : ResponseInput
    {
        public JsonObject Query { get; private set; }

        internal override Task Init(HttpListenerRequest request)
        {
            Query = new JsonObject();
            foreach (string key in request.QueryString.AllKeys)
                Query.Add(key, request.QueryString[key]);
            return Task.CompletedTask;
        }
    }

    public class QueryInput : ResponseInput
    {
        public NameValueCollection Query { get; private set; }

        internal override Task Init(HttpListenerRequest request)
        {
            Query = request.QueryString;
            return Task.CompletedTask;
        }
    }
    #endregion

    #region Output
    public abstract class ResponseOutput
    {
        internal virtual void Init(HttpListenerResponse response) { }

        internal virtual Task Write() => Task.CompletedTask;
    }

    public class NullOutput : ResponseOutput { }

    public class TextOutput : ResponseOutput
    {
        private HttpListenerResponse response;
        private StreamWriter writer;
        public string ContentType { get => response.ContentType; set => response.ContentType = value; }

        internal override void Init(HttpListenerResponse response)
        {
            this.response = response;
            writer = new(response.OutputStream) { AutoFlush = false };
            ContentType = "text/plain";
        }
        internal override async Task Write() => await writer.FlushAsync();

        public async Task WriteAsync(object obj) => await writer.WriteAsync(obj.ToString());
        public async Task WriteLineAsync(object obj) => await writer.WriteLineAsync(obj.ToString());
        public async Task WriteLineAsync() => await writer.WriteLineAsync();
    }

    public class JsonOutput : ResponseOutput
    {
        private Utf8JsonWriter writer;
        public JsonNode Node { get; set; }

        internal override void Init(HttpListenerResponse response)
        {
            writer = new(response.OutputStream);
            response.ContentType = "application/json";
        }
        internal override Task Write()
        {
            Node.WriteTo(writer);
            return Task.CompletedTask;
        }
    }

    public class SerializedOutput<T> : ResponseOutput
    {
        private Stream writer;
        public T Content { get; set; }

        internal override void Init(HttpListenerResponse response)
        {
            writer = response.OutputStream;
            response.ContentType = "application/json";
        }
        internal override async Task Write() => await JsonSerializer.SerializeAsync(writer, Content);
    }
    #endregion
}
