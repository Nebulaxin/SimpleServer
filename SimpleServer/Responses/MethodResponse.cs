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

    public class MethodResponse<TInput, TOutput> : ServerResponse where TInput : ResponseInput, new() where TOutput : ResponseOutput, new()
    {
        public TInput Input { get; private set; } = new();
        public TOutput Output { get; private set; } = new();

        public override Task Init()
        {
            Input.Init(Request);
            Output.Init(Response);
            return Task.CompletedTask;
        }

        internal override Task Close()
        {
            Output.Write();
            return Task.CompletedTask;
        }
    }
}
