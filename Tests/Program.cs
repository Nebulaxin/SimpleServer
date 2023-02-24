using System;
using System.Threading.Tasks;
using SimpleServer;

var server = new Server("config.json");

server.AddResponse<GetInfoResponse, QueryInput, TextOutput>("/getInfo");
server.AddResponse<DefaultResponse, NullInput, TextOutput>("/");

bool running = true;
Console.CancelKeyPress += (o, e) => running = !(e.Cancel = true);

server.Start();
while (running) Console.ReadKey(true);
server.Stop();

class GetInfoResponse : MethodResponse<QueryInput, TextOutput>
{
    public override async Task Respond()
    {
        await Output.WriteLineAsync($"hello, {Input.Query["name"]}");
        await Output.WriteLineAsync("about this server:");
        await Output.WriteLineAsync("this is cool server");
    }
}

class DefaultResponse : MethodResponse<NullInput, TextOutput>
{
    public override async Task Respond()
    {
        Output.ContentType = "text/html";
        await Output.WriteLineAsync("<!DOCTYPE html><html><body><h1>This is default response.</h1></body></html>");
    }
}
