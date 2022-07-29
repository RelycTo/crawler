using System.Diagnostics;
using System.Text;
using crawler.Models;

namespace crawler.Services;

public class PageLoader
{
    private readonly HttpClient _client;

    public PageLoader(HttpClient client)
    {
        ExcludedMimeTypes = Array.Empty<string>();
        _client = client;
    }

    public string[] ExcludedMimeTypes { get; private set; }

    public async Task<CrawlResponse> GetResponseAsync(Uri uri, CancellationToken token)
    {
        var watcher = new Stopwatch();
        watcher.Start();
        var response = await _client.GetAsync(uri, token);
        watcher.Stop();

        var content = await GetContentAsync(response, ExcludedMimeTypes, token);
        return new CrawlResponse(uri, content, response.StatusCode, watcher.ElapsedMilliseconds);
    }

    public PageLoader SetExcludedMediaTypes(IEnumerable<string> excludedMimeTypes)
    {
        ExcludedMimeTypes = excludedMimeTypes.ToArray();
        return this;
    }

    private static async Task<string> GetContentAsync(HttpResponseMessage response, IEnumerable<string> excludedMimeTypes, CancellationToken token)
    {
        if (response.Content.Headers.ContentType?.MediaType == null ||
            excludedMimeTypes.Any(t => t == response.Content.Headers.ContentType?.MediaType))
            return string.Empty;
        using var sr = new StreamReader(await response.Content.ReadAsStreamAsync(token),
            Encoding.GetEncoding("iso-8859-1"));
        return await sr.ReadToEndAsync();
    }
}