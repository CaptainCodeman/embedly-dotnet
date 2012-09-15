using Embedly.OEmbed;

namespace Embedly.Caching
{
    /// <summary>
    /// Null response cache implementation for no caching
    /// </summary>
    public class NullResponseCache : IResponseCache
    {
        #region IResponseCache Members

        public Response Get(UrlRequest request)
        {
            return null;
        }

        public void Put(UrlRequest request, Response response)
        {
            // no-op
        }

        #endregion
    }
}