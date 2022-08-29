namespace Crawler.Entities.Models.Enums;

public enum ProcessStep
{
    Start = 0,
    Site,
    SiteMap,
    PostProcess,
    Persist,
    Output
}