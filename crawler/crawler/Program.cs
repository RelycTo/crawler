// See https://aka.ms/new-console-template for more information

using crawler;

var crawler = new Crawler("https://google.com", new HttpClient());
await crawler.Crawl();