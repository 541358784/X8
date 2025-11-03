using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupGarageCleanupSubmitController : UIWindowController
{
    private Button _buttonClose;
    private Button _button;
    private Image _itemIcon;
    private Button _tipsBtn;
    private LocalizeTextMeshProUGUI _itemText;
    
    private Transform _rewardItem;

    private TableMergeItem _itemConfig;
    private int _index;
    private Transform _rewardGroup1;
    private Transform _rewardGroup2;
    bool isUseToken=false;
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnBtnClose);
        _tipsBtn= GetItem<Button>("Root/Item/TipsBtn");
        _tipsBtn.onClick.AddListener(OnBtnTips);
        _button = GetItem<Button>("Root/Button");
        _button.onClick.AddListener(OnBtnTurn);
        _itemText = GetItem<LocalizeTextMeshProUGUI>("Root/Item/Text");
        _itemIcon = GetItem<Image>("Root/Item/Icon");
        
        _rewardItem = transform.Find("Root/RewardGroup/1-3/RewardItem");
        _rewardGroup1 = transform.Find("Root/RewardGroup/1-3");
        _rewardGroup2 = transform.Find("Root/RewardGroup/4-6");
        _rewardItem.gameObject.SetActive(false);
    }


    protected override void OnOpenWindow(params object[] objs)
    {
        if (objs == null || objs.Length <= 0)
            return;
        _index = (int) objs[0];
        isUseToken = (bool) objs[1];
        var boardItem= GarageCleanupModel.Instance.GetBoardItem(_index);
         _itemConfig = GameConfigManager.Instance.GetItemConfig(boardItem.Id);
    
        if (_itemConfig==null)
        {
            DebugUtil.LogError("Item 错误 "+boardItem.Id);
        }
        _itemIcon.sprite= MergeConfigManager.Instance.mergeIcon.GetSprite(_itemConfig.image);
        if (isUseToken)
        {
            _itemIcon.sprite = UserData.GetResourceIcon(UserData.ResourceId.Fishpond_token);
            _itemText.SetText( GarageCleanupModel.Instance.GetFishTokenCount(_index).ToString());
            _tipsBtn.gameObject.SetActive(false);
        }
        var rewards=  GarageCleanupModel.Instance.GetTurnInReward(_index);
        if (rewards.Count > 3)
        {
            _rewardGroup2.gameObject.SetActive(true);
        }
        for (int i = 0; i < rewards.Count; i++)
        {
            Transform item;
            if (i > 2)
            {
                item= Instantiate(_rewardItem,_rewardGroup2);
            }
            else
            {
                item= Instantiate(_rewardItem,_rewardGroup1);
            }
           item.gameObject.SetActive(true);
           if (UserData.Instance.IsResource(rewards[i].id))
           {
               item.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(rewards[i].id,UserData.ResourceSubType.Big);
               item.Find("TipsBtn").gameObject.SetActive(false);
           }
           else
           {
               var itemConfig = GameConfigManager.Instance.GetItemConfig(rewards[i].id);
               item.Find("Icon").GetComponent<Image>().sprite= MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
               item.Find("TipsBtn").GetComponent<Button>().onClick.AddListener(() =>
               {
                   MergeInfoView.Instance.OpenMergeInfo(itemConfig);
                   UIPopupMergeInformationController controller =
                       UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as
                           UIPopupMergeInformationController;
                   if (controller != null)
                       controller.SetResourcesActive(false);
               });
           }
           item.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText("x"+rewards[i].count.ToString()); 

        }
    }
    
    private void OnBtnTurn()
    {
        AnimCloseWindow(() =>
        {
            GarageCleanupModel.Instance.TurnIn(_index,isUseToken);
        });
    }

    private void OnBtnTips()
    {
        MergeInfoView.Instance.OpenMergeInfo(_itemConfig);
        UIPopupMergeInformationController controller =
            UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
        if (controller != null)
            controller.SetResourcesActive(false);
    }
    private void OnBtnClose()
    {
        AnimCloseWindow();
    }

   
}