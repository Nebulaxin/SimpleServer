using System.Net;
using System.Threading.Tasks;

namespace SimpleServer
{
    internal abstract class ResponseBuilder
    {
        public virtual Task Init() => Task.CompletedTask;

        public abstract ServerResponse Build();
    }

    public abstract class ServerResponse
    {
        public HttpListenerResponse Response { get; private set; }
        public HttpListenerRequest Request { get; private set; }

        internal virtual Task Init(HttpListenerRequest req, HttpListenerResponse resp)
        {
            Request = req;
            Response = resp;
            return Task.CompletedTask;
        }
        internal virtual Task Close() => Task.CompletedTask;
        public virtual Task Respond() => Task.CompletedTask;
    }
}
