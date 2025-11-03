using UnityEngine;
using UnityEngine.EventSystems;

namespace OneLine
{
    partial class OneLineGame
    {
        private sealed class InteractionBehaviour : MonoBehaviour,
            IPointerDownHandler, IPointerUpHandler,
            IBeginDragHandler, IEndDragHandler, IDragHandler
        {
            private OneLineGame m_Game;
            private Vector2     m_PreviousPosition;

            public void Initialize(OneLineGame game)
            {
                m_Game = game;
            }

            public void OnPointerDown(PointerEventData eventData)
            {
                if (m_Game.IsDrawing)
                {
                    Debug.LogError("当前已经开始了绘制，但是又重新落笔了，这其中一定出了什么原因导致没有触发到OnEndDrag，需要检查为什么！但为保证游戏正常，会继续绘制。");
                    return;
                }

                m_Game.BeginDraw(eventData.position);
                m_PreviousPosition = eventData.position;
            }

            public void OnPointerUp(PointerEventData eventData)
            {
                if (!m_Game.IsDrawing) return;
                m_Game.Fail(0);
            }

            public void OnBeginDrag(PointerEventData eventData)
            {
            }

            public void OnEndDrag(PointerEventData eventData)
            {
            }

            public void OnDrag(PointerEventData eventData)
            {
                if (!m_Game.IsDrawing) return;
#if DEVELOPMENT_BUILD
                UnityEngine.Profiling.Profiler.BeginSample("OneLineDraw");
#endif
                Pixel deltaPosition = m_Game.ScreenPositionToGraphicPosition(eventData.position) - m_Game.ScreenPositionToGraphicPosition(m_PreviousPosition);
                if (deltaPosition == Pixel.Zero) return;
                m_PreviousPosition = eventData.position;
                m_Game.Draw(deltaPosition);
#if DEVELOPMENT_BUILD
                UnityEngine.Profiling.Profiler.EndSample();
#endif
            }
        }
    }
}