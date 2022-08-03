using System.Diagnostics;
using System.Text;
using Crawler.Models;
using Microsoft.Net.Http.Headers;

namespace Crawler.Infrastructure;

public class PageLoader
{
    private readonly HttpClient _client;

    public PageLoader(HttpClient client)
    {
        _client = client;
        _client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "Other");
    }

    public async Task<CrawlResponse> GetResponseAsync(Uri uri, IEnumerable<string> excludedMediaTypes,
        CancellationToken token = default)
    {
        var watcher = new Stopwatch();
        watcher.Start();
        var response = await _client.GetAsync(uri, token);
        watcher.Stop();

        var content = await GetContentAsync(response, excludedMediaTypes, token);
        return new CrawlResponse(uri, content, response.StatusCode, watcher.ElapsedMilliseconds);
    }

    private static async Task<string> GetContentAsync(HttpResponseMessage response,
        IEnumerable<string> excludedMimeTypes, CancellationToken token)
    {
        if (response.Content.Headers.ContentType?.MediaType == null ||
            excludedMimeTypes.Any(t => t == response.Content.Headers.ContentType?.MediaType))
            return string.Empty;
        using var sr = new StreamReader(await response.Content.ReadAsStreamAsync(token),
            Encoding.GetEncoding("iso-8859-1"));
        return await sr.ReadToEndAsync();
    }
}