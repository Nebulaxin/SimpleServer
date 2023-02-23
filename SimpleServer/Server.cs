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
            var methods = new Dictionary<string, Type>();
            foreach (var type in Assembly.GetEntryAssembly().GetTypes())
            {
                if (!typeof(MethodResponse).IsAssignableFrom(type)) continue;
                var attr = type.GetCustomAttribute<ServerMethodAttribute>();
                if (attr == null) continue;
                methods.Add(attr.Name, type);
            }
            foreach (var method in config["methods"].AsArray())
                requests.Add((string)method["request"], new MethodResponseBuilder(method, methods));
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
                await builder.InitAsync();
                var builtResponse = builder.Build(request, response);
                await builtResponse.InitAsync();
                await builtResponse.ApplyAsync();
                await builtResponse.CloseAsync();
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
