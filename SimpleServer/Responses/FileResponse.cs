using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SimpleServer
{
    internal class FileResponseBuilder : ResponseBuilder
    {
        private byte[] file;
        private string filePath, contentType;

        public FileResponseBuilder(JsonNode options)
        {
            filePath = (string)options["file"];
            contentType = (string)options["contentType"];
        }

        public override async Task InitAsync()
        {
            file = await File.ReadAllBytesAsync(filePath);
        }

        public override ServerResponse Build()
        {
            return new FileResponse(contentType, file);
        }
    }

    internal class FileResponse : ServerResponse
    {
        private byte[] file;
        private string contentType;
        public FileResponse(string contentType, byte[] file)
        {
            this.file = file;
            this.contentType = contentType;
        }

        public override async Task Apply()
        {
            Response.ContentType = contentType;
            await Response.OutputStream.WriteAsync(file);
        }
    }
}
