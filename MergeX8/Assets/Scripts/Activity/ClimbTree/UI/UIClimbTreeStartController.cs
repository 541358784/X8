using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.ClimbTree;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
public class UIClimbTreeStartController:UIWindowController
{
    private Button _buttonClose;
    private Button _buttonStart;
    
    private Transform _rewardItem;
    private List<Transform> _rewardsList;
    private Transform _rewardLayoutGroup;
    private Transform _rewardStaticGroup;
    
    public override void PrivateAwake()
    {
        ClimbTreeModel.Instance.ShowPreStartView();
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnCloseBtn);
        _buttonStart = GetItem<Button>("Root/Button");
        _buttonStart.onClick.AddListener(OnStartBtn);

        _rewardLayoutGroup = transform.Find("Root/Scroll View/Viewport/Content/ItemGroup");
        _rewardStaticGroup = transform.Find("Root/Scroll View/Viewport/Content/ItemGroupStatic");
        _rewardStaticGroup.gameObject.SetActive(false);
        _rewardItem = transform.Find("Root/Scroll View/Viewport/Content/ItemGroup/Item");
        _rewardItem.gameObject.SetActive(false);
        _rewardsList = new List<Transform>();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        for (var i = 0; i < _rewardsList.Count; i++)
        {
            if (_rewardsList[i].name.Contains("(Clone)"))
            {
                GameObject.Destroy(_rewardsList[i].gameObject);   
            }
        }
        _rewardsList.Clear();
        var rewards = ClimbTreeModel.Instance.GetLevelRewards(ClimbTreeModel.Instance.MaxLevel);
        var useStatic = rewards.Count == 3;
        _rewardLayoutGroup.gameObject.SetActive(!useStatic);
        _rewardStaticGroup.gameObject.SetActive(useStatic);
        for (var i = 0; i < rewards.Count; i++)
        {
            var reward = rewards[i];
            Transform rewardObject = useStatic?
                _rewardStaticGroup.Find("Item" + (i + 1)):
                GameObject.Instantiate(_rewardItem,_rewardItem.parent);
            rewardObject.GetComponent<Image>().sprite = UserData.GetResourceIcon(reward.id, UserData.ResourceSubType.Big);
            _rewardsList.Add(rewardObject.transform);
        }
    }

    private void OnStartBtn()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMonkeyOn);
        AnimCloseWindow(OpenGuide);
    }

    private void OnCloseBtn()
    {
       AnimCloseWindow(OpenGuide);
    }
    
    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        AnimCloseWindow(OpenGuide);
    }

    public void OpenGuide()
    {
        if (!GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ClimbTreeStart, null))
        {
            UIManager.Instance.OpenUI(UINameConst.UIClimbTreeMain);
        }
    }
    public static bool CanShowUI()
    {
        if (!ClimbTreeModel.Instance.IsPrivateOpened())
            return false;

        if (ClimbTreeModel.Instance.CanShowPreStartView())
        {           
            if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Home && SceneFsm.mInstance.GetCurrSceneType() != StatusType.BackHome)
                return false;
            
            UIManager.Instance.OpenUI(UINameConst.UIClimbTreeStart);
            return true;
        }

        // if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        // {
        //     CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
        //     UIManager.Instance.OpenUI(UINameConst.UIClimbTreeMain);
        //     return true;
        // }
        return false;
    }
}
