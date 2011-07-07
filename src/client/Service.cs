using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using Common.Logging;

namespace Embedly.Client
{
	/// <summary>
	/// Retrieves the list of supported services and provides a way to check if a URL is supported
	/// </summary>
	/// <remarks>
	/// This uses a string splitting and matching algorithm to check for supported URLs which is
	/// significantly faster than using regular expressions
	/// </remarks>
    public sealed class Service
    {
    	private static readonly ILog Log = LogManager.GetCurrentClassLogger();

		// singleton pattern from: http://csharpindepth.com/Articles/General/Singleton.aspx
		private static readonly Lazy<Service> Lazy = new Lazy<Service>(() => new Service());
    
		public static Service Instance { get { return Lazy.Value; } }

		private const string ServiceApi = "http://api.embed.ly/1/services";

		public IEnumerable<Provider> Providers { get; private set; }

		private Service()
		{
			DownloadServices();
        }

		private void DownloadServices()
		{
			Log.InfoFormat("Loading embed.ly service list");

			try
			{
				using (var client = new WebClient())
				{
					var data = client.DownloadData(ServiceApi);
					DeserializeServices(data);
				}
			}
			catch (Exception ex)
			{
				Log.ErrorFormat("Error downloading embed.ly service list", ex);
			}
		}

		private void DeserializeServices(byte[] data)
		{
			try
			{
				using (var stream = new MemoryStream(data))
				{
					var serializer = new DataContractJsonSerializer(typeof (IEnumerable<Provider>));
					Providers = (IEnumerable<Provider>)serializer.ReadObject(stream);
				}
			}
			catch (Exception ex)
			{
				Log.ErrorFormat("Error deserializing embed.ly service list", ex);
			}
		}

		public bool IsUrlSupported(Uri url)
        {
			if (url == null)
				throw new ArgumentNullException("url");
			
			return url.IsAbsoluteUri && Providers.Any(p => p.IsMatch(url));
        }

        public bool IsUrlSupported(string url)
        {
			if (url == null)
				throw new ArgumentNullException("url");
			
			return IsUrlSupported(new Uri(url));
        }

		public Provider GetProvider(Uri url)
		{
			if (url == null)
				throw new ArgumentNullException("url");
			
			if (!url.IsAbsoluteUri)
				return null;

			return Providers.FirstOrDefault(p => p.IsMatch(url));
		}

		public Provider GetProvider(string url)
		{
			if (url == null)
				throw new ArgumentNullException("url");
			
			return GetProvider(new Uri(url));
		}
	}
}
