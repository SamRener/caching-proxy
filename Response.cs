namespace CachingProxy
{
    public record Response
    {
        public Response(HttpResponseMessage response, ResponseOrigin origin)
        {
            Message = response;
            Origin = origin;
        }

        public HttpResponseMessage Message { get; }
        public ResponseOrigin Origin { get; }
    }
}
