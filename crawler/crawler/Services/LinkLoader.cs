using System.Diagnostics;
using System.Net;
using System.Text;

namespace crawler.Services;

public class LinkLoader
{
    private readonly ILinkParser _parser;
    private readonly string[] _excludedMimeTypes;
    private readonly HttpClient _client;

    public LinkLoader(HttpClient client, ILinkParser parser, IEnumerable<string> excludedMimeTypes)
    {
        _parser = parser;
        _excludedMimeTypes = excludedMimeTypes?.ToArray() ?? Array.Empty<string>();
        _client = client;
    }

    public async Task<(long duration, IEnumerable<string> links)> GetLinks(string link)
    {
        var watcher = new Stopwatch();
        watcher.Start();
        var response = await _client.GetAsync(link);
        watcher.Stop();
        if (response.StatusCode != HttpStatusCode.OK ||
            _excludedMimeTypes.Any(t => t == response.Content.Headers.ContentType?.MediaType))
            return (-1, new List<string>());

        var bytes = await response.Content.ReadAsByteArrayAsync();
        var responseString = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        return (watcher.ElapsedMilliseconds, _parser.GetLinks(responseString));
    }
}
