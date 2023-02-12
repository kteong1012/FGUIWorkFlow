
using FairyGUI;
using Scriban;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

public class FGUICodeGenerator
{
    [MenuItem("Assets/生成FGUI组件代码")]
    public static void GenerateFGUICode()
    {
        var settings = AssetDatabase.LoadAssetAtPath<FGUIWorkFlowSettings>("Assets/FGUIWorkFlow/FGUIWorkFlowSettings.asset");
        var generator = new FGUICodeGenerator(settings);
        generator.Generate();
    }

    private const string ATTR_CUSTOM_NAME = "FGUICustomObject";
    private const string ATTR_ROOT_NAME = "FGUIComponentRoot";
    private const string ATTR_NAME = "FGUIObject";
    private List<FGUICodeClassInfo> _classInfos = new List<FGUICodeClassInfo>();
    private List<FGUICodeClassInfo> _localClassInfos = new List<FGUICodeClassInfo>();
    private FGUIWorkFlowSettings _settings;

    public FGUICodeGenerator(FGUIWorkFlowSettings settings)
    {
        _settings = settings;
    }


    public void Generate()
    {
        try
        {
            CompilationPipeline.RequestScriptCompilation();
            _classInfos.Clear();
            _localClassInfos.Clear();
            UIPackage.RemoveAllPackages();
            var sources = Selection.GetFiltered<TextAsset>(SelectionMode.Assets).Where(ta => ta.name.EndsWith("_fui"));
            foreach (var commonPackage in _settings.commonPackages)
            {
                var source = AssetDatabase.LoadAssetAtPath<TextAsset>(Path.Combine(_settings.resourceDir, $"{commonPackage}_fui.bytes"));
                UIPackage package = UIPackage.AddPackage(source.bytes, source.name.Replace("_fui", ""), (string name, string extension, System.Type type, PackageItem item) => { });
            }
            foreach (var source in sources)
            {
                var pkgName = source.name.Replace("_fui", "");
                var package = UIPackage.GetPackages().FirstOrDefault(x => x.name == pkgName);
                if (package == null)
                {
                    package = UIPackage.AddPackage(source.bytes, pkgName, (string name, string extension, System.Type type, PackageItem item) => { });
                }
                foreach (var item in package.GetItems())
                {
                    if (!item.exported)
                    {
                        continue;
                    }
                    GComponent obj = package.CreateObject(item.name).asCom;
                    if (obj != null)
                    {
                        HandleComponent(obj, package.name, item.name, IsLocal(package.name));
                    }
                }
            }
            WriteFiles(_classInfos, _settings.outputDir);
            WriteFiles(_localClassInfos, _settings.localOutputDir);
            EditorUtility.DisplayDialog("", "生成FGUI代码成功", "确定");
            AssetDatabase.Refresh();
        }
        finally
        {
            _classInfos.Clear();
            _localClassInfos.Clear();
            UIPackage.RemoveAllPackages();
        }
    }
    private void HandleComponent(GComponent root, string packageName, string name, bool local)
    {
        try
        {
            var classInfo = new FGUICodeClassInfo();
            classInfo.name = name;
            classInfo.packageName = packageName;
            classInfo.componentName = name;
            classInfo.url = root.resourceURL;
            classInfo.fields.Add(new FGUICodeFieldInfo
            {
                attribute = ATTR_ROOT_NAME,
                comment = "脚本绑定的FGUI物体",
                name = "FGUIObj",
                type = root.GetType().Name
            });

            foreach (Controller controller in root.Controllers)
            {
                if (classInfo.fields.Any(x => x.name == controller.name))
                {
                    throw new Exception($"{packageName}-{root.name}中存在重复字段：{controller.name}");
                }
                classInfo.fields.Add(new FGUICodeFieldInfo
                {
                    attribute = ATTR_NAME,
                    name = controller.name,
                    type = nameof(Controller)
                });
            }
            foreach (Transition transition in root.Transitions)
            {
                if (classInfo.fields.Any(x => x.name == transition.name))
                {
                    throw new Exception($"{packageName}-{root.name}中存在重复字段：{transition.name}");
                }
                classInfo.fields.Add(new FGUICodeFieldInfo
                {
                    attribute = ATTR_NAME,
                    name = transition.name,
                    type = nameof(Transition)
                });
            }
            foreach (GObject obj in root.GetChildren())
            {
                if (obj.id.StartsWith(obj.name + "_"))
                {
                    continue;
                }
                if (classInfo.fields.Any(x => x.name == obj.name))
                {
                    throw new Exception($"{packageName}-{root.name}中存在重复字段：{obj.name}");
                }
                if (obj.GetType() == typeof(GComponent))
                {
                    if (string.IsNullOrEmpty(obj.resourceURL))
                    {
                        throw new Exception($"{packageName}包的组件{name}中{obj.name}出现了跨包引用,请检查一下");
                    }
                    GComponent com = UIPackage.CreateObjectFromURL(obj.resourceURL).asCom;
                    bool tempLocal = IsLocal(com.packageItem.owner.name);
                    if (tempLocal != local)
                    {
                        throw new Exception($"{packageName}生成错误：本地包不得与非本地包相互引用");
                    }
                    HandleComponent(com, com.packageItem.owner.name, com.packageItem.name, tempLocal);
                    classInfo.fields.Add(new FGUICodeFieldInfo
                    {
                        attribute = ATTR_CUSTOM_NAME,
                        name = obj.name,
                        type = com.packageItem.name
                    });
                    com.Dispose();
                }
                else if (obj.GetType() == typeof(GButton))
                {
                    classInfo.fields.Add(new FGUICodeFieldInfo
                    {
                        attribute = ATTR_NAME,
                        name = obj.name,
                        type = nameof(GButton),
                        isButton = true,
                        buttonCallback = $"OnClick_{obj.name}"
                    });
                }
                else
                {
                    classInfo.fields.Add(new FGUICodeFieldInfo
                    {
                        attribute = ATTR_NAME,
                        name = obj.name,
                        type = obj.GetType().Name
                    });
                }
            }
            if (local)
            {
                _localClassInfos.Add(classInfo);
            }
            else
            {
                _classInfos.Add(classInfo);
            }

        }
        finally
        {
            root.Dispose();
        }
    }

