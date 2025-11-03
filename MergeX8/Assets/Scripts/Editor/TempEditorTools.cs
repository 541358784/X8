using DragonU3DSDK.Asset;
using UnityEditor;
using UnityEngine;

public class TempEditorTools
{
    [MenuItem("GameObject/复制层级路径", false, 0)]
    static public void CopyPath(MenuCommand menuCommand)
    {
        var path = Selection.activeGameObject.name;
        var parent = Selection.activeGameObject.transform.parent;
        while (parent != null)
        {
            if (parent.name.Equals("Canvas (Environment)"))
            {
                parent = null;
            }
            else
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
        }

        GUIUtility.systemCopyBuffer = path;
    }

    [MenuItem("Assets/复制资源路径", false, 0)]
    static public void CopyAssetPath()
    {
        var path = string.Empty;
        if (Selection.assetGUIDs != null && Selection.assetGUIDs.Length == 1)
        {
            path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        }

        GUIUtility.systemCopyBuffer = path;
    }

    public const string localstr = "SpriteAtlas/Local"; //进初始包图集前缀
    public const string activityStr = "SpriteAtlas/Activities"; //活动图集前缀
    public const string localDir = "LocalSpriteAtlas"; //完全进初始包的 图集组名
    public const string remoteDir = "RemoteSpriteAtlas"; //部分进初始包的 图集组名

    /// <summary>
    /// 考虑下载图集的路径问题，Room 和 Map 和 World 下的图集的路径统一，全部放在remote下，但是一般第一个和第二个房间会放在初始包中，如果在remote下有需要放入初始包中的图集，把路径放在这里。
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static bool ContainInInitialPacket(string path)
    {
        return (
            path.Contains("SpriteAtlas/Remote/Cooking/Map1Atlas") ||
            path.Contains("SpriteAtlas/Remote/Cooking/Map1GameAtlas") ||
            //path.Contains ("SpriteAtlas/Remote/Cooking/Map2Atlas") ||
            //path.Contains ("SpriteAtlas/Remote/Cooking/Map2GameAtlas") ||
            path.Contains("SpriteAtlas/Remote/Cooking/World1") ||
            path.Contains("SpriteAtlas/Remote/Cooking/World2") ||
            path.Contains("SpriteAtlas/Remote/Room/1004") ||
            //path.Contains ("SpriteAtlas/Remote/Room/1002") ||
            // path.Contains ("SpriteAtlas/Remote/Room/Icon1002") ||
            path.Contains("SpriteAtlas/Remote/Room/Icon1004")
        );
    }

    /// <summary>
    /// 说明：SpriteAtlas/Local 进初始包的图集
    /// SpriteAtlas/Activities 活动的图集
    /// SpriteAtlas/Remote 不进初始包，需要下载更新的图集, 
    /// </summary>
    [MenuItem("AssetBundle/SpriteAtlas/AtlasConfig2AssetConfig_4CK7 &K")]
    public static void MoveAtlasToFolder()
    {
        AtlasConfigController asset = AtlasConfigController.Instance;
        if (asset == null)
        {
            EditorUtility.DisplayDialog("警告", "没有生成AtlasConfig文件或者生成失败，请先执行AssetBundle/SpriteAtlas/AtlasConfig，再重试一次",
                "ok");
            return;
        }

        var list = asset.AtlasPathNodeList;
        BundleGroup localGroup = null;
        BundleGroup remoteGroup = null;

        AssetConfigController configController = AssetConfigController.Instance;
        if (configController == null)
        {
            EditorUtility.DisplayDialog("警告", "AssetConfigController 没有 LocalSpriteAtlas 或者 RemoteSpriteAtlas 资源组",
                "ok");
            return;
        }

        foreach (var item in configController.Groups)
        {
            if (localDir == item.GroupName)
            {
                localGroup = item;
            }
            else if (remoteDir == item.GroupName)
            {
                remoteGroup = item;
            }
        }

        if (localGroup == null && remoteGroup == null)
        {
            return;
        }

        localGroup.Paths.Clear();
        localGroup.UpdateWholeAB = true;
        remoteGroup.Paths.Clear();
        remoteGroup.UpdateWholeAB = false;
        foreach (var item in list)
        {
            if (item.HdPath.StartsWith(activityStr))
            {
                continue;
            }

            bool local = false;
            if (item.HdPath.StartsWith(localstr))
            {
                local = true;
            }

            if (!string.IsNullOrEmpty(item.HdPath))
            {
                var path = item.HdPath.Substring(0, item.HdPath.LastIndexOf('/'));
                Debug.Log("Hdpath :" + path);
                BundleState ab = new BundleState()
                {
                    InInitialPacket = ContainInInitialPacket(item.HdPath) ? true : local,
                    Path = path
                };
                if (local)
                {
                    localGroup.Paths.Add(ab);
                }
                else
                {
                    remoteGroup.Paths.Add(ab);
                }
            }

            if (item.HdPath == item.SdPath)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(item.SdPath))
            {
                var path = item.SdPath.Substring(0, item.SdPath.LastIndexOf('/'));
                Debug.Log("Sdpath :" + path);
                BundleState ab = new BundleState()
                {
                    InInitialPacket = ContainInInitialPacket(item.SdPath) ? true : local,
                    Path = path
                };
                if (local)
                {
                    localGroup.Paths.Add(ab);
                }
                else
                {
                    remoteGroup.Paths.Add(ab);
                }
            }
        }

        EditorUtility.SetDirty(configController);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}