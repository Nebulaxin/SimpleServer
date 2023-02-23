using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SimpleServer
{
    public class FileResponseBuilder : ResponseBuilder
    {
        private byte[] file;
        private string filePath, contentType;

        public FileResponseBuilder(JsonNode options) : base(options)
        {
            filePath = (string)options["file"];
            contentType = (string)options["contentType"];
        }

        public override async Task InitAsync()
        {
            file = await File.ReadAllBytesAsync(filePath);
        }

        public override Response Build(HttpListenerRequest req, HttpListenerResponse resp)
        {
            return new FileResponse(req, resp, contentType, file);
        }
    }

    public class FileResponse : Response
    {
        private byte[] file;
        private string contentType;
        public FileResponse(HttpListenerRequest req, HttpListenerResponse resp, string contentType, byte[] file) : base(req, resp)
        {
            this.file = file;
            this.contentType = contentType;
        }

        public override async Task ApplyAsync()
        {
            Resp.ContentType = contentType;
            await Resp.OutputStream.WriteAsync(file);
        }
    }
}
