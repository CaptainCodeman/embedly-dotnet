using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using Embedly.Caching;
using Embedly.OEmbed;

namespace Embedly.Sample
{
	class Program
	{
		static void Main(string[] args)
		{
			var key = ConfigurationManager.AppSettings["embedly.key"];
			var database = ConfigurationManager.ConnectionStrings["embedly.cache"];

			// using the in memory Cache
			var cache = new InMemoryResponseCache(new TimeSpan(24, 0, 0));

			// using the Ado Cache (e.g. SQL Server)
			/*
			var factory = DbProviderFactories.GetFactory(database.ProviderName);
			var cache = new AdoResponseCache(factory, database.ConnectionString);
			*/
			
			// using the MongoDB Cache
			// var cache = new MongoResponseCache(database.ConnectionString);
			
			try
			{
				var client = new Client(key, cache);

                Providers(client);
                ProviderInformation(client);
                ProviderPerUrl(client);
                SingleRich(client);
                SingleVideo(client);
                MultipleFilterByProvider(client);
                MultipleFilterByType(client);
                MultipleAll(client);
				UrlNotFound(client);
			}
			catch (ArgumentException)
			{
				Console.WriteLine("Enter your embedly account key in the config file");
			}

			Console.WriteLine();
			Console.WriteLine("Finished");
			Console.ReadLine();
		}

		private static void Providers(Client client)
		{
			Console.WriteLine("Providers");

			foreach (var provider in client.Providers)
			{
				Console.WriteLine("{0} {1}", provider.Type, provider.Name);
				foreach (var regex in provider.Regexs)
				{
					Console.WriteLine("  {0}", regex);
				}
			}
			Console.WriteLine();
		}

		private static void ProviderInformation(Client client)
		{
			Console.WriteLine("ProviderInformation");

			var url = TestVideoUrl();

			Console.WriteLine("Url {0}", url);
			Console.WriteLine();

			var supported = client.IsUrlSupported(url);
			Console.WriteLine("Supported      : {0}", supported);
			Console.WriteLine();

			var provider = client.GetProvider(url);
			Console.WriteLine("PROVIDER");
			Console.WriteLine("About          : {0}", provider.About);
			Console.WriteLine("DisplayName    : {0}", provider.DisplayName);
			Console.WriteLine("Domain         : {0}", provider.Domain);
			Console.WriteLine("Favicon        : {0}", provider.Favicon);
			Console.WriteLine("Name           : {0}", provider.Name);
			Console.WriteLine("Regexs         : {0}", string.Join(", ", provider.Regexs));
			Console.WriteLine("Subdomains     : {0}", string.Join(", ", provider.Subdomains));
			Console.WriteLine("Types          : {0}", provider.Type);
			Console.WriteLine();
		}

		private static void SingleRich(Client client)
		{
			Console.WriteLine("SingleRich");

			var url = TestRichUrl();

			var result = client.GetOEmbed(url, new RequestOptions { MaxWidth = 320 });

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
			var rich = result.Response.AsRich;
			Console.WriteLine("Width          : {0}", rich.Width);
			Console.WriteLine("Height         : {0}", rich.Height);
			Console.WriteLine("Html           : {0}", rich.Html);
			Console.WriteLine();
		}

		private static void UrlNotFound(Client client)
		{
			Console.WriteLine("UrlNotFound");

			var url = new Uri(@"http://vimeo.com/12345678");

			var result = client.GetOEmbed(url, new RequestOptions {MaxWidth = 320});
		}

		private static void SingleVideo(Client client)
		{
			Console.WriteLine("SingleVideo");

			var url = TestVideoUrl();

			var result = client.GetOEmbed(url, new RequestOptions { MaxWidth = 320 });
			
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
		}

		private static void ProviderPerUrl(Client client)
		{
			Console.WriteLine("ProviderPerUrl");
			var urls = TestUrls();
			foreach (var url in urls)
			{
				var provider = client.GetProvider(url);
				Console.WriteLine("{0} handles {1}", provider.Name, url);
			}
			Console.WriteLine();
		}

