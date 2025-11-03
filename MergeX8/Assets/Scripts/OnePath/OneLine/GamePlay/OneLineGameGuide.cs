using System;
using UnityEngine;
using UnityEngine.UI;

namespace OneLine
{
    public class OneLineGameGuide
    {
        public OneLineGameGuide(OneLineGame game, RectTransform hand, RawImage drawImage)
        {
            m_Game      = game;
            m_Handle    = hand;
            m_DrawImage = drawImage;
        }

        private OneLineGame   m_Game;
        private RectTransform m_Handle;
        private RawImage      m_DrawImage;

        private OneLineGraphic.Point m_DrawingPoint;
        private OneLineGraphic.Edge  m_DrawingEdge;
        private Pixel                m_DrawingPosition;
        private float                m_Timer;
        private float                m_TotalOffset2;

        private const float Speed2 = 18000;
        private const float Delay  = 0.5f;
        private int LoopNum = 0;
        
        public void Start()
        {
            m_DrawImage.raycastTarget = false;
            m_TotalOffset2            = 0f;
            m_Handle.gameObject.SetActive(true);
            StartNewEdge(m_Game.Graphic.Points[0], m_Game.Graphic.Points[0].Edges[0]);

            m_Handle.anchoredPosition = m_DrawingPosition;
            m_Timer                   = Delay;
        }

        public bool MoveNext(float deltaTime)
        {
            if ((m_Timer -= deltaTime) > 0f) return true;

            m_TotalOffset2 += deltaTime * deltaTime * Speed2;
            var  iterator              = m_DrawingEdge.GetPathIterator(m_DrawingPoint);
            bool arriveDrawingPosition = false;
            while (iterator.MoveNext())
            {
                arriveDrawingPosition |= m_DrawingPosition == iterator.Current;
                if (arriveDrawingPosition == false)
                {
                    continue;
                }

                float currentOffset2 = (iterator.Current - m_DrawingPosition).sqrMagnitude;
                if (currentOffset2 <= m_TotalOffset2)
                {
                    m_TotalOffset2            -= currentOffset2;
                    m_Handle.anchoredPosition =  m_DrawingPosition = iterator.Current;
                    m_Game.SetGraphicColorAtPosition(m_DrawingPosition, m_Game.DrawColor);
                }
                else
                {
                    m_Game.CommitGraphic();
                    return true;
                }
            }

            m_Game.CommitGraphic();
            var endPoint = m_DrawingEdge.GetEndPoint(m_DrawingPoint);
            if (endPoint == m_Game.Graphic.Points[0])
            {
                OnStop();
                if (LoopNum > 0)
                {
                    LoopNum--;
                    Start();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            var nextEdge = endPoint.Edges[Array.IndexOf(endPoint.Edges, m_DrawingEdge) + 1];
            StartNewEdge(endPoint, nextEdge);
            return true;
        }

        private void OnStop()
        {
            m_DrawImage.raycastTarget = true;
            m_DrawingPoint            = null;
            m_DrawingEdge             = null;
            m_DrawingPosition         = new Pixel();
            m_Handle.gameObject.SetActive(false);
            m_Game.ResetGraphic();
        }

        private void StartNewEdge(OneLineGraphic.Point point, OneLineGraphic.Edge edge)
        {
            m_DrawingPoint    = point;
            m_DrawingEdge     = edge;
            m_DrawingPosition = point.Position;
        }
    }
}