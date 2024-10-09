using System.Net;
using System.Text;

namespace CachingProxy
{
    public class Headers
    {
        public ResponseOrigin Origin { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        public Headers(HttpResponseMessage response, int dataLength, ResponseOrigin origin)
        {
            StatusCode = response.StatusCode;
            ReasonPhrase = response.ReasonPhrase;

            if (response.IsSuccessStatusCode)
            {
                ContentType = response.Content.Headers.ContentType!.MediaType!;
                Origin = origin;
                ContentLength = dataLength;
            }
        }

        public string ToString(string httpVersion)
        {
            StringBuilder headerBuilder = new StringBuilder();
            headerBuilder.Append(httpVersion.Replace("\r", ""));
            headerBuilder.AppendLine($" {(int)StatusCode} {ReasonPhrase}");
            headerBuilder.AppendLine($"X-Cache: {(Origin == ResponseOrigin.Cache ? "HIT" : "MISS")}");
            headerBuilder.AppendLine($"Content-Type: {ContentType}");
            headerBuilder.AppendLine($"Content-Length: {ContentLength}");
            headerBuilder.AppendLine();

            return headerBuilder.ToString();
        }
    }
}
