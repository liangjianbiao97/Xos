using System.Collections.Generic;

namespace Xos;

public class XProperty
{
    public string ViewName { get; set; } = string.Empty;
    public Dictionary<string, string> Settings { get; set; } = [];
}