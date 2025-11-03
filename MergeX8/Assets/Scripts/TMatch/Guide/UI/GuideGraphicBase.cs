using UnityEngine;

namespace OutsideGuide
{
    public abstract class GuideGraphicBase : MonoBehaviour
    {
        protected virtual void Awake()
        {
            Init();
        }

        protected abstract void Init();

        public abstract bool IsShow();
        public abstract void Show();
        public abstract void Hide();
    }
}