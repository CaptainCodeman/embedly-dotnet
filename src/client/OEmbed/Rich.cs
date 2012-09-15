using System.Runtime.Serialization;

namespace Embedly.OEmbed
{
    /// <summary>
    /// Represents an oEmbed rich content response
    /// </summary>
    [DataContract]
    public class Rich : Link
    {
        /// <summary>
        /// Gets the HTML.
        /// </summary>
        [DataMember(Name = "html")]
        public string Html { get; private set; }

        /// <summary>
        /// Gets the width.
        /// </summary>
        [DataMember(Name = "width")]
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        [DataMember(Name = "height")]
        public int Height { get; private set; }
    }
}