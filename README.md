# SimpleServer

This is simple single threaded server that can host files and response to HTTP requests

[**Nuget package**](https://www.nuget.org/packages/SimpleNebulaServer)

---

## Usage

+ Create server with config as path to it:

```cs
var server = new Server("config.json");
```

+ Create `config.json` with url

```json
{
    "url": "http://127.0.0.1:8080/"
}
```

+ Add response:

```cs
class DefaultResponse : MethodResponse<NullInput, TextOutput>
{
    public override async Task Respond()
    {
        Output.ContentType = "text/html";
        await Output.WriteLineAsync("<!DOCTYPE html><html><body><h1>This is default response.</h1></body></html>");
    }
}
```

+ Add response to server:

```cs
server.AddResponse<DefaultResponse, NullInput, NullOutput>("/default");
```

+ Start server:

```cs
server.Start();
// Wait for request to stop server, for example Console.ReadLine()
server.Stop();
```

---

## Example

Code:

```cs
using System;
using System.Threading.Tasks;
using SimpleServer;

var server = new Server("config.json");

server.AddResponse<GetInfoResponse, QueryInput, TextOutput>("/getInfo");

server.Start();
Console.ReadLine();
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
    ]
}
```