    private void WriteFiles(IEnumerable<FGUICodeClassInfo> list, string outputPath)
    {
        var sortedInfos = list.OrderByDescending(x => x.OutputPath);
        foreach (var info in sortedInfos)
        {
            RenderByTemplate("Assets/FGUIWorkFlow/Editor/templateDefine.txt", Path.Combine(outputPath, "AutoGenerate", info.OutputPath), info);
            if (_settings.createPartialCode)
            {
                var logicCodePath = Path.Combine(outputPath, "Logic", info.OutputPath);
                if (!File.Exists(logicCodePath))
                {
                    RenderByTemplate("Assets/FGUIWorkFlow/Editor/templateLogic.txt", logicCodePath, info);
                }
            }
        }
    }

    private void RenderByTemplate(string templatePath, string outputPath, FGUICodeClassInfo info)
    {
        var templateText = File.ReadAllText(templatePath);
        var template = Template.Parse(templateText);
        var content = template.Render(new
        {
            classInfo = info,
        });
        var fi = EnsureFileDirectory(outputPath);
        File.WriteAllText(fi.FullName, content);
    }

    private bool IsLocal(string packageName)
    {
        return _settings.localPackages.Contains(packageName);
    }
    private bool IsCommon(string packageName)
    {
        return _settings.commonPackages.Contains(packageName);
    }
    private FileInfo EnsureFileDirectory(string filePath)
    {
        var fi = new FileInfo(filePath);
        if (!fi.Directory.Exists)
        {
            fi.Directory.Create();
        }
        return fi;
    }
    //private void WriteScript(string packageName, string name, string fields)
    //{
    //    string compDirPath = Application.dataPath.Replace("/Unity/Assets", GENERATE_CODE_DIR);
    //    string dirPath = $"{compDirPath}/{packageName}";
    //    string path = $"{dirPath}/{name}.cs";
    //    if (_generatingCodes.Contains(path))
    //    {
    //        return;
    //    }
    //    _generatingCodes.Add(path);
    //    if (!Directory.Exists(dirPath))
    //    {
    //        Directory.CreateDirectory(dirPath);
    //    }
    //    if (!File.Exists(path))
    //    {
    //        File.Create(path).Dispose();
    //    }
    //    string content = File.ReadAllText(path);
    //    content = InsertStr(content, fields);
    //    if (string.IsNullOrEmpty(content))
    //    {
    //        string template = File.ReadAllText(TEMPLATE_PATH);
    //        content = template.Replace("#CLASS_NAME#", name);
    //        content = InsertStr(content, fields);
    //    }
    //    FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    //    fs.SetLength(0);
    //    byte[] contentBytes = System.Text.Encoding.UTF8.GetBytes(content);
    //    fs.Write(contentBytes, 0, contentBytes.Length);
    //    fs.Close();
    //}
}
