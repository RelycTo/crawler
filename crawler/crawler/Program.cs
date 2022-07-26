// See https://aka.ms/new-console-template for more information

using crawler;
using crawler.Parsers;

var crawler = new Crawler("https://example.com", new HttpClient(), new HtmlParser(), new SiteMapXMLParser());
await crawler.Crawl();