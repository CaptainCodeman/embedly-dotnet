using System.Runtime.Serialization;

namespace Embedly.OEmbed
{
	/// <summary>
	/// Represents an oEmbed link response
	/// </summary>
	[DataContract]
	[KnownType(typeof(Photo))]
	[KnownType(typeof(Rich))]
	[KnownType(typeof(Video))]
	public class Link : Response
	{
		/// <summary>
		/// Gets the URL.
		/// </summary>
		[DataMember(Name = "url")]
		public string Url { get; private set; }

		/// <summary>
		/// Gets the title.
		/// </summary>
		[DataMember(Name = "title")]
		public string Title { get; private set; }

		/// <summary>
		/// Gets the author.
		/// </summary>
		[DataMember(Name = "author_name")]
		public string Author { get; private set; }

		/// <summary>
		/// Gets the author URL.
		/// </summary>
		[DataMember(Name = "author_url")]
		public string AuthorUrl { get; private set; }

		/// <summary>
		/// Gets the provider.
		/// </summary>
		[DataMember(Name = "provider_name")]
		public string Provider { get; private set; }

		/// <summary>
		/// Gets the provider URL.
		/// </summary>
		[DataMember(Name = "provider_url")]
		public string ProviderUrl { get; private set; }

		/// <summary>
		/// Gets the cache age.
		/// </summary>
		[DataMember(Name = "cache_age")]
		public string CacheAge { get; private set; }

		/// <summary>
		/// Gets the thumbnail URL.
		/// </summary>
		[DataMember(Name = "thumbnail_url")]
		public string ThumbnailUrl { get; private set; }

		/// <summary>
		/// Gets the width of the thumbnail.
		/// </summary>
		[DataMember(Name = "thumbnail_width")]
		public int ThumbnailWidth { get; private set; }

		/// <summary>
		/// Gets the height of the thumbnail.
		/// </summary>
		[DataMember(Name = "thumbnail_height")]
		public int ThumbnailHeight { get; private set; }

		/// <summary>
		/// Gets the description.
		/// </summary>
		[DataMember(Name = "description")]
		public string Description { get; private set; }
	}
}