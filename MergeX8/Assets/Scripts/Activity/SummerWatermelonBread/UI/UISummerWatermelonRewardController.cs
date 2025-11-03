using System.Threading.Tasks;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class UISummerWatermelonBreadRewardController:UIWindowController
{
    private Image _itemImage;
    private Image _finalItemImage;
    private Button _closeBtn;
    private LocalizeTextMeshProUGUI _itemNameText;
    private LocalizeTextMeshProUGUI _finalItemNameText;
    private TaskCompletionSource<bool> _callbackTask;
    private Transform _finalEffect;
    private Transform _normalReward;
    public override void PrivateAwake()
    {
        _itemImage = GetItem<Image>("Root/Reward/Icon");
        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _itemNameText = GetItem<LocalizeTextMeshProUGUI>("Root/Reward/Text");
        _closeBtn.onClick.AddListener(OnCloseBtn);
        _finalEffect = GetItem("Root/FinalReward").transform;
        _finalEffect.gameObject.SetActive(false);
        _finalItemImage = GetItem<Image>("Root/FinalReward/Icon1");
        _finalItemNameText = GetItem<LocalizeTextMeshProUGUI>("Root/FinalReward/Text");
        _normalReward = GetItem("Root/Reward").transform;
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
        _finalItemImage.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
        _itemNameText.SetText(LocalizationManager.Instance.GetLocalizedString(itemConfig.name_key));
        _finalItemNameText.SetText(LocalizationManager.Instance.GetLocalizedString(itemConfig.name_key));
        _finalEffect.gameObject.SetActive(itemConfig.next_level < 0);
        _normalReward.gameObject.SetActive(itemConfig.next_level > 0);
    }
}