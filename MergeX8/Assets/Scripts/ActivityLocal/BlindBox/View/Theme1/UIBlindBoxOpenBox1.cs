using System;
using DG.Tweening;
using DragonPlus;
using Spine.Unity;
using UnityEngine.UI;

public class UIBlindBoxOpenBox1 : UIBlindBoxOpenBoxBase
{
    private Image Icon;
    // private SkeletonGraphic SkeletonGraphic;
    public override void PrivateAwake()
    {
        Icon = GetItem<Image>("Root/Icon");
        // SkeletonGraphic = transform.Find("Root/SkeletonGraphic (blind_box)").GetComponent<SkeletonGraphic>();
    }

    public override void InitCloseBtn()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Icon.sprite = Config.GetItemSprite(false);
        // Icon.gameObject.SetActive(false);
        // SkeletonGraphic.PlaySkeletonAnimation("animation");
        CloseBtn.interactable = false;
        AudioManager.Instance.PlaySoundById(178);
        DOVirtual.DelayedCall(1.5f, () =>
        {
            // Icon.gameObject.SetActive(true);
            CloseBtn.interactable = true;
        }).SetTarget(transform);
    }
}