using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using System.Threading.Tasks;
// using Common.UI;
using DragonU3DSDK;
// using GameGuide;
// using OutsideGuide;

namespace TMatch
{


    public abstract class UIWindowData
    {
    }

    public abstract class UIWindow : MonoBehaviour
    {
        private bool isShowItemBar;

        public Canvas Canvas;
        protected GraphicRaycaster graphicRaycaster;
        public virtual int JumpOrder => 1; //启动此UI后，跳跃层级

        // 是否播放默认的打开对话框的声音
        public bool isPlayDefaultOpenAudio = true;

        // 是否播放默认的关闭对话框的声音
        public bool isPlayDefaultCloseAudio = true;

        public bool OnSnaping;
        public bool Pausing;

        public UIWidgets widgets;

        public string WindowName { get; set; }

        public bool IsWindowOpened => mIsOpen;

        [HideInInspector] public bool mIsOpen = false;

        public virtual UIWindowType WindowType => UIWindowType.Popup;
        public virtual UIWindowLayer WindowLayer => UIWindowLayer.Normal;

        protected virtual bool IsTouchBgClose => false;

        protected virtual bool IsChildView => false;

        public virtual bool UseFixedSort => false;
        public virtual string FixedSortName => "Default";
        public virtual int FixedSortOrder => 0;

        /// <summary>
        /// 是否计入任务阻塞
        /// </summary>
        public virtual bool IsUsedInTaskChokedEvent => true;

        /// <summary>
        /// 触发UI动画
        /// 主界面，资源栏
        /// </summary>
        public virtual bool EffectUIAnimation { get; set; } = true;

        protected virtual List<string> TaskCheckName => new List<string>();

        public virtual void OnPause(UIWindow fromWindow)
        {
        }

        public virtual void OnResume()
        {
        }

        public virtual void OnSnap(UIWindow fromWindow)
        {
        }

        public virtual void OnRestoreSnap()
        {
        }

        /// 打开界面时调用
        protected virtual void OnOpenWindow(UIWindowData data)
        {
        }

        /// 关闭界面时调用
        protected virtual void OnCloseWindow(bool destroy = true)
        {
        }

        //界面动画
        protected virtual Animator WindowAnimator { set; get; }
        protected virtual string ShowAnimationName => "appear";
        protected virtual string HideAnimationName => "disappear";

        protected virtual bool HideMain => true;

        protected virtual bool PlayRootAnimation => true;

        /// <summary>
        /// 私有Awake方法，会在基类Awake执行后调用
        /// </summary>
        public abstract void PrivateAwake();

        protected virtual void Awake()
        {
            widgets = gameObject.GetComponent<UIWidgets>();

            if (!IsChildView)
            {
                Canvas = gameObject.GetOrCreateComponent<Canvas>();
                Canvas.overrideSorting = true;
                graphicRaycaster = gameObject.GetOrCreateComponent<GraphicRaycaster>();
            }

            WindowAnimator = GetComponent<Animator>();

            var labels = this.transform.GetComponentsInChildren<Text>(true);

            foreach (var label in labels)
            {
                label.text = LocalizationManager.Instance.GetLocalizedString(label.text.Trim());
            }

            if (IsTouchBgClose)
            {
                var bg = gameObject.FindChild("BG");
                if (bg != null) UGUIEventListener.Get(bg).onClick = OnClickBG;
            }

            PrivateAwake();
        }

        public async void OpenWindow(UIWindowData data)
        {
            if (graphicRaycaster != null) graphicRaycaster.enabled = true;
            // if (IsUsedInTaskChokedEvent) TaskSystem.Model.Instance.ChangeChokeTaskEvent(true, name);
            if (HideMain)
                EventDispatcher.Instance.DispatchEventImmediately(new BaseEvent(EventEnum.OpenUIWindow, this));
            ShowUI();
            OnOpenWindow(data);
            OpenWindowAudio();
            if (WindowAnimator && PlayRootAnimation)
            {
                if (!string.IsNullOrEmpty(ShowAnimationName))
                {
                    var animTime = (int) (1000f * CommonUtils.GetAnimTime(WindowAnimator, ShowAnimationName));
                    if (animTime > 0)
                    {
                        WindowAnimator.Play(ShowAnimationName);
                        await Task.Delay(animTime);
                    }
                }
            }

            OnOpenWindowCompleted();

            /*if (WindowAnimator && !string.IsNullOrEmpty(ShowAnimationName))
            {
                WindowAnimator.Play(ShowAnimationName);
            }*/
        }

        protected virtual void OnOpenWindowCompleted()
        {

        }

        protected virtual void OpenWindowAudio()
        {
            if (isPlayDefaultOpenAudio)
            {
                DebugUtil.Log($"Play window {name} open audio");
                AudioManager.Instance.PlaySound(SfxNameConst.UITabOpen);
            }
        }

        void CloseWindowAudio()
        {
            if (isPlayDefaultCloseAudio && mIsOpen)
            {
                DebugUtil.Log($"Play window {name} close audio");
                AudioManager.Instance.PlaySound(SfxNameConst.UITabClose);
            }
        }

