using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SimpleServer
{
    public abstract class ResponseBuilder
    {
        public ResponseBuilder(JsonNode options) { }

        public virtual Task InitAsync() => Task.CompletedTask;

        public abstract Response Build(HttpListenerRequest req, HttpListenerResponse resp);
    }

    public abstract class Response
    {
        public HttpListenerResponse Resp { get; private set; }

        public Response(HttpListenerRequest req, HttpListenerResponse resp)
        {
            Resp = resp;
        }

        public virtual Task InitAsync() => Task.CompletedTask;

        public virtual Task ApplyAsync() => Task.CompletedTask;
        public virtual Task CloseAsync() => Task.CompletedTask;
    }
}
