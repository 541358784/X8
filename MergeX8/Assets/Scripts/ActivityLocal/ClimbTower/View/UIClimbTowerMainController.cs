using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config;
using DragonPlus.Config.ClimbTower;
using DragonU3DSDK.Network.API.Protocol;
using Facebook.Unity;
using Gameplay;
using Spine;
using UnityEngine;
using UnityEngine.UI;

namespace ActivityLocal.ClimbTower.Model
{
    public class UIClimbTowerMainController: UIWindowController
    {
        private List<Transform> _uiBgGroup = new List<Transform>();
        private List<LevelCell> _levelGroups = new List<LevelCell>();
        private List<WindowsGroup> _windowsGroups = new List<WindowsGroup>();
        
        public Transform _moveNode;
        private Vector3 _startPosition;
        
        private Transform _rewardItem;
        private ScrollRect _scrollView;
        
        private Button _closeButton;
        private Button _restButton;
        private Button _clamButton;
        private Button _rebornButton;
        private Button _rvRebornButton;

        private const int _levelMoveDis = 87;
        private int _currentLevelId = 0;

        private RewardData _finallyRewardData = new RewardData();

        private List<RewardData> _rewardDatas = new List<RewardData>();
        private List<RewardData> _grayRewardDatas = new List<RewardData>();
        
        private LocalizeTextMeshProUGUI _rebornText;
        private int _rebornConsume = 0;

        private bool _isCanClick = false;

        private LocalizeTextMeshProUGUI _tipText;

        private GameObject _nextStageObj;

        private bool _isPayLevel = false;

        private GridLayoutGroup _gridLayoutGroup;
        private bool _isFinish = false;
        
