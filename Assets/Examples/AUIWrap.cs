﻿using FairyGUI;
using System.Reflection;
using System;
using UnityEngine;

public abstract class AUIWrap
{
    public GComponent Root { get; set; }
    public abstract string PackageName { get; }
    public abstract string Componentname { get; }
    public abstract string Url { get; }

    public void Init(GComponent root)
    {
        BindRoot(this.GetType(), this, root);
        AddButtonCallbacks();
        OnInit();
    }
    public void Destroy()
    {
        OnDestroy();
        Root?.Dispose();
    }
    public void Show()
    {
        OnShow();
    }
    public void Hide()
    {
        OnHide();
    }
    protected virtual void AddButtonCallbacks() { }
    protected virtual void OnInit() { }
    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
    protected virtual void OnDestroy() { }

    protected void SetButtonEvent(GButton button, Action callback)
    {
        button.onClick.Set(() => callback?.Invoke());
    }




    //注册FGUI元件（也可以理解为绑定FGUI节点）
    private void BindRoot(Type type, object bindObj, GComponent gComponent)
    {
        //遍历type对象的字段
        foreach (var propertyInfo in type.GetProperties())
        {
            var rootObj = propertyInfo.GetCustomAttribute<FGUIComponentRoot>(false);   //获取FGUIComponentRoot标签的对象
            var fguiObj = propertyInfo.GetCustomAttribute<FGUIObjectAttribute>(false);   //获取FGUIObject标签的对象
            var customObj = propertyInfo.GetCustomAttribute<FGUICustomObjectAttribute>(false);   //获取FGUICustomObject标签的对象

            if (rootObj != null)
            {
                propertyInfo.SetValue(bindObj, gComponent);
            }
            else if (customObj != null)
            {
                FGUICustomObjectAttribute attribute = (FGUICustomObjectAttribute)customObj;
                string name = propertyInfo.Name;
                GComponent com = gComponent;

                if (!string.IsNullOrEmpty(attribute.name))  //如果标签的名称名称不为空，则获取标签名称
                {
                    name = attribute.name;
                }

                if (!string.IsNullOrEmpty(attribute.path))  //如果标签的路径信息不为空，则获取标签路径
                {
                    string path = attribute.path.Replace('\\', '/');
                    string[] pathSplit = path.Split('/');
                    GObject temp = com;

                    foreach (var comName in pathSplit)
                    {
                        com = temp.asCom;
                        if (com == null)
                        {
                            Debug.LogError($"找不到组件{comName}，不存在或者不是组件类型。 {path} , {type.Name}");
                            break;
                        }
                        temp = com.GetChild(comName);
                    }

                    if (temp == null)
                    {
                        Debug.LogError($"找不到组件,不存在或者不是组件类型。 {path}, {type.Name}");
                    }
                    else
                    {
                        com = temp.asCom;
                        if (com == null)
                        {
                            Debug.LogError($"找不到组件,不存在或者不是组件类型。 {path}, {type.Name}");
                            continue;
                        }
                    }
                }

                GObject gobj = com.GetChild(name);
                if (gobj == null)
                {
                    Debug.LogError($"找不到元件{name},不存在。, {type.Name}");
                    continue;
                }

                GComponent customRoot = gobj.asCom;
                if (gobj == null)
                {
                    Debug.LogError($"找不到组件{name},不存在或者不是组件类型。, {type.Name}");
                    continue;
                }

                var customObjCom = Activator.CreateInstance(propertyInfo.PropertyType);
                BindRoot(propertyInfo.PropertyType, customObjCom, customRoot);
                propertyInfo.SetValue(bindObj, customObjCom);
            }
            else if (fguiObj != null)
            {
                FGUIObjectAttribute attribute = (FGUIObjectAttribute)fguiObj;
                string name = propertyInfo.Name;
                GComponent com = gComponent;

                if (!string.IsNullOrEmpty(attribute.name))
                {
                    name = attribute.name;
                }

                if (!string.IsNullOrEmpty(attribute.path))
                {
                    string path = attribute.path.Replace('\\', '/');
                    string[] pathSplit = path.Split('/');
                    GObject temp = com;
                    foreach (var comName in pathSplit)
                    {
                        com = temp.asCom;
                        if (com == null)
                        {
                            Debug.LogError($"找不到组件{comName}，不存在或者不是组件类型。 {path}, {type.Name}");
                            break;
                        }
                        temp = com.GetChild(comName);
                    }

                    if (temp == null)
                    {
                        Debug.LogError($"找不到组件,不存在或者不是组件类型。 {path}, {type.Name}");
                    }
                    else
                    {
                        com = temp.asCom;
                        if (com == null)
                        {
                            Debug.LogError($"找不到组件,不存在或者不是组件类型。 {path}, {type.Name}");
                            continue;
                        }
                    }
                }
                if (propertyInfo.PropertyType == typeof(Controller))
                {
                    Controller ctrl = com.GetController(name);
                    if (ctrl == null)
                    {
                        Debug.LogError($"找不到控制器{name},不存在或者不是控制器类型。, {type.Name}");
                        continue;
                    }
                    propertyInfo.SetValue(bindObj, ctrl);
                }
                else if (propertyInfo.PropertyType == typeof(Transition))
                {
                    Transition tran = com.GetTransition(name);
                    if (tran == null)
                    {
                        Debug.LogError($"找不到动效{name},不存在或者不是动效类型。, {type.Name}");
                        continue;
                    }
                    propertyInfo.SetValue(bindObj, tran);
                }
                else if (propertyInfo.PropertyType == typeof(GComponent))
                {
                    GObject gobj = com.GetChild(name);
                    if (gobj == null)
                    {
                        Debug.LogError($"找不到元件{name},不存在, {type.Name}");
                        continue;
                    }
                    GComponent c = gobj.asCom;
                    if (gobj == null)
                    {
                        Debug.LogError($"找不到组件{name},不存在或者不是组件类型, {type.Name}");
                        continue;
                    }
                    propertyInfo.SetValue(bindObj, c);
                }
                else
                {
                    GObject gObj = com.GetChild(name);
                    if (gObj != null)
                    {
                        if (gObj.GetType() != propertyInfo.PropertyType)
                        {
                            Debug.LogError($"{type.Name}的{name}绑定失败,字段名{propertyInfo.Name},字段类型:{propertyInfo.PropertyType.Name},组件类型{gObj.GetType().Name}。");
                        }
                        else
                        {
                            propertyInfo.SetValue(bindObj, gObj);
                        }
                    }
                    else
                    {
                        Debug.LogError($"找不到元件{name},可能不存在, {type.Name}");
                        continue;
                    }
                }
            }
        }
    }
    private void BindRoot<T>(T bindObj, GComponent gComponent)
    {
        Type type = typeof(T);
        BindRoot(type, bindObj, gComponent);
    }
}

