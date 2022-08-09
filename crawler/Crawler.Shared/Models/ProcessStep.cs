namespace Crawler.Shared.Models;

public enum ProcessStep
{
    Start = 0,
    Site,
    SiteMap,
    PostProcess,
    Persist,
    Output
}