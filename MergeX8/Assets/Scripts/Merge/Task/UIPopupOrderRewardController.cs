
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupOrderRewardController:UIWindowController
{
    private Transform _rewardItem;
    private Button _buttonOk;
    private Button _buttonClose;
    public override void PrivateAwake()
    {
        _rewardItem = transform.Find("Root/ContentGroup/RewardGroup/Item");
        _buttonOk = GetItem<Button>("Root/ContentGroup/Button");
        _buttonOk.onClick.AddListener(OnCloseBtn);
        _buttonClose = GetItem<Button>("Root/ContentGroup/CloseButton");
        _buttonClose.onClick.AddListener(OnCloseBtn);
        _rewardItem.gameObject.SetActive(false);

    }

    private void OnCloseBtn()
    {
        AnimCloseWindow();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        if (objs == null || objs.Length <= 0)
            return;
        StorageTaskItem storageTaskItem = (StorageTaskItem) objs[0];
        for (int i = 0; i < storageTaskItem.RewardTypes.Count; i++)
        {
            var item= Instantiate(_rewardItem, _rewardItem.parent);
            item.gameObject.SetActive(true);
        
            if (UserData.Instance.IsResource(storageTaskItem.RewardTypes[i]))
            {
                item.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(storageTaskItem.RewardTypes[i],UserData.ResourceSubType.Big);
                item.Find("TipsBtn").gameObject.SetActive(false);
            }
            else
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(storageTaskItem.RewardTypes[i]);
                item.Find("Icon").GetComponent<Image>().sprite= MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
                item.Find("TipsBtn").GetComponent<Button>().onClick.AddListener(() =>
                {
                    MergeInfoView.Instance.OpenMergeInfo(itemConfig, isShowGetResource:false);
                });
            }

            LocalizeTextMeshProUGUI numText = item.Find("Num").GetComponent<LocalizeTextMeshProUGUI>();
            numText.SetText("x"+storageTaskItem.RewardNums[i]); 
            if(storageTaskItem.RewardTypes[i] == (int)UserData.ResourceId.Seal 
               || storageTaskItem.RewardTypes[i] == (int)UserData.ResourceId.Dolphin 
               || storageTaskItem.RewardTypes[i] == (int)UserData.ResourceId.Capybara)
                numText.gameObject.SetActive(false);
        }
    }
    
}
