# About embedly-dotnet

embedly-dotnet is a .Net client for [Embed.ly](http://embed.ly/).

Available as a [NuGet Package](http://nuget.org/List/Packages/embedly)

## Features

* High-performance URL matching (without regex's)
* Auto batching of url lookups
* Async downloading with configurable timeout
* Request filtering based on provider properties 
* Caching of embedly responses to prevent re-requesting urls

## Examples
See the sample project for complete examples.

### Create an Embed.ly client
The client handles all interaction with embedly and requires a key to use. No caching will be performed by default.

    var key = ConfigurationManager.AppSettings["embedly.key"];
    var client = new Client(key);

### Use a Cache
A cache (implementing a simple `IResponseCache` interface) can be passed to the `Client` constructor and ensures any requests to previously requested urls are served from the cache instead of being re-requested.

    var key = ConfigurationManager.AppSettings["embedly.key"];
    var database = ConfigurationManager.ConnectionStrings["embedly.cache"];
    var cache = new MongoResponseCache(database.ConnectionString);
    var client = new Client(key, cache);

Example `InMemoryResponseCache`, `MongoResponseCache` (MongoDB) and `AdoResponseCache` (e.g. SQL Server) implementation are included.

### Check if URL supported by Embed.ly
This is used internally to ensure the only Urls sent to embedly are supported ones. 

    client.IsUrlSupported(url);

### Retrieve Video Information
This shows a request for a video Url from YouTube.

    var result = client.GetOEmbed(new Uri(@"http://www.youtube.com/watch?v=YwSZvHqf9qM"), new RequestOptions { MaxWidth = 320 });
			
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

    var results = client.GetOEmbeds(urls, provider => provider.Name == "youtube", new RequestOptions { MaxWidth = 320 });

## Lookups Urls based on provider type
Similar to the previous example, this expands the lookup to cover all providers of videos.

    var results = client.GetOEmbeds(urls, provider => provider.Type == ProviderType.Video, new RequestOptions { MaxWidth = 320 });

## Limitations
The current version supports [oEmbed](http://embed.ly/docs/endpoints/1/oembed).
