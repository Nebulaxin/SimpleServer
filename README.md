# SimpleServer

This is simple single-thread server

## How to use

Create server with url or config as `JsonNode` from `System.Text.Json.Nodes`:

```cs
var server = new Server("http://127.0.0.1:8080/");
// or
var server = new Server(JsonNode.Parse(File.ReadAllTextAsync("config.json")));
```

## Example

Code:

```cs
using System;
using System.IO;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using SimpleServer;

var config = JsonNode.Parse(await File.ReadAllTextAsync("config.json"));
var server = new Server(config);

server.Start();
Console.ReadLine();
server.Stop();

[ServerMethod("GetInfo")]
class GetInfoResponse : TextMethodResponse
{
    private string name;
    public GetInfoResponse(HttpListenerRequest req, HttpListenerResponse resp) 
        : base(req, resp)
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
```

Config:

```json
{
    "url": "http://127.0.0.1:8080/",
    "files": [
        {
            "request": "/favicon.ico",
            "contentType": "image/png",
            "file": "favicon.png"
        },
        {
            "request": "/",
            "contentType": "text/html",
            "file": "index.html"
        }
    ],
    "methods": [
        {
            "request": "/getInfo",
            "method": "GetInfo"
        }
    ]
}
```
