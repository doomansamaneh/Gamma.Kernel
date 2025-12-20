namespace Gamma.Kernel.Enums;

public enum ChangeAction
{
    Insert = 1,
    Update = 2,
}

public enum SanitizerType
{
    None = -1,
    HtmlFragment = 0,
    Html = 1,
    Xml = 2,
    URL = 3,
    XmlAttribute = 4,
    Css = 5,
    HtmlFormUrl = 6
}

public enum SqlClause
{
    DeleteFrom = 1,
    Update = 2,
    Set = 3,
    Select = 10,
    From = 11,
    Join = 12,
    Where = 13,
    GroupBy = 14,
    Having = 15,
    OrderBy = 16
}