		private static void MultipleAll(Client client)
		{
			Console.WriteLine("Multiple");
			var urls = TestUrls();
			var results = client.GetOEmbeds(urls);
			DisplayResults(results);
			Console.WriteLine();
		}

		private static void SupportedOnly(Client client)
		{
			Console.WriteLine("SupportedOnly");
			var urls = TestUrls();
			var results = client.GetOEmbeds(urls, provider => provider.IsSupported);
			DisplayResults(results);
			Console.WriteLine();
		}

		private static void MultipleFilterByProvider(Client client)
		{
			Console.WriteLine("MultipleFilterByProvider");
			var urls = TestUrls();
			var results = client.GetOEmbeds(urls, provider => provider.Name == "youtube", new RequestOptions { MaxWidth = 320 });
			DisplayResults(results);
			Console.WriteLine();
		}

		private static void MultipleFilterByType(Client client)
		{
			Console.WriteLine("MultipleFilterByType");
			var urls = TestUrls();
			var results = client.GetOEmbeds(urls, provider => provider.Type == ProviderType.Product, new RequestOptions { MaxWidth = 320 });
			DisplayResults(results);
			Console.WriteLine();
		}

		private static void DisplayResults(IEnumerable<Result> results)
		{
			foreach (var result in results.Successful())
			{
				if (result.Exception == null)
				{
					Console.WriteLine("{0} found for {1} ({2})", result.Response.Type, result.Request.Url, result.Request.Provider.Name);
					switch (result.Response.Type)
					{
						case ResourceType.Error:
							var error = result.Response.AsError;
							Console.WriteLine("  code:{0} message:{1}", error.ErrorCode, error.ErrorMessage);
							break;
						case ResourceType.Link:
							var link = result.Response.AsLink;
							Console.WriteLine("  title:{0}", link.Title);
							Console.WriteLine("  url:{0}", link.Url);
							break;
						case ResourceType.Photo:
							var photo = result.Response.AsPhoto;
							Console.WriteLine("  title:{0} ({1}x{2})", photo.Title, photo.Width, photo.Height);
							Console.WriteLine("  url:{0}", photo.Url);
							break;
						case ResourceType.Rich:
							var rich = result.Response.AsRich;
							Console.WriteLine("  title:{0} ({1}x{2})", rich.Title, rich.Width, rich.Height);
							Console.WriteLine("  url:{0}", rich.Url);
							break;
						case ResourceType.Video:
							var video = result.Response.AsVideo;
							Console.WriteLine("  title:{0} ({1}x{2})", video.Title, video.Width, video.Height);
							Console.WriteLine("  url:{0}", video.Url);
							break;
					}
				}
				else
				{
					Console.WriteLine("Exception requesting {0} : {1}", result.Request.Url, result.Exception);				
				}
			}
		}

