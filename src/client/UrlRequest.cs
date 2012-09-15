using System;
using System.Security.Cryptography;
using System.Text;

namespace Embedly
{
    /// <summary>
    /// Represents an individual embedly url request with the associated provider that is expected to fulfill it
    /// </summary>
    public class UrlRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlRequest"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="url">The URL.</param>
        public UrlRequest(Provider provider, Uri url)
        {
            Provider = provider;
            Url = url;
            CacheKey = GetUrlHash(url);
        }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        public Provider Provider { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public Uri Url { get; set; }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        public Guid CacheKey { get; private set; }

        /// <summary>
        /// Gets a hash of a Url using MD5
        /// </summary>
        /// <param name="url">The Url.</param>
        /// <returns></returns>
        public static Guid GetUrlHash(Uri url)
        {
            var provider = new MD5CryptoServiceProvider();

            byte[] inputBytes = Encoding.Default.GetBytes(url.AbsoluteUri);
            byte[] hashBytes = provider.ComputeHash(inputBytes);

            var hashGuid = new Guid(hashBytes);
            return hashGuid;
        }
    }
}