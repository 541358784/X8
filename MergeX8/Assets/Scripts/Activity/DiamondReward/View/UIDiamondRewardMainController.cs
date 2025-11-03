using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Activity.DiamondRewardModel.Model;
using DragonPlus;
using DragonPlus.Config.DiamondReward;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class UIDiamondRewardMainController : UIWindowController
{
    public class RewardItem
    {
        private Image _image;
        private LocalizeTextMeshProUGUI _text;
        private Button _tipButton;
        private GameObject _gameObject;
        private ResData _resData;

        public GameObject GameObject
        {
            get { return _gameObject; }
        }

        public void SetTipButtonActive(bool isActive)
        {
            _tipButton.gameObject.SetActive(isActive);
        }
        
        public RewardItem(GameObject gameObject, ResData resData)
        {
            _gameObject = gameObject;
            _resData = resData;

            _image = _gameObject.transform.Find("Icon").GetComponent<Image>();
            _text = _gameObject.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            _tipButton = _gameObject.transform.Find("TipsBtn").GetComponent<Button>();
            _tipButton.gameObject.SetActive(false);
            _tipButton.onClick.AddListener(() =>
            {
                var itemConfig= GameConfigManager.Instance.GetItemConfig(_resData.id);
                MergeInfoView.Instance.OpenMergeInfo(itemConfig, null, false,true);
            });

            UpdateReward();
        }

        private void UpdateReward()
        {
            if(_resData == null)
                return;
            
            _image.sprite = UserData.GetResourceIcon(_resData.id,UserData.ResourceSubType.Big);
            
            _tipButton.gameObject.SetActive(!UserData.Instance.IsResource(_resData.id));
            
            _text.SetText(_resData.count.ToString());
        }

        public void UpdateReward(ResData resData)
        {
            _resData = resData;

            UpdateReward();
        }
    }
    
    public class RewardGroup
    {
        private RewardItem _noramlGroup;
        private RewardItem _finishGroup;
        private GameObject _gameObject;
        private int _index;
        private ResData _resData;
        private bool _isGet;

        public int index
        {
            get { return _index; }
        }
        public void Init(GameObject gameObject, int index, ResData resData)
        {
            _gameObject = gameObject;
            _resData = resData;
            _index = index;
            _isGet = DiamondRewardModel.Instance.IsGetReward(_index);;

            _noramlGroup = new RewardItem(gameObject.transform.Find("Normal").gameObject, resData);
            _finishGroup = new RewardItem(gameObject.transform.Find("Finish").gameObject, resData);

            UpdateStatus(_isGet, true);
        }

        public void UpdateStatus(bool isGet, bool isInit)
        {
            if (isInit)
            {
                UpdateStatus(isGet);
                return;
            }
            
            if(_isGet == isGet)
                return;
            
            UpdateStatus(isGet);
        }

        private void UpdateStatus(bool isGet)
        {
            _noramlGroup.GameObject.SetActive(!isGet);
            _finishGroup.GameObject.SetActive(isGet);
        }
    }
    
    public class ConchGroup
    {
        private GameObject _gameObject;
        private GameObject _openObject;
        private RewardItem _openItem;
        private int _index;
        private ResData _resData;
        private bool _isGet;
        private Button _button;
        private SkeletonGraphic _skeletonGraphic;
        
        public RewardItem rewardItem
        {
            get
            {
                return _openItem;
            }
        }
        public int index
        {
            get { return _index; }
        }
        
        public void Init(GameObject gameObject, int index)
        {
            _gameObject = gameObject;
            _index = index;
            _resData = DiamondRewardModel.Instance.GetBoxData(index);
            _isGet = DiamondRewardModel.Instance.IsOpenConch(index);

            _skeletonGraphic = gameObject.transform.Find("Root/Spine").GetComponent<SkeletonGraphic>();
            
            _openObject = gameObject.transform.Find("Root/Open").gameObject;
            _openItem = new RewardItem(_openObject.transform.Find("Item").gameObject, _resData);
            _openItem.SetTipButtonActive(false);

            _button = gameObject.transform.GetComponent<Button>();
            _button.onClick.AddListener(OpenReward);
            UpdateStatus(_isGet, true);
        }
        
        public void UpdateStatus(bool isGet, bool isInit)
        {
            if (isInit)
            {
                UpdateStatus(isGet);
                return;
            }
            
            if(_isGet == isGet)
                return;
            
            UpdateStatus(isGet);
        }

        private void UpdateStatus(bool isGet)
        {
            _openObject.SetActive(isGet);
            PlaySkeletonAnimation(isGet ? "open_idle" : "idle");
        }

        public float OpenConchAnimation()
        {
            return PlaySkeletonAnimation("open");
        }
        
        private void OpenReward()
        {
            if(DiamondRewardModel.Instance.IsOpenConch(_index))
                return;

            Dictionary<string, string> ex = new Dictionary<string, string>();
            ex.Add("level",DiamondRewardModel.Instance.DiamondReward.Level.ToString());
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDiamondrewardClick,
                DiamondRewardModel.Instance.DiamondReward.PoolId.ToString(),index.ToString(),DiamondRewardModel.Instance.DiamondReward.PoolIndex.ToString(),extras:ex);
            
            if (DiamondRewardModel.Instance.IsIgnorePopUI)
            {
                EventDispatcher.Instance.DispatchEventImmediately(EventEnum.BuyDiamondReward, _index);
                return;
            }
            
            UIManager.Instance.OpenUI(UINameConst.UIPopupDiamondRewardBuy, _index);
        }
        
    
        private float PlaySkeletonAnimation(string animName)
        {
            if (_skeletonGraphic == null)
                return 0;
        
            TrackEntry trackEntry = _skeletonGraphic.AnimationState.GetCurrent(0);
            if (trackEntry != null && trackEntry.Animation != null && trackEntry.Animation.Name == animName)
                return trackEntry.AnimationEnd;
        
            _skeletonGraphic.AnimationState?.SetAnimation(0, animName, true);
            _skeletonGraphic.Update(0);
            if (trackEntry == null)
                return 0;
            return trackEntry.AnimationEnd;
        }
    }
    
    private GameObject _rewardItem;
    private List<RewardGroup> _rewardGroups = new List<RewardGroup>();
    private List<ConchGroup> _conchGroups = new List<ConchGroup>();
    
    private LocalizeTextMeshProUGUI _coolDownText;
    private LocalizeTextMeshProUGUI _tipText;
    public override void PrivateAwake()
    {
        _rewardItem = transform.Find("Root/TopGroup/RewardGroup/Item").gameObject;
        _rewardItem.gameObject.SetActive(false);

        _tipText = transform.Find("Root/Content/Text").GetComponent<LocalizeTextMeshProUGUI>();

        var resultDic = DiamondRewardModel.Instance.DiamondResultConfigLevelDic;
        var keyList = resultDic.Keys.ToList();
        for (var i = 0; i < keyList.Count; i++)
        {
            var result = keyList[i];
            var item=Instantiate(_rewardItem, _rewardItem.transform.parent);
            item.SetActive(true);

            var config = resultDic[result];
            int index = i + 1;
            ResData resData;
            resData = new ResData(config.RewardId, config.RewardNum);
            RewardGroup rewardGroup = new RewardGroup();
            rewardGroup.Init(item, index, resData);
            _rewardGroups.Add(rewardGroup);

            ConchGroup conchGroup = new ConchGroup();
            conchGroup.Init(transform.Find("Root/Content/" + index).gameObject, index);
            _conchGroups.Add(conchGroup);
        }

        _coolDownText = transform.Find("Root/TopGroup/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            
        var closeBtn = GetItem<Button>("Root/ButtonClose");
        closeBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        
        InvokeRepeating("RefreshCountDown", 0, 1f);
        
        
        EventDispatcher.Instance.AddEventListener(EventEnum.BuyDiamondReward, BuyDiamondReward);
        
        DiamondRewardModel.Instance.UpdateIgnorePopUIStatus();
        
        _tipText.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("ui_gembox_tips"),DiamondRewardModel.Instance.GetConsume()));
    }
    
    private void RefreshCountDown()
    {
        _coolDownText.SetText(DiamondRewardModel.Instance.GetActivityLeftTimeString());
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.BuyDiamondReward, BuyDiamondReward);
    }

    private IEnumerator UpdateStatus(int index)
    {
        float time = _conchGroups[index-1].OpenConchAnimation();
        _conchGroups[index-1].rewardItem.UpdateReward(DiamondRewardModel.Instance.GetBoxData(index));

        yield return new WaitForSeconds(0.5f);
        
        _rewardGroups.ForEach(a=>a.UpdateStatus(DiamondRewardModel.Instance.IsGetReward(a.index), false));
        _conchGroups.ForEach(a=>a.UpdateStatus(DiamondRewardModel.Instance.IsOpenConch(a.index), false));
    }
    private void BuyDiamondReward(BaseEvent e)
    {
        int index = (int)e.datas[0];
        StartCoroutine(BuyDiamondReward(index));
       
    }

    private IEnumerator BuyDiamondReward(int index)
    {
        var consumeValue =DiamondRewardModel.Instance.GetConsume();
        bool isEnough = UserData.Instance.CanAford(UserData.ResourceId.Diamond, consumeValue);
        if (isEnough)
        {
            var resData = DiamondRewardModel.Instance.OpenConch(index);
            Dictionary<string, string> ex = new Dictionary<string, string>();
            ex.Add("level",DiamondRewardModel.Instance.DiamondReward.Level.ToString());
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDiamondrewardBuy,
                DiamondRewardModel.Instance.DiamondReward.PoolId.ToString(),index.ToString(),resData.id.ToString(),extras:ex);

            var reason = new GameBIManager.ItemChangeReasonArgs
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.DiamondrewardUse,
            };
            UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, consumeValue, reason);
            
            if (Gameplay.UserData.Instance.IsResource(resData.id))
            {
                UserData.Instance.AddRes(resData, new GameBIManager.ItemChangeReasonArgs()
                {
                    reason =  BiEventAdventureIslandMerge.Types.ItemChangeReason.DiamondrewardGet,
                }, false);
            }
            else
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(resData.id);
                if (itemConfig != null)
                {
                    var mergeItem = MergeManager.Instance.GetEmptyItem();
                    mergeItem.Id = resData.id;
                    mergeItem.State = 1;
                    MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.Main);
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType
                            .MergeChangeReasonDiamondrewardGet,
                        itemAId = itemConfig.id,
                        isChange = true,
                    });
                }
            }
            
            yield return StartCoroutine(UpdateStatus(index));
            yield return new WaitForSeconds(0.5f);
            
            CommonRewardManager.Instance.PopCommonReward(new List<ResData>(){resData}, CurrencyGroupManager.Instance.currencyController, false);
        }
        else
        {
            BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, 
                "diamondReward","", "",true,consumeValue);
        }
        
        yield break;
    }
}