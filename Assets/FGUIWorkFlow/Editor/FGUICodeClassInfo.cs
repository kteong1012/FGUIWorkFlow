using System.Collections.Generic;

public class FGUICodeClassInfo
{
    public string name;
    public List<FGUICodeFieldInfo> fields = new();
    public string packageName;
    public string componentName;
    public string url;
    public string OutputPath => $"{packageName}/{name}.cs";
}
