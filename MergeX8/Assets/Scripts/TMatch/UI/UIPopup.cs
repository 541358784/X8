/*
 * 弹窗类UI
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMatch
{
    public class UIPopup : UIView
    {
        public virtual Animator Animator => transform.GetComponent<Animator>();

        public virtual string OpenAnimStateName => "PopupAppear"; //打开UI时的动画
        public virtual string CloseAnimStateName => "UIDisappear"; //关闭UI时的动画

        public virtual string OpenAudio => SfxNameConst.panel_in; //打开UI时的音效
        public virtual string CloseAudio => SfxNameConst.panel_out; //关闭UI时的音效

        public virtual Action EmptyCloseAction => null; //底部空白区域点击后的响应

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);
            RegistEmptyClose();
            if (!string.IsNullOrEmpty(OpenAudio)) AudioSysManager.Instance.PlaySound(OpenAudio);
            if (Animator != null && !string.IsNullOrEmpty(OpenAnimStateName)) Animator.Play(OpenAnimStateName);

            EventDispatcher.Instance.DispatchEvent(EventEnum.POPUP_OPEN);
        }

        public override async Task OnViewClose()
        {
            Image bg = transform.GetComponent<Image>();
            if (bg) bg.raycastTarget = false;

            EventDispatcher.Instance.DispatchEvent(EventEnum.POPUP_CLOSE);

            if (!string.IsNullOrEmpty(CloseAudio)) AudioSysManager.Instance.PlaySound(CloseAudio);
            if (Animator != null && !string.IsNullOrEmpty(CloseAnimStateName))
            {
                Animator.Play(CloseAnimStateName);
                AnimatorClipInfo[] clipInfos = Animator.GetCurrentAnimatorClipInfo(0);
                if (clipInfos.Length > 0)
                {
                    await Task.Delay((int)(clipInfos[0].clip.length * 1000.0f));
                }
            }

            await base.OnViewClose();
        }

        protected virtual void RegistEmptyClose()
        {
            if (EmptyCloseAction != null)
            {
                UIPopupBGClickHandler clickHandler = gameObject.AddComponent<UIPopupBGClickHandler>();
                clickHandler.handle = EmptyCloseAction;
            }
        }
    }
}