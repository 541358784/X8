using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

public class ReferenceFinderWindow : EditorWindow
{
    //依赖模式的key
    const string _isDependPrefKey = "ReferenceFinderData_IsDepend";
    //是否需要更新信息状态的key
    const string _needUpdateStatePrefKey = "ReferenceFinderData_needUpdateState";

    private static ReferenceFinderData _data = new ReferenceFinderData();
    private static bool _initializedData = false;

    private bool _isDepend = false;
    private bool _needUpdateState = true;

    private bool _needUpdateAssetTree = false;
    private bool _initializedGUIStyle = false;
    //工具栏按钮样式
    private GUIStyle _toolbarButtonGUIStyle;
    //工具栏样式
    private GUIStyle _toolbarGUIStyle;
    //选中资源列表
    private List<string> _selectedAssetGuid = new List<string>();

    private AssetTreeView _assetTreeView;

    [SerializeField]
    private TreeViewState _treeViewState;

    //查找资源引用信息
    [MenuItem("Assets/Find References In Project %#f", false, 25)]
    static void FindRef()
    {
        InitDataIfNeeded();
        OpenWindow();
        ReferenceFinderWindow window = GetWindow<ReferenceFinderWindow>();
        window.UpdateSelectedAssets();
    }

    //打开窗口
    [MenuItem("Window/Reference Finder", false, 1000)]
    static void OpenWindow()
    {
        ReferenceFinderWindow window = GetWindow<ReferenceFinderWindow>();
        window.wantsMouseMove = false;
        window.titleContent = new GUIContent("Ref Finder");
        window.Show();
        window.Focus();
    }

    //初始化数据
    static void InitDataIfNeeded()
    {
        if (!_initializedData)
        {
            //初始化数据
            if (!_data.ReadFromCache())
            {
                _data.CollectDependenciesInfo();
            }
            _initializedData = true;
        }
    }

    //初始化GUIStyle
    void InitGUIStyleIfNeeded()
    {
        if (!_initializedGUIStyle)
        {
            _toolbarButtonGUIStyle = new GUIStyle("ToolbarButton");
            _toolbarGUIStyle = new GUIStyle("Toolbar");
            _initializedGUIStyle = true;
        }
    }

