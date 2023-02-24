using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SimpleServer
{
    public class Server
    {
        public HttpListener Listener { get; private set; }

        private bool running;
        private readonly Dictionary<string, ResponseBuilder> requests;

        public Server(string configPath)
        {
            Listener = new();
            requests = new();
            var config = JsonNode.Parse(File.ReadAllText(configPath)).AsObject();
            Listener.Prefixes.Add((string)config["url"]);
            if (!config.ContainsKey("files"))
                return;
            foreach (var file in config["files"].AsArray())
                requests.Add((string)file["request"], new FileResponseBuilder(file));
        }

        /// <summary>
        /// Adds response to server
        /// </summary>
        /// <typeparam name="TResponse">Response to add to server</typeparam>
        /// <typeparam name="TInput">Input used for response</typeparam>
        /// <typeparam name="TOutput">Output used for response</typeparam>
        /// <param name="request">Request to answer. Must start with '/'</param>
        public void AddResponse<TResponse, TInput, TOutput>(string request)
            where TResponse : MethodResponse<TInput, TOutput>, new()
            where TInput : ResponseInput, new()
            where TOutput : ResponseOutput, new()
        {
            requests.Add(request, new MethodResponseBuilder<TResponse, TInput, TOutput>());
        }

        /// <summary>
        /// Starts recieving requests
        /// </summary>
        public void Start()
        {
            if (running) return;
            running = true;
            Listener.Start();
            Task.Factory.StartNew(Listen, TaskCreationOptions.LongRunning);
        }

        private async Task Listen()
        {
            foreach (var builder in requests.Values)
                await builder.Init();
            while (running)
            {
                HttpListenerContext context;
                try
                {
                    context = await Listener.GetContextAsync();
                }
                // i havent come up with anything better
                catch (HttpListenerException)
                {
                    return;
                }
                if (!running) return;
                var request = context.Request;
                var reqPath = request.Url.AbsolutePath;
                using var response = context.Response;
                if (!requests.TryGetValue(reqPath, out var builder))
                {
                    response.StatusCode = 404;
                    continue;
                }
                var builtResponse = builder.Build();
                await builtResponse.Init(request, response);
                await builtResponse.Respond();
                await builtResponse.Close();
            }
        }

        /// <summary>
        /// Stops recieving requests
        /// </summary>
        public void Stop()
        {
            if (!running) return;
            running = false;
            Listener.Stop();
        }
    }
}
