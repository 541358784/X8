using System.Collections.Generic;
using UnityEngine;

namespace OneLine
{
    partial class OneLineGame
    {
        #region Fields

        /// <summary>
        /// 当前绘制的贴图
        /// </summary>
        private Texture2D m_DrawingTexture;

        /// <summary>
        /// 绘制经过的点
        /// </summary>
        private List<OneLineGraphic.Point> m_DrawThroughPoints = new List<OneLineGraphic.Point>();

        /// <summary>
        /// 绘制过的边
        /// </summary>
        private List<OneLineGraphic.Edge> m_DrawThroughEdges = new List<OneLineGraphic.Edge>();

        /// <summary>
        /// 绘制中的边
        /// </summary>
        private OneLineGraphic.Edge m_DrawingEdge;

        /// <summary>
        /// 当前绘制的位置
        /// </summary>
        private Pixel m_DrawingPosition;

        /// <summary>
        /// 累计的绘制偏移量
        /// </summary>
        private Pixel m_TotalDeltaPosition;

        /// <summary>
        /// 用于绘制的贴图颜色数组
        /// </summary>
        private Color[] m_DrawingPixels;

        /// <summary>
        /// 用于绘制的像素是否已经确认不改变了
        /// </summary>
        private bool[] m_DrawingPixelsDontChangeFlag;

        public bool IsDrawFlag;

        #endregion

        #region Key Events

        /// <summary>
        /// 触发开始绘制
        /// </summary>
        /// <param name="startScreenPosition">起点屏幕坐标</param>
        private void BeginDraw(Vector2 startScreenPosition)
        {
            if (IsDrawing)
            {
                return;
            }

            OneLineGraphic.Point startPoint = GetNearestPoint(ScreenPositionToGraphicPosition(startScreenPosition));
            m_DrawThroughPoints.Add(startPoint);
            m_View.OnBeginDraw(startPoint);
            m_DrawingPosition = startPoint.Position;
            IsDrawFlag = false;
        }

        /// <summary>
        /// 绘制线条到目标位置
        /// </summary>
        /// <param name="deltaPosition">偏移值</param>
        private void Draw(Pixel deltaPosition)
        {
            if (!IsDrawing)
            {
                return;
            }

            m_TotalDeltaPosition += deltaPosition;
            Pixel targetPosition = m_DrawingPosition;
            targetPosition.x += m_TotalDeltaPosition.x;
            targetPosition.y += m_TotalDeltaPosition.y;

            OneLineGraphic.Edge  drawingEdge       = GetDrawingEdge();
            OneLineGraphic.Point drawingStartPoint = GetDrawingStartPoint();
            bool                 isEdgeCompleted   = false;

            // 不存在或已完成当前绘制中的边，则表示：当前笔触停留在某个点，没有从该点开始沿任意边进行绘制
            if (drawingEdge == null || m_DrawThroughEdges.Contains(drawingEdge))
            {
                // 移动距离过小，还在起点附近，什么都不做
                if (IsVeryApproachPoint(drawingStartPoint, targetPosition))
                {
                    return;
                }

                // 找到【从起点出发，且离即将绘制位置最近】的边，记录将会绘制的边
                drawingEdge = GetNearestUnusedEdgeToPosition(drawingStartPoint.Edges, targetPosition);
                // 当前已经无边可画了
                if (drawingEdge == null)
                {
                    Fail(1);
                    return;
                }

                BeginDrawEdge(drawingEdge);
            }

            OneLineGraphic.Point drawingEndPoint = drawingEdge.GetEndPoint(drawingStartPoint);

            // 把目标点设置到边上
            targetPosition = drawingEdge.GetBestOffsetPositionOnPath(drawingStartPoint, m_DrawingPosition, m_TotalDeltaPosition);

            // 如果偏移量过小，不足以移动到下一个路径点，则什么都不做，让deltaPosition累计到能够移动位置
            if (targetPosition == m_DrawingPosition)
            {
                Debug.LogError("m_TotalDeltaPosition : " + m_TotalDeltaPosition);
                return;
            }

            // 绘制回到了起点，则取消当前绘制中的边
            if (IsVeryApproachPoint(drawingStartPoint, targetPosition))
            {
                CancelDrawEdge(drawingEdge);
                // 把目标点设置回起点，并清空当前累计值
                targetPosition = drawingStartPoint.Position;
            }
            // 接近终点了，则完成当前绘制中的边
            else if (IsVeryApproachPoint(drawingEndPoint, targetPosition))
            {
                CompleteDrawEdge(drawingEdge, drawingStartPoint, drawingEndPoint);
                // 把目标点设置到终点
                targetPosition  = drawingEndPoint.Position;
                isEdgeCompleted = true;
            }

            // 无论如何，把当前绘制中的边绘制到目标点，并清空当前累计的位置变化量
            ExecuteDrawEdge(drawingEdge, drawingStartPoint, targetPosition);

            // 记录当前位置，清空位移累计值
            m_DrawingPosition    = targetPosition;
            m_TotalDeltaPosition = default;

            // 刷新进度
            int drawPixelCount = drawingEdge.GetPixelIndex(drawingStartPoint, targetPosition) + 1;
            foreach (var drawThroughEdge in m_DrawThroughEdges)
            {
                if (drawThroughEdge == drawingEdge) continue; // 避免刚好完成该边时，重复统计了该边
                // 去掉末尾 因为末尾是下一条边的起点，下一条边会把他算进去
                drawPixelCount += drawThroughEdge.Path.Length - 1;
            }

            m_View.OnDraw(m_DrawingPosition, drawPixelCount * 1f / m_Graphic.AllPath.Length);

            if (isEdgeCompleted == false)
            {
                return;
            }

            // 标记该边经过的像素不可再改色
            SetEdgeNotDrawable(drawingEdge);

            // 如果所有的边都绘制了，说明成功了
            if (m_DrawThroughEdges.Count == m_Graphic.Edges.Length)
            {
                Success();
            }
            // 如果结束点所有边都绘制了，说明无路可走了，则失败 
            else
            {
                foreach (OneLineGraphic.Edge edge in drawingEndPoint.Edges)
                {
                    if (m_DrawThroughEdges.Contains(edge) == false)
                    {
                        return;
                    }
                }

                Fail(2);
            }
        }