    //更新选中资源列表
    private void UpdateSelectedAssets()
    {
        _selectedAssetGuid.Clear();
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            //如果是文件夹
            if (Directory.Exists(path))
            {
                string[] folder = new string[] { path };
                //将文件夹下所有资源作为选择资源
                string[] guids = AssetDatabase.FindAssets(null, folder);
                foreach (var guid in guids)
                {
                    if (!_selectedAssetGuid.Contains(guid) &&
                        !Directory.Exists(AssetDatabase.GUIDToAssetPath(guid)))
                    {
                        _selectedAssetGuid.Add(guid);
                    }
                }
            }
            //如果是文件资源
            else
            {
                string guid = AssetDatabase.AssetPathToGUID(path);
                _selectedAssetGuid.Add(guid);
            }
        }
        _needUpdateAssetTree = true;
    }

    //通过选中资源列表更新TreeView
    private void UpdateAssetTree()
    {
        if (_needUpdateAssetTree && _selectedAssetGuid.Count != 0)
        {
            var root = SelectedAssetGuidToRootItem(_selectedAssetGuid);
            if (_assetTreeView == null)
            {
                //初始化TreeView
                if (_treeViewState == null)
                    _treeViewState = new TreeViewState();
                var headerState = AssetTreeView.CreateDefaultMultiColumnHeaderState(position.width);
                var multiColumnHeader = new MultiColumnHeader(headerState);
                _assetTreeView = new AssetTreeView(_treeViewState, multiColumnHeader);
            }
            _assetTreeView.assetRoot = root;
            _assetTreeView.CollapseAll();
            _assetTreeView.Reload();
            _assetTreeView.ExpandAll();
            _needUpdateAssetTree = false;
        }
    }

    private void OnEnable()
    {
        _isDepend = PlayerPrefs.GetInt(_isDependPrefKey, 0) == 1;
        _needUpdateState = PlayerPrefs.GetInt(_needUpdateStatePrefKey, 1) == 1;
    }

    private void OnGUI()
    {
        InitGUIStyleIfNeeded();
        DrawOptionBar();
        UpdateAssetTree();
        if (_assetTreeView != null)
        {
            //绘制Treeview
            _assetTreeView.OnGUI(new Rect(0, _toolbarGUIStyle.fixedHeight, position.width, position.height - _toolbarGUIStyle.fixedHeight));
        }
    }

    //绘制上条
    public void DrawOptionBar()
    {
        EditorGUILayout.BeginHorizontal(_toolbarGUIStyle);
        //刷新数据
        if (GUILayout.Button("刷新缓存", _toolbarButtonGUIStyle))
        {
            _data.CollectDependenciesInfo();
            _needUpdateAssetTree = true;
            EditorGUIUtility.ExitGUI();
        }
        //修改模式
        bool PreIsDepend = _isDepend;
        _isDepend = GUILayout.Toggle(_isDepend, _isDepend ? "引用资源" : "被引用", _toolbarButtonGUIStyle, GUILayout.Width(100));
        if (PreIsDepend != _isDepend)
        {
            OnModelSelect();
        }
        //是否需要更新状态
        bool PreNeedUpdateState = _needUpdateState;
        _needUpdateState = GUILayout.Toggle(_needUpdateState, "Need Update State", _toolbarButtonGUIStyle);
        if (PreNeedUpdateState != _needUpdateState)
        {
            PlayerPrefs.SetInt(_needUpdateStatePrefKey, _needUpdateState ? 1 : 0);
        }
        GUILayout.FlexibleSpace();

        //扩展
        if (GUILayout.Button("展开", _toolbarButtonGUIStyle))
        {
            if (_assetTreeView != null) _assetTreeView.ExpandAll();
        }
        //折叠
        if (GUILayout.Button("收缩", _toolbarButtonGUIStyle))
        {
            if (_assetTreeView != null) _assetTreeView.CollapseAll();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnModelSelect()
    {
        _needUpdateAssetTree = true;
        PlayerPrefs.SetInt(_isDependPrefKey, _isDepend ? 1 : 0);
    }


    //生成root相关
    private HashSet<string> updatedAssetSet = new HashSet<string>();
    //通过选择资源列表生成TreeView的根节点
    private AssetViewItem SelectedAssetGuidToRootItem(List<string> selectedAssetGuid)
    {
        updatedAssetSet.Clear();
        int elementCount = 0;
        var root = new AssetViewItem { id = elementCount, depth = -1, displayName = "Root", data = null };
        int depth = 0;
        var stack = new Stack<string>();
        foreach (var childGuid in selectedAssetGuid)
        {
            var child = CreateTree(childGuid, ref elementCount, depth, stack);
            if (child != null)
                root.AddChild(child);
        }
        updatedAssetSet.Clear();
        return root;
    }
    //通过每个节点的数据生成子节点
    private AssetViewItem CreateTree(string guid, ref int elementCount, int _depth, Stack<string> stack)
    {
        if (stack.Contains(guid))
            return null;

        stack.Push(guid);
        if (_needUpdateState && !updatedAssetSet.Contains(guid))
        {
            _data.UpdateAssetState(guid);
            updatedAssetSet.Add(guid);
        }
        ++elementCount;
        var referenceData = _data.assetDict[guid];
        var root = new AssetViewItem { id = elementCount, displayName = referenceData.name, data = referenceData, depth = _depth };
        var childGuids = _isDepend ? referenceData.dependencies : referenceData.references;
        foreach (var childGuid in childGuids)
        {
            var child = CreateTree(childGuid, ref elementCount, _depth + 1, stack);
            if (child != null)
                root.AddChild(child);
        }

        stack.Pop();
        return root;
    }
}
