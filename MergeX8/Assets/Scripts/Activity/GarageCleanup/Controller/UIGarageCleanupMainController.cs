using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIGarageCleanupMainController : UIWindowController
{
    private LocalizeTextMeshProUGUI _timeText;
    private List<GarageCleanupBoardItem> _boardItems;
    private Slider _slider;
    private Image _sliderIcon;
    private LocalizeTextMeshProUGUI _sliderText;
    private LocalizeTextMeshProUGUI _sliderRewardCountText;
    
    private LocalizeTextMeshProUGUI _activityNumber;
    private LocalizeTextMeshProUGUI _endText;
    
    private Button _buttonClose;
    private Button _buttonHelp;
    private Button _buttonShow;
    private Button _buttonPlay;
    private Button _buttonStart;
    private LocalizeTextMeshProUGUI _startDiamond;
    private List<GarageCleanupRewardItem> _rewardItems;
    private Transform _rewardGroup;
    private Transform _finishGroup;
    private LocalizeTextMeshProUGUI _diamondUnlockText;
    private LocalizeTextMeshProUGUI _couponsText;
    public override void PrivateAwake()
    {
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _diamondUnlockText = GetItem<LocalizeTextMeshProUGUI>("Root/DiamondText");
        _boardItems = new List<GarageCleanupBoardItem>();
        _rewardItems = new List<GarageCleanupRewardItem>();
        for (int i = 1; i < 26; i++)
        {
            GarageCleanupBoardItem boardItem=transform.Find("Root/Grid/" + i).gameObject.AddComponent<GarageCleanupBoardItem>();
            _boardItems.Add(boardItem);
        }

        _slider = GetItem<Slider>("Root/Slider");
        _sliderIcon = GetItem<Image>("Root/Slider/Icon");
        _sliderText = GetItem<LocalizeTextMeshProUGUI>("Root/Slider/Text");
        _sliderRewardCountText = GetItem<LocalizeTextMeshProUGUI>("Root/Slider/Num");
        _couponsText = GetItem<LocalizeTextMeshProUGUI>("Root/Coupons/Text");
        _activityNumber = GetItem<LocalizeTextMeshProUGUI>("Root/ActivityNumber/Text");
        _endText = GetItem<LocalizeTextMeshProUGUI>("Root/Label/Text");

        _buttonClose = GetItem<Button>("Root/ButtonGroup/ButtonClose");
        _buttonClose.onClick.AddListener(OnBtnClose);
        _buttonHelp = GetItem<Button>("Root/ButtonGroup/HelpButton");
        _buttonHelp.onClick.AddListener(OnBtnHelp);
        _buttonShow = GetItem<Button>("Root/ButtonGroup/ShowButton");
        _buttonShow.onClick.AddListener(OnBtnShow);
        _buttonPlay = GetItem<Button>("Root/ButtonGroup/PlayButton");
        _buttonPlay.onClick.AddListener(OnBtnPlay);
        _buttonStart = GetItem<Button>("Root/ButtonGroup/StartButton");
        _buttonStart.onClick.AddListener(OnBtnStart);
        _startDiamond = GetItem<LocalizeTextMeshProUGUI>("Root/ButtonGroup/StartButton/Num");
        for (int i = 1; i < 4; i++)
        {
            _rewardItems.Add(transform.Find("Root/RewardGroup/" + i).gameObject.AddComponent<GarageCleanupRewardItem>()); 
        }

        _rewardGroup=transform.Find("Root/RewardGroup");
        _finishGroup=transform.Find("Root/Finish");
        InvokeRepeating("UpdateTimeText",1,1);
        UpdateTimeText();
        EventDispatcher.Instance.AddEventListener(EventEnum.GARAGE_CLEANUP_TURNIN, TurnIn);
        EventDispatcher.Instance.AddEventListener(EventEnum.GARAGE_CLEANUP_LevelFinish, LevelFinish);
        EventDispatcher.Instance.AddEventListener(EventEnum.AddFishpondToken, RefreshCoupons);
    }

    private void LevelFinish(BaseEvent obj)
    {
        if ((MergeBoardEnum) obj.datas[0] != MergeBoardEnum.Main)
            return;
        StartCoroutine(FinishLevel());
    }
    public IEnumerator FinishLevel()
    {
        yield return new  WaitForSeconds(1.5f);
        _finishGroup.gameObject.SetActive(true);
        yield return new  WaitForSeconds(4f);
        _finishGroup.gameObject.SetActive(false);
        if (GarageCleanupModel.Instance.IsActivityFinish())
        {
            AnimCloseWindow(() =>
            {
                GarageCleanupModel.Instance.StorageGarageCleanup.IsActivityEnd = true;
                // UIManager.Instance.OpenUI(UINameConst.UIPopupGarageCleanupEnd, GarageCleanupModel.Instance.StorageGarageCleanup);
            });
        }
        else
        {
            InitBoard();
            UpdateStatus();
        }

    }

    public void RefreshCoupons(BaseEvent obj)
    {
        _couponsText.SetText(UserData.Instance.GetRes(UserData.ResourceId.Fishpond_token).ToString());
    }
    private void TurnIn(BaseEvent obj)
    {
        if (obj== null || obj.datas == null || obj.datas.Length < 4)
            return;
        if ((MergeBoardEnum) obj.datas[0] != MergeBoardEnum.Main)
            return;
        int index = (int) obj.datas[1];
        List<List<int>> lines =( List<List<int>>) obj.datas[3];
        _boardItems[index].PlayAni("Icon_disappear", () =>
        {
            CoroutineManager.Instance.StartCoroutine(LineAni(lines,index));  

        });
        _slider.DOValue((float) GarageCleanupModel.Instance.GetProgress() / 4, 0.3f);
        _sliderText.SetText(GarageCleanupModel.Instance.GetProgress()+"%");
        _couponsText.SetText(UserData.Instance.GetRes(UserData.ResourceId.Fishpond_token).ToString());
    }
    public IEnumerator LineAni(List<List<int>> lines,int _index)
    {
        if (lines.Count > 0)
        {
            foreach (var line in lines)
            {
                if (line.Count>5)
                {
                    foreach (var item in line)
                    {
                        _boardItems[item].UpdateFinishState();
                        _boardItems[item].PlayAni("Finish_appear",() =>
                        {
                            _boardItems[item].UpdateFinishState();
                        });
                    }
                }
                else
                {
                    foreach (var item in line)
                    {
                        _boardItems[item].UpdateFinishState();
                        _boardItems[item].PlayAni("Finish_appear",() =>
                        {
                            _boardItems[item].UpdateStatus();
                        });
                    }
                }
                
            }
        }
        else
        {
            _boardItems[_index].UpdateFinishState();
            _boardItems[_index].PlayAni("Finish_appear",() =>
            {
                _boardItems[_index].UpdateStatus();
            });
            yield break;
        }
    }
    private void OnBtnStart()
    {
        var garageCleanupConfig= GarageCleanupModel.Instance.GetGarageCleanupConfigByLevel();
        int needCount = garageCleanupConfig.UnlockConsume[1];
        if (!UserData.Instance.CanAford(UserData.ResourceId.Diamond,  garageCleanupConfig.UnlockConsume[1]))
        {
            BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "",
                "", "",true,needCount);
        }
        else
        {
            UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond,  garageCleanupConfig.UnlockConsume[1], new GameBIManager.ItemChangeReasonArgs
            {
                reason = BiEventCooking.Types.ItemChangeReason.GarageCleanConsume
            });
            GarageCleanupModel.Instance.UnlockLevel();
            UpdateStatus();
            StorageGarageCleanupBoard board = GarageCleanupModel.Instance.GetBoard();
            for (int i = 0; i < _boardItems.Count; i++)
            {
                _boardItems[i].UpdateStatus();
            }
        }
      
    }

    private void OnBtnPlay()
    {
        AnimCloseWindow();
        if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
        {
            SceneFsm.mInstance.TransitionGame();
        }

    }

    private void OnBtnShow()
    {
        GarageCleanupModel.Instance.RevealBoard();
        RevealBoard();
        
        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventGarageCleanTapReceal,GarageCleanupModel.Instance.GetCleanupLevel().ToString());
    }

    private void RevealBoard()
    {
        _buttonShow.gameObject.SetActive(false);
        StartCoroutine(AniRevelBoard());
    }

    public IEnumerator AniRevelBoard()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int x = 0; x <=i; x++)
            { 
                int _index = x  + i* 5;
                _boardItems[_index].PlayAni("QuestionMark_disappear", () =>
                {
                    _boardItems[_index].UpdateStatus();
                });
            }
            for (int y = 0; y <=i; y++)
            {
                int _index = i + y* 5;
                _boardItems[_index].PlayAni("QuestionMark_disappear",() =>
                {
                    _boardItems[_index].UpdateStatus();
                });
            }

            yield return new  WaitForSeconds(0.1f);
        }
        UpdateStatus();
    }

    private void OnBtnHelp()
    {
        UIManager.Instance.OpenUI(UINameConst.UIPopupGarageCleanupHelp);
    }

    private void OnBtnClose()
    {
        AnimCloseWindow();
    }

    private void Start()
    {
        InitBoard();
        UpdateStatus();
    }

    private void InitBoard()
    {   
        StorageGarageCleanupBoard board = GarageCleanupModel.Instance.GetBoard();
        for (int i = 0; i < _boardItems.Count; i++)
        {
            _boardItems[i].SetData(board.Items[i],board.IsReveal,i);
        }
        _couponsText.SetText(UserData.Instance.GetRes(UserData.ResourceId.Fishpond_token).ToString());
    }
    private void UpdateStatus()
    {
        StorageGarageCleanupBoard board = GarageCleanupModel.Instance.GetBoard();
     
        var garageCleanupConfig= GarageCleanupModel.Instance.GetGarageCleanupConfigByLevel();
        _rewardItems[0].SetData(garageCleanupConfig.RowReward[0],garageCleanupConfig.RowRewardCount[0]);
        _rewardItems[1].SetData(garageCleanupConfig.DiagonalsReward[0],garageCleanupConfig.DiagonalsRewardCount[0]);
        _rewardItems[2].SetData(garageCleanupConfig.FullReward[0],garageCleanupConfig.FullRewardCount[0]);
        _activityNumber.SetText((GarageCleanupModel.Instance.StorageGarageCleanup.Level+1)+"/3");
        _endText.transform.parent.gameObject.SetActive(GarageCleanupModel.Instance.IsEndSoon());
        var itemConfig = GameConfigManager.Instance.GetItemConfig(garageCleanupConfig.FullReward[0]);
        _sliderIcon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
        _sliderRewardCountText.SetText("x"+garageCleanupConfig.FullRewardCount[0]);
        _slider.value = (float) GarageCleanupModel.Instance.GetProgress()/4;
        _sliderText.SetText(GarageCleanupModel.Instance.GetProgress()+"%");
        _diamondUnlockText.gameObject.SetActive(false);
        if (!board.IsReveal)
        {
            _buttonShow.gameObject.SetActive(true);
            _buttonPlay.gameObject.SetActive(false);
            _buttonStart.gameObject.SetActive(false);
            _slider.gameObject.SetActive(false);
            _rewardGroup.gameObject.SetActive(false);
        }
        else
        {
            if (!board.IsStart )
            {
                _startDiamond.SetText(garageCleanupConfig.UnlockConsume[1].ToString());
                _buttonShow.gameObject.SetActive(false);
                _buttonPlay.gameObject.SetActive(false);
                _buttonStart.gameObject.SetActive(true);
                _slider.gameObject.SetActive(false);
                _rewardGroup.gameObject.SetActive(false);
                _diamondUnlockText.gameObject.SetActive(true);
            }
            else
            {
                _slider.gameObject.SetActive(true);
                _rewardGroup.gameObject.SetActive(true);
                _buttonShow.gameObject.SetActive(false);
                _buttonPlay.gameObject.SetActive(true);
                _buttonStart.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateTimeText()
    {
        _timeText.SetText(GarageCleanupModel.Instance.GetActivityLeftTimeString());
        if (GarageCleanupModel.Instance.GetActivityLeftTime() <= 0)
        {
            UIPopupGarageCleanupSubmitController controller =
                UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupGarageCleanupSubmit) as
                    UIPopupGarageCleanupSubmitController;
            controller?.CloseWindow();
            AnimCloseWindow(() =>
            {
                GarageCleanupModel.Instance.StorageGarageCleanup.IsActivityEnd = true;
                // UIManager.Instance.OpenUI(UINameConst.UIPopupGarageCleanupEnd, GarageCleanupModel.Instance.StorageGarageCleanup);

            });
        }
     
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GARAGE_CLEANUP_TURNIN, TurnIn);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GARAGE_CLEANUP_LevelFinish, LevelFinish);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.AddFishpondToken, RefreshCoupons);

    }
}