        /// <summary>
        /// 开始绘制边
        /// </summary>
        private void BeginDrawEdge(OneLineGraphic.Edge edge)
        {
            m_DrawingEdge = edge;
        }

        /// <summary>
        /// 取消绘制边
        /// </summary>
        private void CancelDrawEdge(OneLineGraphic.Edge edge)
        {
            m_DrawingEdge        = null;
            m_TotalDeltaPosition = default;
        }

        /// <summary>
        /// 完成绘制边
        /// </summary>
        private void CompleteDrawEdge(OneLineGraphic.Edge edge, OneLineGraphic.Point startPoint, OneLineGraphic.Point endPoint)
        {
            // 记录
            m_DrawThroughPoints.Add(endPoint);
            m_DrawThroughEdges.Add(m_DrawingEdge);

            // 清空当前边相关缓存
            m_DrawingEdge        = null;
            m_TotalDeltaPosition = default;
        }

        /// <summary>
        /// 执行绘制边
        /// </summary>
        private void ExecuteDrawEdge(OneLineGraphic.Edge edge, OneLineGraphic.Point startPoint, Pixel targetPosition)
        {
            IsDrawFlag = true;
#if DEVELOPMENT_BUILD
            UnityEngine.Profiling.Profiler.BeginSample("ExecuteDrawEdge");
#endif
            // 从末尾开始画
            int indexOfTargetPosition  = edge.GetPixelIndex(startPoint, targetPosition);
            int indexOfCurrentPosition = edge.GetPixelIndex(startPoint, m_DrawingPosition);
            if (indexOfTargetPosition > indexOfCurrentPosition)
            {
                for (int i = indexOfCurrentPosition; i <= indexOfTargetPosition; i++)
                {
                    SetGraphicColorAtPosition(edge.GetPixelAtIndex(startPoint, i), m_DrawColor);
                }
            }
            else
            {
                for (int i = indexOfTargetPosition + 1; i <= indexOfCurrentPosition; i++)
                {
                    SetGraphicColorAtPosition(edge.GetPixelAtIndex(startPoint, i), m_TemplateColor);
                }
                
                SetGraphicColorAtPosition(edge.GetPixelAtIndex(startPoint, indexOfTargetPosition), m_DrawColor);
            }

            CommitGraphic();
#if DEVELOPMENT_BUILD
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }

        #endregion

        #region Utility

        /// <summary>
        /// 当前绘制起点
        /// </summary>
        private OneLineGraphic.Point GetDrawingStartPoint() => m_DrawThroughPoints.Count > 0 ? m_DrawThroughPoints[m_DrawThroughPoints.Count - 1] : null;

        /// <summary>
        /// 当前绘制的边
        /// </summary>
        /// <returns></returns>
        private OneLineGraphic.Edge GetDrawingEdge() => m_DrawingEdge;

        /// <summary>
        /// 获取目标位置最近的点
        /// </summary>
        /// <param name="position">目标位置</param>
        private OneLineGraphic.Point GetNearestPoint(Pixel position)
        {
            OneLineGraphic.Point result   = null;
            float                distance = float.MaxValue;
            for (int i = 0; i < m_Graphic.Points.Length; i++)
            {
                var temp = (m_Graphic.Points[i].Position - position).sqrMagnitude;
                if (temp < distance)
                {
                    distance = temp;
                    result   = m_Graphic.Points[i];
                }
            }

            return result;
        }

