using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Common.Logging;
using Embedly.Caching;
using Embedly.OEmbed;

namespace Embedly
{
    /// <summary>
    /// Handles lookup of requests in cache
    /// </summary>
    internal class RequestObserver : IObserver<UrlRequest>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IResponseCache _cache;
        private readonly Subject<Result> _cachedResults;
        private readonly Subject<UrlRequest> _downloadRequired;
        private readonly Subject<Result> _downloadedResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestObserver"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="options">The options.</param>
        public RequestObserver(Client client, RequestOptions options)
        {
            _cache = client.Cache;
            _cachedResults = new Subject<Result>();
            _downloadedResults = new Subject<Result>();
            _downloadRequired = new Subject<UrlRequest>();

            var downloader = new DownloadObserver(client.Cache, client.RequestTimeout, _downloadedResults);

            _downloadRequired
                .Buffer(client.BufferTimeout, 20)
                .Where(reqs => reqs.Count > 0)
                .Select(reqs =>
                        new EmbedlyRequest(
                            new Uri(@"http://api.embed.ly/1/oembed?format=json&key=" + client.Key + @"&urls=" + string.Join(",", reqs.Select(req => Uri.EscapeDataString(req.Url.OriginalString))) +
                                    options.GetQueryString()),
                            reqs
                            )
                )
                .Subscribe(downloader);

            Output = _cachedResults.Merge(_downloadedResults);
        }

        /// <summary>
        /// Gets the output.
        /// </summary>
        public IObservable<Result> Output { get; private set; }

        #region IObserver<UrlRequest> Members

        /// <summary>
        /// Called when [next].
        /// </summary>
        /// <param name="value">The value.</param>
        public void OnNext(UrlRequest value)
        {
            Response response = _cache.Get(value);
            if (response == null)
            {
                // push to processing
                Log.DebugFormat("Download needed for url: {0}", value.Url);
                _downloadRequired.OnNext(value);
            }
            else
            {
                // push to cached results
                Log.DebugFormat("Cached response found for url: {0}", value.Url);
                _cachedResults.OnNext(new Result(value, response));
            }
        }

        /// <summary>
        /// Called when [error].
        /// </summary>
        /// <param name="error">The error.</param>
        public void OnError(Exception error)
        {
            // TODO: Handle exceptions
            Log.Error("Exception observing requests", error);
        }

        /// <summary>
        /// Called when [completed].
        /// </summary>
        public void OnCompleted()
        {
            _cachedResults.OnCompleted();
            _downloadRequired.OnCompleted();
        }

        #endregion
    }
}