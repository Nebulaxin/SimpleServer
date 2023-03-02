using System;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SimpleServer
{
    internal class FileResponseBuilder : ResponseBuilder
    {
        private byte[] file;
        private readonly string filePath, contentType;

        public FileResponseBuilder(JsonNode options)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));
            filePath = (string)(options["file"] ?? options["request"]);
            contentType = (string)options["contentType"] ?? "text/plain";
            if (filePath is null) throw new ArgumentException("No file path or request was specified");
            if (!File.Exists(filePath)) throw new ArgumentException($"File '{filePath}' doesn't exist");
        }

        public override async Task Init()
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
        private readonly byte[] file;
        private readonly string contentType;
        public FileResponse(string contentType, byte[] file)
        {
            this.file = file;
            this.contentType = contentType;
        }

        public override async Task Respond()
        {
            Response.ContentType = contentType;
            await Response.OutputStream.WriteAsync(file);
        }
    }
}
