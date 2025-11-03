using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using UnityEngine;

public partial class UIPopupTileMatchFailController
{
    #region FailTipBase
    public class FailTipsBase : MonoBehaviour
    {
        public Animator Animator;

        public virtual void Awake()
        {
            Animator = transform.GetComponent<Animator>();
        }

        public virtual Task Show()
        {
            gameObject.SetActive(true);
            // var canvasGroup = gameObject.AddComponent<CanvasGroup>();
            var task = new TaskCompletionSource<bool>();
            // canvasGroup.alpha = 0;
            // canvasGroup.DOFade(1, 0.2f).OnComplete(() =>
            // {
            //     Destroy(canvasGroup);
            //     task.SetResult(true);
            // });
            Animator.PlayAnimation("appear", () =>
            {
                task.SetResult(true);
            });
            return task.Task;
        }
        public virtual Task Hide()
        {
            // var canvasGroup = gameObject.AddComponent<CanvasGroup>();
            // canvasGroup.alpha = 1;
            // var task = new TaskCompletionSource<bool>();
            // canvasGroup.DOFade(0, 0.2f).OnComplete(() =>
            // {
            //     Destroy(canvasGroup);
            //     gameObject.SetActive(false);
            //     task.SetResult(true);
            // });
            Animator.PlayAnimation("disappear", () =>
            {
                gameObject.SetActive(false);
            });
            return Task.CompletedTask;
        }

        public virtual Task OnClickNext()
        {
            return Task.CompletedTask;
        }

        public virtual void Init()
        {
            gameObject.SetActive(false);
        }
    }
    #endregion
    public class FailTipsController
    {
        private List<FailTipsBase> _tipList;
        private int _showIndex;
        private UIPopupTileMatchFailController Controller;
        public FailTipsController(UIPopupTileMatchFailController controller)
        {
            _tipList = new List<FailTipsBase>();
            _showIndex = -1;
            Controller = controller;
        }

        public void PushTip<T>(T tipNode)where T:FailTipsBase
        {
            _tipList.Add(tipNode);
            tipNode.Init();
        }

        public void InitShow()
        {
            if (_tipList.Count == 0)
                return;
            _showIndex = 0;
            Controller.EnableClick(false);
            _tipList[_showIndex].Show().AddCallBack(
                ()=>Controller.EnableClick(true)
                ).WrapErrors();
        }

        public async Task<bool> ShowNext()
        {
            Controller.EnableClick(false);
            var oldTip = _tipList[_showIndex];
            if (_showIndex + 1 > _tipList.Count - 1)
            {
                await oldTip.OnClickNext();
                Controller.EnableClick(true);
                return false;
            }
            _showIndex++;
            var newTip = _tipList[_showIndex];
            Debug.LogWarning("oldTip="+oldTip.GetType());
            Debug.LogWarning("newTip="+newTip.GetType());
            await oldTip.OnClickNext();
            await oldTip.Hide();
            await newTip.Show();
            Controller.EnableClick(true);
            return true;
        }
    }
    
    private FailTipsController _failTipsController;

    public void CheckExtraFailTip(FailTipsController failTipsController)
    {
    }
}