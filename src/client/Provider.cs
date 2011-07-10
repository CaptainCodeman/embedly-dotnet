using System;
using System.Linq;
using System.Runtime.Serialization;
using Common.Logging;

namespace Embedly
{
	/// <summary>
	/// Represents an embedly provider
	/// </summary>
	[DataContract]
	public class Provider
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Gets the name.
		/// </summary>
		[DataMember(Name = "name")]
		public string Name { get; private set; }

		/// <summary>
		/// Gets the display name.
		/// </summary>
		[DataMember(Name = "displayname")]
		public string DisplayName { get; private set; }

		/// <summary>
		/// Gets the primary domain.
		/// </summary>
		[DataMember(Name = "domain")]
		public string Domain { get; private set; }

		/// <summary>
		/// Gets the additional subdomains.
		/// </summary>
		[DataMember(Name = "subdomains")]
		public string[] Subdomains { get; private set; }

		/// <summary>
		/// Gets the favicon.
		/// </summary>
		[DataMember(Name = "favicon")]
		public string Favicon { get; private set; }

		/// <summary>
		/// Gets or sets the type value.
		/// </summary>
		/// <value>
		/// The type value.
		/// </value>
		/// <remarks>
		/// This is to work-around limitations of the JSON serialization support for enums as strings
		/// </remarks>
		[DataMember(Name = "type")]
		private string TypeValue
		{
			get { return Type.ToString(); }
			set { Type = (ProviderType)Enum.Parse(typeof(ProviderType), value, true); }
		}

		/// <summary>
		/// Gets the type.
		/// </summary>
		public ProviderType Type { get; private set; }

		private string[] _regexs;

		/// <summary>
		/// Gets or sets the regexs used to identify Urls supported by this provider.
		/// </summary>
		/// <value>
		/// The regexs.
		/// </value>
		[DataMember(Name = "regex")]
		public string[] Regexs
		{
			get { return _regexs; }
			set
			{
				_regexs = value;
				_matches = _regexs.Select(regexValue => regexValue.Split(new[] { '*' }, StringSplitOptions.RemoveEmptyEntries)).ToArray();
			}
		}

		/// <summary>
		/// Gets the about.
		/// </summary>
		[DataMember(Name = "about")]
		public string About { get; private set; }

		private string[][] _matches;

		/// <summary>
		/// Determines whether the specified URL is supported by this provider.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns>
		///   <c>true</c> if the specified URL is supported; otherwise, <c>false</c>.
		/// </returns>
		public bool IsMatch(Uri url)
		{
			var result = _matches.Any(parts => IsMatch(url, parts));
			// Log.Debug(m => m("IsMatch {0}: {1}", Name, result));
			return result;
		}

		/// <summary>
		/// Determines whether the specified URL is supported by this provider.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="parts">The parts.</param>
		/// <returns>
		///   <c>true</c> if the specified URL is supported; otherwise, <c>false</c>.
		/// </returns>
		public bool IsMatch(Uri url, string[] parts)
		{
			var start = 0;
			var index = 0;
			while (index < parts.Length)
			{
				var position = url.AbsoluteUri.IndexOf(parts[index], start, StringComparison.InvariantCultureIgnoreCase);
				if (position == -1)
					return false;
				start = position + parts[index].Length;
				index++;
			}
			return true;
		}

		/// <summary>
		/// Gets a value indicating whether this provider is supported.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this provider is supported; otherwise, <c>false</c>.
		/// </value>
		public bool IsSupported 
		{
			get { return Type != ProviderType.Unsupported; }
		}

		/// <summary>
		/// Returns a provider to represent unsupported Urls.
		/// </summary>
		/// <remarks>
		/// Null object pattern
		/// </remarks>
		internal static Provider Unsupported
		{
			get
			{
				return new Provider
				{
					Name = "unsupported",
					DisplayName = "Unsupported",
					About = string.Empty,
					Domain = string.Empty,
					Subdomains = new string[] {},
					Favicon = string.Empty,
					Regexs = new string[] {},
					Type = ProviderType.Unsupported
				};
			}
		}
	}
}