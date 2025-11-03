using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OneLine
{
    /// <summary>
    /// 一笔画小游戏
    /// </summary>
    public sealed partial class OneLineGame : IDisposable
    {
        public OneLineGame(OneLineOrder order)
        {
            m_Graphic               = order.Graphic;
            m_TemplateTextureWidth  = order.Template.width;
            m_TemplateTextureHeight = order.Template.height;
            m_TemplateTexture       = order.Template.GetPixels();
            m_TemplateColor         = order.TemplateColor;
            m_SuccessColor          = order.SuccessColor;
            m_FailedColor           = order.FailedColor;
            m_DrawColor             = order.DrawColor;
            m_AdsorbToPointDistance = order.AdsorbToPointDistance * order.AdsorbToPointDistance;
            m_UICamera              = order.UICamera;

            m_DrawingPixels               = new Color[m_TemplateTexture.Length];
            m_DrawingPixelsDontChangeFlag = new bool[m_TemplateTexture.Length];
        }

        private OneLineGraphic m_Graphic;
        private int            m_TemplateTextureWidth;
        private int            m_TemplateTextureHeight;
        private Color[]        m_TemplateTexture;
        private Color          m_TemplateColor;
        private Color          m_DrawColor;
        private Color          m_SuccessColor;
        private Color          m_FailedColor;
        private float          m_AdsorbToPointDistance;
        private Camera         m_UICamera;
        private IOneLineView   m_View;

        /// <summary>
        /// 是否已经完成一笔画了
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// 是否在绘制中
        /// </summary>
        public bool IsDrawing => !IsCompleted && m_DrawThroughPoints.Count > 0;

        /// <summary>
        /// 图像数据
        /// </summary>
        public OneLineGraphic Graphic => m_Graphic;

        /// <summary>
        /// 模板颜色
        /// </summary>
        public Color TemplateColor => m_TemplateColor;

        /// <summary>
        /// 绘制颜色
        /// </summary>
        public Color DrawColor => m_DrawColor;

        public void Start(IOneLineView view)
        {
            m_View = view;

            Array.Copy(m_TemplateTexture, m_DrawingPixels, m_DrawingPixels.Length);

            m_View.DrawOnImage.gameObject.AddComponent<InteractionBehaviour>().Initialize(this);
            m_View.DrawOnImage.texture = m_DrawingTexture = NewTexture();
            m_View.DrawOnImage.SetNativeSize();
            m_View.OnStart(this);

            Reset();

            Texture2D NewTexture()
            {
                Texture2D texture2D = new Texture2D(m_TemplateTextureWidth, m_TemplateTextureHeight, TextureFormat.ARGB32, false);
                texture2D.hideFlags  = HideFlags.DontSave;
                texture2D.filterMode = FilterMode.Bilinear;
                return texture2D;
            }
        }

        /// <summary>
        /// 重置已绘制的线
        /// </summary>
        public void Reset()
        {
            IsCompleted          = false;
            m_TotalDeltaPosition = default;
            m_DrawingEdge        = null;
            m_DrawThroughPoints.Clear();
            m_DrawThroughEdges.Clear();
            ResetGraphic();
            m_View.OnReset();
        }

        /// <summary>
        /// 释放相关资源
        /// </summary>
        public void Dispose()
        {
            Object.DestroyImmediate(m_DrawingTexture);
            Object.DestroyImmediate(m_View.DrawOnImage.gameObject.GetComponent<InteractionBehaviour>());
        }

        /// <summary>
        /// 成功
        /// </summary>
        private void Success()
        {
            if (IsCompleted) return;
            ClearGraphicDrawable();
            foreach (var path in m_Graphic.AllPath)
            {
                SetGraphicColorAtPosition(path, m_SuccessColor);
            }

            CommitGraphic();

            IsCompleted   = true;
            m_DrawingEdge = null;
            m_DrawThroughPoints.Clear();
            m_DrawThroughEdges.Clear();
            m_View.OnSuccess();
        }

        /// <summary>
        /// 失败
        /// </summary>
        private void Fail(int failType)
        {
            if (IsCompleted) return;
            ClearGraphicDrawable();
            foreach (var drawThroughEdge in m_DrawThroughEdges)
            {
                foreach (var path in drawThroughEdge.Path)
                {
                    SetGraphicColorAtPosition(path, m_FailedColor);
                }
            }

            if (GetDrawingEdge() != null)
            {
                var iterator = GetDrawingEdge().GetPathIterator(GetDrawingStartPoint());
                while (iterator.MoveNext())
                {
                    if (iterator.Current == m_DrawingPosition)
                    {
                        break;
                    }

                    SetGraphicColorAtPosition(iterator.Current, m_FailedColor);
                }
            }

            CommitGraphic();

            m_DrawingEdge = null;
            m_DrawThroughPoints.Clear();
            m_DrawThroughEdges.Clear();
            m_View.OnFailed(IsDrawFlag);
        }
    }
}