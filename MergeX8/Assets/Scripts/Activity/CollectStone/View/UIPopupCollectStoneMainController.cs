using System;
using System.Collections.Generic;
using System.Linq;
using Activity.CollectStone.Model;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CollectStone;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.CollectStone.View
{
    public class UIPopupCollectStoneMainController: UIWindowController
    {
        private LocalizeTextMeshProUGUI _timeText;
        private Button _closeButton;
        private Button _getRewardButton;

        private Transform _rewardItem;
        private Transform _stoneItem;

        private List<RewardData> _rewardDatas = new List<RewardData>();
        private List<StoneItem> _stoneItems = new List<StoneItem>();

        private TableCollectSetting _settingConfig;
        private TableCollectReward _rewardConfig;

        private const int MaxStoneNum  = 6;

        private Animator _animator;
        private Image _animImage;
        
        public override void PrivateAwake()
        {
            _closeButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
            _closeButton.onClick.AddListener(() =>
            {
                AnimCloseWindow();
            });
            
            _getRewardButton = transform.Find("Root/Button").GetComponent<Button>();
            _getRewardButton.onClick.AddListener(OnClickGetReward);
            
            _timeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            _rewardItem = transform.Find("Root/RewardGroup/Item");
            _rewardItem.gameObject.SetActive(false);
            
            _stoneItem = transform.Find("Root/Content/1");
            _stoneItem.gameObject.SetActive(false);

            _animator = transform.Find("Root/StoneAni").GetComponent<Animator>();
            _animator.gameObject.SetActive(false);
            _animImage = transform.Find("Root/StoneAni/Have").GetComponent<Image>();
            
            InvokeRepeating("InvokeUpdate", 0, 1);
        }

        private void Start()
        {
            _settingConfig = CollectStoneModel.Instance.GetCollectSetting();
            _rewardConfig = CollectStoneModel.Instance.GetCollectReward();
            
            RefreshReward();
            InitStoneItem();
            RefreshStoneItem();
            
            _getRewardButton.gameObject.SetActive(false);

            CollectStoneAnim();
        }

        private void InvokeUpdate()
        {
            _timeText.SetText(CollectStoneModel.Instance.GetEndTimeString());
        }
        
        private void OnClickGetReward()
        {
            if (CollectStoneModel.Instance.CollectStone.State.Count < _rewardConfig.CollectNum)
                return;

            List<ResData> data = new List<ResData>();
            for (var i = 0; i < _rewardConfig.RewardId.Count; i++)
            {
                data.Add(new ResData(_rewardConfig.RewardId[i], _rewardConfig.RewardNum[i]));

                if (!UserData.Instance.IsResource((int)_rewardConfig.RewardId[i]))
                {
                    TableMergeItem mergeItemConfig =
                        GameConfigManager.Instance.GetItemConfig((int)_rewardConfig.RewardId[i]);
                    if (mergeItemConfig != null)
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonCollectStoneGet,
                            itemAId = _rewardConfig.RewardId[i],
                            isChange = true,
                        });
                    }
                }
            }
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventCollectStoneReward,_rewardConfig.Id.ToString(),CollectStoneModel.Instance.CollectStone.PayLevelGroup.ToString(), "");

            CollectStoneModel.Instance.NextLevel();
            
            CommonRewardManager.Instance.PopCommonReward(data, CurrencyGroupManager.Instance.currencyController, true, new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.CollectStoneGet}, () =>
            {
                Start();
            });
        }

        private void RefreshReward()
        {
            _rewardDatas.ForEach(a=>a.gameObject.SetActive(false));

            int newNum = _rewardConfig.RewardId.Count - _rewardDatas.Count;
            
            int loopNum = _rewardDatas.Count >= _rewardConfig.RewardId.Count ? _rewardConfig.RewardId.Count : _rewardDatas.Count;
            for (int i = 0; i < loopNum; i++)
            {
                _rewardDatas[i].gameObject.SetActive(true);
                _rewardDatas[i].UpdateReward(_rewardConfig.RewardId[i], _rewardConfig.RewardNum[i]);
            }
            
            if(newNum <= 0)
                return;

            for (int i = 0; i < newNum; i++)
            {
                var item = Instantiate(_rewardItem, _rewardItem.parent);
                _rewardDatas.Add(new RewardData());

                item.gameObject.SetActive(true);
                _rewardDatas.Last().gameObject = item.gameObject;
                _rewardDatas.Last().image = item.Find("Icon").GetComponent<Image>();
                _rewardDatas.Last().numText = item.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
                
                _rewardDatas.Last().UpdateReward(_rewardConfig.RewardId[loopNum+i], _rewardConfig.RewardNum[loopNum+i]);
            }
        }

        private void InitStoneItem()
        {
            for (int i = 0; i < MaxStoneNum; i++)
            {
                var item = Instantiate(_stoneItem, _stoneItem.parent);
                
                item.gameObject.SetActive(true);
                
                var script = item.gameObject.AddComponent<StoneItem>();
                script.Init(i);
                
                _stoneItems.Add(script);
            }
        }

        private void RefreshStoneItem()
        {
            _stoneItems.ForEach(a=>a.gameObject.SetActive(false));

            for (int i = 0; i < _rewardConfig.CollectNum; i++)
            {
                _stoneItems[i].RefreshUI();
                _stoneItems[i].gameObject.SetActive(true);
            }
        }

        private async UniTask CollectStoneAnim()
        {
            if (CollectStoneModel.Instance.CollectStone.State.Count >= _rewardConfig.CollectNum)
            {
                _getRewardButton.gameObject.SetActive(true);
                return;
            }
            
            if(CollectStoneModel.Instance.GetStone() <= 0)
                return;

            for (int i = 0; i < _rewardConfig.CollectNum; i++)
            {
                if(CollectStoneModel.Instance.GetStone() <= 0)
                    break;
                
                if(CollectStoneModel.Instance.IsHave(i))
                    continue;

                CollectStoneModel.Instance.AddStone(-1);
                CollectStoneModel.Instance.CollectStone.State.Add(i);
                
                await ShowStoneAnim(i);
            }
            
            if (CollectStoneModel.Instance.CollectStone.State.Count >= _rewardConfig.CollectNum)
            {
                _getRewardButton.gameObject.SetActive(true);
            }
        }

        private async UniTask ShowStoneAnim(int index)
        {
            _animator.gameObject.SetActive(false);
            _animator.gameObject.SetActive(true);
            _animImage.transform.localPosition = Vector3.zero;

            _animImage.sprite = _stoneItems[index].GetNormalSprite().sprite;
            
            _animator.Play("Appear", -1, 0);
            await UniTask.WaitForSeconds(1.5f);

            await _animImage.transform.DOMove(_stoneItems[index].GetNormalSprite().transform.position, 0.7f).SetEase(Ease.Linear);
            
            _stoneItems[index].RefreshUI();
            _stoneItems[index].ActiveEffect();
            
            await UniTask.WaitForSeconds(0.3f);
            
            _animator.gameObject.SetActive(false);
        }
    }
}