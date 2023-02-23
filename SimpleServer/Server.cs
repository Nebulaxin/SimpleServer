using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SimpleServer
{
    public class Server
    {
        public HttpListener Listener { get; private set; }

        private bool running;
        private Dictionary<string, ResponseBuilder> requests;

        public Server(string url)
        {
            Listener = new();
            requests = new();
            Listener.Prefixes.Add(url);
        }

        public Server(JsonNode config) : this((string)config["url"])
        {
            foreach (var file in config["files"].AsArray())
                requests.Add((string)file["request"], new FileResponseBuilder(file));
        }

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
                await builder.InitAsync();
            while (running)
            {
                HttpListenerContext context;
                try
                {
                    context = await Listener.GetContextAsync();
                }
                catch (HttpListenerException) // i havent come up with anything better
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
                builtResponse.InitReqResp(request, response);
                await builtResponse.Init();
                await builtResponse.Apply();
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
