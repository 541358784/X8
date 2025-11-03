#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneLine.Editor
{
    [ExecuteAlways]
    public partial class OneLineEditor : MonoBehaviour
    {
        public const string ConfigPath          = "Assets/Export/Configs/OnePath";
        public const string PathTexturePath     = "Assets/Export/OneLine/Path";
        public const string TemplateTexturePath = "Assets/Export/OneLine/Textures";

        #region Config

        [HideInInspector, SerializeField]
        private Texture2D m_PathTexture;

        [HideInInspector, SerializeField]
        private int m_BrushSize;

        [HideInInspector, SerializeField]
        private List<Vector2Int> m_AllPath = new List<Vector2Int>();

        [HideInInspector, SerializeField]
        private List<OneLinePointConfig> m_PointConfigs = new List<OneLinePointConfig>();

        [HideInInspector, SerializeField]
        private List<OneLineEdgeConfig> m_EdgeConfigs = new List<OneLineEdgeConfig>();

        #endregion

        #region Editing

        [SerializeField]
        private Color m_HoverPixelColor;

        [SerializeField]
        private Color m_PointPixelColor;

        [SerializeField]
        private Color m_EdgePixelColor;

        [SerializeField]
        private Color m_UsedPixelColor;

        [SerializeField]
        private RawImage m_PlayImage;

        [SerializeField]
        private RawImage m_Image;

        [HideInInspector, SerializeField]
        private Texture2D m_TemplateTexture;

        [HideInInspector, SerializeField]
        private OneLineEdgeConfig m_EditingEdge;

        [HideInInspector, SerializeField]
        private List<Vector2Int> m_UsedPixels = new List<Vector2Int>();

        [HideInInspector, SerializeField]
        private Color[] m_PreviewColors = new Color[0];

        [HideInInspector, SerializeField]
        private List<Vector2Int> m_BrushArea = new List<Vector2Int>();

        private Vector2Int m_HoverPixel;
        private Color      m_HoverPixelOldColor;
        private GUIContent m_TempContent = new GUIContent();

        private bool IsEditingEdge => m_EditingEdge.Editing;

        private Texture2D PreviewTexture => m_Image.texture as Texture2D;

        #endregion

        private void OnEnable()
        {
            if (EditorApplication.isPlaying) return;
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            Undo.undoRedoPerformed   -= UndoRedoPerformed;
            Undo.undoRedoPerformed   += UndoRedoPerformed;
        }

        private void OnDisable()
        {
            if (EditorApplication.isPlaying) return;
            SceneView.duringSceneGui -= OnSceneGUI;
            Undo.undoRedoPerformed   -= UndoRedoPerformed;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            DrawToolbar(sceneView);
            HandleEvents(Event.current, sceneView);
        }

        private void UndoRedoPerformed()
        {
            PreviewTexture.SetPixels(m_PreviewColors);
        }

        private void NewEdit()
        {
            Undo.ClearUndo(this);
            m_EditingEdge = new OneLineEdgeConfig();
            m_UsedPixels.Clear();
            m_EdgeConfigs.Clear();
            m_PointConfigs.Clear();

            if (m_PathTexture != null)
            {
                m_PreviewColors           = m_PathTexture.GetPixels();
                m_TemplateTexture         = AssetDatabase.LoadAssetAtPath<Texture2D>($"{TemplateTexturePath}/{m_PathTexture.name}.png");
                m_Image.texture           = new Texture2D(m_PathTexture.width, m_PathTexture.height, TextureFormat.RGBA32, false);
                m_PlayImage.texture       = m_TemplateTexture;
                PreviewTexture.filterMode = FilterMode.Point;
                PreviewTexture.hideFlags  = HideFlags.DontSave;
                UpdateBrushSize(m_BrushSize, false);
            }
            else
            {
                m_PreviewColors     = Array.Empty<Color>();
                m_PathTexture       = null;
                m_Image.texture     = null;
                m_PlayImage.texture = null;
            }
        }

        private void UpdateBrushSize(int brushSize, bool record)
        {
            if (m_PathTexture == null) return;
            if (record) Undo.RecordObject(this, nameof(UpdateBrushSize));

            m_BrushSize = brushSize;
            m_BrushArea.Clear();
            for (int x = -brushSize; x <= brushSize; x++)
            {
                for (int y = -brushSize; y <= brushSize; y++)
                {
                    Vector2Int p = new Vector2Int(x, y);
                    if (p.sqrMagnitude <= brushSize * brushSize)
                    {
                        m_BrushArea.Add(p);
                    }
                }
            }

            for (int y = 0; y < m_PathTexture.height; y++)
            {
                for (int x = 0; x < m_PathTexture.width; x++)
                {
                    SetPreviewPixelColor(x, y, Color.clear);
                }
            }

            for (int y = 0; y < m_PathTexture.height; y++)
            {
                for (int x = 0; x < m_PathTexture.width; x++)
                {
                    Color c = m_PathTexture.GetPixel(x, y);
                    if (c.a < 0.01f)
                    {
                        continue;
                    }

                    foreach (var aroundP in m_BrushArea)
                    {
                        Vector2Int p = new Vector2Int(x + aroundP.x, y + aroundP.y);
                        SetPreviewPixelColor(p.x, p.y, IsEditablePixel(p) ? Color.white : new Color(0.22f, 0.4f, 0.13f, 0.6f));
                    }
                }
            }

            PreviewTexture.Apply();
            m_Image.SetMaterialDirty();

            void SetPreviewPixelColor(int x, int y, Color color)
            {
                PreviewTexture.SetPixel(x, y, color);
                m_PreviewColors[x + y * m_PathTexture.width] = color;
            }
        }

        private bool CanSave()
        {
            return m_PathTexture != null;
        }

        private void Save(SceneView sceneView)
        {
            m_AllPath.Clear();

            for (int i = 0; i < m_PointConfigs.Count; i++)
            {
                m_PointConfigs[i].Edges.Clear();
            }

            for (int i = 0; i < m_EdgeConfigs.Count; i++)
            {
                OneLineEdgeConfig  config = m_EdgeConfigs[i];
                OneLinePointConfig pointA = m_PointConfigs.Find(x => x.Position == config.APosition);
                OneLinePointConfig pointB = m_PointConfigs.Find(x => x.Position == config.BPosition);
                if (pointA == null || pointB == null)
                {
                    sceneView.ShowNotification(TempContent("保存失败！有边使用了不存在的点，找程序来看！"));
                    return;
                }

                config.A = m_PointConfigs.IndexOf(pointA);
                config.B = m_PointConfigs.IndexOf(pointB);
                pointA.Edges.Add(i);
                pointB.Edges.Add(i);
            }

            for (var y = 0; y < m_PathTexture.height; y++)
            {
                for (int x = 0; x < m_PathTexture.width; x++)
                {
                    Vector2Int p = new Vector2Int(x, y);
                    if (IsEditablePixel(p))
                    {
                        m_AllPath.Add(p);
                    }
                }
            }

            OneLineGraphic configInstance = JsonUtility.FromJson<OneLineGraphic>(JsonUtility.ToJson(this));
            File.WriteAllText(GetConfigPath(), JsonUtility.ToJson(configInstance));

            m_AllPath.Clear();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void SetPixelColor(Vector2Int pixel, Color color, bool isTemp, bool apply = true)
        {
            PreviewTexture.SetPixel(pixel.x, pixel.y, color);
            if (apply)
            {
                ApplyPixels();
            }

            if (isTemp == false)
            {
                m_PreviewColors[pixel.x + pixel.y * PreviewTexture.width] = color;
            }
        }

        private Color GetPreviewColor(Vector2Int pixel)
        {
            return m_PreviewColors[pixel.x + pixel.y * PreviewTexture.width];
        }

        private void ResetPixelColor(Vector2Int pixel)
        {
            PreviewTexture.SetPixel(pixel.x, pixel.y, m_PreviewColors[pixel.x + pixel.y * PreviewTexture.width]);
            PreviewTexture.Apply();
            m_Image.SetMaterialDirty();
        }

        private void ApplyPixels()
        {
            PreviewTexture.Apply();
            m_Image.SetMaterialDirty();
        }

        private bool IsEditablePixel(Vector2Int pixel)
        {
            return m_PathTexture.GetPixel(pixel.x, pixel.y).a > 0;
        }

        private string GetConfigPath()
        {
            return $"{ConfigPath}/{m_PathTexture.name}.json";
        }

        private GUIContent TempContent(string text)
        {
            m_TempContent.text = text;
            return m_TempContent;
        }
    }
}
#endif