using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class PrefabParser
{
    private static readonly List<string> PrefixMustExport = new List<string>
    {
        "__", "ROOT"
    };
    
    private static readonly List<string> Components = new List<string>
    {
        "RectTransform", 
        "Slider", "Button", "Toggle", "ScrollRect", 
        "Image", "Animator", "Canvas", 
        "LocalizeTextMeshProUGUI", "Text",
        "Animation",
    };
    
    private static readonly List<string> ComponentsMustExport = new List<string>
    {
        "Slider", "Button", "Toggle", "ScrollRect",
    };
    
    private static readonly List<string> PrefixAsPrefab = new List<string>
    {
        "**",
    };
    
    private static readonly Dictionary<string, string> SimplifyNameDictionary = new Dictionary<string, string>
    {
        {"LocalizeTextMeshProUGUI", "TextMesh"},
        {" ", ""},
        {"(", ""},
        {")", ""},
        {"*", "x"},
    };
    
    /// <summary>
    /// order is important
    /// </summary>
    private static readonly Dictionary<string, string> SimplifyClassNameDictionary = new Dictionary<string, string>
    {
        {"UnityEngine.UI.", ""},
        {"UnityEngine.", ""},
        {"DragonPlus.", ""},
    };

    private static string SimplifyName(string name)
    {
        return SimplifyNameDictionary.Aggregate(name, (current, n) => current.Replace(n.Key, n.Value));
    }
    
    private static string SimplifyClassName(string name)
    {
        return SimplifyClassNameDictionary.Aggregate(name, (current, n) => current.Replace(n.Key, n.Value));
    }

    public string ViewClassName => _currentClassName;
    public string ViewClassMemberDeclarations => _codeDeclarations;
    public string ViewClassMemberFind => _codeBinds;
    public Dictionary<string, PrefabParser> ViewSubClasses => _subClasses;

    private Dictionary<string, PrefabParser> _subClasses;
    private Dictionary<string, string> _pathAndTransformMemberName;
    private Dictionary<string, string> _memberVarNameAndPath;
    private string _currentClassName;
    private string _codeDeclarations;
    private string _codeBinds;

    public PrefabParser(GameObject go)
    {
        Parse(go, null);
    }

    private PrefabParser(GameObject go, string viewClassName)
    {
        Parse(go, viewClassName);
    }

    private void Parse(GameObject go, string viewClassName)
    {
        _subClasses = new Dictionary<string, PrefabParser>();
        _pathAndTransformMemberName = new Dictionary<string, string>();
        _memberVarNameAndPath = new Dictionary<string, string>();
        _currentClassName = string.IsNullOrEmpty(viewClassName) ? go.name : viewClassName;
        _codeDeclarations = "";
        _codeBinds = "";

        _pathAndTransformMemberName.Add("", "transform");
        ParseGameObject(go, "");
    }
    
    private string CheckMemberVarName(string name, string path)
    {
        if (_memberVarNameAndPath.ContainsKey(name))
        {
            name = $"{name}_{CodeGen.GetMd5Hash(path)}";
        }
        _memberVarNameAndPath.Add(name, path);
        return name;
    }
    
    private void ParseGameObject(GameObject go, string path)
    {
        ParseComponent(go, path);
        var asPrefabList = new List<Transform>();
        for (var i = 0; i < go.transform.childCount; ++i)
        {
            var childTransform = go.transform.GetChild(i);
            var childPath = string.IsNullOrEmpty(path) ? $"{childTransform.name}" : $"{path}/{childTransform.name}";
            var embedPrefabTransform = PrefabUtility.GetCorrespondingObjectFromSource(childTransform);
            if (embedPrefabTransform != null)
            {
                ParseEmbedPrefab(childTransform.gameObject, childPath, embedPrefabTransform.name);
            }
            else if (!string.IsNullOrEmpty(PrefixAsPrefab.Find(a => childTransform.name.StartsWith(a))))
            {
                asPrefabList.Add(childTransform);
            }
            else
            {
                ParseGameObject(childTransform.gameObject, childPath);
            }
        }
        ParseAsPrefab(go, path, asPrefabList);
    }

    private string ParseEmbedPrefab(GameObject go, string path, string prefabName)
    {
        var transformVarName = TransformCode(SimplifyName(go.name), path);
        
        var className = $"{prefabName}View";
        var memberName = CheckMemberVarName(SimplifyName($"{go.name.Replace("View", "")}View"), path);
        
        _codeDeclarations += DeclarationCode(className, memberName, path);

        _codeBinds += $"        {memberName} = new {className}();\n";
        _codeBinds += $"        {memberName}.FindChildren({transformVarName}, false);\n";

        return className;
    }

    private void ParseAsPrefab(GameObject parent, string parentPath, List<Transform> asPrefabList)
    {
        if (asPrefabList.Count == 0) return;

        var prefabNameTransformDict = new Dictionary<string, List<Transform>>();
        // for example : [**AsPrefab0, **AsPrefab10]
        foreach (var p in asPrefabList)
        {
            var gameObjectName = p.name;
            var i = gameObjectName.Length - 1;
            for (; i >= 2; --i)
            {
                if (gameObjectName[i] < '0' || gameObjectName[i] > '9') break;
            }

            var prefabName = SimplifyName(gameObjectName.Substring(2, i - 2 + 1));
            List<Transform> list = null;
            if (prefabNameTransformDict.TryGetValue(prefabName, out list))
            {
                list.Add(p);
            }
            else
            {
                prefabNameTransformDict.Add(prefabName, new List<Transform>{p});
            }
        }
        
        foreach (var pair in prefabNameTransformDict)
        {
            var prefabName = pair.Key;
            foreach (var transform in pair.Value)
            {
                var path = string.IsNullOrEmpty(parentPath) ? $"{transform.name}" : $"{parentPath}/{transform.name}";
                var className = ParseEmbedPrefab(transform.gameObject, path, _currentClassName + prefabName);
                if (!_subClasses.ContainsKey(className))
                {
                    _subClasses.Add(className, new PrefabParser(transform.gameObject, className));
                }
            }
        }
    }

    private void ParseComponent(GameObject go, string path)
    {
        var hasPrefixMustExport = !string.IsNullOrEmpty(PrefixMustExport.Find(a => go.name.StartsWith(a))) || string.IsNullOrEmpty(path);
        if (hasPrefixMustExport)
        {
            GameObjectCode(go, path, Components);
            return;
        }

        var hasAnyComponentMustExport = ComponentsMustExport.Find(a => go.GetComponent(a) != null) != null;
        if (hasAnyComponentMustExport)
        {
            GameObjectCode(go, path, ComponentsMustExport);
            return;
        }
    }

    private string TransformCode(string gameObjectSimplifyName, string path)
    {
        var transformVarName = CheckMemberVarName($"{gameObjectSimplifyName}Transform", path);

        if (!_pathAndTransformMemberName.ContainsKey(path))
        {
            _codeDeclarations += DeclarationCode("Transform", transformVarName, path);
            _codeBinds += $"        // {path}\n";
            _codeBinds += FindCode(transformVarName, path);

            _pathAndTransformMemberName.Add(path, transformVarName);
        }
        else
        {
            transformVarName = _pathAndTransformMemberName[path];
        }

        return transformVarName;
    }

    private void GameObjectCode(GameObject go, string path, List<string> componentsMustExport)
    {
        var name = string.IsNullOrEmpty(path) ? "" : SimplifyName(go.name);
        var transformVarName = TransformCode(name, path);

        foreach (var cn in componentsMustExport)
        {
            var component = go.GetComponent(cn);
            if (component == null) continue;
            
            var className = SimplifyClassName(component.GetType().ToString());
            var memberName = CheckMemberVarName(SimplifyName($"{name}{className}"), path);

            _codeDeclarations += DeclarationCode(className, memberName, path);
            _codeBinds += GetComponentCode(transformVarName, className, memberName);

            if (cn == "ScrollRect")
            {
                var viewport = $"{memberName}Viewport";
                var content = $"{memberName}Content";
                _codeDeclarations += DeclarationCode("Transform", viewport, $"{path}/Viewport");
                _codeDeclarations += DeclarationCode("Transform", content, $"{path}/Viewport/Content");
                _codeBinds += $"        {viewport} = {transformVarName}.Find(\"Viewport\");\n";
                _codeBinds += $"        {content} = {viewport}.Find(\"Content\");\n";
            }
        }
    }

    private string DeclarationCode(string className, string memberName, string path)
    {
        var declaration = @"
    /// <summary>
    /// path * className
    /// </summary>
    public className memberName;";
        return declaration
            .Replace("className", className)
            .Replace("memberName", memberName)
            .Replace("path", path);
    }

    private string FindCode(string memberName, string path)
    {
        var levels = path.Split('/');
        var curr = "";
        var p = "";
        var mem = _pathAndTransformMemberName[p];
        foreach (var t in levels)
        {
            if (!string.IsNullOrEmpty(curr)) curr += "/";
            curr += t;
            if (_pathAndTransformMemberName.ContainsKey(curr))
            {
                p = curr;
                mem = _pathAndTransformMemberName[p];
            }
        }

        // if parent transform has been exported, 
        // use parent transform to find current transform
        if (!string.IsNullOrEmpty(p) && path.StartsWith(p))
        {
            var parentPath = $"{p}/";
            var regex = new Regex(Regex.Escape(parentPath));
            path = regex.Replace(path, "", 1).TrimStart('/');
        }
        return $"        {memberName} = {mem}.Find(\"{path}\");\n";
    }

    private string GetComponentCode(string transformVarName, string className, string memberName)
    {
        return $"        {memberName} = {transformVarName}.GetComponent<{className}>();\n";
    }
}