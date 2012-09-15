namespace Embedly
{
    /// <summary>
    /// Embedly provider type
    /// </summary>
    /// <remarks>
    /// Can be used to filter requests of a particular type
    /// </remarks>
    public enum ProviderType
    {
        Unsupported,
        Photo,
        Video,
        Link,
        Rich,
        Product,
        Audio
    }
}