using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class WaitingController : UIWindow
{
    private Image ImageImage { get; set; }
    private Image bg { get; set; }
    private readonly Vector3 Axis = new Vector3(0, 0, 1);
    private float Tick { get; set; }
    private float TimeOut { get; set; }
    private float Delay { get; set; }
    public System.Action FinishAction;
    private float OriginAlpha { get; set; }

    public override void PrivateAwake()
    {
        isPlayDefaultAudio = false;

        ImageImage = transform.Find("Image").GetComponent<Image>();
        Canvas[] canvas = gameObject.GetComponentsInChildren<Canvas>(true);
        for (int i = 0, count = canvas.Length; i < count; i++)
        {
            canvas[i].sortingOrder = (int) UIWindowLayer.Waiting;
        }

        bg = GetComponent<Image>();
        OriginAlpha = bg.color.a;
        EventDispatcher.Instance.DispatchEvent(EventEnum.NOTICE_POPUP);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        Tick = 0.0f;
        if (objs.Length > 0)
        {
            TimeOut = (float) objs[0];
        }
        else
        {
            TimeOut = 30.0f;
        }

        if (objs.Length > 1)
        {
            Delay = (float) objs[1];
        }
        else
        {
            Delay = 0.0f;
        }

        ImageImage.gameObject.SetActive(false);

        if (Delay > 0)
        {
            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 0);
        }

        StartCoroutine(ShowCircle());
        DragonU3DSDK.DebugUtil.Log("Waiting Show");
    }

    private IEnumerator ShowCircle()
    {
        yield return new WaitForSecondsRealtime(Delay);

        this.ImageImage.gameObject.SetActive(true);
        bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, OriginAlpha);
    }

    protected override void OnCloseWindow(bool destroy)
    {
    }

    protected override void OnReloadWindow()
    {
    }

    void Update()
    {
        ImageImage.gameObject.transform.Rotate(Axis, -Time.deltaTime * 200);
        Tick += Time.deltaTime;
        if (Tick >= TimeOut)
        {
            FinishAction?.Invoke();
            CloseWindowWithinUIMgr();
        }
    }

    public static void Open(float timeout, float delay, Action onTimeout)
    {
        WaitingController dlg =
            UIManager.Instance.OpenUI(UINameConst.UIWaiting, timeout, delay) as WaitingController;
        dlg.FinishAction = onTimeout;
    }
}