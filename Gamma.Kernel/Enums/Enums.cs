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