using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine.UI;

public class UIExtraOrderRewardGetController:UIWindowController
{
    private LocalizeTextMeshProUGUI DescribeText;
    private LocalizeTextMeshProUGUI TitleText;
    private LocalizeTextMeshProUGUI NumText;
    private Image Icon;
    private Button CloseBtn;
    public override void PrivateAwake()
    {
        DescribeText = GetItem<LocalizeTextMeshProUGUI>("Root/Text");
        TitleText = GetItem<LocalizeTextMeshProUGUI>("Root/TextTitle");
        NumText = GetItem<LocalizeTextMeshProUGUI>("Root/Reward/Text");
        Icon = GetItem<Image>("Root/Reward");
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        InvokeRepeating("UpdateTime",0f,1f);
    }

    private StorageExtraOrderRewardCouponItem StorageItem;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        StorageItem = objs[0] as StorageExtraOrderRewardCouponItem;
        var config = ExtraOrderRewardCouponModel.Instance.Config[StorageItem.CouponId];
        Icon.sprite = UserData.GetResourceIcon(config.id, UserData.ResourceSubType.Big);
        DescribeText.SetTerm(config.describeText);
        UpdateTime();
    }

    public void UpdateTime()
    {
        var leftTime = StorageItem.GetLeftTime();
        NumText.SetText(CommonUtils.FormatLongToTimeStr((long)leftTime));
    }

    public void OnClickCloseBtn()
    {
        AnimCloseWindow();
    }

    public static UIExtraOrderRewardGetController Open(StorageExtraOrderRewardCouponItem storageItem)
    {
        var existPopup =
            UIManager.Instance
                .GetOpenedUIByPath<UIExtraOrderRewardGetController>(UINameConst.UIExtraOrderRewardGet);
        if (existPopup)
        {
            existPopup.CloseWindowWithinUIMgr(true);
        }
        return UIManager.Instance.OpenUI(UINameConst.UIExtraOrderRewardGet,storageItem) as UIExtraOrderRewardGetController;
    }
}