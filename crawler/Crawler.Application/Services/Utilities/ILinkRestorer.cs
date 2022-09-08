namespace Crawler.Application.Services.Utilities;

public interface ILinkRestorer
{
    bool IsLinkAcceptable(Uri uri, Uri baseUri);

    string RestoreAbsolutePath(string link, Uri parentUri);
}
