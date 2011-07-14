using System;
using System.IO;
using System.Reactive.Subjects;
using System.Runtime.Serialization.Json;
using System.Threading;
using Embedly.Caching;
using Embedly.Http;
using Embedly.OEmbed;

namespace Embedly
{
	/// <summary>
	/// Handles downloading requests and publishing results
	/// </summary>
	internal class DownloadObserver : IObserver<EmbedlyRequest>
	{
		private readonly Subject<Result> _results;
		private readonly TimeSpan _timeout;
		private readonly IResponseCache _cache;
		private int _count = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="DownloadObserver"/> class.
		/// </summary>
		/// <param name="cache">The cache.</param>
		/// <param name="timeout">The timeout.</param>
		/// <param name="results">The results.</param>
		public DownloadObserver(IResponseCache cache, TimeSpan timeout, Subject<Result> results)
		{
			_results = results;
			_timeout = timeout;
			_cache = cache;
		}

		/// <summary>
		/// Called when [next].
		/// </summary>
		/// <param name="value">The value.</param>
		public void OnNext(EmbedlyRequest value)
		{
			Interlocked.Increment(ref _count);

			HttpSocket.GetAsync(value.EmbedlyUrl.AbsoluteUri, _timeout, callbackState =>
			{
			    var state = (EmbedlyRequest) callbackState.State;
			    if (callbackState.Exception == null)
			    {
			        if (callbackState.Exception == null)
			        {
			            var responses = Deserialize(callbackState.ResponseStream);
			            for (var i = 0; i < state.UrlRequests.Length; i++)
			            {
			                _cache.Put(state.UrlRequests[i], responses[i]);
			                _results.OnNext(new Result(state.UrlRequests[i], responses[i]));
			            }
			        }
			    }
			    else
			    {
			        foreach (var urlRequest in state.UrlRequests)
			        {
			            _results.OnNext(new Result(urlRequest, callbackState.Exception));
			        }
			    }

			    // signal when we're done so consumers can finish
			    if (Interlocked.Decrement(ref _count) == 0)
			        _results.OnCompleted();

			}, value);
		}

		/// <summary>
		/// Called when [error].
		/// </summary>
		/// <param name="error">The error.</param>
		public void OnError(Exception error)
		{
			//
		}

		/// <summary>
		/// Called when [completed].
		/// </summary>
		public void OnCompleted()
		{
			if (Interlocked.CompareExchange(ref _count, 0, 0) == 0)
				_results.OnCompleted();
		}

		/// <summary>
		/// Deserializes the specified stream into an array of OEmbed responses.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns></returns>
		private static Response[] Deserialize(Stream stream)
		{
			var serializer = new DataContractJsonSerializer(typeof(Response[]), new Type[] { }, int.MaxValue, false, new ResponseDataContractSurrogate(), false);
			var result = (Response[])serializer.ReadObject(stream);

			return result;
		}
	}
}