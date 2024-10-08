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
                return new Response(response, ResponseOrigin.Cache);

            response = await CallServer();
            Cache.AddToCache(Address, response);

            return new Response(response, ResponseOrigin.Server);
        }

        private Task<HttpResponseMessage> CallServer()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(Origin);

            return client.GetAsync(Address);
        }
    }
}
