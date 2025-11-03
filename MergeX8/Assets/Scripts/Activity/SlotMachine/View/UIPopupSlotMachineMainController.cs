using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupSlotMachineMainController : UIWindowController
{
    private StorageSlotMachine Storage;
    private List<SlotMachineRoll> RollList = new List<SlotMachineRoll>();
    private Button SpinBtn;
    private List<Button> ReSpinBtnList = new List<Button>();
    private Button CollectBtn;
    private Button CloseBtn;
    private Button HelpBtn;
    private List<LocalizeTextMeshProUGUI> ReSpinBtnTextList = new List<LocalizeTextMeshProUGUI>();
    private LocalizeTextMeshProUGUI TimeText;
    private LocalizeTextMeshProUGUI SpinTimesText;
    private Transform DefaultRewardPreview;
    private Image CurRewardIcon;
    private LocalizeTextMeshProUGUI CurRewardText;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageSlotMachine;
        for (var i = 0; i < RollList.Count; i++)
        {
            RollList[i].Init(i,Storage.ElementIndexList[i]);
        }
        SpinTimesText.SetText(Storage.SpinCount.ToString());
        UpdateBtnVisible();
        for (var i = 0; i < SlotMachineModel.Instance.RewardConfigList.Count; i++)
        {
            var rewardConfig = SlotMachineModel.Instance.RewardConfigList[i];
            if (!rewardConfig.ShowRuleFlag)
                continue;
            var previewNode = Instantiate(DefaultRewardPreview, DefaultRewardPreview.parent);
            previewNode.gameObject.SetActive(true);
            Image rewardIcon = previewNode.Find("Icon4").GetComponent<Image>();
            LocalizeTextMeshProUGUI rewardNum = previewNode.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            var elementList = new List<Transform>();
            elementList.Add(previewNode.Find("Icon3"));
            elementList.Add(previewNode.Find("Icon2"));
            elementList.Add(previewNode.Find("Icon1"));
            foreach (var element in elementList)
            {
                if (element.TryGetComponent<Image>(out var image))
                {
                    image.enabled = false;
                }
            }
            for (var j = 0; j < rewardConfig.ResultList.Count; j++)
            {
                var elementIndex = rewardConfig.ResultList[j];
                var asset = ResourcesManager.Instance.LoadResource<GameObject>(ElementAssetPath + elementIndex);
                var element = Instantiate(asset);
                element.transform.SetParent(elementList[j],false);
                var keepScale = elementList[j].gameObject.AddComponent<KeepProportionScaleByRectTransform>();
                keepScale.ControllRect = element.GetComponent<RectTransform>();
                keepScale.enabled = false;
                keepScale.enabled = true;
            }
            rewardIcon.sprite = UserData.GetResourceIcon(rewardConfig.RewardId[0],UserData.ResourceSubType.Big);
            rewardNum.SetText("= "+rewardConfig.RewardNum[0]);
        }
        UpdateCurReward();
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SlotMachineSpin))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(SpinBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SlotMachineSpin, SpinBtn.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SlotMachineSpin, null))
            {
                SlotMachineModel.Instance.AddScore(1,"Debug");
            }
        }
        
        ShieldButtonOnClick[] shieldButtons = gameObject.GetComponentsInChildren<ShieldButtonOnClick>(false);
        foreach (var shieldBtn in shieldButtons)
        {
            shieldBtn.isUse = false;
        }
    }
    private const string ElementAssetPath = "Prefabs/Activity/SlotMachine/Element_";
    public override void PrivateAwake()
    {
        for (var i = 0; i < 3; i++)
        {
            var roll = transform.Find("Root/RollGroup/Roll" + (i+1)).gameObject.AddComponent<SlotMachineRoll>();
            RollList.Add(roll);
            var reSpinBtn = transform.Find("Root/RollGroup/Button" + (i+1)).gameObject.GetComponent<Button>();
            var localI = i;
            reSpinBtn.onClick.AddListener(() =>
            {
                if (IsSpin())
                    return;
                OnClickReSpinBtn(localI);
            });
            ReSpinBtnList.Add(reSpinBtn);
            ReSpinBtnTextList.Add(transform.Find("Root/RollGroup/Button" + (i+1)+"/Text").gameObject.GetComponent<LocalizeTextMeshProUGUI>());
        }
        SpinBtn = transform.Find("Root/ButtonSpin").GetComponent<Button>();
        SpinBtn.onClick.AddListener(OnClickSpinBtn);
        CollectBtn = transform.Find("Root/ButtonGet").GetComponent<Button>();
        CollectBtn.onClick.AddListener(OnClickCollectBtn);
        CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
        {
            if (IsSpin())
                return;
            AnimCloseWindow();
        });
        HelpBtn = transform.Find("Root/ButtonHelp").GetComponent<Button>();
        HelpBtn.onClick.AddListener(() =>
        {
            UISlotMachineHelpController.Open(Storage);
        });
        TimeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateTime",0f,1f);
        SpinTimesText = transform.Find("Root/NumGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        DefaultRewardPreview = transform.Find("Root/Preview/1");
        DefaultRewardPreview.gameObject.SetActive(false);
        CurRewardIcon = transform.Find("Root/Reward/Item/Icon").GetComponent<Image>();
        CurRewardText = transform.Find("Root/Reward/Text").GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEvent<EventSlotMachinePerformSpinReel>(PerformSpinReel);
        EventDispatcher.Instance.AddEvent<EventSlotMachinePerformSingleSpinReel>(PerformSpinSingleReel);
        EventDispatcher.Instance.AddEvent<EventSlotMachineCollectReward>(OnCollectReward);
        EventDispatcher.Instance.AddEvent<EventSlotMachineScoreChange>(ChangeSpinTimesText);
    }

    public void UpdateCurReward()
    {
        if (IsSpin() || !Storage.HasUnCollectResult)
        {
            CurRewardIcon.gameObject.SetActive(false);
            CurRewardText.gameObject.SetActive(false);
        }
        else
        {
            var reward = SlotMachineUtils.GetTriggerReward(Storage.ElementIndexList);
            if (reward == null)
            {
                CurRewardIcon.gameObject.SetActive(false);
                CurRewardText.gameObject.SetActive(false);
            }
            else
            {
                CurRewardIcon.gameObject.SetActive(true);
                CurRewardText.gameObject.SetActive(true);
                CurRewardIcon.sprite = UserData.GetResourceIcon(reward.RewardId[0], UserData.ResourceSubType.Big);
                CurRewardText.SetText(reward.RewardNum[0].ToString());
            }
        }
    }
    public void ChangeSpinTimesText(EventSlotMachineScoreChange evt)
    {
        SpinTimesText.SetText(Storage.SpinCount.ToString());
    }
    public void UpdateTime()
    {
        TimeText.SetText(SlotMachineModel.Instance.GetActivityLeftTimeString());
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventSlotMachinePerformSpinReel>(PerformSpinReel);
        EventDispatcher.Instance.RemoveEvent<EventSlotMachinePerformSingleSpinReel>(PerformSpinSingleReel);
        EventDispatcher.Instance.RemoveEvent<EventSlotMachineCollectReward>(OnCollectReward);
        EventDispatcher.Instance.RemoveEvent<EventSlotMachineScoreChange>(ChangeSpinTimesText);
    }
    public float BaseTiem = 2f;
    public float IntervalTime = 0.5f;
    public void PerformSpinReel(EventSlotMachinePerformSpinReel evt)
    {
        if (evt.Storage != Storage)
            return;
        AudioManager.Instance.PlaySound(132);
        var taskList = new List<Task>();
        for (var i = 0; i < Storage.ElementIndexList.Count; i++)
        {
            if (i < RollList.Count)
            {
                var result = Storage.ElementIndexList[i];
                var roll = RollList[i];
                if (roll.IsSpin)
                    continue;
                taskList.Add(roll.StartSpin(result, BaseTiem + IntervalTime * i)/*.AddCallBack(UpdateBtnVisible)*/);
            }
        }
        UpdateBtnVisible();
        Task.WhenAll(taskList).AddCallBack(UpdateBtnVisible).AddCallBack(() =>
        {
            if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SlotMachineInfo))
            {
                List<Transform> topLayer = new List<Transform>();
                var guideReSpinBtn = ReSpinBtnList[SlotMachineModel.Instance.GlobalConfig.GuideReSpinReelIndex];
                topLayer.Add(guideReSpinBtn.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SlotMachineReSpin, guideReSpinBtn.transform as RectTransform,
                    topLayer: topLayer);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SlotMachineInfo, null))
                {
                    ReSpinBtnTextList[SlotMachineModel.Instance.GlobalConfig.GuideReSpinReelIndex].SetText("O");
                    ReSpinBtnList[SlotMachineModel.Instance.GlobalConfig.GuideReSpinReelIndex].interactable = true;
                }
            }
        }).WrapErrors();
    }
    public void PerformSpinSingleReel(EventSlotMachinePerformSingleSpinReel evt)
    {
        if (evt.Storage != Storage)
            return;
        var result = Storage.ElementIndexList[evt.ReelIndex];
        if (evt.ReelIndex >= RollList.Count)
            return;
        var roll = RollList[evt.ReelIndex];
        if (roll.IsSpin)
            return;
        AudioManager.Instance.PlaySound(133);
        roll.StartSpin(result, BaseTiem).AddCallBack(UpdateBtnVisible).AddCallBack(() =>
        {
            if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SlotMachineCollect))
            {
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(CollectBtn.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SlotMachineCollect, CollectBtn.transform as RectTransform,
                    topLayer: topLayer);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SlotMachineCollect, null))
                {
                }
            }
        }).WrapErrors();
        UpdateBtnVisible();
    }

    public void OnCollectReward(EventSlotMachineCollectReward evt)
    {
        if (evt.Storage != Storage)
            return;
        UpdateBtnVisible();
    }
    public void OnClickReSpinBtn(int reelIndex)
    {
        if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.SlotMachineReSpin))
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SlotMachineReSpin);
            Storage.SpinSingleReelGuide(reelIndex);
            return;
        }
        if (!Storage.HasUnCollectResult)
            return;
        var price = Storage.GetReSpinPrice();
        if (!UserData.Instance.CanAford(UserData.ResourceId.Diamond, price))
            return;
        Storage.SpinSingleReel(reelIndex);
    }

    public void OnClickSpinBtn()
    {
        if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.SlotMachineSpin))
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SlotMachineSpin);
            Storage.SpinGuide();
            return;
        }
        if (IsSpin())
            return;
        if (Storage.HasUnCollectResult)
            return;
        if (Storage.SpinCount <= 0)
        {
            UIPopupSlotMachineNoSpinController.Open();
            return;
        }
        Storage.Spin();
    }

    public void OnClickCollectBtn()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SlotMachineCollect);
        if (IsSpin())
            return;
        if (!Storage.HasUnCollectResult)
            return;
        Storage.CollectReward();
    }

    public bool IsSpin()
    {
        foreach (var roll in RollList)
        {
            if (roll.IsSpin)
                return true;
        }
        return false;
    }
    public void UpdateBtnVisible()
    {
        var isSpin = IsSpin();
        SpinBtn.gameObject.SetActive(!Storage.HasUnCollectResult);
        SpinBtn.interactable = !isSpin;
        var price = Storage.GetReSpinPrice();
        var affordAble = UserData.Instance.CanAford(UserData.ResourceId.Diamond, price);
        for (var i=0;i< ReSpinBtnList.Count;i++)
        {
            var reSpinBtn = ReSpinBtnList[i];
            reSpinBtn.gameObject.SetActive(Storage.HasUnCollectResult);
            var btnAble = affordAble && !isSpin/*&&!RollList[i].IsSpin*/;
            reSpinBtn.interactable = btnAble;
            reSpinBtn.gameObject.GetComponent<Animator>().PlayAnimation(btnAble?"Normal":"Disable");
        }
        foreach (var text in ReSpinBtnTextList)
        {
            text.SetText(price.ToString());
        }
        CollectBtn.gameObject.SetActive(Storage.HasUnCollectResult);
        CollectBtn.interactable = !isSpin;
        UpdateCurReward();
    }
    public static UIPopupSlotMachineMainController Open(StorageSlotMachine storage)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupSlotMachineMain,storage) as UIPopupSlotMachineMainController;
    }
}