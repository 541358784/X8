using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Storage;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupPhotoAlbumSpineController : UIWindowController
{
    public static UIPopupPhotoAlbumSpineController Instance;
    public static UIPopupPhotoAlbumSpineController Open(PhotoAlbumPhotoConfig photoConfig, bool isAll,bool trigger)
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupPhotoAlbumSpine, photoConfig,isAll,trigger) as
            UIPopupPhotoAlbumSpineController;
        return Instance;
    }

    private bool IsAll;
    private bool Trigger;
    private PhotoAlbumPhotoConfig Config;
    private SkeletonGraphic Spine;
    private Transform DefaultText;
    private Button CloseBtn;
    private Transform CloseBtnText;
    public override void PrivateAwake()
    {
        Spine = transform.Find("Root/Mask/Spine").GetComponent<SkeletonGraphic>();
        DefaultText = transform.Find("Root/Mask/Text");
        DefaultText.gameObject.SetActive(false);
        CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        CloseBtn.interactable = false;
        CloseBtnText = transform.Find("Root/ButtonClose/Text");
        CloseBtnText.gameObject.SetActive(false);
    }

    public void EnableCloseBtn()
    {
        CloseBtnText.gameObject.SetActive(true);
        CloseBtn.interactable = true;
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Config = objs[0] as PhotoAlbumPhotoConfig;
        IsAll = (bool)objs[1];
        Trigger = (bool)objs[2];
        var spineName = "scene" + (Config.Id - PhotoAlbumModel.Instance.GlobalConfig.PhotoOffset);
        if (!IsAll)
            spineName += "_loop";
        if (IsAll)
        {
            if (Trigger)
            {
                Spine.PlaySkeletonAnimationAsync(spineName).AddCallBack(() =>
                {
                    if (!this)
                        return;
                    AnimCloseWindow();
                }).WrapErrors();       
                PlayText().WrapErrors();
            }
            else
            {
                Spine.PlaySkeletonAnimationAsync(spineName).AddCallBack(() =>
                {
                    if (!this)
                        return;
                    AnimCloseWindow();
                }).WrapErrors();       
                PlayText().WrapErrors();
            }
        }
        else
        {
            if (Trigger)
            {
                Spine.PlaySkeletonAnimation(spineName, true);
                PlayText().AddCallBack(async () =>
                {
                    await XUtility.WaitSeconds(0.3f);
                    if (!this)
                        return;
                    EnableCloseBtn();
                }).WrapErrors();
            }
            else
            {
                Spine.PlaySkeletonAnimation(spineName, true);
                PlayText().WrapErrors();
            }
        }
    }

    public List<Action> CallbackList = new List<Action>();
    public void AddCallback(Action callback)
    {
        CallbackList.Add(callback);
    }

    private void OnDestroy()
    {
        foreach (var cb in CallbackList)
        {
            cb?.Invoke();
        }
    }

    public async Task PlayText()
    {
        var textIndex = 1;
        var offset = 0.5f;
        var key = GetKey(textIndex);
        var wrods = LocalizationManager.Instance.GetLocalizedString(key);
        while (key != wrods)
        {
            var textObj = Instantiate(DefaultText, DefaultText.parent);
            textObj.gameObject.SetActive(true);
            var animator = textObj.GetComponent<Animator>();
            var text = textObj.GetComponent<LocalizeTextMeshProUGUI>();
            text.SetText(wrods);
            animator.PlayAnimation("appear");
            XUtility.WaitSeconds(1.5f+offset, () =>
            {
                if (!this)
                    return;
                animator.PlayAnimation("disappear", () =>
                {
                    if (!this)
                        return;
                    Destroy(textObj.gameObject);
                });
            });
            textIndex++;
            key = GetKey(textIndex);
            wrods = LocalizationManager.Instance.GetLocalizedString(key);
            await XUtility.WaitSeconds(2f+offset);
            if (!this)
                return;
        }
    }

    public string GetKey(int index)
    {
        if (IsAll)
            return "UI_LivePhoto_PhotoAll_desc"+Config.Id+"_"+index;
        else
            return "UI_LivePhoto_Photo" + Config.Id + "_desc" + index;
    }
}