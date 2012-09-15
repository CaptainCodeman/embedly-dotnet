using System;
using System.Runtime.Serialization;

namespace Embedly.OEmbed
{
    /// <summary>
    /// Represents a base oEmbed response primarily used to identify the type
    /// </summary>
    [DataContract]
    [KnownType(typeof (Error))]
    [KnownType(typeof (Link))]
    public class Response
    {
        /// <summary>
        /// Gets or sets the type value.
        /// </summary>
        /// <value>
        /// The type value.
        /// </value>
        [DataMember(Name = "type")]
        private string TypeValue
        {
            get { return Type.ToString(); }
            set { Type = (ResourceType)Enum.Parse(typeof (ResourceType), value, true); }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        [IgnoreDataMember]
        public ResourceType Type { get; private set; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        [DataMember(Name = "version")]
        public string Version { get; private set; }

        /// <summary>
        /// Gets as error.
        /// </summary>
        [IgnoreDataMember]
        public Error AsError
        {
            get { return this as Error; }
        }

        /// <summary>
        /// Gets as a link.
        /// </summary>
        [IgnoreDataMember]
        public Link AsLink
        {
            get { return this as Link; }
        }

        /// <summary>
        /// Gets as a photo.
        /// </summary>
        [IgnoreDataMember]
        public Photo AsPhoto
        {
            get { return this as Photo; }
        }

        /// <summary>
        /// Gets as rich content.
        /// </summary>
        [IgnoreDataMember]
        public Rich AsRich
        {
            get { return this as Rich; }
        }

        /// <summary>
        /// Gets as a video.
        /// </summary>
        [IgnoreDataMember]
        public Video AsVideo
        {
            get { return this as Video; }
        }
    }
}