        public override void PrivateAwake()
        {
            for (int i = 1; i <= 5; i++)
            {
                _uiBgGroup.Add(transform.Find("Root/GameGroup/BG/"+i));
            }

            _gridLayoutGroup = transform.Find("Root/RewardGroup/Scroll View/Viewport/Content").GetComponent<GridLayoutGroup>();
            
            _nextStageObj = transform.Find("Root/Spine/Finish").gameObject;
            
            _tipText = transform.Find("Root/RewardGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
            
            _scrollView = transform.Find("Root/RewardGroup/Scroll View").GetComponent<ScrollRect>();
            
            _rewardItem = transform.Find("Root/RewardGroup/Scroll View/Viewport/Content/Item");
            _rewardItem.gameObject.SetActive(false);

            _rebornText = transform.Find("Root/RewardGroup/ButtonGroup/ButtonRevive/Text").GetComponent<LocalizeTextMeshProUGUI>();
            
            _closeButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
            _closeButton.onClick.AddListener(() =>
            {
                if(_isCanClick)
                    AnimCloseWindow();
            });
#if  UNITY_EDITOR
            _closeButton.gameObject.SetActive(true);
#else
            _closeButton.gameObject.SetActive(false);
#endif
            
            
            _restButton = transform.Find("Root/RewardGroup/ButtonGroup/ButtonQuit").GetComponent<Button>();
            _restButton.onClick.AddListener(RestHandle);
            
            _clamButton = transform.Find("Root/RewardGroup/ButtonGroup/ButtonClaim").GetComponent<Button>();
            _clamButton.onClick.AddListener(ClamHandle);
            
            _rebornButton = transform.Find("Root/RewardGroup/ButtonGroup/ButtonRevive").GetComponent<Button>();
            // _rebornButton.onClick.AddListener(RebornHandle);
            _rvRebornButton = transform.Find("Root/RewardGroup/ButtonGroup/ButtonRv").GetComponent<Button>();
            
            InitLevelCell();
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            _isFinish = false;
            _isPayLevel = ClimbTowerModel.Instance.ClimbTower.IsPayLevel;
            if (objs != null && objs.Length > 0)
            {
                _isPayLevel = (bool)objs[0];
            }
            
            _currentLevelId = ClimbTowerModel.Instance.LevelId(_isPayLevel);

            if (_currentLevelId > ClimbTowerConfigManager.Instance.GetRewardConfig(_isPayLevel).Last().Id)
            {
                _currentLevelId = ClimbTowerConfigManager.Instance.GetRewardConfig(_isPayLevel).Last().Id;
                _isFinish = true;
            }
            
            InitFinallyReward();
            InitWindowCell();
            InitClimReward();
            RefreshLevelProgress(_currentLevelId, true);
            RefreshStage();
            RefreshAllButtons();
            RefreshAllRewardState();
            
            _windowsGroups.ForEach(a=>CommonUtils.SetShieldButUnEnable(a._button.gameObject));
            _isCanClick = true;
            
            AudioManager.Instance.PlayMusic(245, true);
        }

        private void InitLevelCell()
        {
            _levelGroups.Clear();
            _moveNode = transform.Find("Root/LvGroup/1/Lv/Content");
            _startPosition = _moveNode.transform.localPosition;
            
            for (int i = 1; i <= 15; i++)
            {
                _levelGroups.Add(transform.Find("Root/LvGroup/1/Lv/Content/" + i).gameObject.AddComponent<LevelCell>());
            }
        }

        private void InitWindowCell()
        {
            List<int> rewardId;
            List<int> rewardNum;
            List<int> openState;

            ClimbTowerModel.Instance.GetRewardCache(_isPayLevel,out rewardId, out rewardNum, out openState);
            
            _windowsGroups.Clear();
            
            for (int i = 1; i <= 4; i++)
            {
                int index = i-1;
                _windowsGroups.Add(new WindowsGroup(index, transform, transform.Find("Root/GameGroup/Content/Content" + i), rewardId[i-1], rewardNum[i-1], openState[i-1], OnClickOpenWindow));
            }
        }

        private void InitFinallyReward()
        {
            _finallyRewardData.gameObject = transform.Find("Root/LvGroup/Reward/Item").gameObject;
            _finallyRewardData.gameObject.SetActive(true);
            _finallyRewardData.image = transform.Find("Root/LvGroup/Reward/Item/Icon").GetComponent<Image>();
            //_finallyRewardData.numText = transform.Find("Root/LvGroup/Reward/Text").GetComponent<LocalizeTextMeshProUGUI>();

            var config = ClimbTowerConfigManager.Instance.GetSettingConfig();
            if(!_isPayLevel)
                _finallyRewardData.UpdateReward(config.RewardItem[0], config.RewardNum[0]);
            else
                _finallyRewardData.UpdateReward(config.PayRewardItem[0], config.PayRewardNum[0]);
            
            _finallyRewardData.gameObject.SetActive(!_isFinish);
        }

        private void InitClimReward()
        {
            foreach (var rewardData in _rewardDatas)
            {
              DestroyImmediate(rewardData.gameObject.transform.parent.gameObject);  
            }
            _rewardDatas.Clear();
            _grayRewardDatas.Clear();
            
            foreach (var reward in ClimbTowerModel.Instance.ClimbTower.Rewards)
            {
                CreateClimReward(reward.Id, reward.Count);
            }

            RefreshGridLayoutSpace();
        }

        private void RefreshGridLayoutSpace()
        {
            int count = ClimbTowerModel.Instance.ClimbTower.Rewards.Count;
            var space = _gridLayoutGroup.spacing;
            space.x = count <= 6 ? -15f : 0f;

            _gridLayoutGroup.spacing = space;
        }
        private RewardData CreateClimReward(int id, int num)
        {
            var item = Instantiate(_rewardItem, _rewardItem.parent);
                
            item.transform.localScale=Vector3.one;
            item.gameObject.SetActive(true);
                
            _rewardDatas.Add(new RewardData());

            _rewardDatas.Last().gameObject = item.Find("Root").gameObject;
            _rewardDatas.Last().image =  _rewardDatas.Last().gameObject.transform.Find("Icon").GetComponent<Image>();
            _rewardDatas.Last().numText =  _rewardDatas.Last().gameObject.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            _rewardDatas.Last().UpdateReward(id, num);

            _grayRewardDatas.Add(new RewardData());
            _grayRewardDatas.Last().gameObject = item.Find("Gray").gameObject;
            _grayRewardDatas.Last().image =  _grayRewardDatas.Last().gameObject.transform.Find("Icon").GetComponent<Image>();
            _grayRewardDatas.Last().numText =  _grayRewardDatas.Last().gameObject.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            _grayRewardDatas.Last().UpdateReward(id, num);
            _grayRewardDatas.Last().gameObject.SetActive(false);
            
            return _rewardDatas.Last();
        }

        private RewardData GetClimReward(int id)
        {
            var rewardData = _rewardDatas.Find(a => a.type == id);
            if (rewardData == null)
                return null;

            return rewardData;
        }
        
        private RewardData GetGrayClimReward(int id)
        {
            var rewardData = _grayRewardDatas.Find(a => a.type == id);
            if (rewardData == null)
                return null;

            return rewardData;
        }
        
        private void RestHandle()
        {
            if(!_isCanClick)
                return;

            Action action = () =>
            {
                // ClimbTowerModel.Instance.RestClimbTower();
                // InitClimReward();
                // RefreshAllButtons();
                // RefreshLevelProgress(ClimbTowerModel.Instance.LevelId(), true);
                // RefreshStage();
                //
                // List<int> rewardId;
                // List<int> rewardNum;
                // List<int> openState;
                // ClimbTowerModel.Instance.GetRewardCache(out rewardId, out rewardNum, out openState);
                //
                // for (var i = 0; i < _windowsGroups.Count; i++)
                // {
                //     _windowsGroups[i].RestWindowInfo(rewardId[i], rewardNum[i], openState[i]);
                // }
                 
                AnimCloseWindow();
                if (ClimbTowerModel.Instance.ClimbTower.FreeTimes > 0)
                {
                    ClimbTowerModel.Instance.RestClimbTower();
                }
                else
                {
                    ClimbTowerModel.Instance.ClimbTower.State = (int)ClimbTowerModel.GameState.Finish;
                }
                
                if (ClimbTowerModel.Instance.ClimbTower.IsPayLevel)
                    ClimbTowerModel.Instance.ClimbTower.IsPayLevel = false;
            };
            
            UIManager.Instance.OpenWindow(UINameConst.UIPopupClimbTowerQuit, action);
        }

        private void ClamHandle()
        {
            if(!_isCanClick)
                return;

            Action action = () =>
            {
                ClimReward();
            };
            
            UIManager.Instance.OpenWindow(UINameConst.UIPopupClimbTowerCollect, action);
        }

        public void RvRebornHandle()
        {
            ClimbTowerModel.Instance.SetGameState(ClimbTowerModel.GameState.Free);
            _isCanClick = true;
            RefreshAllButtons();
            RefreshAllRewardState();
            _windowsGroups.ForEach(a=>a.RefreshSpine(false));
        }
        private void RebornHandle()
        {
            if(!_isCanClick)
                return;
            if (_rebornConsume > 0)
            {
                if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, _rebornConsume))
                {
                    UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, _rebornConsume, new GameBIManager.ItemChangeReasonArgs
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ClimbTowerConsume,
                    });
                
