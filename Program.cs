using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
namespace CachingProxy
{
    internal class Program
    {

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
            Console.WriteLine($"Listening on {port}");
            
            listener.Start();
            while (true)
            {
                using(var socket = listener.AcceptSocket())
                {
                    if (!socket.Connected)
                        continue;


                    Requester req = new Requester(socket);

                    var content = req.GetContent();

                    Console.WriteLine($"Receiving request on address {content.Address}");
                    Receiver rec = new Receiver(origin, content.Address);

                    var response = await rec.GetResponse();
                    
                    string header = await req.Send(response);
                    Console.WriteLine(header);
                }
            }
        }
    }

    
}