		public static IEnumerable<Uri> TestUrls()
		{
			// example URLs from https://gist.github.com/356104

			var urls = new[] {
				// Domain - http://slideshare.net
				"http://www.slideshare.net/doina/happy-easter-from-holland-slideshare",
				"http://www.slideshare.net/stinson/easter-1284190",
				"http://www.slideshare.net/angelspascual/easter-events",
				"http://www.slideshare.net/sirrods/happy-easter-3626014",
				"http://www.slideshare.net/sirrods/happy-easter-wide-screen",
				"http://www.slideshare.net/carmen_serbanescu/easter-holiday",
				"http://www.slideshare.net/Lithuaniabook/easter-1255880",
				"http://www.slideshare.net/hues/easter-plants",
				"http://www.slideshare.net/Gospelman/passover-week",
				"http://www.slideshare.net/angelspascual/easter-around-the-world-1327542",

				// Domain - http://scribd.com
				"http://www.scribd.com/doc/13994900/Easter",
				"http://www.scribd.com/doc/27425714/Celebrating-Easter-ideas-for-adults-and-children",
				"http://www.scribd.com/doc/28010101/Easter-Foods-No-Name",
				"http://www.scribd.com/doc/28452730/Easter-Cards",
				"http://www.scribd.com/doc/19026714/The-Easter-Season",
				"http://www.scribd.com/doc/29183659/History-of-Easter",
				"http://www.scribd.com/doc/15632842/The-Last-Easter",
				"http://www.scribd.com/doc/28741860/The-Plain-Truth-About-Easter",
				"http://www.scribd.com/doc/23616250/4-27-08-ITS-EASTER-AGAIN-ORTHODOX-EASTER-by-vanderKOK",

				// Domain - http://screenr.com",
				"http://screenr.com/t9d",
				"http://screenr.com/yLS",
				"http://screenr.com/gzS",
				"http://screenr.com/IwU",
				"http://screenr.com/FM7",
				"http://screenr.com/Ejg",
				"http://screenr.com/u4h",
				"http://screenr.com/QiN",
				"http://screenr.com/zts",

				// Domain - http://5min.com
				"http://www.5min.com/Video/How-to-Decorate-Easter-Eggs-with-Decoupage-142076462",
				"http://www.5min.com/Video/How-to-Color-Easter-Eggs-Dye-142076281",
				"http://www.5min.com/Video/How-to-Make-an-Easter-Egg-Diorama-142076482",
				"http://www.5min.com/Video/How-to-Make-Sequined-Easter-Eggs-142076512",
				"http://www.5min.com/Video/How-to-Decorate-Wooden-Easter-Eggs-142076558",
				"http://www.5min.com/Video/How-to-Blow-out-an-Easter-Egg-142076367",
				"http://www.5min.com/Video/Learn-About-Easter-38363995",

				// Domain - http://www.howcast.com
				"http://www.howcast.com/videos/328008-How-To-Marble-Easter-Eggs",
				"http://www.howcast.com/videos/220110-The-Meaning-Of-Easter",


				// Domain - http://my.opera.com
				"http://my.opera.com/nirvanka/albums/show.dml?id=519866",

				// Category - photos

				// Domain - http://yfrog.com
				"http://img402.yfrog.com/i/mfe.jpg/",
				"http://img20.yfrog.com/i/dy6.jpg/",
				"http://img145.yfrog.com/i/4mu.mp4/",
				"http://img15.yfrog.com/i/mygreatmovie.mp4/",
				"http://img159.yfrog.com/i/500x5000401.jpg/",

				// Domain - http://tweetphoto.com
				"http://tweetphoto.com/14784358",
				"http://tweetphoto.com/16044847",
				"http://tweetphoto.com/16718883",
				"http://tweetphoto.com/16451148",
				"http://tweetphoto.com/16133984",
				"http://tweetphoto.com/8069529",
				"http://tweetphoto.com/16207556",
				"http://tweetphoto.com/7448361",
				"http://tweetphoto.com/16069325",
				"http://tweetphoto.com/4791033",

				// Domain - http://flickr.com
				"http://www.flickr.com/photos/10349896@N08/4490293418/",
				"http://www.flickr.com/photos/mneylon/4483279051/",
				"http://www.flickr.com/photos/xstartxtodayx/4488996521/",
				"http://www.flickr.com/photos/mommyknows/4485313917/",
				"http://www.flickr.com/photos/29988430@N06/4487127638/",
				"http://www.flickr.com/photos/excomedia/4484159563/",
				"http://www.flickr.com/photos/sunnybrook100/4471526636/",
				"http://www.flickr.com/photos/jaimewalsh/4489497178/",
				"http://www.flickr.com/photos/29988430@N06/4486475549/",
				"http://www.flickr.com/photos/22695183@N08/4488681694/",

				// Domain - http://twitpic.com
				"http://twitpic.com/1cnsf6",
				"http://twitpic.com/1cgtti",
				"http://twitpic.com/1coc0n",
				"http://twitpic.com/1cm8us",
				"http://twitpic.com/1cia31",
				"http://twitpic.com/1cgks4",

				// Domain - http://imgur.com
				"http://imgur.com/6pLoN",

				// Domain - http://posterous.com
				"http://onegoodpenguin.posterous.com/golden-tee-live-2010-easter-egg",
				"http://post.ly/Zhg0",
				"http://apartmentliving.posterous.com/biggest-easter-egg-hunts-in-the-dc-area",

				// Domain - http://twitgoo.com
				"http://twitgoo.com/1as",
				"http://twitgoo.com/1p94",
				"http://twitgoo.com/4kg2",
				"http://twitgoo.com/6c9",
				"http://twitgoo.com/1w5",
				"http://twitgoo.com/6mu",
				"http://twitgoo.com/1w3",
				"http://twitgoo.com/1om",
				"http://twitgoo.com/1mh",

				// Domain - http://photobucket.com

				// Domain - http://phodroid.com

				// Domain - http://xkcd.com

				// Domain - http://asofterword.com

				// Domain - http://qwantz.com
				"http://www.qwantz.com/index.php?comic=1686",
				"http://www.qwantz.com/index.php?comic=773",
				"http://www.qwantz.com/index.php?comic=1018",
				"http://www.qwantz.com/index.php?comic=1019",

				// Domain - http://www.23hq.com
				"http://www.23hq.com/mhg/photo/5498347",
				"http://www.23hq.com/Greetingdesignstudio/photo/5464607",
				"http://www.23hq.com/Greetingdesignstudio/photo/5464590",
				"http://www.23hq.com/Greetingdesignstudio/photo/5464605",
				"http://www.23hq.com/Greetingdesignstudio/photo/5464604",
				"http://www.23hq.com/dvilles2/photo/5443192",
				"http://www.23hq.com/Greetingdesignstudio/photo/5464606",

				// Category - products

				// Domain - "http://amazon.com

				"http://www.amazon.com/Blood-On-The-Tracks/dp/B00138H876/ref=sr_1_5?ie=UTF8&qid=1310003888&sr=8-5",
				"http://www.amazon.com/Bob-Dylan-Concert-Brandeis-University/dp/B004RSCTXC/ref=sr_1_15?ie=UTF8&qid=1310003888&sr=8-15",

				// Category - user

				// Domain - http://youtube.com
				"http://www.youtube.com/watch?v=gghKdx558Qg",
				"http://www.youtube.com/watch?v=yPid9BLQQcg",
				"http://www.youtube.com/watch?v=uEo2vboUYUk",
				"http://www.youtube.com/watch?v=geUhtoHbLu4",
				"http://www.youtube.com/watch?v=Zk7dDekYej0",
				"http://www.youtube.com/watch?v=Q3tgMosx_tI",
				"http://www.youtube.com/watch?v=s9P8_vgmLfs",
				"http://www.youtube.com/watch?v=1cmtN1meMmk",
				"http://www.youtube.com/watch?v=AVzj-U5Ihm0",

				// Domain - http://veoh.com
				"http://www.veoh.com/collection/easycookvideos/watch/v366931kcdgj7Hd",
				"http://www.veoh.com/collection/easycookvideos/watch/v366991zjpANrqc",
				"http://www.veoh.com/browse/videos/category/educational/watch/v7054535EZGFJqyX",
				"http://www.veoh.com/browse/videos/category/lifestyle/watch/v18155013XBBtnYwq",

				// Domain - http://justin.tv
				"http://www.justin.tv/easter7presents",
				"http://www.justin.tv/easterfraud",
				"http://www.justin.tv/cccog27909",
				"http://www.justin.tv/clip/6e8c18f7050",
				"http://www.justin.tv/venom24",

				// Domain - http://ustream.com

				// Domain - http://qik.com
				"http://qik.com/video/1622287",
				"http://qik.com/video/1503735",
				"http://qik.com/video/40504",
				"http://qik.com/video/1445763",
				"http://qik.com/video/743285",
				"http://qik.com/video/1445299",
				"http://qik.com/video/1443200",
				"http://qik.com/video/1445889",
				"http://qik.com/video/174242",
				"http://qik.com/video/1444897",

				// Domain - http://revision3.com
				"http://revision3.com/hak5/DualCore",
				"http://revision3.com/popsiren/charm",
				"http://revision3.com/tekzilla/eyefinity",
				"http://revision3.com/diggnation/2005-10-06",
				"http://revision3.com/hak5/netcat-virtualization-wordpress/",
				"http://revision3.com/infected/forsaken",
				"http://revision3.com/hak5/purepwnage",
				"http://revision3.com/tekzilla/wowheadset",

				// Domain - http://dailymotion.com
				"http://www.dailymotion.com/video/xcstzd_greek-wallets-tighten-during-easter_news",
				"http://www.dailymotion.com/video/xcso4y_exclusive-easter-eggs-easter-basket_lifestyle",
				"http://www.dailymotion.com/video/x2sgkt_evil-easter-bunny",
				"http://www.dailymotion.com/video/xco7oc_invitation-to-2010-easter-services_news",
				"http://www.dailymotion.com/video/xcss6b_big-cat-easter_animals",
				"http://www.dailymotion.com/video/xcszw1_easter-bunny-visits-buenos-aires-zo_news",
				"http://www.dailymotion.com/video/xcsfvs_forecasters-warn-of-easter-misery_news",

				// Domain - http://collegehumor.com
				"http://www.collegehumor.com/video:1682246",

				// Domain - http://twitvid.com
				"http://www.twitvid.com/D9997",
				"http://www.twitvid.com/902B9",
				"http://www.twitvid.com/C33F8",
				"http://www.twitvid.com/63F73",
				"http://www.twitvid.com/BC0BA",
				"http://www.twitvid.com/1C33C",
				"http://www.twitvid.com/8A8E2",
				"http://www.twitvid.com/51035",
				"http://www.twitvid.com/5C733",

				// Domain - http://break.com
				"http://www.break.com/game-trailers/game/just-cause-2/just-cause-2-lost-easter-egg?res=1",
				"http://www.break.com/usercontent/2010/3/10/easter-holiday-2009-slideshow-1775624",
				"http://www.break.com/index/a-very-sexy-easter-video.html",
				"http://www.break.com/usercontent/2010/3/11/this-video-features-gizzi-erskine-making-easter-cookies-1776089",
				"http://www.break.com/usercontent/2007/4/4/happy-easter-265717",
				"http://www.break.com/usercontent/2007/4/17/extreme-easter-egg-hunting-276064",
				"http://www.break.com/usercontent/2006/11/18/the-evil-easter-bunny-184789",
				"http://www.break.com/usercontent/2006/4/16/hoppy-easter-kitty-91040",

				// Domain - http://vids.myspace.com
				"http://vids.myspace.com/index.cfm?fuseaction=vids.individual&videoid=104063637",
				"http://vids.myspace.com/index.cfm?fuseaction=vids.individual&VideoID=103920954",
				"http://vids.myspace.com/index.cfm?fuseaction=vids.individual&videoid=103928002",
				"http://vids.myspace.com/index.cfm?fuseaction=vids.individual&videoid=103999188",
				"http://vids.myspace.com/index.cfm?fuseaction=vids.individual&videoid=103920940",
				"http://vids.myspace.com/index.cfm?fuseaction=vids.individual&videoid=104004673",
				"http://vids.myspace.com/index.cfm?fuseaction=vids.individual&videoid=104046456",

				// Domain - http://metacafe.com
				"http://www.metacafe.com/watch/105023/the_easter_bunny/",
				"http://www.metacafe.com/watch/4376131/easter_lay/",
				"http://www.metacafe.com/watch/2245996/how_to_make_ukraine_easter_eggs/",
				"http://www.metacafe.com/watch/4374339/easter_eggs/",
				"http://www.metacafe.com/watch/2605860/filled_easter_baskets/",
				"http://www.metacafe.com/watch/2372088/easter_eggs/",
				"http://www.metacafe.com/watch/3043671/www_goodnews_ws_easter_island/",
				"http://www.metacafe.com/watch/1652057/easter_eggs/",
				"http://www.metacafe.com/watch/1173632/ultra_kawaii_easter_bunny_party/",

				// Domain - http://blip.tv
				"http://celluloidremix.blip.tv/file/3378272/",
				"http://blip.tv/file/449469",
				"http://blip.tv/file/199776",
				"http://blip.tv/file/766967",
				"http://blip.tv/file/770127",
				"http://blip.tv/file/854925",
				"http://www.blip.tv/file/22695?filename=Uncle_dale-THEEASTERBUNNYHATESYOU395.flv",
				"http://iofa.blip.tv/file/3412333/",
				"http://blip.tv/file/190393",
				"http://blip.tv/file/83152",

				// Domain - http://video.google.com
				"http://video.google.com/videoplay?docid=-5427138374898988918&q=easter+bunny&pl=true",
				"http://video.google.com/videoplay?docid=7785441737970480237",
				"http://video.google.com/videoplay?docid=2320995867449957036",
				"http://video.google.com/videoplay?docid=-2586684490991458032&q=peeps&pl=true",
				"http://video.google.com/videoplay?docid=5621139047118918034",
				"http://video.google.com/videoplay?docid=4232304376070958848",
				"http://video.google.com/videoplay?docid=-6612726032157145299",
				"http://video.google.com/videoplay?docid=4478549130377875994&hl=en",
				"http://video.google.com/videoplay?docid=9169278170240080877",
				"http://video.google.com/videoplay?docid=2551240967354893096",

				// Domain - http://revver.com
				"http://revver.com/video/263817/happy-easter/",
				"http://www.revver.com/video/1574939/easter-bunny-house/",
				"http://revver.com/video/771140/easter-08/",

				// Domain - http://video.yahoo.com
				"http://video.yahoo.com/watch/7268801/18963438",
				"http://video.yahoo.com/watch/2224892/7014048",
				"http://video.yahoo.com/watch/7244748/18886014",
				"http://video.yahoo.com/watch/4656845/12448951",
				"http://video.yahoo.com/watch/363942/2249254",
				"http://video.yahoo.com/watch/2232968/7046348",
				"http://video.yahoo.com/watch/4530253/12135472",
				"http://video.yahoo.com/watch/2237137/7062908",
				"http://video.yahoo.com/watch/952841/3706424",

				// Domain - http://viddler.com
				"http://www.viddler.com/explore/BigAppleChannel/videos/113/",
				"http://www.viddler.com/explore/cheezburger/videos/379/",
				"http://www.viddler.com/explore/warnerbros/videos/350/",
				"http://www.viddler.com/explore/tvcgroup/videos/169/",
				"http://www.viddler.com/explore/thebrickshow/videos/12/",

				// Domain - http://www.liveleak.com
				"http://www.liveleak.com/view?i=e0b_1239827917",
				"http://www.liveleak.com/view?i=715_1239490211",
				"http://www.liveleak.com/view?i=d30_1206233786&p=1",
				"http://www.liveleak.com/view?i=d91_1239548947",
				"http://www.liveleak.com/view?i=f58_1190741182",
				"http://www.liveleak.com/view?i=44e_1179885621&c=1",
				"http://www.liveleak.com/view?i=451_1188059885",
				"http://www.liveleak.com/view?i=3f5_1267456341&c=1",

				// Category - popculture

				// Domain - http://hulu.com
				"http://www.hulu.com/watch/67313/howcast-how-to-make-braided-easter-bread",
				"http://www.hulu.com/watch/133583/access-hollywood-glees-matthew-morrison-on-touring-and-performing-for-president-obama",
				"http://www.hulu.com/watch/66319/saturday-night-live-easter-album",
				"http://www.hulu.com/watch/80229/explorer-end-of-easter-island",
				"http://www.hulu.com/watch/139020/nbc-today-show-lamb-and-ham-create-easter-feast",
				"http://www.hulu.com/watch/84272/rex-the-runt-easter-island",
				"http://www.hulu.com/watch/132203/everyday-italian-easter-pie",
				"http://www.hulu.com/watch/23349/nova-secrets-of-lost-empires-ii-easter-island",

				// Domain - http://movieclips.com
				"http://movieclips.com/watch/dirty_harry_1971/do_you_feel_lucky_punk/",
				"http://movieclips.com/watch/napoleon_dynamite_2004/chatting_online_with_babes/",
				"http://movieclips.com/watch/dumb__dumber_1994/the_toilet_doesnt_flush/",
				"http://movieclips.com/watch/jaws_1975/youre_gonna_need_a_bigger_boat/",
				"http://movieclips.com/watch/napoleon_dynamite_2004/chatting_online_with_babes/61.495/75.413",
				"http://movieclips.com/watch/super_troopers_2001/the_cat_game/12.838/93.018",
				"http://movieclips.com/watch/this_is_spinal_tap_1984/these_go_to_eleven/79.703/129.713",

				// Domain - http://crackle.com
				"http://crackle.com/c/Originals/What_s_the_deal_with_Easter_candy_/2303243",
				"http://crackle.com/c/How_To/Dryer_Lint_Easter_Bunny_Trailer_Park_Craft/2223902",
				"http://crackle.com/c/How_To/Pagan_Origin_of_Easter_Easter_Egg_Rabbit_Playb_/2225124",
				"http://crackle.com/c/Funny/Happy_Easter/2225363",
				"http://crackle.com/c/Funny/Crazy_and_Hilarious_Easter_Egg_Hunt/2225737",
				"http://crackle.com/c/How_To/Learn_About_Greek_Orthodox_Easter/2262294",
				"http://crackle.com/c/How_To/How_to_Make_Ukraine_Easter_Eggs/2262274",
				"http://crackle.com/c/How_To/Symbolism_Of_Ukrainian_Easter_Eggs/2262267",
				"http://crackle.com/c/Funny/Easter_Retard/931976",

				// Domain - http://fancast.com
				"http://www.fancast.com/tv/It-s-the-Easter-Beagle,-Charlie-Brown/74789/1078053475/Peanuts:-Specials:-It-s-the-Easter-Beagle,-Charlie-Brown/videos",
				"http://www.fancast.com/movies/Easter-Parade/97802/687440525/Easter-Parade/videos",
				"http://www.fancast.com/tv/Saturday-Night-Live/10009/1083396482/Easter-Album/videos",
				"http://www.fancast.com/movies/The-Proposal/147176/1140660489/The-Proposal:-Easter-Egg-Hunt/videos",

				// Domain - http://funnyordie.com
				"http://www.funnyordie.com/videos/f6883f54ae/the-unsettling-ritualistic-origin-of-the-easter-bunny",
				"http://www.funnyordie.com/videos/3ccb03863e/easter-tail-keaster-bunny",
				"http://www.funnyordie.com/videos/17b1d36ad0/easter-bunny-from-leatherfink",
				"http://www.funnyordie.com/videos/0c55aa116d/easter-exposed-from-bryan-erwin",
				"http://www.funnyordie.com/videos/040dac4eff/easter-eggs",

				// Domain - http://vimeo.com
				"http://vimeo.com/10446922",
				"http://vimeo.com/10642542",
				"http://www.vimeo.com/10664068",
				"http://vimeo.com/819176",
				"http://www.vimeo.com/10525353",
				"http://vimeo.com/10429123",
				"http://www.vimeo.com/10652053",
				"http://vimeo.com/10572216",

				// Domain - http://ted.com
				"http://www.ted.com/talks/jared_diamond_on_why_societies_collapse.html",
				"http://www.ted.com/talks/nathan_myhrvold_on_archeology_animal_photography_bbq.html",
				"http://www.ted.com/talks/johnny_lee_demos_wii_remote_hacks.html",
				"http://www.ted.com/talks/robert_ballard_on_exploring_the_oceans.html",

				// Domain - http://omnisio.com
				"http://www.omnisio.com/v/Z3QxbTUdjhG/wall-e-collection-of-videos",
				"http://www.omnisio.com/v/3ND6LTvdjhG/php-tutorial-4-login-form-updated",

				// Domain - http://nfb.ca

				// Domain - http://www.thedailyshow.com
				"http://www.thedailyshow.com/watch/thu-december-14-2000/intro---easter",
				"http://www.thedailyshow.com/watch/tue-april-18-2006/headlines---easter-charade",
				"http://www.thedailyshow.com/watch/tue-april-18-2006/egg-beaters",
				"http://www.thedailyshow.com/watch/tue-april-18-2006/moment-of-zen---scuba-diver-hiding-easter-eggs",
				"http://www.thedailyshow.com/watch/tue-april-7-2009/easter---passover-highlights",
				"http://www.thedailyshow.com/watch/tue-february-29-2000/headlines---leap-impact",
				"http://www.thedailyshow.com/watch/thu-march-1-2007/tomb-with-a-jew",
				"http://www.thedailyshow.com/watch/mon-april-24-2000/the-meaning-of-passover",

				// Domain - http://movies.yahoo.com

				// Domain - http://www.colbertnation.com
				"http://www.colbertnation.com/the-colbert-report-videos/268800/march-31-2010/easter-under-attack---peeps-display-update",
				"http://www.colbertnation.com/the-colbert-report-videos/268797/march-31-2010/intro---03-31-10",
				"http://www.colbertnation.com/full-episodes/wed-march-31-2010-craig-mullaney",
				"http://www.colbertnation.com/the-colbert-report-videos/60902/march-28-2006/the-word---easter-under-attack---marketing",
				"http://www.colbertnation.com/the-colbert-report-videos/83362/march-07-2007/easter-under-attack---bunny",
				"http://www.colbertnation.com/the-colbert-report-videos/61404/april-06-2006/easter-under-attack---recalled-eggs?videoId=61404",
				"http://www.colbertnation.com/the-colbert-report-videos/223957/april-06-2009/colbert-s-easter-parade",
				"http://www.colbertnation.com/the-colbert-report-videos/181772/march-28-2006/intro---3-28-06",

				// Domain - http://wordpress.tv

				// Domain - http://www.traileraddict.com
				"http://www.traileraddict.com/trailer/despicable-me/easter-greeting",
				"http://www.traileraddict.com/trailer/easter-parade/trailer",
				"http://www.traileraddict.com/clip/the-proposal/easter-egg-hunt",
				"http://www.traileraddict.com/trailer/despicable-me/international-teaser-trailer",
				"http://www.traileraddict.com/trailer/despicable-me/today-show-minions",

				// Category - audio

				// Domain - http://www.lala.com
				"http://www.lala.com/#album/432627041169206995/Rihanna/Rated_R",
				"http://www.lala.com/#album/432627041169204967/Lady_GaGa/Bad_Romance",

				// Unsupported
				"http://news.cnet.com/8301-13579_3-20077402-37/apple-loses-bid-for-injunction-against-amazon/?tag=topTechContentWrap;editorPicks"
			};

			return urls.Select(url => new Uri(url));
		}

		public static Uri TestRichUrl()
		{
			return new Uri(@"http://www.amazon.com/Times-They-Are--Changin/dp/B0009MAP9A/ref=sr_1_34?ie=UTF8&qid=1310348558&sr=8-34");
		}

		public static Uri TestVideoUrl()
		{
			return new Uri(@"http://www.youtube.com/watch?v=YwSZvHqf9qM");
		}
	}
}
