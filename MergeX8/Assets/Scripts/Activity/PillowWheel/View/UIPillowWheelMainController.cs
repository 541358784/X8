using System.Collections.Generic;
using System.Threading.Tasks;
using ConnectLine.Logic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPillowWheelMainController:UIWindowController
{
    public static UIPillowWheelMainController Instance;

    public static UIPillowWheelMainController Open()
    {
        if (Instance)
            return Instance;
        // Instance = UIManager.Instance.OpenUI(UINameConst.UIPillowWheelMain) as UIPillowWheelMainController;
        UIManager.Instance.OpenUI(UINameConst.UIPillowWheelMain);
        return Instance;
    }

    public Button CloseBtn;
    public Button SpinBtn;
    public LocalizeTextMeshProUGUI TimeText;
    public LocalizeTextMeshProUGUI ItemCountText;
    private StoragePillowWheel Storage => PillowWheelModel.Instance.Storage;
    
    public override void PrivateAwake()
    {
        Instance = this;
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        SpinBtn = GetItem<Button>("Root/Button");
        SpinBtn.onClick.AddListener(() =>
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.PillowWheelPlay);
            if (IsSpin)
                return;
            var cost = PillowWheelModel.Instance.GlobalConfig.SpinCost;
            if (Storage.ItemCount < cost)
            {
                UIPopupPillowWheelNoItemController.Open();
                return;   
            }
            PillowWheelModel.Instance.AddItem(-cost,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.PillowWheelGet));
            UpdateItemCountText();
            var result = PillowWheelModel.Instance.Spin();
            if (result == null)
                return;
            PerformSpin(result).WrapErrors();
        });
        transform.Find("Root/Button/Icon/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(PillowWheelModel.Instance.GlobalConfig.SpinCost.ToString());
        ItemCountText = transform.Find("Root/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
        UpdateItemCountText();
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/Top/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0,1);
        UpdateSpinBtnState();
        var helpBtn = GetItem<Button>("Root/ButtonHelp");
        helpBtn.onClick.AddListener(()=>UIPillowWheelHelpController.Open(Storage));
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        InitWheelGroup();
        InitSpecialReward();
        InitShopEntrance();
        AwakeRank();
        InvokeRepeating("UpdateTime",0,1);
        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.PillowWheelDes, null))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(SpinBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.PillowWheelPlay, SpinBtn.transform as RectTransform,
                topLayer: topLayer);
        }
        else
        {
            CheckLeaderBoardGuide();
        }
        XUtility.DisableShieldBtn(gameObject);
    }
    public void UpdateSpinBtnState()
    {
        SpinBtn.interactable = /*Storage.ItemCount <PillowWheelModel.Instance.GlobalConfig.SpinCost &&*/ !IsSpin && PillowWheelModel.Instance.ResultList().Count > 0;
    }
    public void UpdateTime()
    {
        TimeText.SetText(PillowWheelModel.Instance.Storage.GetLeftTimeText());
    }

    public void UpdateItemCountText()
    {
        ItemCountText.SetText(Storage.ItemCount.ToString());
    }

    public bool IsSpin;
    public async Task PerformSpin(PillowWheelResultConfig result)
    {
        IsSpin = true;
        UpdateSpinBtnState();
        var indexList = new List<int>();
        var wheelConfigs = PillowWheelModel.Instance.TurntableConfigList;
        for (var i = 0; i < result.Level-1; i++)
        {
            var index = wheelConfigs[i].TurntableResultList.FindIndex(a => a == 0);
            indexList.Add(index);
        }
        var resultIndex = wheelConfigs.Find(a => a.Id == result.Level).TurntableResultList
            .FindIndex(a => a == result.Id);
        indexList.Add(resultIndex);

        var startIndex = 0;
        for (var i = 0; i < indexList.Count; i++)
        {
            var wheel = WheelDic[i + 1];
            await wheel.SpinToIndex(startIndex, indexList[i]);
            if (!this)
                return;
            startIndex = GetInnerIndex(indexList[i], wheel.Config.TurntableResultList.Count / 4);
            var lightEffect = transform.Find("Root/Wheel/Selected/FX");
            if (result.RewardId < 0)
            {
                AudioManager.Instance.PlaySound(30);
            }

            lightEffect.gameObject.SetActive(true);
            await XUtility.WaitSeconds(0.5f);
            if (!this)
                lightEffect.gameObject.SetActive(false);
        }
        if (!this)
            return;
        var resultWheel = WheelDic[result.Level];
        resultWheel.ItemList[resultIndex].UpdateFinishState();
        
        var lightObj = transform.Find("Root/Wheel/Selected");
        lightObj.gameObject.SetActive(false);
        
        
        var reason = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.PillowWheelGet);
        if (result.RewardId < 0)
        {
            var specialKey = -result.RewardId;
            var specialConfig = PillowWheelModel.Instance.SpecialRewardConfigList.Find(a => a.Id == specialKey);
            var collectCount = PillowWheelModel.Instance.Storage.SpecialCollectState.TryGetValue(specialKey, out var count) ? count : 0;
            var node = SpecialRewardDic[specialKey].HaveList[collectCount - 1];
            var clone = Instantiate(node, transform);
            clone.gameObject.SetActive(true);
            clone.position = lightObj.position;
            clone.DOMove(node.position, 0.3f).SetEase(Ease.Linear);
            await XUtility.WaitSeconds(0.3f);
            if (!this)
                return;
            Destroy(clone.gameObject);
            SpecialRewardDic[specialKey].UpdateCollectState();
            if (Storage.SpecialCollectState[specialKey] == specialConfig.Count)
            {
                var rewards = new List<ResData>();
                rewards.Add(new ResData(specialConfig.RewardId,specialConfig.RewardNum));
                CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                    false, reason,animEndCall: UpdateItemCountText);
            }
        }
        else
        {
            var rewards = new List<ResData>();
            rewards.Add(new ResData(result.RewardId,result.RewardNum));
            CommonRewardManager.Instance.PopCommonReward(rewards, CurrencyGroupManager.Instance.currencyController,
                false, reason,animEndCall: UpdateItemCountText);
        }
        IsSpin = false;
        UpdateSpinBtnState();
    }
    public int GetInnerIndex(int index,int lineCount)
    {
        if (lineCount < 3)
            return 0;
        var mod1 = lineCount;
        var innerMod1 = mod1 - 2;
        var innerIndexCount = innerMod1 * 4;
        var left1 = index % mod1;
        var line1 = index / mod1;
        if (left1 == 0)
        {
            return GetInnerIndex(index + 1,lineCount);
        }
        else
        {
            return (left1 - 1 + innerMod1 * line1)%innerIndexCount;
        }
    }
}