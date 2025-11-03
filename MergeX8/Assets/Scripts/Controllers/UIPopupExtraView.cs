using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.UI
{
    public interface IUIPopupExtraView
    {
        public bool CanShow();
    }
    public abstract class UIPopupExtraView:MonoBehaviour,IUIPopupExtraView
    {
        public abstract bool CanShow();
        public abstract void Init();
        public static readonly Dictionary<Type, List<UIPopupExtraView>> ExtraViewPool = new Dictionary<Type, List<UIPopupExtraView>>();
        private void Awake()
        {
            var type = this.GetType();
            if (!ExtraViewPool.TryGetValue(type,out var extraViewList))
            {
                extraViewList = new List<UIPopupExtraView>();
                ExtraViewPool.Add(type,extraViewList);
            }
            extraViewList.Add(this);
        }
        private void OnDestroy()
        {
            var type = this.GetType();
            if (ExtraViewPool.TryGetValue(type,out var extraViewList))
            {
                extraViewList.Remove(this);
            }
        }

        public static bool CheckExtraViewOpenState<T>()where T:UIPopupExtraView
        {
            var existExtraView = false;
            var type = typeof(T);
            if (ExtraViewPool.TryGetValue(type, out var extraViewList))
            {
                foreach (var extraView in extraViewList)
                {
                    if (extraView.CheckOpenState())
                        existExtraView = true;       
                }
            }
            return existExtraView;
        }
        public bool CheckOpenState()
        {
            if (!this)
                return false;
            if (!gameObject.activeInHierarchy)
            {
                return false;
            }
            if (CanShow())
            {
                return false;
            }
            _onViewCanNotShow?.Invoke();
            return true;
        }

        private Action _onViewCanNotShow;
        public void OnViewCanNotShow(Action refreshAction)
        {
            _onViewCanNotShow = refreshAction;
        }
    }
}