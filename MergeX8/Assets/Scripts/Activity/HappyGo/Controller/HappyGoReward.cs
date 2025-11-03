using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using TMPro;
using System;
using Deco.Item;
using Deco.World;
using DG.Tweening;
using DragonPlus.Config.HappyGo;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;

public class HappyGoReward : MonoBehaviour
{
    private Slider _topSlider;
    private LocalizeTextMeshProUGUI _sliderText;
    private Image _icon1;
    private LocalizeTextMeshProUGUI _lvText;
    private Image _icon2;
    private LocalizeTextMeshProUGUI _rewardNum;
    private Button _helpBtn;

    public Transform Icon1Trans
    {
        get
        {
            return _icon1.transform;
        }
    }

    private HGVDLevel _currentLevel;
    private HGVDLevel _nextLevel;
    private void Awake()
    {
        _topSlider = transform.Find("Slider").GetComponent<Slider>();
        _sliderText = transform.Find("Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _icon1 = transform.Find("Icon1").GetComponent<Image>();
        _icon2 = transform.Find("Icon2").GetComponent<Image>();
        _lvText = transform.Find("LvGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _rewardNum = transform.Find("RewardNum").GetComponent<LocalizeTextMeshProUGUI>();
        _helpBtn = transform.Find("HelpBtn").GetComponent<Button>();
        _helpBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIHappyGoHelp);
        });
        EventDispatcher.Instance.AddEventListener(MergeEvent.HAPPYGO_EXP_REFRESH, RefreshHappyGoExp);
        EventDispatcher.Instance.AddEventListener(EventEnum.HAPPYGO_CLAIM_REWARD, UpdateInfo);
        GetComponent<Button>().onClick.AddListener(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoReward);
        });
    }

    private void UpdateInfo(BaseEvent obj)
    {
        UpdateInfo();
    }

    private void OnEnable()
    {
        UpdateInfo();
    }

    public void Init()
    {
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        _currentLevel= HappyGoModel.Instance.GetCurrentLevel();
        _nextLevel= HappyGoModel.Instance.GetNextLevelConfig();
        _lvText.SetText(HappyGoModel.Instance.GetLevel().ToString());
        _rewardNum.SetText(_nextLevel.amount[0].ToString());
        if (UserData.Instance.IsResource(_nextLevel.reward[0]))
        {
            _icon2.sprite = UserData.GetResourceIcon(_nextLevel.reward[0]);
        }
        else
        {
            var itemConfig= GameConfigManager.Instance.GetItemConfig(_nextLevel.reward[0]);
            if(itemConfig != null)
                _icon2.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            else
            {
                DecoItem item = null;
                if (DecoWorld.ItemLib.ContainsKey(_nextLevel.reward[0]))
                    item = DecoWorld.ItemLib[_nextLevel.reward[0]];

                if (item != null)
                {
                    _icon2.sprite = ResourcesManager.Instance.GetSpriteVariant("HappyGoAtlas", item.Config.buildingIcon);
                    _rewardNum.gameObject.SetActive(false);
                } 
            }
        }
        _topSlider.value = HappyGoModel.Instance.GetNextLevelProgress();
        _sliderText.SetText(HappyGoModel.Instance.GetProgressStr());
    }
    void RefreshHappyGoExp(BaseEvent e)
    {
        int value=(int)e.datas[0];
        var next = HappyGoModel.Instance.GetNextLevelConfig();
        if (next.lv>_nextLevel.lv)//升级
        {
            int oldValue = HappyGoModel.Instance.GetExp() - value - _currentLevel.xp;
            int newValue = _nextLevel.xp-_currentLevel.xp;
            _topSlider.DOValue(1f, 1);
            DOTween.To(() => oldValue, x => oldValue = x, newValue, 1f).OnUpdate(() =>
            {   
                _sliderText.SetText(oldValue+ "/"+newValue);
            }).OnComplete(() =>
            {
                _nextLevel= HappyGoModel.Instance.GetNextLevelConfig();
                _currentLevel= HappyGoModel.Instance.GetCurrentLevel();
                _lvText.SetText(_currentLevel.lv.ToString());
                if (UserData.Instance.IsResource(_nextLevel.reward[0]))
                {
                    _icon2.sprite = UserData.GetResourceIcon(_nextLevel.reward[0]);
                }
                else
                {
                    var itemConfig= GameConfigManager.Instance.GetItemConfig(_nextLevel.reward[0]);
                    if(itemConfig != null)
                        _icon2.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
                }
                _rewardNum.SetText(_nextLevel.amount[0].ToString());
                if (HappyGoModel.Instance.IsMaxExp())
                {
                    _lvText.SetText(_nextLevel.lv.ToString());
                    _sliderText.SetText(HappyGoModel.Instance.GetProgressStr());
                    return;
                }
                _topSlider.value = 0;
                oldValue = 0;
                newValue = HappyGoModel.Instance.GetExp()- _currentLevel.xp;
          
                DOTween.To(() => oldValue, x => oldValue = x, newValue, 1f).OnUpdate(() =>
                {
                    _sliderText.SetText(oldValue + "/" +( _nextLevel.xp-_currentLevel.xp));
                });
                _topSlider.DOValue(1f *  (HappyGoModel.Instance.GetExp() - _currentLevel.xp)  / ( _nextLevel.xp-_currentLevel.xp), 1);


                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.HappyGoLevelUp, null);
            });
        }
        else
        {
            int oldValue = HappyGoModel.Instance.GetExp() - value - _currentLevel.xp;
            int newValue = HappyGoModel.Instance.GetExp() - _currentLevel.xp;
            if (HappyGoModel.Instance.IsMaxExp())
            {
                _lvText.SetText(_nextLevel.lv.ToString());
                _sliderText.SetText(HappyGoModel.Instance.GetProgressStr());
                _topSlider.value = 1;
                return;
            }
            DOTween.To(() => oldValue, x => oldValue = x, newValue, 1f).OnUpdate(() =>
            {
                _sliderText.SetText(oldValue + "/" + (_nextLevel.xp-_currentLevel.xp));
            });
            _topSlider.DOValue(1f * (HappyGoModel.Instance.GetExp() - _currentLevel.xp) /(_nextLevel.xp-_currentLevel.xp), 1);

        }
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.HAPPYGO_EXP_REFRESH, RefreshHappyGoExp);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.HAPPYGO_CLAIM_REWARD, UpdateInfo);

    }
}