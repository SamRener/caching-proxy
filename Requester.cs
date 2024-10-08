using System.Net.Sockets;
using System.Text;

namespace CachingProxy
{
    public class Requester
    {
        public Socket Socket { get; }
        public RequesterContent Content { get; private set; }
        public Requester(Socket socket)
        {
            Socket = socket;
        }

        public RequesterContent GetContent()
        {
            if (Content != null)
                return Content;

            var receivedBytes = new byte[1024];
            Socket.Receive(receivedBytes);

            string strContent = Encoding.Default.GetString(receivedBytes);

            RequesterContent content = new RequesterContent(strContent);

            Content = content;
            return content;
        }

        public async Task<string> Send(Response response)
        {
            if (!Socket.Connected)
                return string.Empty;

            var data = await response.Message.Content.ReadAsByteArrayAsync();

            string header = $"{Content.HttpVersion} 200 OK \r\n";
            header += $"X-Cache: {(response.Origin == ResponseOrigin.Cache? "HIT" : "MISS")} \r\n";
            header += $"Content-Type: {response.Message.Content.Headers.ContentType.MediaType} \r\n";
            header += $"Content-Length: {data.Count()} \r\n\r\n";

            Socket.Send(Encoding.Default.GetBytes(header));
            Socket.Send(data);

            return header;
        }
    }
}
