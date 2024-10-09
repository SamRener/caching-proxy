namespace CachingProxy
{
    public class Response
    {
        public Headers Headers { get; }
        public byte[] Data { get; }
        public Response(HttpResponseMessage response)
        {
            Data = response.Content.ReadAsByteArrayAsync().Result;
            Headers = new Headers(response, Data.Length, ResponseOrigin.Server);
        }
        public Response(Response response)
        {
            Data = response.Data;
            Headers = response.Headers;

            Headers.Origin = ResponseOrigin.Cache;
        }
    }
}
