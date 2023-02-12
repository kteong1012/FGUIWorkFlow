using FairyGUI;
using MyUnityProject;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class FGUIManager
{
    public static FGUIManager Instance = new FGUIManager();

    private FGUIManager()
    {
        //加载通用资源
        UIPackage.AddPackage("AACommon");
    }

    public void Open<T>() where T : AUIWrap
    {
        var obj = Activator.CreateInstance(typeof(T)) as T;
        UIPackage.AddPackage(obj.PackageName);
        var gobj = UIPackage.CreateObjectFromURL(obj.Url);
        GRoot.inst.AddChild(gobj);
    }
}
