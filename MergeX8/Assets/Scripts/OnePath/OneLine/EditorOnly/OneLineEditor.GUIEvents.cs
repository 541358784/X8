#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OneLine.Editor
{
    partial class OneLineEditor
    {
        private void HandleEvents(Event e, SceneView sceneView)
        {
            if (PreviewTexture == null) return;

            if (GUIPositionToTexturePixel(sceneView, e.mousePosition, out Vector2Int pixel))
            {
                bool isEditablePixel = IsEditablePixel(pixel);
                if (m_HoverPixel != pixel)
                {
                    ResetPixelColor(m_HoverPixel);
                    m_HoverPixel = pixel;
                    if (isEditablePixel)
                    {
                        SetPixelColor(pixel, m_HoverPixelColor * GetPreviewColor(pixel), true);
                    }
                }

                if (isEditablePixel && e.type == EventType.MouseDown)
                {
                    HandleMouseDown(e, sceneView, pixel);
                }
            }
        }

        private void HandleMouseDown(Event e, SceneView sceneView, Vector2Int pixel)
        {
            switch (e.button)
            {
                case 0:
                    if (isEditingPoint)
                    {
                        if (IsPoint(pixel))
                        {
                            RemovePoint(sceneView, pixel);
                        }
                        else
                        {
                            AddPoint(sceneView, pixel);
                        }
                    }
                    else if (IsEditingEdge)
                    {
                        if (IsPoint(pixel))
                        {
                            CompleteEditEdge(sceneView, pixel);
                        }
                        else if (m_EditingEdge.Path.Contains(pixel))
                        {
                            RemoveEdgePath(sceneView, pixel);
                        }
                        else
                        {
                            AddEdgePath(sceneView, pixel);
                        }
                    }
                    else
                    {
                        StartEditEdge(sceneView, pixel);
                    }

                    e.Use();
                    break;
            }
        }

        private void AddPoint(SceneView sceneView, Vector2Int pixel)
        {
            Undo.RecordObject(this, nameof(StartEditEdge));
            m_PointConfigs.Add(new OneLinePointConfig(pixel));
            SetPixelColor(pixel, m_PointPixelColor, false);
        }

        private void RemovePoint(SceneView sceneView, Vector2Int pixel)
        {
            Undo.RecordObject(this, nameof(StartEditEdge));
            for (int i = 0; i < m_PointConfigs.Count; i++)
            {
                if (m_PointConfigs[i].Position == pixel)
                {
                    m_PointConfigs.RemoveAt(i);
                    break;
                }
            }
        }

        private void StartEditEdge(SceneView sceneView, Vector2Int pixel)
        {
            if (IsPoint(pixel) == false)
            {
                sceneView.ShowNotification(TempContent("选择一个点作为起点开始"));
                return;
            }

            Undo.RecordObject(this, nameof(StartEditEdge));
            m_EditingEdge           = new OneLineEdgeConfig();
            m_EditingEdge.Editing   = true;
            m_EditingEdge.APosition = pixel;
            m_EditingEdge.Path      = new List<Pixel>(256) {pixel};
        }

        private void CompleteEditEdge(SceneView sceneView, Vector2Int pixel)
        {
            if (IsAround(m_EditingEdge.LastPixel, pixel) == false)
            {
                sceneView.ShowNotification(TempContent("所选路径点未与当前路径的点相连"));
                return;
            }

            Undo.RecordObject(this, nameof(CompleteEditEdge));
            m_EditingEdge.BPosition = pixel;
            m_EditingEdge.Editing   = false;
            m_EditingEdge.Path.Add(m_EditingEdge.BPosition);
            m_EdgeConfigs.Add(m_EditingEdge.Copy());
            for (var i = 1; i < m_EditingEdge.Path.Count - 1; i++)
            {
                var temp = m_EditingEdge.Path[i];
                SetPixelColor(temp, m_UsedPixelColor, false, false);
                if (m_UsedPixels.Contains(temp)) continue;
                m_UsedPixels.Add(temp);
            }

            ApplyPixels();
            sceneView.ShowNotification(TempContent($"已生成一条边，当前共计{m_EdgeConfigs.Count}条"));
        }

        private void AddEdgePath(SceneView sceneView, Vector2Int pixel)
        {
            if (IsAround(m_EditingEdge.LastPixel, pixel) == false)
            {
                sceneView.ShowNotification(TempContent("所选路径点未与当前路径的点相连"));
                return;
            }

            Undo.RecordObject(this, nameof(AddEdgePath));
            AddToPath(pixel);
            ApplyPixels();

            void AddToPath(Vector2Int p)
            {
                if (IsPoint(p)) return;
                m_EditingEdge.Path.Add(p);
                SetPixelColor(p, m_EdgePixelColor, false, false);
                int        aroundCount = 0;
                Vector2Int aroundPixel = Vector2Int.zero;
                for (var i = 0; i < AroundOffset.Length; i++)
                {
                    var temp = p + AroundOffset[i];
                    if (IsEditablePixel(temp) && m_EditingEdge.Path.Contains(temp) == false)
                    {
                        aroundPixel = temp;
                        aroundCount++;
                    }
                }

                if (aroundCount == 1)
                {
                    AddToPath(aroundPixel);
                }
            }
        }

        private void RemoveEdgePath(SceneView sceneView, Vector2Int pixel)
        {
            if (m_EditingEdge.LastPixel != pixel)
            {
                sceneView.ShowNotification(TempContent("只能从末尾移除路径点"));
                return;
            }

            Undo.RecordObject(this, nameof(RemoveEdgePath));
            m_EditingEdge.Path.Remove(pixel);
            SetPixelColor(pixel, m_UsedPixels.Contains(pixel) ? m_UsedPixelColor : Color.white, false);
        }

        private bool GUIPositionToTexturePixel(SceneView sceneView, Vector2 position, out Vector2Int pixel)
        {
            var ray = HandleUtility.GUIPointToWorldRay(position);
            if (Physics.Raycast(ray, out var hit))
            {
                Vector2 sp = RectTransformUtility.WorldToScreenPoint(Camera.current, hit.point);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Image.rectTransform, sp, Camera.current, out Vector2 lp);
                lp    += m_Image.rectTransform.rect.size / 2f;
                pixel =  new Vector2Int((int) lp.x, (int) lp.y);
                return true;
            }

            pixel = Vector2Int.zero;
            return false;
        }

        private bool IsAround(Vector2Int a, Vector2Int b)
        {
            return Array.IndexOf(AroundOffset, a - b) >= 0;
        }

        private bool IsPoint(Vector2Int pixel)
        {
            for (int i = 0; i < m_PointConfigs.Count; i++)
            {
                if (m_PointConfigs[i].Position == pixel)
                {
                    return true;
                }
            }

            return false;
        }

        private static Vector2Int[] AroundOffset = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1),
        };
    }
}
#endif