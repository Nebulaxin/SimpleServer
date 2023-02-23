using System;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using SimpleServer;

var config = JsonNode.Parse(await File.ReadAllTextAsync("config.json"));
var server = new Server((string)config["host"]);

bool running = true;
Console.CancelKeyPress += (o, e) => running = !(e.Cancel = true);

server.Start();
while (running) Console.ReadKey(true);
server.Stop();
