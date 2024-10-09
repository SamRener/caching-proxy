namespace CachingProxy
{
    public class Receiver
    {
        public string Origin { get; }
        public string Address { get; }
        public Receiver(string origin, string address)
        {
            Origin = origin;
            Address = address;
        }
        public async Task<Response> GetResponse()
        {
            var response = Cache.GetFromCache(Address);

            if (response != null)
            {
                return new Response(response);
            }

            int attempts = 5;
            HttpResponseMessage responseMessage = null;
            while (attempts > 0 || responseMessage == null)
            {
                try
                {
                    responseMessage = await CallServer();
                    break;
                }
                catch
                {
                    attempts--;
                    Console.WriteLine($"Failed to connect to server. Attempts remaining: {attempts}/5");
                }
            }
            if (responseMessage is null)
                throw new CannotConnectToServerException();

            response = new Response(responseMessage);
            Cache.AddToCache(Address, response);

            return response;
        }

        private Task<HttpResponseMessage> CallServer()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(Origin);

            return client.GetAsync(Address);
        }
    }
}