                    ClimbTowerModel.Instance.SetGameState(ClimbTowerModel.GameState.Free);
                    _isCanClick = true;
                    RefreshAllButtons();
                    RefreshAllRewardState();
                    _windowsGroups.ForEach(a=>a.RefreshSpine(false));
                }
                else
                {
                    UIManager.Instance.CloseUI(UINameConst.UIStore, true);
                    BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "","", "climb",true,_rebornConsume);
                }   
            }
            else
            {
                if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, -_rebornConsume))
                {
                    UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, -_rebornConsume, new GameBIManager.ItemChangeReasonArgs
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ClimbTowerConsume,
                    });
                
                    ClimbTowerModel.Instance.SetGameState(ClimbTowerModel.GameState.Free);
                    _isCanClick = true;
                    RefreshAllButtons();
                    RefreshAllRewardState();
                    _windowsGroups.ForEach(a=>a.RefreshSpine(false));
                }
                else
                {
                    UIManager.Instance.CloseUI(UINameConst.UIStore, true);
                    BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "","", "climb",true,-_rebornConsume);
                }
            }
        }

        private void RefreshLevelProgress(int levelId, bool isImmediately)
        {
            int id = Math.Min(levelId, ClimbTowerConfigManager.Instance.GetRewardConfig(_isPayLevel).Last().Id);

            int moveDis = (id - 1) * _levelMoveDis;
            foreach (var levelGroup in _levelGroups)
            {
                _moveNode.transform.DOLocalMoveX(_startPosition.x - moveDis, isImmediately ? 0f : 0.5f).SetEase(Ease.Linear);
                
                for (var i = 0; i < _levelGroups.Count; i++)
                {
                    int index = i;
                    

                    if (id - 1 == i)
                    {
                        if (isImmediately)
                        {
                            _levelGroups[index].Animator.Play("get_bigger", -1, 0);
                        }
                        else
                        {
                            StartCoroutine(CommonUtils.DelayWork(0.5f, () =>
                            {
                                _levelGroups[index].Animator.Play("get_bigger", -1, 0);
                            }));
                        }
                    }
                    else
                    {
                        if (!isImmediately)
                        {
                            if (id - 2 == i)
                                _levelGroups[i].Animator.Play("zoom_out", -1, 0);
                            else
                            {
                                _levelGroups[i].Animator.Play("Normal"); 
                            }
                        }
                        else
                        {
                            _levelGroups[i].Animator.Play("Normal"); 
                        }
                    }
                }
            }

            _currentLevelId = id;
        }

        private void RefreshStage()
        {
            int stage = ClimbTowerModel.Instance.ClimbTower.Stage;
            _windowsGroups.ForEach(a=>a._windowCells.ForEach(a=>a.gameObject.SetActive(a._index == stage)));
            for (var i = 0; i < _uiBgGroup.Count; i++)
            {
                _uiBgGroup[i].gameObject.SetActive(stage == i);
            }
        }

        private void RefreshAllButtons()
        {
            ClimbTowerModel.GameState state = (ClimbTowerModel.GameState)ClimbTowerModel.Instance.ClimbTower.State;

            _restButton.gameObject.SetActive(false);
            _clamButton.gameObject.SetActive(false);
            _rebornButton.gameObject.SetActive(false);
            _rvRebornButton.gameObject.SetActive(false);
            
            switch (state)
            {
                case ClimbTowerModel.GameState.Free:
                {
                    if(ClimbTowerModel.Instance.ClimbTower.Rewards.Count > 0)
                        _clamButton.gameObject.SetActive(true);
                    
                    break;
                }
                case ClimbTowerModel.GameState.Finish:
                {
                    //_closeButton.gameObject.SetActive(true);
                    break;
                }
                case ClimbTowerModel.GameState.Fail:
                {
                    _restButton.gameObject.SetActive(true);

                    var config = ClimbTowerConfigManager.Instance.GetRewardConfig(_isPayLevel).Find(a => a.Id == ClimbTowerModel.Instance.LevelId(_isPayLevel));
                    _rebornConsume = config.RebornPrice;

                    if (_rebornConsume > 0)
                    {
                        _rebornButton.gameObject.SetActive(true);
                        _rebornButton.onClick.RemoveAllListeners();
                        _rebornButton.onClick.AddListener(RebornHandle);
                        _rebornText.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_button_usegem_continue", _rebornConsume.ToString()));   
                    }
                    else
                    {
                        _rvRebornButton.gameObject.SetActive(true);
                        _rvRebornButton.onClick.RemoveAllListeners();
                        UIAdRewardButton rvBtn = null;
                        rvBtn = UIAdRewardButton.Create(ADConstDefine.R_CLIMB_TOWER_REBORN, UIAdRewardButton.ButtonStyle.Disable, _rvRebornButton.gameObject,
                            (s) =>
                            {
                                if (s)
                                {
                                    DestroyImmediate(rvBtn);
                                    RvRebornHandle();
                                }
                            }, false, null, () =>
                            {
                            });
                        if (!_rvRebornButton.interactable)
                        {
                            _rvRebornButton.gameObject.SetActive(false);
                            _rebornButton.gameObject.SetActive(true);
                            _rebornButton.onClick.RemoveAllListeners();
                            _rebornButton.onClick.AddListener(RebornHandle);
                            _rebornText.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_button_usegem_continue", (-_rebornConsume).ToString()));   
                        }
                    }
                    break;
                }
            }
        }

        private void OnClickOpenWindow(int index)
        {
            if(_isFinish)
                return;
            
            if(index < 0 || index >= _windowsGroups.Count)
                return;

            if(!_isCanClick)
                return;
            
            var group = _windowsGroups[index];
            if((ClimbTowerModel.OpenState)group._openState == ClimbTowerModel.OpenState.Open)
                return;
            
            if (ClimbTowerModel.Instance.ClimbTower.State == (int)ClimbTowerModel.GameState.Fail)
            {
              return;
            }

            string levelId = ClimbTowerModel.Instance.LevelId(_isPayLevel).ToString();
            string isPlay = _isPayLevel ? "true" : "false";

            switch ((ClimbTowerModel.OpenState)group._openState)
            {
                case ClimbTowerModel.OpenState.Close:
                {
                    _isCanClick = false;
                    ClimbTowerModel.Instance.RecordOpenState(index, ClimbTowerModel.OpenState.Open);

                    if (group._rewardId == -10)
                    {
                        int rewardId;
                        int rewardNum;
                        ClimbTowerModel.Instance.FillRewardCache(index, _isPayLevel,_currentLevelId, out rewardId, out rewardNum);
                        
                        group.UpdateReward(rewardId, rewardNum);
                    }
                    
                    
                    AudioManager.Instance.PlaySound(246);
                    
                    group.RefreshState(ClimbTowerModel.OpenState.Open, true);
                    group._openState = (int)ClimbTowerModel.OpenState.Open;
                    if (group._rewardId < 0)
                    {
                        ClimbTowerModel.Instance.SetGameState(ClimbTowerModel.GameState.Fail);
                        RefreshAllButtons();
                        RefreshAllRewardState();
                        _isCanClick = true;
                    }
                    else
                    {
                        _currentLevelId = ClimbTowerModel.Instance.ClimbTower.LevelId;
                        ClimbTowerModel.Instance.ClimbTower.LevelId++;
                        ClimbTowerModel.Instance.AddClimReward(group._rewardId, group._rewardNum);

                        FlyReward(index, group);
                    }
                    break;
                }
                case ClimbTowerModel.OpenState.Open:
                {
                    break;
                }
            }
            
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventClimbTowerChooseNew, levelId, isPlay,group._rewardId.ToString());
            //GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventClimbTowerChoose, ClimbTowerModel.Instance.LevelId(_isPayLevel).ToString(), group._rewardId.ToString(), group._rewardNum.ToString());
        }

        private async UniTask FlyReward(int index, WindowsGroup windowsGroup)
        {
            int level = ClimbTowerModel.Instance.LevelId(_isPayLevel);
            if (level > ClimbTowerConfigManager.Instance.GetRewardConfig(_isPayLevel).Last().Id)
            {
                if (_isPayLevel)
                {
                    ClimbTowerModel.Instance.AddClimReward(ClimbTowerConfigManager.Instance.GetSettingConfig().PayRewardItem[0], ClimbTowerConfigManager.Instance.GetSettingConfig().PayRewardNum[0]);
                }
                else
                {
                    ClimbTowerModel.Instance.AddClimReward(ClimbTowerConfigManager.Instance.GetSettingConfig().RewardItem[0], ClimbTowerConfigManager.Instance.GetSettingConfig().RewardNum[0]);
                }
            }
            
            await UniTask.WaitForSeconds(0.5f);
            for (var i = 0; i < _windowsGroups.Count; i++)
            {
                var group = _windowsGroups[i];
                if (group._rewardId == -10)
                {
                    int rewardId;
                    int rewardNum;
                    ClimbTowerModel.Instance.FillRewardCache(i, _isPayLevel, _currentLevelId, out rewardId, out rewardNum);
                        
                    group.UpdateReward(rewardId, rewardNum);
                }
                
                if (i != index && group._openState != (int)ClimbTowerModel.OpenState.Open)
                {
                    group.RefreshState(ClimbTowerModel.OpenState.Open, false);
                    AudioManager.Instance.PlaySound(246);
                    await UniTask.WaitForSeconds(0.2f);
                }   
            }
            await UniTask.WaitForSeconds(0.5f);
            windowsGroup.GetActiveRewardData().gameObject.SetActive(false);

            await FlyReward(windowsGroup.GetActiveRewardData(), windowsGroup._rewardId, windowsGroup._rewardNum);
            
            if (level > ClimbTowerConfigManager.Instance.GetRewardConfig(_isPayLevel).Last().Id)
            {
                if (_isPayLevel)
                {
                    await FlyReward(_finallyRewardData, ClimbTowerConfigManager.Instance.GetSettingConfig().PayRewardItem[0], ClimbTowerConfigManager.Instance.GetSettingConfig().PayRewardNum[0]);
                }
                else
                {
                    await FlyReward(_finallyRewardData, ClimbTowerConfigManager.Instance.GetSettingConfig().RewardItem[0], ClimbTowerConfigManager.Instance.GetSettingConfig().RewardNum[0]);
                }
            }

            await NextLevel();
            RefreshAllButtons();
            _isCanClick = true;
        }

        private async UniTask FlyReward(RewardData rewardData, int rewardId, int rewardNum)
        {
            var climReward = GetClimReward(rewardId);
            var grayClimReward = GetGrayClimReward(rewardId);
            
            rewardData.gameObject.SetActive(false);

            var flyItem = Instantiate(rewardData.image.gameObject, transform);
            flyItem.transform.localScale = Vector3.one;
            flyItem.transform.position = rewardData.image.transform.position;
            flyItem.gameObject.SetActive(true);

            if (climReward == null)
            {
                climReward = CreateClimReward(rewardId, rewardNum);
                climReward.gameObject.SetActive(false);

                grayClimReward = GetGrayClimReward(rewardId);
                
                RefreshGridLayoutSpace();
                await UniTask.WaitForEndOfFrame();
                
                _scrollView.horizontalNormalizedPosition = 1f;
            }

            int index = _rewardDatas.FindIndex(a => a == climReward);
            if (index > 0)
            {
                _scrollView.horizontalNormalizedPosition = 1.0f*index/(_rewardDatas.Count-1);
            }
            await flyItem.transform.DOMove(climReward.gameObject.transform.position, 0.5f).SetEase(Ease.Linear);
            DestroyImmediate(flyItem);

            climReward.gameObject.SetActive(true);

            var storageResData = ClimbTowerModel.Instance.GatClimReward(climReward.type);
            int oldValue = storageResData.Count - rewardNum;
            int newValue = storageResData.Count;
            climReward.numText.SetText(oldValue.ToString());
            
            grayClimReward?.numText.SetText(newValue.ToString());
            
            await DOTween.To(() => oldValue, x => oldValue = x, newValue, 0.2f).OnUpdate(() =>
            {
                climReward.numText.SetText(oldValue.ToString());
            }).SetEase(Ease.Linear);
            
        }
        private async UniTask NextLevel()
        {
            if (ClimbTowerModel.Instance.ClimbTower.LevelId > ClimbTowerConfigManager.Instance.GetRewardConfig(_isPayLevel).Last().Id)
            {
                ClimReward();
            }
            else
            {
                int oldStage = ClimbTowerModel.Instance.ClimbTower.Stage;
                ClimbTowerModel.Instance.ClimbTower.Stage = (ClimbTowerModel.Instance.ClimbTower.LevelId-1) / 3;

                if (oldStage != ClimbTowerModel.Instance.ClimbTower.Stage)
                {
                    AudioManager.Instance.PlaySound(249);
                    _nextStageObj.SetActive(true);
                    await UniTask.WaitForSeconds(2f);
                    _nextStageObj.SetActive(false);
                }
                
                RefreshStage();
                RefreshLevelProgress(ClimbTowerModel.Instance.LevelId(_isPayLevel), false);
                
                
                List<int> rewardId;
                List<int> rewardNum;
                List<int> openState;

                ClimbTowerModel.Instance.GetRewardCache(_isPayLevel,out rewardId, out rewardNum, out openState);
                
                for (var i = 0; i < _windowsGroups.Count; i++)
                {
                    _windowsGroups[i].RestWindowInfo(rewardId[i], rewardNum[i], openState[i]);
                }
            }
            _isCanClick = true;
        }

        private void ClimReward()
        {
            List<ResData> resDatas = GetRewards();

            if (ClimbTowerModel.Instance.ClimbTower.FreeTimes > 0)
            {
                ClimbTowerModel.Instance.RestClimbTower();
            }
            else
            {
                ClimbTowerModel.Instance.ClimbTower.State = (int)ClimbTowerModel.GameState.Finish;
            }

            if (ClimbTowerModel.Instance.ClimbTower.IsPayLevel)
                ClimbTowerModel.Instance.ClimbTower.IsPayLevel = false;
            
            for (int i = 0; i < resDatas.Count; i++)
            {
                if (!UserData.Instance.IsResource(resDatas[i].id))
                {
                    var itemConfig=GameConfigManager.Instance.GetItemConfig(resDatas[i].id);
                    if (itemConfig != null)
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonClimbTowerGet,
                            itemAId =itemConfig.id,
                            isChange = true,
                        });
                    }
                }
            }
            
            CommonRewardManager.Instance.PopCommonReward(resDatas, CurrencyGroupManager.Instance.currencyController, true, new GameBIManager.ItemChangeReasonArgs() {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.ClimbTowerGet}, () =>
            {
                if (ClimbTowerModel.Instance.ClimbTower.FreeTimes > 0)
                {
                    ClimbTowerModel.Instance.ClimbTower.FreeTimes--;
                    UIManager.Instance.CloseUI(UINameConst.UIClimbTowerMainPay, true);
                    UIManager.Instance.OpenWindow(UINameConst.UIClimbTowerMain);
                }
                else if(ClimbTowerModel.Instance.IsCanPay())
                {
                    UIManager.Instance.CloseUI(UINameConst.UIClimbTowerMain, true);
                    UIManager.Instance.OpenWindow(UINameConst.UIClimbTowerMainPay, true);
                    UIManager.Instance.OpenWindow(UINameConst.UIPopupClimbTowerPay);
                }
                else
                {
                    AnimCloseWindow();
                }
            });
        }

        private List<ResData> GetRewards()
        {
            List<ResData> resDatas = new List<ResData>();
            foreach (var climbTowerReward in ClimbTowerModel.Instance.ClimbTower.Rewards)
            {
                resDatas.Add(new ResData(climbTowerReward.Id, climbTowerReward.Count));
            }

            return resDatas;
        }

        private void RefreshAllRewardState()
        {
            switch ((ClimbTowerModel.GameState)ClimbTowerModel.Instance.ClimbTower.State)
            {
                case ClimbTowerModel.GameState.Free:
                {
                    _grayRewardDatas.ForEach(a=>a.gameObject.SetActive(false));
                    _rewardDatas.ForEach(a=>a.gameObject.SetActive(true));
                    _windowsGroups.ForEach(a=>a.SetGaryObjActive(false));
                    _tipText.SetTerm("ui_tower_rush_your_rewards");
                    break;
                }
                case ClimbTowerModel.GameState.Fail:
                {
                    _grayRewardDatas.ForEach(a=>a.gameObject.SetActive(true));
                    _rewardDatas.ForEach(a=>a.gameObject.SetActive(false));
                    _windowsGroups.ForEach(a=>a.SetGaryObjActive(true));
                    _tipText.SetTerm("ui_tower_rush_lose_rewards");
                    break;
                }
            }
            
        }

        private void OnDestroy()
        {
            AudioManager.Instance.PlayMusic(1, true);
        }
    }
}