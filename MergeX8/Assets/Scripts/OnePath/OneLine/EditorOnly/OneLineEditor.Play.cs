#if UNITY_EDITOR
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OneLine.Editor
{
    partial class OneLineEditor : IOneLineView
    {
        private OneLineGame m_Game;

        private void Start()
        {
            if (EditorApplication.isPlaying)
            {
                m_Game = new OneLineGame(new OneLineOrder()
                {
                    Graphic               = JsonUtility.FromJson<OneLineGraphic>(File.ReadAllText(GetConfigPath())),
                    Template              = m_TemplateTexture,
                    TemplateColor         = new Color(0.29f, 0.13f, 0.06f, 0.3f),
                    DrawColor             = new Color(0.2f, 0.28f, 0.33f, 1f),
                    SuccessColor          = new Color(0.15f, 0.82f, 0.07f, 1f),
                    FailedColor           = new Color(0.88f, 0.01f, 0.04f, 1f),
                    AdsorbToPointDistance = 5f,
                    UICamera              = Camera.current,
                });

                m_Game.Start(this);
            }
        }

        RawImage IOneLineView.DrawOnImage => m_Image;

        void IOneLineView.OnStart(OneLineGame game)
        {
        }

        void IOneLineView.OnBeginDraw(OneLineGraphic.Point startPoint)
        {
        }

        void IOneLineView.OnDraw(Pixel drawingPixel, float completeProgress)
        {
        }

        void IOneLineView.OnSuccess()
        {
            EditorWindow.focusedWindow.ShowNotification(TempContent("恭喜你完成了一场壮举！！！"));
            StartCoroutine(DelayReset(1f));
        }

        void IOneLineView.OnFailed(bool moveFlag)
        {
            EditorWindow.focusedWindow.ShowNotification(TempContent("菜"));
            StartCoroutine(DelayReset(1f));
        }

        void IOneLineView.OnReset()
        {
        }

        IEnumerator DelayReset(float delay)
        {
            yield return new WaitForSeconds(delay);
            m_Game.Reset();
        }
    }
}
#endif