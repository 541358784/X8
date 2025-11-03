using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using GamePool;
using TMatch;
using UnityEngine;
using UnityEngine.UI;
using FishEatFishSpace;
using AudioManager = TMatch.AudioManager;


public class UIFishEatFishMainController:UIWindowController
    {
        private Button _startBtn;
        private Button _finishBtn;
        private Button _closeBtn;
        private Transform _showTargetView;
        public override void PrivateAwake()
        {
            _startBtn = GetItem<Button>("Root/Start");
            _finishBtn = GetItem<Button>("Root/Finish");
            _closeBtn = GetItem<Button>("Root/PauseButton");
            _showTargetView = transform.Find("Root/Tip");
            if (_showTargetView)
                _showTargetView.gameObject.SetActive(false);

            _startBtn.onClick.AddListener(() =>
            {
                _startBtn.gameObject.SetActive(false);
            });
            _startBtn.gameObject.SetActive(false);
            
            _finishBtn.onClick.AddListener(OnClickFinish);
            _finishBtn.gameObject.SetActive(false);
            
            _closeBtn.onClick.AddListener(OnClickQuitGame);
        }

        private int _levelId;
        private bool _isFirstTimePlay;
        public void BindLevelConfig(int levelId,bool isFirstTimePlay,int levelType)
        {
            _isFirstTimePlay = isFirstTimePlay;
            _levelId = levelId;
            _startBtn.transform.Find("Group").gameObject.SetActive(false);
            _startBtn.transform.Find("Text (1)").gameObject.SetActive(false);
            _startBtn.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetTerm("start_key");
            _finishBtn.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetTerm("ui_minigame_FishEnd_Success");
            _showTargetView.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetTerm("tip_key");
            _finishBtn.transform.Find("Bag")?.gameObject.SetActive(levelId == 1);
            if (levelId == 1)
            {
                _finishBtn.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetTerm("ui_minigame_End_Dive");
            }

            transform.Find("pg2").gameObject.SetActive(levelType == 1);
            transform.Find("pg1").gameObject.SetActive(levelType == 1);
        }
        public void OnClickFinish()
        {
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.CompletedFishEatFishGame);
            TMatch.UILoadingEnter.Open(() =>
            {
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameFishLevelend,
                    data1:_levelId.ToString(),data2:"0");
                EventDispatcher.Instance.DispatchEventImmediately(EventEnum.FinishFishEatFishGame);
            });
            if (_levelId != 1)
            {
                if (AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.IN_AsmrContinue))
                {
                    AdSubSystem.Instance.PlayInterstital(ADConstDefine.IN_AsmrContinue, b =>
                    {
                    });
                }   
            }
        }

        public void OnClickQuitGame()
        {
            CommonUtils.OpenCommon1ConfirmWindow(new NoticeUIData
            {
                DescString =
                    LocalizationManager.Instance.GetLocalizedString("ui_minigame_11"),
                OKCallback = () =>
                {
                    TMatch.UILoadingEnter.Open(() =>
                    {
                        GameBIManager.Instance.SendGameEvent(
                            BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameFishLevelend,
                            data1:_levelId.ToString(),data2:"1");
                        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.FinishFishEatFishGame);
                    });
                },
                HasCloseButton = true,
                HasCancelButton = true,
                IsHighSortingOrder = true,
            });
        }

        public void SetFinishLevel()
        {
            AudioManager.Instance.PlaySound("sfx_fig_win");
            _finishBtn.gameObject.SetActive(true);
        }

        public void HideCloseBtn()
        {
            _closeBtn.gameObject.SetActive(false);
        }

        public async void ShowTargetText()
        {
            _showTargetView.gameObject.SetActive(true);
            await XUtility.WaitSeconds(3f);
            if (_showTargetView)
            {
                _showTargetView.gameObject.SetActive(false);
            }
        }
    }