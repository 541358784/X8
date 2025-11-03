using System.Collections.Generic;
using ConnectLine;
using ConnectLine.Model;
using Decoration;
using Decoration.DaysManager;
using DigTrench;
using Ditch.Model;
using DragonPlus;
using DragonPlus.Config.DigTrench;
using DragonPlus.Config.Ditch;
using DragonPlus.Config.FishEatFish;
using DragonPlus.Config.Makeover;
using DragonU3DSDK.Storage;
using Filthy.Game;
using Filthy.Model;
using Gameplay;
using Gameplay.UI.MiniGame;
using Makeover;
using OnePath;
using Psychology;
using Psychology.Model;
using UnityEngine;
using UnityEngine.UI;
using Utils = Makeover.Utils;

class UITask_DayItem
{
    private GameObject _curtObj;
    private GameObject _finishObj;
    private GameObject _rewardGroup;
    private Image _rewardItem;
    private int _index;
    private TableDays _config;
    public GameObject _gameObject;
    private List<Image> _rewards = new List<Image>();
    private GameObject _fishEatFish;
    private GameObject _digTrenchGame;
    private GameObject _onePathGame;
    private GameObject _connectLine;
    private GameObject _psychology;

    public void Init(GameObject obj, TableDays config, int index)
    {
        _gameObject = obj;
        _index = index;
        _config = config;

        _curtObj = obj.transform.Find("Progress").gameObject;
        _finishObj = obj.transform.Find("Finish").gameObject;

        _rewardGroup = obj.transform.Find("Reward").gameObject;
        _fishEatFish = obj.transform.Find("Game").gameObject;
        _digTrenchGame = obj.transform.Find("DigTrenchGame").gameObject;
        _onePathGame = obj.transform.Find("OnePathGame").gameObject;
        _connectLine = obj.transform.Find("ConnectLine").gameObject;
        _psychology = obj.transform.Find("Psychology").gameObject;

        _rewardItem = obj.transform.Find("Reward/Icon").GetComponent<Image>();
        _rewardItem.gameObject.SetActive(false);

        _fishEatFish.gameObject.SetActive(false);
        _digTrenchGame.gameObject.SetActive(false);
        _onePathGame.gameObject.SetActive(false);
        _connectLine.gameObject.SetActive(false);
        _psychology.gameObject.SetActive(false);

        UpdateUI();
    }


    public void UpdateUI(int index = -1, TableDays config = null)
    {
        _index = index < 0 ? _index : index;
        _config = config == null ? _config : config;

        _curtObj.gameObject.SetActive(DaysManager.Instance.DayStep + 1 == _index);
        _finishObj.gameObject.SetActive(_index <= DaysManager.Instance.DayStep);

        UpdateMiniGameView();

        if (_config.rewardIndex == null || _config.rewardIndex.Length == 0)
            return;

        List<ResData> resDatas = DaysManager.Instance.GetDayStepRewardByIndex(_config, _index);
        if (resDatas == null || resDatas.Count == 0)
            return;

        _rewardGroup.gameObject.SetActive(true);
        int newNum = resDatas.Count - _rewards.Count;
        newNum = 1;
        for (int i = 0; i < newNum; i++)
        {
            var item = GameObject.Instantiate(_rewardItem.gameObject, _rewardGroup.transform);
            item.gameObject.SetActive(true);

            var image = item.GetComponent<Image>();
            image.sprite = UserData.GetResourceIcon(resDatas[i].id, UserData.ResourceSubType.Big);

            _rewards.Add(image);
        }

        if (_index <= DaysManager.Instance.DayStep)
            _rewardGroup.gameObject.SetActive(false);


        _rewards.ForEach(a => a.gameObject.SetActive(false));
        for (int i = 0; i < newNum; i++)
        {
            _rewards[i].gameObject.SetActive(true);
            _rewards[i].sprite = UserData.GetResourceIcon(resDatas[i].id, UserData.ResourceSubType.Big);
        }
    }


