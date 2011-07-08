# About embedly-dotnet

embedly-dotnet is a .Net client for [Embed.ly](http://embed.ly/).

Available as a [NuGet Package](http://nuget.org/List/Packages/embedly)

## Features

* High-performance URL matching (without regex's)
* Auto batching of url lookups
* Async downloading with configurable timeout
* Request filtering based on provider properties 

## Examples
See the sample project for complete examples.

### Check if URL supported by Embed.ly
This is used internally to ensure the only Urls sent to embedly are supported ones. 

    Service.Instance.IsUrlSupported(url);

### Retrieve Video Information
This shows a request for a video Url from YouTube.

    var result = Service.Instance.GetOEmbed(new Uri(@"http://www.youtube.com/watch?v=YwSZvHqf9qM"), new RequestOptions { MaxWidth = 320 });
			
    // basic response information
    var response = result.Response;
    Console.WriteLine("Type           : {0}", response.Type);
    Console.WriteLine("Version        : {0}", response.Version);

    // link details
    var link = result.Response.AsLink;
    Console.WriteLine("Author         : {0}", link.Author);
    Console.WriteLine("AuthorUrl      : {0}", link.AuthorUrl);
    Console.WriteLine("CacheAge       : {0}", link.CacheAge);
    Console.WriteLine("Description    : {0}", link.Description);
    Console.WriteLine("Provider       : {0}", link.Provider);
    Console.WriteLine("ProviderUrl    : {0}", link.ProviderUrl);
    Console.WriteLine("ThumbnailHeight: {0}", link.ThumbnailHeight);
    Console.WriteLine("ThumbnailUrl   : {0}", link.ThumbnailUrl);
    Console.WriteLine("ThumbnailWidth : {0}", link.ThumbnailWidth);
    Console.WriteLine("Title          : {0}", link.Title);
    Console.WriteLine("Url            : {0}", link.Url);
    Console.WriteLine();

    // video specific details
    var video = result.Response.AsVideo;
    Console.WriteLine("Width          : {0}", video.Width);
    Console.WriteLine("Height         : {0}", video.Height);
    Console.WriteLine("Html           : {0}", video.Html);
    Console.WriteLine();

## Lookups Urls matching specific provider
A list of urls can be automatically filtered based on any property of the provider. In this case, we're only interested in getting information about YouTube videos ('urls' is `IEnumerable<Uri>`).

    var results = Service.Instance.GetOEmbeds(urls, provider => provider.Name == "youtube", new RequestOptions { MaxWidth = 320 });

## Lookups Urls based on provider type
Similar to the previous example, this expands the lookup to cover all providers of videos.

    var results = Service.Instance.GetOEmbeds(urls, provider => provider.Type == ProviderType.Video, new RequestOptions { MaxWidth = 320 });

## Limitations
The current version supports [oEmbed](http://embed.ly/docs/endpoints/1/oembed) only. Support for the [Preview](http://embed.ly/docs/endpoints/1/preview) and [Objectify](http://embed.ly/docs/endpoints) endpoints is planned for future releases.
