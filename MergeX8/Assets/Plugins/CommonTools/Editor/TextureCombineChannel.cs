using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class TextureCombineChannel : EditorWindow
{
    private Texture2D _textureAO;
    private Texture2D _textureMetallic;
    private Texture2D _textureCombine;


    private List<Color> _colorAO;
    private List<Color> _colorMetallic;
    private List<Color> _colorFinal;


    [MenuItem("Tools/合并贴图")]
    static void window()
    {
        var win = (TextureCombineChannel)EditorWindow.GetWindow(typeof(TextureCombineChannel), false, "合并通道贴图", false);
        win.Show();


    }

    private void Awake()
    {
        _colorAO = new List<Color>();
        _colorMetallic = new List<Color>();
        _colorFinal = new List<Color>();

    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("1. 保证AO和Metallic图片尺寸一样，Unity设置的尺寸也一样");
        EditorGUILayout.LabelField("2.AO和Metallic贴图设置read write enable");
        EditorGUILayout.LabelField("3.拖入工具栏，点击合并");
        EditorGUILayout.LabelField("4.自动生成一张合并后的图（绿色）在AO图目录内");
        EditorGUILayout.LabelField("5.Shader已修改，替换metallic和occlusion项为绿色贴图");
        GUILayout.Space(10);

        _textureAO = EditorGUILayout.ObjectField("AO贴图", _textureAO, typeof(Texture), true) as Texture2D;
        _textureMetallic = EditorGUILayout.ObjectField("Metallic贴图", _textureMetallic, typeof(Texture), true) as Texture2D;


        _textureCombine = EditorGUILayout.ObjectField("合并后", _textureCombine, typeof(Texture), true) as Texture2D;

        GUILayout.Space(10);
        if (GUILayout.Button("合并"))
        {
            if (_textureAO.width != _textureMetallic.width || _textureAO.height != _textureMetallic.height)
            {
                this.ShowNotification(new GUIContent("贴图大小不一样"));
            }
            else
            {
                if (_textureAO != null && _textureMetallic != null)
                {
                    _colorAO.Clear();
                    _colorMetallic.Clear();
                    _colorFinal.Clear();
                    TextureCombin();
                }
                else
                {
                    this.ShowNotification(new GUIContent("先设置贴图"));
                }
            }
        }

        GUILayout.EndVertical();
    }

    void TextureCombin()
    {
        var width = _textureAO.width;
        var height = _textureAO.height;

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                _colorAO.Add(_textureAO.GetPixel(x, y));
                _colorMetallic.Add(_textureMetallic.GetPixel(x, y));
            }
        }

        for (var i = 0; i < _colorAO.Count; i++)
        {
            var color = new Color();
            color.r = _colorMetallic[i].r;
            color.g = _colorAO[i].g;
            color.b = 0;
            color.a = _colorMetallic[i].a;

            _colorFinal.Add(color);
        }

        _textureCombine = new Texture2D(width, height, TextureFormat.RGBA32, true);
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                _textureCombine.SetPixel(x, y, _colorFinal[y + width * x]);
            }
        }

        _textureCombine.Apply();
        var path = AssetDatabase.GetAssetPath(_textureAO);
        path = path.Split('.')[0];

        var texture3Bytes = _textureCombine.EncodeToTGA();
        var filename = path.Replace("_AO", "_AO_Metallic_Combin.tga");//将合并的贴图储存到第一张贴图的路径下
        System.IO.File.WriteAllBytes(filename, texture3Bytes);
        EditorApplication.ExecuteMenuItem("Assets/Refresh");
    }

}