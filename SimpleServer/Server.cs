using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SimpleServer
{
    public class Server
    {
        public HttpListener Listener { get; private set; }

        public Server(string host)
        {
            Listener = new();
            Listener.Prefixes.Add(host);
        }

        public async Task Start()
        {
            Listener.Start();
            while (true)
            {
                Console.WriteLine("waiting");
                var context = await Listener.GetContextAsync();

                Console.WriteLine("responding");
                using var response = context.Response;
                using var writer = new StreamWriter(response.OutputStream);
                await writer.WriteAsync("<html><body><h1>This is default response</h1></body></html>");
                Console.WriteLine("responded");
            }
        }
    }
}