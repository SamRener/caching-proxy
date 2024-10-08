namespace CachingProxy
{
    public record RequesterContent
    {
        public string Method { get; }
        public string Address { get; }
        public string HttpVersion { get; set; }

        public RequesterContent(string content)
        {
            string[] lineSplitContent = content.Split("\n");

            string firstLine = lineSplitContent[0];

            string[] spaceSplitFirstLine = firstLine.Split(" ");

            Method = spaceSplitFirstLine[0];
            Address = spaceSplitFirstLine[1];
            HttpVersion = spaceSplitFirstLine[2];
        }
    }
}
