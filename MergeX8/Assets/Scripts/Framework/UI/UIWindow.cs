using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Framework;
using DragonPlus;
using DragonU3DSDK;
using UnityEngine.Serialization;

public abstract class UIWindowData
{
}

public abstract class UIWindow : MonoBehaviour
{
    public Canvas canvas = null;
    public CanvasGroup canvasGroup = null;
    //public Canvas[] childCanvas = null;
    
    public Action UIcloseAction;
    private string _mWindowPath;

    // 是否播放默认对话框的声音
    public bool isPlayDefaultAudio = true;

    protected Animator _animator;
    public string windowPath
    {
        set { _mWindowPath = value; }
        get { return _mWindowPath; }
    }

    [FormerlySerializedAs("m_WindowType")] [HideInInspector] public UIWindowType windowType;

    //[HideInInspector]
    [FormerlySerializedAs("uiWindowLayer")] public UIWindowLayer windowLayer;

    [HideInInspector] public bool mIsOpen = false;

    public bool IsWindowOpened
    {
        get { return mIsOpen; }
    }

    protected bool canClickMask = false;

    void Awake()
    {
        InitCanvas();

        _animator = transform.GetComponent<Animator>();
        
        gameObject.GetOrCreateComponent<GraphicRaycaster>();

        //todo : it is only a i18n check, remove it..
        Text[] labels = this.transform.GetComponentsInChildren<Text>(true);
        foreach (Text label in labels)
        {
            //Debug.Log("find label : /"+label.text+"/");
            if (LocalizationManager.Instance.TryGetLocalizedString(label.text.Trim(), out var result))
            {
                label.text = result;
            }
//            else
//            {
//                if (Regex.IsMatch(label.text, "[a-zA-Z]+"))
//                {
//                    DebugUtil.LogError($"[{name}:{label.name}] cannot get i18n, text:[{label.text}] " +
//                                       $"if it is a local text, use locale text pro instead and add i18n");
//                }
//            }

            //label.text = DragonPlus.LocalizationManager.Instance.GetLocalizedString(label.text.Trim());
        }

        PrivateAwake();
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(CommonUtils.DelayWork(0.5f, () => { canClickMask = true; }));
    }

    public void InitCanvas()
    {
        if (canvas != null)
            return;

        canvas = gameObject.GetOrCreateComponent<Canvas>();
        canvas.overrideSorting = true;
    }
    // void Start()
    // {
    // }

    public void OpenWindow(params object[] objs)
    {
        ShowUI();
        OnOpenWindow(objs);
        OpenWindowAudio();
    }

    protected virtual void OpenWindowAudio()
    {
        if (isPlayDefaultAudio)
        {
            if (_mWindowPath.Contains("Popup"))
            {
                DragonPlus.AudioManager.Instance.PlaySound(SfxNameConst.sfx_pop_open);
            }
            else
            {
                DragonPlus.AudioManager.Instance.PlaySound(SfxNameConst.panel_in);
            }
        }
    }

    void CloseWindowAudio()
    {
        if (isPlayDefaultAudio && mIsOpen)
        {
            // if (_mWindowPath.Contains("Popup"))
            // {
            //     DragonPlus.AudioManager.Instance.PlaySound(SfxNameConst.sfx_pop_close);
            // }
            // else
            // {
            //     DragonPlus.AudioManager.Instance.PlaySound(SfxNameConst.panel_out);
            // }
        }
    }

    public void CloseWindow(bool destroy = false)
    {
        OnCloseWindow(destroy);
        CloseWindowAudio();
        //Global.hideZhezhao();

        if (destroy)
        {
            DestroyUI();
        }
        else
        {
            HideUI();
        }

    }

    public virtual bool OnBack()
    {
        DragonPlus.AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        return UIManager.Instance.CloseUI(_mWindowPath, true);
    }

    // public void CloseWindowWithinUIMgr(bool destroy = false)
    // {
    //     UIManager.Instance.CloseUI(_mWindowPath, destroy);
    // }
    public void CloseWindowWithinUIMgr(bool destroy = false, Action afterCloseFunc = null, Action beforeCloseFunc = null) 
    {
        beforeCloseFunc?.Invoke();
        UIManager.Instance.CloseUI(_mWindowPath, destroy);
        afterCloseFunc?.Invoke();
    }

    public void ReloadWindow()
    {
        ShowUI();
        OnReloadWindow();
    }

    private void ShowUI()
    {
        mIsOpen = true;
        if (gameObject == null)
        {
            Debug.Log("UI已被销毁:" + windowPath);
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

    #region 子类继承重写

    /// <summary>
    /// 打开界面时调用
    /// </summary>
    /// <param name="objs"></param>
    protected virtual void OnOpenWindow(params object[] objs)
    {
    }

    /// <summary>
    /// 关闭界面时调用
    /// </summary>
    protected virtual void OnCloseWindow(bool destroy = false)
    {
    }

    /// <summary>
    /// 重新加载界面时调用
    /// </summary>
    protected virtual void OnReloadWindow()
    {
    }

    /// <summary>
    /// 私有Awake方法，会在基类Awake执行后调用
    /// </summary>
    public abstract void PrivateAwake();

    public GameObject BindEvent(string target, GameObject par = null, Action<GameObject> action = null,
        bool playAudio = true)
    {
        if (par == null)
        {
            par = this.gameObject;
        }

        GameObject obj = par?.transform.Find(target)?.gameObject;
        if (obj != null)
        {
            Button button = obj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener
                (
                    delegate()
                    {
                        if (playAudio)
                        {
                            AudioManager.Instance.PlaySound(SfxNameConst.button_s);
                        }

                        action?.Invoke(obj);
                    }
                );
            }
        }
        else
        {
            DragonU3DSDK.DebugUtil.LogError("未找到　" + gameObject.name + "/" + target);
        }

        return obj;
    }

    public GameObject FindObj(string path, GameObject par = null)
    {
        if (par == null)
        {
            par = this.gameObject;
        }

        GameObject obj = par.transform.Find(path)?.gameObject;
        return obj;
    }

    #endregion

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
                    AudioManager.Instance.PlaySound(SfxNameConst.button_s);
                action?.Invoke(obj);
            });
        }
        else
        {
            DragonU3DSDK.DebugUtil.LogError("未找到　" + gameObject.name + "/" + target);
        }

        return obj;
    }

    public virtual void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        AnimCloseWindow();
    }

    public void SetCanClickMask(bool canClick)
    {
        canClickMask = canClick;
    }
    
    public virtual void AnimCloseWindow(Action afterCloseFunc = null, bool destroy = true, Action beforeCloseFunc = null)
    {
        if (_animator == null)
        {
            CloseWindowWithinUIMgr(destroy, afterCloseFunc, beforeCloseFunc);
            return;
        }
        
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(destroy, afterCloseFunc, beforeCloseFunc);
        }));
    }
    
    public virtual void UpdateCanvasSortOrder()
    {
    }
}