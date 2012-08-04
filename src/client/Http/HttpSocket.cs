using System;
using System.IO;
using System.Net;
using System.Threading;
using Common.Logging;

namespace Embedly.Http
{
	/// <summary>
	/// Handle asynchronous downloads
	/// </summary>
	/// <remarks>
	/// Adapted from the excellent article by  http://www.matlus.com/httpwebrequest-asynchronous-programming/
	/// Added timeout, configurable request headers and support for http compression
	/// </remarks>
	internal static class HttpSocket
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Creates the HTTP web request.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="httpMethod">The HTTP method.</param>
		/// <param name="contentType">Type of the content.</param>
		/// <returns></returns>
		private static HttpWebRequest CreateHttpWebRequest(string url, string httpMethod, string contentType)
		{
			Log.Debug(m => m("CreateHttpWebRequest({0},{1},{2})", url, httpMethod, contentType));

			var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

			httpWebRequest.ContentType = contentType;
			httpWebRequest.Method = httpMethod;
			httpWebRequest.Headers[HttpRequestHeader.AcceptEncoding] = "compress, gzip"; // use http compression
		    httpWebRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Pipelined = true;
            httpWebRequest.UnsafeAuthenticatedConnectionSharing = true;
            httpWebRequest.UseDefaultCredentials = false;
            httpWebRequest.Proxy = WebRequest.DefaultWebProxy;

			return httpWebRequest;
		}

		/// <summary>
		/// Begins the get response callback.
		/// </summary>
		/// <param name="asyncResult">The async result.</param>
		static void BeginGetResponseCallback(IAsyncResult asyncResult)
		{
			WebResponse webResponse = null;
			Stream responseStream = null;
			HttpWebRequestAsyncState asyncState = null;
			try
			{
				asyncState = (HttpWebRequestAsyncState)asyncResult.AsyncState;
				webResponse = asyncState.HttpWebRequest.EndGetResponse(asyncResult);
				responseStream = webResponse.GetResponseStream();
				var webRequestCallbackState = new HttpWebRequestCallbackState((HttpWebResponse)webResponse, asyncState.State);
				asyncState.ResponseCallback(webRequestCallbackState);
				responseStream.Close();
				responseStream = null;
				webResponse.Close();
				webResponse = null;
			}
			catch (Exception ex)
			{
				if (asyncState != null)
				{
					Log.WarnFormat("Exception requesting url: {0}", ex, asyncState.HttpWebRequest.RequestUri);
					asyncState.ResponseCallback(new HttpWebRequestCallbackState(ex, asyncState.State));
				}
				else
				{
					Log.Warn("BeginGetResponseCallback", ex);
					throw;
				}
			}
			finally
			{
				if (responseStream != null)
					responseStream.Close();
				if (webResponse != null)
					webResponse.Close();
			}
		}

		/// <summary>
		/// If the response from a remote server is in text form
		/// you can use this method to get the text from the ResponseStream
		/// This method Disposes the stream before it returns
		/// </summary>
		/// <param name="responseStream">The responseStream that was provided in the callback delegate's HttpWebRequestCallbackState parameter</param>
		/// <returns></returns>
		public static string GetResponseText(Stream responseStream)
		{
			using (var reader = new StreamReader(responseStream))
			{
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Abort the request on timeout
		/// </summary>
		/// <param name="state">The state.</param>
		/// <param name="timedOut">if set to <c>true</c> [timed out].</param>
		private static void TimeOutCallback(object state, bool timedOut)
		{
			if (!timedOut) return;

			var request = state as HttpWebRequest;

			Log.Warn(m => m("RequestTimeout requesting url: {0}", request.RequestUri));

			if (request != null)
				request.Abort();
		}

		/// <summary>
		/// This method does an Http GET to the provided url and calls the responseCallback delegate
		/// providing it with the response returned from the remote server.
		/// </summary>
		/// <param name="url">The url to make an Http GET to</param>
		/// <param name="timeout">The timeout.</param>
		/// <param name="responseCallback">The callback delegate that should be called when the response returns from the remote server</param>
		/// <param name="state">Any state information you need to pass along to be available in the callback method when it is called</param>
		/// <param name="contentType">The Content-Type of the Http request</param>
		public static void GetAsync(string url, TimeSpan timeout, Action<HttpWebRequestCallbackState> responseCallback, object state = null, string contentType = "application/x-www-form-urlencoded")
		{
			HttpWebRequest httpWebRequest;
			try
			{
				httpWebRequest = CreateHttpWebRequest(url, "GET", contentType);
			}
			catch (Exception ex)
			{
				Log.WarnFormat("Exception creating WebRequest for url: {0}", ex, url);
				responseCallback(new HttpWebRequestCallbackState(ex, state));
				return;
			}

			var result = httpWebRequest.BeginGetResponse(BeginGetResponseCallback,
				new HttpWebRequestAsyncState()
				{
					HttpWebRequest = httpWebRequest,
					ResponseCallback = responseCallback,
					State = state
				});

			ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, TimeOutCallback, httpWebRequest, timeout, true);
		}
	}
}
