using System.IO;
using System.Text.Json.Nodes;
using SimpleServer;

var config = JsonNode.Parse(await File.ReadAllTextAsync("~/config.json"));
var server = new Server((string)config["host"]);

await server.Start();