using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SimpleServer
{
    public class MethodResponseBuilder : ResponseBuilder
    {
        private Type methodClass;
        public MethodResponseBuilder(JsonNode options, Dictionary<string, Type> methods) : base(options)
        {
            methodClass = methods[(string)options["method"]];
        }

        public override Response Build(HttpListenerRequest req, HttpListenerResponse resp)
        {
            return (Response)Activator.CreateInstance(methodClass, req, resp);
        }
    }

    public class MethodResponse : Response
    {
        public MethodResponse(HttpListenerRequest req, HttpListenerResponse resp) : base(req, resp)
        {
        }
    }

    public class TextMethodResponse : MethodResponse
    {
        private StreamWriter writer;
        public TextMethodResponse(HttpListenerRequest req, HttpListenerResponse resp) : base(req, resp)
        {
            writer = new StreamWriter(resp.OutputStream);
        }

        public async Task Write(object obj) => await writer.WriteAsync(obj.ToString());
        public async Task WriteLine(object obj) => await writer.WriteLineAsync(obj.ToString());

        public override async Task CloseAsync()
        {
            await writer.FlushAsync();
            writer.Close();
        }
    }
}
