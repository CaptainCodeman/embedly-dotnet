using System;
using System.Runtime.Caching;
using Embedly.OEmbed;

namespace Embedly.Caching
{
    /// <summary>
    /// In-memory cache implementation using the System.Runtime.Caching.MemoryCache
    /// </summary>
    public class InMemoryResponseCache : IResponseCache
    {
        private readonly TimeSpan _expiration;

        public InMemoryResponseCache(TimeSpan expiration)
        {
            _expiration = expiration;
        }

        #region IResponseCache Members

        /// <summary>
        /// Gets the cached response for the specified key.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public Response Get(UrlRequest request)
        {
            MemoryCache cache = MemoryCache.Default;
            var response = (Response)cache.Get(request.CacheKey.ToString());
            return response;
        }

        /// <summary>
        /// Caches the response for the specified key.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        public void Put(UrlRequest request, Response response)
        {
            MemoryCache cache = MemoryCache.Default;
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.Add(_expiration)
                // SlidingExpiration = _expiration
            };
            cache.Add(request.CacheKey.ToString(), response, policy);
        }

        #endregion
    }
}