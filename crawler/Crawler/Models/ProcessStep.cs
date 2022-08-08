namespace Crawler.Models;

public enum ProcessStep
{
    Start = 0,
    Site,
    SiteMap,
    PostProcess,
    Transform,
    Persist
}