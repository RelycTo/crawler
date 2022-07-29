namespace Crawler.Tests.Mocks;

internal static class Stubs
{
    public const string HtmlResponseStub =
        @"<html>HTML without some internal anchor tags  <a href='some_path'>Click here<a> <a href='#bookmark2'>Bookmark<a></html>";

    public const string XMLResponseStub =
        @"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"" xmlns:xhtml=""http://www.w3.org/1999/xhtml"" >
            <url>
              <loc>/api/</loc>
              <lastmod>2018-11-28T15:14:39+10:00</lastmod>
            </url>
            <url>
              <loc>/docs/</loc>
              <lastmod>2018-11-28T15:14:39+10:00</lastmod>
            </url>
            <url>
              <loc>/docs/getting-started/</loc>
              <lastmod>2018-11-28T15:14:39+10:00</lastmod>
            </url>
        </urlset>";
}
