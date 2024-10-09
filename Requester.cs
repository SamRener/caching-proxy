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
            if (!strContent.Contains("HTTP"))
                return null;

            RequesterContent content = new RequesterContent(strContent);

            Content = content;
            return content;
        }

        public async Task<string> Send(Response response)
        {
            if (!Socket.Connected)
                return null;

            var headers = response.Headers.ToString(Content.HttpVersion);

            Socket.Send(Encoding.Default.GetBytes(headers));
            if(response.Data.Length > 0)
                Socket.Send(response.Data);
            Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
            return headers;
        }
    }
}
