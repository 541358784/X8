
using System;
using System.Collections;
using System.Collections.Generic;
using Activity.TreasureMap;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupTreasureMapController : UIWindowController
{
    private LocalizeTextMeshProUGUI _timeText;
    private Button _closeBtn;
    private Transform _rewardItem;
    private Slider _slider;
    private LocalizeTextMeshProUGUI _sliderText;
    private Transform _rewardFinish;
    private List<TreasureMapItem> _maps = new List<TreasureMapItem>();
    private Animator _boxAnimator;
    private Transform _rewardGroup;
    private Button _boxCloseBtn;
    private bool animFinished = false;
    private float animStep = 0.05f;
    private float waitTime = 0.1f;
    private int[][] showOrder = new[]
    {
        new[] {6},
        new[] {1, 3},
        new[] {1, 2, 3},
        new[] {1, 2, 3, 6},
        new[] {0, 1, 2, 3, 4},
        new[] {0, 1, 2, 3, 4, 6},
        new[] {0, 1, 2, 3, 4, 5, 7},
        new[] {0, 1, 2, 3, 4, 5, 6, 7},
    };
    private List<RewardData> rewardDatas = new List<RewardData>();
    private List<RewardData> showRewardDatas = new List<RewardData>();

    public override void PrivateAwake()
    {
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _closeBtn = GetItem<Button>("Root/CloseButton");
        _closeBtn.onClick.AddListener(OnBtnClose);
        _rewardItem = GetItem<Transform>("Root/RewardGroup/Item");
        _rewardItem.gameObject.SetActive(false);
        _slider = GetItem<Slider>("Root/Slider");
        _sliderText = GetItem<LocalizeTextMeshProUGUI>("Root/Slider/Text");
        _rewardFinish = GetItem<Transform>("Root/RewardFinish");

        _boxAnimator = GetItem<Animator>("Root/Box");
        _boxAnimator.gameObject.AddComponent<TreasureMapBox>().Init(this);
        _boxCloseBtn = GetItem<Button>("Root/Box/CloseButton");
        _boxCloseBtn.onClick.AddListener(OnClickGetReward);
        _rewardGroup = GetItem<Transform>("Root/Box/rewardGroup");
    }

    private Action Callback;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (objs.Length > 0)
            Callback = objs[0] as Action;
        InvokeRepeating("RefreshTime", 0, 1);
        Init();
        Invoke("ShowUnlock",1);
    }

    public void Init()
    {
        var config = TreasureMapModel.Instance.GetCurrentTreasureMapConfig();
        for (int i = 0; i < config.Reward.Count; i++)
        {
            var item=Instantiate(_rewardItem, _rewardItem.parent);
            InitRewardItem(item,config.Reward[i],config.Count[i]);
            item.gameObject.SetActive(true);
        }

        var activityConfig = TreasureMapModel.Instance.TreasureMapActivityConfig;
        for (int i = 0; i <activityConfig.MapList.Count ; i++)
        {
            var item= transform.Find("Root/Map/Map" + activityConfig.MapList[i]);
            var mapItem = item.GetOrCreateComponent<TreasureMapItem>();
            mapItem.Init( TreasureMapModel.Instance.GetTreasureMapConfig(activityConfig.MapList[i]));
            _maps.Add(mapItem);
            item.gameObject.SetActive(config.Id==activityConfig.MapList[i]);
        }

        int collectCount = TreasureMapModel.Instance.GetCurrentMapChips().Count;
        _slider.maxValue = config.ChipCount;
        _slider.value = collectCount;
        _sliderText.SetText(collectCount+"/"+config.ChipCount);
        _rewardFinish.gameObject.SetActive(TreasureMapModel.Instance.TreasureMap.IsFinish);
        _rewardItem.parent.gameObject.SetActive(!TreasureMapModel.Instance.TreasureMap.IsFinish);
        
        for (int i = 1; i < 9; i++)
        {
            RewardData rdData = new RewardData();

            GameObject rdObj = GetItem("Root/Box/rewardGroup/reward" + i);
            rdData.gameObject = rdObj;
            rdData.image = GetItem<Image>("Icon", rdObj);
            rdData.tipIcon = GetItem<Image>("Icon/TipIcon", rdObj);
            rdData.numText = GetItem<LocalizeTextMeshProUGUI>("Text", rdObj);
            rdData.animator = GetItem<Animator>(rdObj);
            rdData.SetActive(false);

            rewardDatas.Add(rdData);
        }

        InitBoxReward(config);
    }

    public void InitBoxReward(TreasureMapConfig config)
    {
       var resDatas=  CommonUtils.FormatReward(config.Reward, config.Count);
       
       int count = Math.Min(resDatas.Count, showOrder.Length);
       count = count - 1;
       int[] order = showOrder[count];
       for (int i = 0; i <= count; i++)
       {
           int index = order[i];
           rewardDatas[index].UpdateReward(resDatas[i]);
           showRewardDatas.Add(rewardDatas[index]);
       }

    }
    
    public IEnumerator PlayRewardAnimation()
    {
        if (showRewardDatas == null || showRewardDatas.Count == 0)
        {
            animFinished = true;
            yield break;
        }
        _rewardGroup.gameObject.SetActive(true);
        ShakeManager.Instance.ShakeMedium();

        for (int i = 0; i < showRewardDatas.Count; i++)
        {
            showRewardDatas[i].SetActive(true);
            showRewardDatas[i].PlayAnimation("appear");

            yield return new WaitForSeconds(animStep);
        }

        yield return new WaitForSeconds(waitTime);
        animFinished = true;
    }
    
    public void OnClickGetReward()
    {
        if (!animFinished)
            return;

        animFinished = false;

        _boxCloseBtn.gameObject.SetActive(false);
        _animator.Play("disappear", 0, 0);
        
        FlyGameObjectManager.Instance.FlyObject(showRewardDatas, CurrencyGroupManager.Instance.GetCurrencyUseController(), () =>
        {
       
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);

            _boxAnimator.gameObject.SetActive(false);
            CommonRewardManager.Instance.PopupCacheReward();
            foreach (var resData in showRewardDatas)
            {
                GameObject.Destroy(resData.gameObject);
            }
            CloseWindowWithinUIMgr(true);
            Callback?.Invoke();
        });
    }
    public void ShowUnlock()
    {
        if (TreasureMapModel.Instance.TreasureMap.NewChip > 0)
        {
            AudioManager.Instance.PlaySoundById(146);
            _maps[0].PlayUnlock(TreasureMapModel.Instance.TreasureMap.NewChip,(() =>
            {
                TreasureMapModel.Instance.TreasureMap.NewChip = 0;
                CheckReward();
            }));
            // TreasureMapModel.Instance.TreasureMap.NewChip = 0;
        }
    }

    private void CheckReward()
    {
        if (TreasureMapModel.Instance.CanGetReward())
        {
            TreasureMapModel.Instance.GetReward(TreasureMapModel.Instance.TreasureMap.MapId);
            _maps[0].PlayFinish(() =>
            {
                _rewardFinish.gameObject.SetActive(true);
                _rewardItem.parent.gameObject.SetActive(false);
                _boxAnimator.gameObject.SetActive(true);
                AudioManager.Instance.PlaySoundById(148);
                StartCoroutine(CommonUtils.PlayAnimation(_animator, "Open", "", () =>
                {
                
                }));
            });
        }
    }

    public void RefreshTime()
    {
        _timeText.SetText(TreasureMapModel.Instance.GetActivityLeftTimeString());
    }
    private void InitRewardItem(Transform rewardItem, int rewardId, int rewardCount)
    {
        var rewardImage = rewardItem.Find("Icon").GetComponent<Image>();
        if (rewardImage == null)
            return;

        var tipsBtn = rewardItem.Find("TipsBtn")?.GetComponent<Button>();
        if (UserData.Instance.IsResource(rewardId))
        {
            rewardImage.sprite = UserData.GetResourceIcon(rewardId, UserData.ResourceSubType.Reward);
        }
        else
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(rewardId);
            if (itemConfig != null)
                rewardImage.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            if (tipsBtn != null)
            {
                tipsBtn.gameObject.SetActive(true);
                tipsBtn.onClick.AddListener(() => { MergeInfoView.Instance.OpenMergeInfo(itemConfig, null, false, true); });
            }

        }

        var text = rewardItem.Find("Text")?.GetComponent<LocalizeTextMeshProUGUI>();
        if (text != null)
            text.SetText("x" + rewardCount);

    }
    
    private void OnBtnClose()
    {
        CloseWindowWithinUIMgr(true);      
        Callback?.Invoke();  
    }

    public static UIPopupTreasureMapController Open(Action callback = null)
    {
        if (!TreasureMapModel.Instance.IsOpen())
        {
            return null;
        }
        return UIManager.Instance.OpenUI(UINameConst.UIPopupTreasureMap,callback) as UIPopupTreasureMapController;
    }
}
