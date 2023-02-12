using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FGUIWorkFlowSettings : ScriptableObject
{
    public string resourceDir;
    public string outputDir;
    public string localOutputDir;
    public List<string> localPackages = new();
    public List<string> commonPackages = new();
}
