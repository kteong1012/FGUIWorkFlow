using System;


[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class FGUIObjectAttribute : Attribute
{
    public string path = null;
    public string name = null;
    public FGUIObjectAttribute(string path = null, string name = null)
    {
        this.name = name;
        this.path = path;
    }
}
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class FGUIComponentRoot : Attribute
{
}
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Field | AttributeTargets.Property)]
public class FGUICustomObjectAttribute : Attribute
{
    public string path = null;
    public string name = null;
    public FGUICustomObjectAttribute(string path = null, string name = null)
    {
        this.name = name;
        this.path = path;
    }
}
