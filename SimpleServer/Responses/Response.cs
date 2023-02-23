using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SimpleServer
{
    internal abstract class ResponseBuilder
    {
        public virtual Task InitAsync() => Task.CompletedTask;
        public abstract ServerResponse Build();
    }

    public abstract class ServerResponse
    {
        public HttpListenerResponse Response { get; private set; }
        public HttpListenerRequest Request { get; private set; }

        public ServerResponse() { }

        internal void InitReqResp(HttpListenerRequest req, HttpListenerResponse resp)
        {
            Request = req;
            Response = resp;
        }

        public virtual Task Init() => Task.CompletedTask;
        public virtual Task Apply() => Task.CompletedTask;
        internal virtual Task Close() => Task.CompletedTask;
    }
}
