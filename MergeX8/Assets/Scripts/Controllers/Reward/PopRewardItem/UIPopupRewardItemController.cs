
using System.Collections.Generic;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
public class UIPopupRewardItemController : UIWindowController
{
    private LocalizeTextMeshProUGUI _titleText;
    private LocalizeTextMeshProUGUI _contextText;
    private Transform _rewardItem;
    private Button _closeBtn;
    protected List<ResData> _rewardList;
    protected System.Action _claimBtnCallBack;
    public override void PrivateAwake()
    {
        _rewardItem = transform.Find("Root/Scroll View/Viewport/Content/RewardGroup/Horizontal/Item");
        _rewardItem.gameObject.SetActive(false);
        _titleText = GetItem<LocalizeTextMeshProUGUI>("Root/Title/Text");
        // _contextText = GetItem<LocalizeTextMeshProUGUI>("Root/Text");
        _closeBtn = GetItem<Button>("Root/CloseButton");
        _closeBtn.onClick.AddListener(OnBtnClose);
    }

    private void OnBtnClose()
    {
        ClickUIMask();
    }

    public override void ClickUIMask()
    {
        AnimCloseWindow(() =>
        {
            _claimBtnCallBack?.Invoke();
        });
     
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        _rewardList = (List<ResData>) objs[0];
        _claimBtnCallBack = (System.Action) objs[1];
        foreach (var res in _rewardList)
        {
            var obj=Instantiate(_rewardItem, _rewardItem.parent);
            obj.gameObject.SetActive(true);
            var _icon = obj.Find("Icon").GetComponent<Image>();
            if (UserData.Instance.IsResource(res.id))
            {
                _icon.sprite = UserData.GetResourceIcon(res.id, UserData.ResourceSubType.Reward);
            }
            else
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(res.id);
                _icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }
           obj.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(res.count.ToString());
        }

    }
    
    public static UIPopupRewardItemController Show(List<ResData> rewards, System.Action callback = null)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupRewardItem, rewards, callback) as
            UIPopupRewardItemController;
    }

}