        /// <summary>
        /// 找到最近的边
        /// </summary>
        /// <param name="edges">边</param>
        /// <param name="position">目标点</param>
        private OneLineGraphic.Edge GetNearestUnusedEdgeToPosition(OneLineGraphic.Edge[] edges, Pixel position)
        {
            OneLineGraphic.Edge result   = null;
            int                 distance = int.MaxValue;
            for (int i = 0; i < edges.Length; i++)
            {
                var tempEdge = edges[i];
                if (m_DrawThroughEdges.Contains(tempEdge))
                {
                    continue;
                }

                var tempDistance = tempEdge.GetSmallestDistance(position);
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    result   = tempEdge;
                }
            }

            return result;
        }

        /// <summary>
        /// 目标位置是否十分接近目标点
        /// </summary>
        /// <param name="point">目标点</param>
        /// <param name="position">目标位置</param>
        private bool IsVeryApproachPoint(OneLineGraphic.Point point, Pixel position)
        {
            return (point.Position - position).sqrMagnitude <= m_AdsorbToPointDistance;
        }

        /// <summary>
        /// 屏幕坐标转到图像绘制坐标
        /// </summary>
        private Pixel ScreenPositionToGraphicPosition(Vector2 screenPosition)
        {
            var imageRect = m_View.DrawOnImage.rectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(imageRect, screenPosition, m_UICamera, out Vector2 result);
            result += imageRect.pivot * new Vector2(m_TemplateTextureWidth, m_TemplateTextureHeight);
            Pixel position = new Pixel((int) result.x, (int) result.y);
            return position;
        }

        #endregion

        #region Draw

        /// <summary>
        /// 设置图像目标点颜色
        /// </summary>
        public void SetGraphicColorAtPosition(Pixel position, Color color)
        {
#if DEVELOPMENT_BUILD
            UnityEngine.Profiling.Profiler.BeginSample("SetGraphicColorAtPosition");
#endif
            foreach (Pixel brushPixel in m_Graphic.BrushArea)
            {
#if DEVELOPMENT_BUILD
                UnityEngine.Profiling.Profiler.BeginSample("CheckPixelChangeable");
#endif
                int index = (brushPixel.x + position.x) + (brushPixel.y + position.y) * m_TemplateTextureWidth;
                if (m_DrawingPixelsDontChangeFlag[index])
                {
                    continue;
                }
#if DEVELOPMENT_BUILD
                UnityEngine.Profiling.Profiler.EndSample();
#endif

#if DEVELOPMENT_BUILD
                UnityEngine.Profiling.Profiler.BeginSample("SetPixel");
#endif
                Color targetColor = m_TemplateTexture[index];
                targetColor.r          *= color.r;
                targetColor.g          *= color.g;
                targetColor.b          *= color.b;
                targetColor.a          *= color.a;
                m_DrawingPixels[index] =  targetColor;
#if DEVELOPMENT_BUILD
                UnityEngine.Profiling.Profiler.EndSample();
#endif
            }
#if DEVELOPMENT_BUILD
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }

        /// <summary>
        /// 重置图像
        /// </summary>
        public void ResetGraphic()
        {
            ClearGraphicDrawable();

            foreach (var path in m_Graphic.AllPath)
            {
                SetGraphicColorAtPosition(path, m_TemplateColor);
            }

            CommitGraphic();
        }

        /// <summary>
        /// 提交图像
        /// </summary>
        public void CommitGraphic()
        {
#if DEVELOPMENT_BUILD
            UnityEngine.Profiling.Profiler.BeginSample("CommitGraphic");
#endif
            m_DrawingTexture.SetPixels(m_DrawingPixels);
            m_DrawingTexture.Apply(false);
            m_View.DrawOnImage.SetMaterialDirty();
#if DEVELOPMENT_BUILD
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }

        /// <summary>
        /// 情况绘制图像不可绘制标记
        /// </summary>
        private void ClearGraphicDrawable()
        {
            for (var i = 0; i < m_DrawingPixelsDontChangeFlag.Length; i++)
            {
                m_DrawingPixelsDontChangeFlag[i] = false;
            }
        }

        /// <summary>
        /// 设置边不可绘制
        /// </summary>
        private void SetEdgeNotDrawable(OneLineGraphic.Edge drawingEdge)
        {
            foreach (var pixel in drawingEdge.Path)
            {
                foreach (var brushArea in m_Graphic.BrushArea)
                {
                    int index = (pixel.x + brushArea.x) + (pixel.y + brushArea.y) * m_TemplateTextureWidth;
                    m_DrawingPixelsDontChangeFlag[index] = true;
                }
            }
        }

        #endregion
    }
}