using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
namespace CachingProxy
{
    internal class Program
    {
        private static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        static async Task Main(string[] args)
        {
            string port = "3000", origin = null;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--port": port = args[i + 1]; break;
                    case "--origin": origin = args[i + 1]; break;
                }

                if (port != null && origin != null)
                    break;
            }
            await ListenOn(int.Parse(port), origin);
        }

        private static async Task ListenOn(int port, string origin)
        {
            TcpListener listener = new TcpListener(port);

            listener.Start();
            while (true)
            {
                using(var socket = listener.AcceptSocket())
                {
                    if (!socket.Connected)
                        continue;

                    var receivedBytes = new byte[1024];

                    socket.Receive(receivedBytes);

                    string strContent = Encoding.Default.GetString(receivedBytes);

                    SocketContent content = new SocketContent(strContent);

                    var response = GetFromCache(content.Address);
                    bool fromCache = response != null;
                    if (!fromCache)
                    {
                        response = await CallServer(origin, content.Address);
                        AddToCache(content.Address, response);
                    }
                    await ReturnToRequester(response, content, socket, fromCache);
                }
            }
        }

        private static void AddToCache(string address, HttpResponseMessage response)
        {
            _cache.Set(address, response);
        }

        private static HttpResponseMessage GetFromCache(string address)
        {
            if(_cache.TryGetValue(address, out HttpResponseMessage? result))
            return result;

            return null;
        }

        private static Task<HttpResponseMessage> CallServer(string origin, string address)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(origin);

            return client.GetAsync(address);
        }

        private static async Task ReturnToRequester(HttpResponseMessage response, SocketContent content, Socket socket, bool fromCache)
        {
            if (!socket.Connected)
                return;

            var data = await response.Content.ReadAsByteArrayAsync();

            string header = $"{content.HttpVersion} 200 OK \r\n";
            header += $"X-Cache: { (fromCache? "HIT" : "MISS") } \r\n";
            header += $"Content-Type: { response.Content.Headers.ContentType.MediaType } \r\n";
            header += $"Content-Length: {data.Count()} \r\n\r\n";

            Console.WriteLine(header);
            socket.Send(Encoding.Default.GetBytes(header));
            socket.Send(data);

        }
    }

    
}
