using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using Common.Logging;

namespace Embedly.Http
{
	/// <summary>
	/// This class is passed on to the user supplied callback method as a parameter. If there was an exception during the process
	/// then the Exception property will not be null and will hold a reference to the Exception that was raised.
	/// The ResponseStream property will be not null in the case of a sucessful request/response cycle. Use this stream to
	/// exctract the response.
	/// </summary>
	internal class HttpWebRequestCallbackState
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();

		public Stream ResponseStream { get; private set; }
		public Exception Exception { get; private set; }
		public Object State { get; set; }

		public WebHeaderCollection Headers { get; private set; }
		public HttpStatusCode StatusCode { get; private set; }
		public string ContentType { get; private set; }
		public long ContentLength { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpWebRequestCallbackState"/> class.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <param name="state">The state.</param>
		public HttpWebRequestCallbackState(HttpWebResponse response, object state)
		{
			StatusCode = response.StatusCode;
			Headers = response.Headers;
			ContentType = response.ContentType.Split(new[] { ';' })[0];
			ContentLength = response.ContentLength;

			var contentEncoding = response.Headers[HttpResponseHeader.ContentEncoding] ?? string.Empty;
			if (contentEncoding.Contains("gzip"))
			{
				ResponseStream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
			}
			else if (contentEncoding.Contains("deflate"))
			{
				ResponseStream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress);
			}
			else
			{
				ResponseStream = response.GetResponseStream();
			}

			Log.Debug(m => m("HttpWebRequestCallbackState Ctor statusCode:{0}, contentLength:{1}, contentType:{2}", StatusCode, ContentLength, ContentType));

			State = state;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpWebRequestCallbackState"/> class.
		/// </summary>
		/// <param name="responseStream">The response stream.</param>
		/// <param name="state">The state.</param>
		public HttpWebRequestCallbackState(Stream responseStream, object state)
		{
			Log.Debug(m => m("HttpWebRequestCallbackState Ctor"));
			ResponseStream = responseStream;
			State = state;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpWebRequestCallbackState"/> class.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="state">The state.</param>
		public HttpWebRequestCallbackState(Exception exception, object state)
		{
			Log.Warn("HttpWebRequestCallbackState", exception);
			Exception = exception;
			State = state;
		}
	}
}