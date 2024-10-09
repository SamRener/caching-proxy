using System.Net.Sockets;
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

            Thread th = new Thread(new ThreadStart(() => StartListen(origin, listener)));
            th.Start();

            ;
        }

        private static void StartListen(string origin, TcpListener listener)
        {
            while (true)
            {
                using (var socket = listener.AcceptSocket())
                {
                    if (!socket.Connected)
                        continue;
                    try
                    {

                        Requester req = new Requester(socket);

                        var content = req.GetContent();
                        if (content is null)
                            continue;

                        Console.WriteLine($"Receiving request on address {content.Address}");
                        Receiver rec = new Receiver(origin, content.Address);

                        var response = rec.GetResponse().Result;

                        var header = req.Send(response).Result;
                        Console.WriteLine(header);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }

    
}
