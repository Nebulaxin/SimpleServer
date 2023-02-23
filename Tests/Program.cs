using System;
using System.IO;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using SimpleServer;

var config = JsonNode.Parse(await File.ReadAllTextAsync("config.json"));
var server = new Server(config);

bool running = true;
Console.CancelKeyPress += (o, e) => running = !(e.Cancel = true);

server.Start();
while (running) Console.ReadKey(true);
server.Stop();

[ServerMethod("GetInfo")]
class GetInfoResponse : TextMethodResponse
{
    private string name;
    public GetInfoResponse(HttpListenerRequest req, HttpListenerResponse resp) : base(req, resp)
    {
        name = req.QueryString["name"];
    }

    public override async Task ApplyAsync()
    {
        Resp.ContentType = "text/plain";
        await WriteLine($"hello, {name}. about this server:");
        await WriteLine("this is cool server");
    }
}