using System;
using System.Collections.Generic;
using Activity.TreasureHuntModel;
using DragonPlus;
using DragonPlus.Config.TreasureHunt;
using DragonPlus.Config.TreasureMap;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UITreasureHuntMainController : UIWindowController
{
    private Button _closeBtn;
    private Button _helpBtn;
    private LocalizeTextMeshProUGUI _hammerNum;
    public Button _hammerBtn;
    private Transform _rewardItem;
    private Transform _mapContent;
    private Animator _contentAnimator;
    
    //-----top
    private Slider _topSlider;
    private LocalizeTextMeshProUGUI _topSliderText;
    private LocalizeTextMeshProUGUI _timeText;
    private Button _rewardTip;
    private Transform _tips;
    private Transform _tipsItem;

    private List<TreasureHuntLevel> _levels;
    private List<Transform> _levelRewards = new List<Transform>();
    private TreasureHuntLevel _currentLevel;

    private Transform _hammerAni;
    private Transform _keyIcon;
    private Transform _finish;
    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/CloseButton");
        _closeBtn.onClick.AddListener(OnBtnCLose);
        _helpBtn = GetItem<Button>("Root/HelpButton");
        _helpBtn.onClick.AddListener(OnBtnHelp);
        _hammerNum = GetItem<LocalizeTextMeshProUGUI>("Root/Hammer/NumText");
        _hammerBtn = GetItem<Button>("Root/Hammer/Add");
        _hammerBtn.onClick.AddListener(OnBtnHammer);
        _rewardItem = GetItem<Transform>("Root/RewardGroup/Item");
        _rewardItem.gameObject.SetActive(false);
        _mapContent = GetItem<Transform>("Root/Content");
        _contentAnimator = _mapContent.GetComponent<Animator>();
        _hammerAni = GetItem<Transform>("Root/HammerAnim");
        
        _topSlider = GetItem<Slider>("Root/TopGroup/Slider");
        _topSliderText = GetItem<LocalizeTextMeshProUGUI>("Root/TopGroup/Slider/Text");
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TopGroup/TimeGroup/TimeText");
        _rewardTip = GetItem<Button>("Root/TopGroup/RewardItem/TipsBtn");
        _rewardTip.onClick.AddListener(OnRewardTip);
        _tips = GetItem<Transform>("Root/TopGroup/RewardItem/Tips");
        _tipsItem = GetItem<Transform>("Root/TopGroup/RewardItem/Tips/Item");
        _tipsItem.gameObject.SetActive(false);

        _keyIcon = GetItem<Transform>("Root/TopGroup/Icon");
        _finish = GetItem<Transform>("Root/Finish");
        InvokeRepeating("RefreshTime",0,1);
        Init();
        EventDispatcher.Instance.AddEventListener(EventEnum.TREASURE_HUNT_ITEM_BREAK, OnBreak);
        EventDispatcher.Instance.AddEventListener(EventEnum.TREASURE_HUNT_PURCHASE, OnPurchase);

    }

 
    public void Init()
    {
        _hammerNum.SetText(TreasureHuntModel.Instance.GetHammer().ToString());
     
        InitTop();
        InitLevel();
        InitLevelReward();
        TreasureHuntModel.Instance.CanBreak = true;
    }

    public void InitLevelReward()
    {
        
        for (int i = _levelRewards.Count-1; i >=0; i--)
        {
            _levelRewards[i].gameObject.SetActive(false);
        }
        var huntLevelConfig = TreasureHuntModel.Instance.GetTreasureHuntLevelConfig();
        for (int i = 0; i < huntLevelConfig.FinishReward.Count; i++)
        {
            if (_levelRewards.Count > i)
            {
                InitItem(_levelRewards[i],huntLevelConfig.FinishReward[i],huntLevelConfig.FinishRewardCount[i]);
                _levelRewards[i].gameObject.SetActive(true);
            }
            else
            {
                var item = Instantiate(_rewardItem, _rewardItem.parent);
                item.gameObject.SetActive(true);
                _levelRewards.Add(item);
                InitItem(item,huntLevelConfig.FinishReward[i],huntLevelConfig.FinishRewardCount[i]);
            }
        
        }

    }
    public void InitLevel()
    {
        _levels = new List<TreasureHuntLevel>();
        var levelConfigs = TreasureHuntConfigManager.Instance.TreasureHuntLevelConfigList;
        for (int i = 0; i < levelConfigs.Count; i++)
        {
            var prefab = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/TreasureHunt/Level"+(i+1));
            var levelItem= Instantiate(prefab, _mapContent);
            var huntLevel = levelItem.AddComponent<TreasureHuntLevel>();
            huntLevel.Init(levelConfigs[i],i==TreasureHuntModel.Instance.TreasureHunt.Level);
            huntLevel.gameObject.SetActive(i==TreasureHuntModel.Instance.TreasureHunt.Level);
            if (i == TreasureHuntModel.Instance.TreasureHunt.Level)
                _currentLevel = huntLevel;
            _levels.Add(huntLevel);
        }
        
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.TreasureHuntBreak,null);
    }
    void Update()
    {
        // 检测点击任意位置关闭
        if (Input.GetMouseButtonDown(0))
        {
            
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            if (results.Count == 0)
                return ;
            foreach (var result in results)
            {
                if (result.gameObject == _tips)
                    return ;
            }
            HideTip();
        }
    }
    public void InitTop()
    {
        _topSlider.value =(float) TreasureHuntModel.Instance.TreasureHunt.Level /
                           TreasureHuntConfigManager.Instance.TreasureHuntLevelConfigList.Count;
        _topSliderText.SetText((TreasureHuntModel.Instance.TreasureHunt.Level)+"/"+(TreasureHuntConfigManager.Instance.TreasureHuntLevelConfigList.Count));
        for (int i = 0; i < TreasureHuntModel.Instance.TreasureHuntActivityConfig.Reward.Count; i++)
        {
            var item=Instantiate(_tipsItem, _tipsItem.parent);
            item.gameObject.SetActive(true);
            InitItem(item, TreasureHuntModel.Instance.TreasureHuntActivityConfig.Reward[i], TreasureHuntModel.Instance.TreasureHuntActivityConfig.Count[i]);
        }
    }
    private void InitItem(Transform item, int itemID, int ItemCount)
    {
        if (UserData.Instance.IsResource(itemID))
        {
            item.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(itemID, UserData.ResourceSubType.Reward);
        }
        else
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(itemID);
            if (itemConfig != null)
            {
                item.Find("Icon").GetComponent<Image>().sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }
            else
            {
                Debug.LogError("Get MergeItemConfig---null " + itemID);
            }
            Button tipsBtn = transform.Find("TipsBtn")?.GetComponent<Button>();
            if (tipsBtn != null)
            {
                tipsBtn.gameObject.SetActive(true);
                tipsBtn.onClick.AddListener(() =>
                {
                    MergeInfoView.Instance.OpenMergeInfo(itemConfig, null, false,true);
                });
            }
        }
        
        item.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(ItemCount.ToString());
        item.gameObject.SetActive(true);
    }
    
    private void OnPurchase(BaseEvent obj)
    {
        _hammerNum.SetText(TreasureHuntModel.Instance.GetHammer().ToString());
    }

    private void OnBreak(BaseEvent obj)
    {
        int index=(int)obj.datas[0];
        bool isFindKey=(bool)obj.datas[1];
        int randomReward=(int)obj.datas[2];
        int randomRewardCount=(int)obj.datas[3];
        _hammerNum.SetText(TreasureHuntModel.Instance.GetHammer().ToString());

        _hammerAni.position = _currentLevel._levelItems[index].Normal.position;
        _hammerAni.gameObject.SetActive(false);
        _hammerAni.gameObject.SetActive(true);
        CommonUtils.DelayedCall(0.5f, (() =>
        {
            AudioManager.Instance.PlaySound(13);
            _currentLevel._levelItems[index].Normal.gameObject.SetActive(false);
            if (isFindKey)
            {
                FlyKey(_currentLevel._levelItems[index].Normal.position,1,true, () =>
                {
                   var res= CommonUtils.FormatReward(_currentLevel._config.FinishReward,
                        _currentLevel._config.FinishRewardCount);
                   CommonRewardManager.Instance.PopCommonReward(res, CurrencyGroupManager.Instance.currencyController, false,animEndCall:
                       () =>
                       {
                           if (TreasureHuntModel.Instance.TreasureHunt.IsFinish)
                           {
                               var res2= CommonUtils.FormatReward(TreasureHuntModel.Instance.TreasureHuntActivityConfig.Reward,
                                   TreasureHuntModel.Instance.TreasureHuntActivityConfig.Count);
                               CommonRewardManager.Instance.PopCommonReward(res2,
                                   CurrencyGroupManager.Instance.currencyController, false, animEndCall:
                                   () =>
                                   {
                                       _finish.gameObject.SetActive(false);
                                       _finish.gameObject.SetActive(true);
                                       CommonUtils.DelayedCall(2, () =>
                                       {
                                           AnimCloseWindow();
                                       });
                                   });
                           }
                           else
                           {
                               StartCoroutine(CommonUtils.PlayAnimation(_contentAnimator,"Disappear","",()=>
                               {
                                   ChangeLevel();
                                   _contentAnimator.Play("Appear");
                                   TreasureHuntModel.Instance.CanBreak = true;
                               }));
                           }
                       });
                });
            }
            else
            {
                TreasureHuntModel.Instance.CanBreak = true;
                if (randomReward > 0)
                {
                    FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.currencyController,(UserData.ResourceId)randomReward,randomRewardCount,(Vector2)_currentLevel._levelItems[index].Normal.position,0.2f,true,true,0.1f);
                }else{
                    _currentLevel._levelItems[index].Null.gameObject.SetActive(false);
                    _currentLevel._levelItems[index].Null.gameObject.SetActive(true);
                }
            }
        }));
    }

    public void ChangeLevel()
    {

        for (int i = 0; i < _levels.Count; i++)
        {
            if (i == TreasureHuntModel.Instance.TreasureHunt.Level)
                _currentLevel = _levels[i];
            _levels[i].gameObject.SetActive(i==TreasureHuntModel.Instance.TreasureHunt.Level);
        }

        InitLevelReward();
        _topSlider.value =(float) TreasureHuntModel.Instance.TreasureHunt.Level /
                          TreasureHuntConfigManager.Instance.TreasureHuntLevelConfigList.Count;
        _topSliderText.SetText((TreasureHuntModel.Instance.TreasureHunt.Level)+"/"+(TreasureHuntConfigManager.Instance.TreasureHuntLevelConfigList.Count));
    }
    public  void FlyKey( Vector3 srcPos, float time, bool showEffect,
        Action action = null)
    {
        Transform target = _keyIcon;
        float delayTime = 0.3f;
    
        Vector3 position = target.position;

        FlyGameObjectManager.Instance.FlyObject(target.gameObject, srcPos, position, showEffect, time,
            delayTime, () =>
            {
                FlyGameObjectManager.Instance.PlayHintStarsEffect(position);
                ShakeManager.Instance.ShakeLight(); 
                action?.Invoke();
            });
    }

    public void RefreshTime()
    {
        _timeText.SetText(TreasureHuntModel.Instance.GetActivityLeftTimeString());
        if (TreasureHuntModel.Instance.GetActivityLeftTime() <= 0)
        {
            AnimCloseWindow();
        }
    }

    private bool isShowTip = false;
    private void OnRewardTip()
    {
        if (isShowTip)
        {
            isShowTip = false;
            _tips.gameObject.SetActive(false);
            CancelInvoke("HideTip");
            return;
        }
        isShowTip = true;
        _tips.gameObject.SetActive(true);
        Invoke("HideTip",3);
    }

    private void HideTip()
    {
        CancelInvoke("HideTip");
        isShowTip = false;
        _tips.gameObject.SetActive(false);
    }
    private void OnBtnHammer()
    {
        UIManager.Instance.OpenUI(UINameConst.UIPopupTreasureHuntGift);
    }

    private void OnBtnHelp()
    {
        UIManager.Instance.OpenUI(UINameConst.UITreasureHuntHelp);
    }

    private void OnBtnCLose()
    {
        AnimCloseWindow();
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.TREASURE_HUNT_ITEM_BREAK, OnBreak);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.TREASURE_HUNT_PURCHASE, OnPurchase);
    }
}
