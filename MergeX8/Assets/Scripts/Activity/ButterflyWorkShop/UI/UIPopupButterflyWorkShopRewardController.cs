using System.Threading.Tasks;
using DragonPlus;
using UnityEngine.UI;

public class UIPopupButterflyWorkShopRewardController:UIWindowController
{
    private Image _itemImage;
    private Button _closeBtn;
    private LocalizeTextMeshProUGUI _itemNameText;
    private TaskCompletionSource<bool> _callbackTask;
    public override void PrivateAwake()
    {
        _itemImage = GetItem<Image>("Root/Reward");
        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _itemNameText = GetItem<LocalizeTextMeshProUGUI>("Root/Reward/Text");
        _closeBtn.onClick.AddListener(OnCloseBtn);
    }
    void OnCloseBtn()
    {
        AnimCloseWindow(() => _callbackTask.SetResult(true));
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        var itemConfig = objs[0] as TableMergeItem;
        _callbackTask = objs[1] as TaskCompletionSource<bool>;
        _itemImage.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
        _itemNameText.SetText(LocalizationManager.Instance.GetLocalizedString(itemConfig.name_key));
    }
}