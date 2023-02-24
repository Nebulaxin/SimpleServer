using System.Net;
using System.Threading.Tasks;

namespace SimpleServer
{
    internal class MethodResponseBuilder<TResponse, TInput, TOutput> : ResponseBuilder
        where TResponse : MethodResponse<TInput, TOutput>, new()
        where TInput : ResponseInput, new()
        where TOutput : ResponseOutput, new()
    {
        public override ServerResponse Build()
        {
            return new TResponse();
        }
    }

    public class MethodResponse<TInput, TOutput> : ServerResponse
        where TInput : ResponseInput, new()
        where TOutput : ResponseOutput, new()
    {
        public TInput Input { get; private set; } = new();
        public TOutput Output { get; private set; } = new();


        internal override async Task Init(HttpListenerRequest req, HttpListenerResponse resp)
        {
            await Input.Init(req);
            Output.Init(resp);
            await base.Init(req, resp);
        }

        internal override async Task Close()
        {
            await Output.Write();
        }
    }
}
