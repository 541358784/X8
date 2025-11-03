using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static partial class CodeGen
{
    /// <summary>
    /// GameObject which name starts with PrefixMustExport will use Components,
    /// others will use ComponentsMustExport
    /// </summary>
    private static void GenPrefab()
    {
        foreach (var go in Selection.gameObjects)
        {
            GenCode(go);
        }
    }

    private static void GenCode(GameObject go)
    {
        var code = PrefabTemplateCode();
        code += ControllerTemplateCode();
        
        var assetPath = AssetDatabase.GetAssetPath(go.GetInstanceID());
        var assetPath4UIWindow = assetPath.Replace("Assets/Export/UI/", "").Replace(".prefab", "");
        var assetPath4MonoBehaviour = assetPath.Replace("Assets/Export/", "").Replace(".prefab", "");
        
        code = code.Replace("FULL_PATH", assetPath);
        code = code.Replace("4WINDOW", $"\"{assetPath4UIWindow}\"");
        code = code.Replace("4MONO", $"\"{assetPath4MonoBehaviour}\"");

        var view = new PrefabParser(go);
        code = code.Replace("CLASS_NAME", view.ViewClassName);
        code = code.Replace("DECLARATION", view.ViewClassMemberDeclarations);
        code = code.Replace("BIND", view.ViewClassMemberFind);

        foreach (var subClass in view.ViewSubClasses)
        {
            var template = AsPrefabTemplateCode();
            template = template.Replace("CLASS_NAME", subClass.Value.ViewClassName);
            template = template.Replace("DECLARATION", subClass.Value.ViewClassMemberDeclarations);
            template = template.Replace("BIND", subClass.Value.ViewClassMemberFind);
            code = template + code;
        }
        
        code = NameSpaceUsedTemplateCode() + code;
        SaveFile("View", $"{go.name}View", code);
    }

    private static string NameSpaceUsedTemplateCode()
    {
        return @"/*CodeGen Prefab SDK*/
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;
";
    }
    
    private static string PrefabTemplateCode()
    {
        return @"
/// <summary>
/// CodeGen Prefab SDK : CLASS_NAME
/// FULL_PATH
/// </summary>
public class CLASS_NAMEView : CodeGenView
{
    public static readonly string AssetPath4UIWindow = 4WINDOW;
    public static readonly string AssetPath4MonoBehaviour = 4MONO;
DECLARATION

    public override void FindChildren(Transform transform, bool resetPosition = true)
    {
        this.transform = transform;
        this.gameObject = transform.gameObject;
        if (resetPosition) transform.localPosition = Vector3.zero;

BIND    }
}";
    }

    private static string AsPrefabTemplateCode()
    {
        return @"
/// <summary>
/// CLASS_NAME
/// </summary>
public class CLASS_NAME : CodeGenView
{DECLARATION

    public override void FindChildren(Transform transform, bool resetPosition = true)
    {
        this.transform = transform;
        this.gameObject = transform.gameObject;
        if (resetPosition) transform.localPosition = Vector3.zero;

BIND    }
}
";
    }

    private static string ControllerTemplateCode()
    {
        return @"

/*
using DragonPlus;
using UnityEngine;

public class CLASS_NAMEController : UIWindow
{
    private CLASS_NAMEView _view = new CLASS_NAMEView();

    public static CLASS_NAMEController Open()
    {
        // var ctrl = UIManager.Instance.OpenWindow<CLASS_NAMEController>(_activityModel.GetResFolder() + /CLASS_NAME, UIWindowType.PopupTip);
        var ctrl = UIManager.Instance.OpenWindow<CLASS_NAMEController>(CLASS_NAMEView.AssetPath4UIWindow, UIWindowType.PopupTip);
        return ctrl;
    }

    public override void PrivateAwake()
    {
        _view.FindChildren(transform);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        CommonUtils.TweenOpen(_view.__ROOTRectTransform);
    }

    // protected override void OnCloseWindow(bool destroy = false) {}

    private void OnCloseClick()
    {
        CommonUtils.TweenClose(_view.__ROOTRectTransform, () =>
        {
            CloseWindowWithinUIMgr(true);
        });
    }
}

// ---------------------------------------------------------------------------------------------------------------------

using DragonPlus;
using UnityEngine;

public class CLASS_NAMEController : MonoBehaviour
{
    private CLASS_NAMEView _view = new CLASS_NAMEView();

    public static CLASS_NAMEController Create(Transform parent)
    {
        // var go = ResourcesManager.Instance.LoadResource<GameObject>(UI/ + _activityModel.GetResFolder() + /CLASS_NAME);
        var go = ResourcesManager.Instance.LoadResource<GameObject>(CLASS_NAMEView.AssetPath4MonoBehaviour);
        return Instantiate(go, parent, false).AddComponent<CLASS_NAMEController>();
    }

    private void Awake()
    {
        // _view.FindChildren(transform, false);
        _view.FindChildren(transform);
    }
}
*/";
    }
}