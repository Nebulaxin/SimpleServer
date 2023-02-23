using System;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using SimpleServer;

var config = JsonNode.Parse(await File.ReadAllTextAsync("config.json"));
var server = new Server(config);

server.AddResponse<GetInfoResponse, QueryInput, TextOutput>("/getInfo");

bool running = true;
Console.CancelKeyPress += (o, e) => running = !(e.Cancel = true);

server.Start();
while (running) Console.ReadKey(true);
server.Stop();

class GetInfoResponse : MethodResponse<QueryInput, TextOutput>
{
    private string name;
    public override Task Init()
    {
        name = Input.Query["name"];
        return base.Init();
    }

    public override async Task Apply()
    {
        await Output.WriteLineAsync($"hello, {name}");
        await Output.WriteLineAsync("about this server:");
        await Output.WriteLineAsync("this is cool server");
    }
}