        public async void Internal_CloseWindow(bool destroy = true)
        {
            if (graphicRaycaster != null) graphicRaycaster.enabled = false;
            if (WindowAnimator && PlayRootAnimation)
            {
                if (!string.IsNullOrEmpty(HideAnimationName))
                {
                    var animTime = (int) (1000f * CommonUtils.GetAnimTime(WindowAnimator, HideAnimationName));
                    if (animTime > 0)
                    {
                        WindowAnimator.Play(HideAnimationName);
                        await Task.Delay(animTime);
                    }
                }
            }

            OnCloseWindow(destroy);
            CloseWindowAudio();
            TryHideItemBar();
            if (HideMain)
                EventDispatcher.Instance.DispatchEventImmediately(new BaseEvent(EventEnum.CloseUIWindow, this));

            if (destroy)
            {
                DestroyUI();
            }
            else
            {
                HideUI();
            }

            // for (int i = 0; i < TaskCheckName.Count; i++)
            // {
            //     EventDispatcher.Instance.DispatchEvent(new BaseEvent(EventEnum.TaskStepComplete, TaskCheckName[i]));
            // }
            //
            // if (IsUsedInTaskChokedEvent) TaskSystem.Model.Instance.ChangeChokeTaskEvent(false, name);
        }

        public virtual bool OnBack()
        {
            if (WindowType != UIWindowType.FullScreen)
            {
                OnBackButtonCallBack();
                return true;
            }

            return false;
        }

        protected virtual void OnClickBG(GameObject go)
        {
            PlayBtnTapSound();
            CloseWindowWithinUIMgr();
        }

        public void CloseWindowWithinUIMgr(bool destroy = true, bool resumeFullWindow = true)
        {
            UIManager.Instance.InternalCloseWindow(GetType(), destroy, resumeFullWindow);
        }

        private void ShowUI()
        {
            mIsOpen = true;
            if (!gameObject)
            {
                DebugUtil.Log("UI已被销毁:" + WindowName);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        private void HideUI()
        {
            mIsOpen = false;
            gameObject.SetActive(false);
        }

        private void DestroyUI()
        {
            mIsOpen = false;
            Destroy(gameObject);
        }

        protected virtual void OnBackButtonCallBack()
        {
            if (!string.IsNullOrEmpty(WindowName))
                Debug.LogError($"返回键触发,当前界面为:{WindowName}");
        }

        public GameObject BindEvent(string target, Action action = null, bool playAudio = true)
        {
            return BindEvent(target, null, (obj) => action?.Invoke(), playAudio);
        }

        public GameObject BindEvent(string target, GameObject par = null, Action<GameObject> action = null,
            bool playAudio = true)
        {
            if (!par) par = gameObject;

            var obj = par.transform.Find(target).gameObject;
            if (obj != null)
            {
                var button = obj.GetComponent<Button>();
                if (button)
                {
                    button.onClick.AddListener
                    (
                        delegate()
                        {
                            if (playAudio)
                            {
                                AudioManager.Instance.PlaySound(SfxNameConst.UIBtnTap);
                            }

                            action?.Invoke(obj);
                        }
                    );
                }
            }
            else
            {
                DebugUtil.LogError("未找到　" + gameObject.name + "/" + target);
            }

            return obj;
        }

        public GameObject FindObj(string path, GameObject par = null)
        {
            if (!par) par = gameObject;

            var obj = par.transform.Find(path).gameObject;
            return obj;
        }

        public override bool Equals(object obj)
        {
            return obj is UIWindow window && WindowName == window.WindowName;
        }

        /// <summary>
        /// 展示ItemBar
        /// </summary>
        /// <param name="left1">左1</param>
        /// <param name="left2">左2</param>
        /// <param name="right">右1</param>
        /// <param name="style">风格</param>
        protected void ShowItemBar(ItemBar.Data left1 = null, ItemBar.Data left2 = null, ItemBar.Data right = null,
            ItemBarModel.ItemBarStyle style = ItemBarModel.ItemBarStyle.Normal)
        {
            isShowItemBar = true;
            // ItemBarModel.Instance.Show(this, left1, left2, right, style);
        }

        // /// <summary>
        // /// 显示默认的ItemBar
        // /// </summary>
        // /// <param name="style">风格</param>
        // protected void ShowDefaultItemBar(ItemBarModel.ItemBarStyle style = ItemBarModel.ItemBarStyle.Normal)
        // {
        //     var diamondData = new ItemBar.Data((int) ResourceId.Gem);
        //     var coinData = new ItemBar.Data((int) ResourceId.Coin);
        //     var lifeData = new ItemBar.Data((int) ResourceId.Energy);
        //     ShowItemBar(coinData, diamondData, lifeData, style);
        // }

        /// <summary>
        /// 隐藏ItemBar
        /// </summary>
        protected void HideItemBar()
        {
            ShowItemBar();
        }

        /// <summary>
        /// 尝试隐藏ItemBar
        /// </summary>
        private void TryHideItemBar()
        {
            if (isShowItemBar)
            {
                isShowItemBar = false;
                ItemBarModel.Instance.Hide(this);
            }
        }

        /// <summary>
        /// 播放按钮点击音效
        /// </summary>
        protected void PlayBtnTapSound()
        {
            AudioManager.Instance.PlayBtnTap();
        }

        public GameObject BindClick(string target, Action<GameObject> action = null, GameObject par = null,
            bool playAudio = true)
        {
            if (par == null)
            {
                par = this.gameObject;
            }

            GameObject obj = par.transform.Find(target)?.gameObject;
            if (obj != null)
            {
                Button button = obj.GetComponent<Button>();
                if (button == null)
                {
                    button = obj.AddComponent<Button>();
                    obj.AddComponent<CKEmptyRaycast>();
                }

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    if (playAudio)
                        AudioManager.Instance.PlaySound(SfxNameConst.UIBtnTap);
                    action?.Invoke(obj);
                });
            }
            else
            {
                DragonU3DSDK.DebugUtil.LogError("未找到　" + gameObject.name + "/" + target);
            }

            return obj;
        }
    }
}