    private void UpdateMiniGameView()
    {
        _rewardGroup.gameObject.SetActive(false);
        _fishEatFish.gameObject.SetActive(false);
        _digTrenchGame.gameObject.SetActive(false);
        _psychology.gameObject.SetActive(false);
        _onePathGame.gameObject.SetActive(false);
        _connectLine.gameObject.SetActive(false);

        return;
        if (!Makeover.Utils.IsOpen)
            return;

        if (_index <= DaysManager.Instance.DayStep)
            return;

        int totalNodes = DaysManager.Instance.GetDayTotalNodes();

        if (DitchModel.Instance.Ios_Ditch_Plan_D())
        {
            foreach (var config in DitchConfigManager.Instance.TableDitchLevelList)
            {
                if (config.UnlockNodeNum == totalNodes + _index + 1)
                {
                    _digTrenchGame.gameObject.SetActive(true);
                    break;
                }
            }
            return;
        }
        if (Utils.IsUseNewMiniGame())
        {
            if (!FilthyGameLogic.Instance.IsOpenFilthy())
            {
                var setting = Utils.GetMiniGameSettings();
                for (var i = 0; i < setting.Count; i++)
                {
                    var config = setting[i];
                    UIPopupMiniGameController.MiniGameType type = (UIPopupMiniGameController.MiniGameType)config.type;
                    if (!Utils.IsOn(type))
                        continue;

                    switch (type)
                    {
                        case UIPopupMiniGameController.MiniGameType.ConnectLine:
                        {
                            foreach (var tableMoLevel in ConnectLineConfigManager.Instance._configs)
                            {
                                if (tableMoLevel.unlockNodeNum == totalNodes + _index + 1)
                                {
                                    _connectLine.gameObject.SetActive(true);
                                    break;
                                }
                            }

                            break;
                        }
                        case UIPopupMiniGameController.MiniGameType.DigTrench:
                        {
                            foreach (var tableMoLevel in DigTrenchConfigManager.Instance.DigTrenchLevelList)
                            {
                                if (tableMoLevel.unlockNodeNum == totalNodes + _index + 1)
                                {
                                    _digTrenchGame.gameObject.SetActive(true);
                                    break;
                                }
                            }

                            break;
                        }
                        case UIPopupMiniGameController.MiniGameType.Psychology:
                        {
                            foreach (var tableMoLevel in PsychologyConfigManager.Instance._configs)
                            {
                                if (tableMoLevel.unlockNodeNum == totalNodes + _index + 1)
                                {
                                    _psychology.gameObject.SetActive(true);
                                    break;
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }
        else
        {
#if UNITY_ANDROID || UNITY_EDITOR
            if (StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey("1.0.69"))
            {
                return;
            }  
#endif
            
            foreach (var tableMoLevel in FishEatFishConfigManager.Instance.FishEatFishLevelList)
            {
                if (tableMoLevel.unlockNodeNum == totalNodes + _index + 1)
                {
                    _fishEatFish.gameObject.SetActive(true);
                    break;
                }
            }

            foreach (var tableMoLevel in DigTrenchConfigManager.Instance.DigTrenchLevelList)
            {
                if (tableMoLevel.unlockNodeNum == totalNodes + _index + 1)
                {
                    _digTrenchGame.gameObject.SetActive(true);
                    break;
                }
            }

            foreach (var tableMoLevel in OnePathConfigManager.Instance._configs)
            {
                if (tableMoLevel.unlockNodeNum == totalNodes + _index + 1)
                {
                    _onePathGame.gameObject.SetActive(true);
                    break;
                }
            }

            foreach (var tableMoLevel in ConnectLineConfigManager.Instance._configs)
            {
                if (tableMoLevel.unlockNodeNum == totalNodes + _index + 1)
                {
                    _connectLine.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }
}

public partial class UIPopupTaskController
{
    private LocalizeTextMeshProUGUI _dayNum;
    private GameObject _dayItem;
    private GameObject _dayContentGroup;
    private List<UITask_DayItem> _dayItems = new List<UITask_DayItem>();

    private void Awake_Day()
    {
        _dayNum = GetItem<LocalizeTextMeshProUGUI>("Root/Task/Slider/Icon/Num");
        _dayItem = GetItem("Root/Task/Slider/RewardGroup/1");
        _dayItem.gameObject.SetActive(false);

        _dayContentGroup = GetItem("Root/Task/Slider/RewardGroup");
    }

    private void InitDayView()
    {
        _dayNum.SetText((DaysManager.Instance.DayNum + 1).ToString());

        var dayConfig = DaysManager.Instance.GetDayConfig();
        if (dayConfig == null)
            return;

        for (int i = 0; i < dayConfig.nodeNumber; i++)
        {
            var item = Instantiate(_dayItem, _dayContentGroup.transform);
            item.gameObject.SetActive(true);

            UITask_DayItem dayItem = new UITask_DayItem();
            dayItem.Init(item, dayConfig, i);

            _dayItems.Add(dayItem);
        }
    }
}