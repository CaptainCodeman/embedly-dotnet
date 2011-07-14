﻿using System;
using System.Collections.Generic;
using System.Linq;
using Embedly.Caching;

namespace Embedly
{
	/// <summary>
	/// Client used as the base for all communications with embedly
	/// </summary>
	public class Client
	{
		/// <summary>
		/// Gets the embedly key.
		/// </summary>
		public string Key { get; private set; }

		/// <summary>
		/// Gets the timeout to use for requests.
		/// </summary>
		public TimeSpan Timeout { get; private set; }

		/// <summary>
		/// Gets the response cache.
		/// </summary>
		public IResponseCache Cache { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		public Client(string key) : this(key, new TimeSpan(0, 0, 30), new NullResponseCache()) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="timeout">The timeout.</param>
		public Client(string key, TimeSpan timeout) : this(key, timeout, new NullResponseCache()) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="cache">The response cache.</param>
		public Client(string key, IResponseCache cache) : this(key, new TimeSpan(0, 0, 30), cache) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Client"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="timeout">The timeout.</param>
		/// <param name="cache">The response cache.</param>
		public Client(string key, TimeSpan timeout, IResponseCache cache)
		{
			if (string.IsNullOrEmpty(key))
				throw new ArgumentException("Embedly account key cannot be empty", "key");

			Key = key;
			Timeout = timeout;
			Cache = cache;
		}

		/// <summary>
		/// Gets the providers.
		/// </summary>
		public IEnumerable<Provider> Providers 
		{
			get { return new List<Provider>(Service.Instance.Providers).AsReadOnly(); }
		}

		/// <summary>
		/// Determines whether the specified URL is supported.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns>
		///   <c>true</c> if the specified URL is supported; otherwise, <c>false</c>.
		/// </returns>
		public bool IsUrlSupported(Uri url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			return url.IsAbsoluteUri && Service.Instance.Providers.Any(p => p.IsMatch(url));
		}

		/// <summary>
		/// Gets the provider for the specified URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		public Provider GetProvider(Uri url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			if (!url.IsAbsoluteUri)
				return Provider.Unsupported;

			var result = Service.Instance.Providers.FirstOrDefault(p => p.IsMatch(url));

			return result ?? Provider.Unsupported;
		}
	}
}
