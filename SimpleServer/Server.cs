using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SimpleServer
{
    public class Server
    {
        public HttpListener Listener { get; private set; }

        private bool running;

        public Server(string url)
        {
            Listener = new();
            Listener.Prefixes.Add(url);
        }

        /// <summary>
        /// Starts recieving requests
        /// </summary>
        public void Start()
        {
            if (running) return;
            running = true;
            Listener.Start();
            Task.Factory.StartNew(Listen, TaskCreationOptions.LongRunning);
        }

        private async Task Listen()
        {
            while (running)
            {
                HttpListenerContext context;
                try
                {
                    context = await Listener.GetContextAsync();
                }
                catch (HttpListenerException)
                {
                    return;
                }
                if (!running) return;
                var request = context.Request;
                using var response = context.Response;
                using var writer = new StreamWriter(response.OutputStream);
                await writer.WriteAsync("<html><body><h1>This is default response</h1></body></html>");
            }
        }

        /// <summary>
        /// Stops recieving requests
        /// </summary>
        public void Stop()
        {
            if (!running) return;
            running = false;
            Listener.Stop();
        }
    }
}