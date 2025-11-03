using System;
using System.Collections;
using System.Collections.Generic;
using Activity.CrazeOrder.Model;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CrazeOrder;
using Gameplay;
using IFix.Core;
using UnityEngine;
using UnityEngine.UI;

public class CrazeRewardItem
{
    private GameObject _normalGameObject;
    private GameObject _finishGameObject;
    private LocalizeTextMeshProUGUI _levelText;
    private LocalizeTextMeshProUGUI _finishLevelText;
    private GameObject _gameObject;
    private int _index;
    
    public void Init(GameObject gameObject, int index, List<int> rewardIds, List<int> rewardNums)
    {
        _gameObject = gameObject;
        _index = index;
        
        _normalGameObject = gameObject.transform.Find("Normal").gameObject;
        var normalGameObject = gameObject.transform.Find("Normal/Reward").gameObject;
        var normalItem = gameObject.transform.Find("Normal/Reward/Item").gameObject;
        normalItem.gameObject.SetActive(false);

        var finishGameObject = gameObject.transform.Find("Finish/Reward").gameObject;
        var finishItem = gameObject.transform.Find("Finish/Reward/Item").gameObject;
        finishItem.gameObject.SetActive(false);
        
        _levelText = gameObject.transform.Find("Normal/Lv/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _levelText.SetText(index.ToString());
        
        _finishGameObject = gameObject.transform.Find("Finish").gameObject;
        _finishLevelText = gameObject.transform.Find("Finish/Lv/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _finishLevelText.SetText(index.ToString());
        
        for(int i = 0; i < rewardIds.Count; i++)
        {
            Sprite sprite = UserData.GetResourceIcon(rewardIds[i]);
            var itemCell = GameObject.Instantiate(normalItem, normalGameObject.transform);
            itemCell.gameObject.SetActive(true);
            itemCell.transform.Find("Icon").GetComponent<Image>().sprite = sprite;
            var text = itemCell.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            text.SetText("X"+rewardNums[i].ToString());
            text.gameObject.SetActive(rewardNums[i] > 1);
            
            var finishItemCell = GameObject.Instantiate(finishItem, finishGameObject.transform);
            finishItemCell.gameObject.SetActive(true);
            finishItemCell.transform.Find("Icon").GetComponent<Image>().sprite = sprite;
            var text2 = finishItemCell.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            text2.SetText("X"+rewardNums[i].ToString());
            text2.gameObject.SetActive(rewardNums[i] > 1);
        }

        SetFinish(false);
    }

    public void SetFinish(bool isFinish)
    {
        _finishGameObject.gameObject.SetActive(isFinish);
        _normalGameObject.gameObject.SetActive(!isFinish);
    }
}
public class UICrazeOrderMainController : UIWindowController
{
    private int[] _stageSliderValue = new[] {4, 3, 3, 2 };
    
    private Button _closeButton;
    private Button _joinButton;
    private bool _isCanClose = true;
    private LocalizeTextMeshProUGUI _coolTimeText;
    private List<CrazeRewardItem> _rewardItems = new List<CrazeRewardItem>();
    private Slider _slider;
    private Animator _slider2;
    private int lastStage = 0;
    
    public override void PrivateAwake()
    {
        _slider = transform.Find("Root/RewardGroup/Slider").GetComponent<Slider>();
        _slider2 = transform.Find("Root/RewardGroup/Slider2").GetComponent<Animator>();
        
        _closeButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
        _closeButton.onClick.AddListener(CloseUI);

        _coolTimeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        
        _joinButton = transform.Find("Root/Button").GetComponent<Button>();
        _joinButton.onClick.AddListener(CloseUI);
        
        InvokeRepeating("InvokeUpdate", 0, 1);
    }

    public void Start()
    {
        for (int i = 1; i <= 5; i++)
        {
            var obj = transform.Find("Root/RewardGroup/"+i).gameObject;

            CrazeRewardItem rewardItem = new CrazeRewardItem();
            if (i <= 4)
            {
                var config = CrazeOrderConfigManager.Instance.GetStageConfigs()[i - 1];
                rewardItem.Init(obj, config.OrderNum, config.RewardIds, config.RewardNums);
            }
            else
            {
                var config = CrazeOrderConfigManager.Instance.CrazeOrderSettingList[0];
                rewardItem.Init(obj, i, config.RewardIds, config.RewardNums);
            }
            _rewardItems.Add(rewardItem);
        }
        
        _isCanClose = false;

        int stage = CalculateStage(CrazeOrderModel.Instance.AnimStage);

        _rewardItems.ForEach(a=>a.SetFinish(false));
        for (int i = 0; i < stage; i++)
        {
            _rewardItems[i].SetFinish(true);
        }

        var stageConfigs =  CrazeOrderConfigManager.Instance.GetStageConfigs();
        if(CrazeOrderModel.Instance.AnimStage == stageConfigs[stageConfigs.Count-1].OrderNum)
            _rewardItems[_rewardItems.Count-1].SetFinish(true);
        
        _slider.value = CalculateStageSlider(CrazeOrderModel.Instance.AnimStage);
        _slider2.PlayAnimation(stage.ToString());
        lastStage = stage;
        StartCoroutine(PlayAnim(stage));
    }

    private IEnumerator PlayAnim(int currentStage)
    {
        if (CrazeOrderModel.Instance.AnimStage == CrazeOrderModel.Instance.CompleteNum)
        {
            _isCanClose = true;
            yield break;
        }

        _isCanClose = false;
        yield return new WaitForSeconds(0.5f);
        
        float moveTime = (CrazeOrderModel.Instance.CompleteNum - CrazeOrderModel.Instance.AnimStage) * 0.2f;

        int stage = currentStage;

        CrazeOrderModel.Instance.AnimStage = CrazeOrderModel.Instance.CompleteNum;
        float endValue = CalculateStageSlider(CrazeOrderModel.Instance.AnimStage);
        var newStage = CalculateStage(CrazeOrderModel.Instance.AnimStage);
        var stageConfigs = CrazeOrderConfigManager.Instance.GetStageConfigs();
        if (CrazeOrderModel.Instance.AnimStage == stageConfigs[stageConfigs.Count - 1].OrderNum)
        {
            _slider2.PlayAnimation((newStage)+"-"+(newStage+1));
        }
        else if (stage != newStage)
        {
            _slider2.PlayAnimation((newStage-1)+"-"+newStage);
        }
        _slider.DOValue(endValue, moveTime).SetEase(Ease.Linear).OnUpdate(() =>
        {
            int animStage = 0;
            int totalValue = 0;
            for (var i = 0; i < _stageSliderValue.Length; i++)
            {
                totalValue += _stageSliderValue[i];
                if (totalValue >= _slider.value)
                {
                    animStage = i;
                    break;
                }
            }
            
            if(animStage == stage)
                return;
            
            stage = animStage;
            _rewardItems[animStage-1].SetFinish(true);
        }).OnComplete(() =>
        {
            var stageConfigs = CrazeOrderConfigManager.Instance.GetStageConfigs();
            if(CrazeOrderModel.Instance.AnimStage == stageConfigs[stageConfigs.Count-1].OrderNum)
                _rewardItems[_rewardItems.Count-1].SetFinish(true);

            int stage = CalculateStage(CrazeOrderModel.Instance.AnimStage);
            if(stage >= 1)
                _rewardItems[stage-1].SetFinish(true);
            
            AnimEndLogic();
            _isCanClose = true;
        });
    }
    
    private void CloseUI()
    {
        if(!_isCanClose)
            return;
            
        AnimCloseWindow(() =>
        {
        });
    }
    private void InvokeUpdate()
    {
        _coolTimeText.SetText(CrazeOrderModel.Instance.GetJoinEndTimeString());
    }
    
    private void AnimEndLogic()
    {
        _isCanClose = true;
        
        var stageConfigs = CrazeOrderConfigManager.Instance.GetStageConfigs();
        if(CrazeOrderModel.Instance.Stage <= stageConfigs[stageConfigs.Count-1].Id)
            return;
        
        List<ResData> resDatas = new List<ResData>();
        for(int i = 0;i < CrazeOrderConfigManager.Instance.CrazeOrderSettingList[0].RewardIds.Count; i++)
        {
            ResData resData = new ResData(CrazeOrderConfigManager.Instance.CrazeOrderSettingList[0].RewardIds[i], CrazeOrderConfigManager.Instance.CrazeOrderSettingList[0].RewardNums[i]);
            resDatas.Add(resData);
        }
        CommonRewardManager.Instance.PopCommonReward(resDatas, CurrencyGroupManager.Instance.currencyController,false);
    }

    private int CalculateStage(int value)
    {
        var stageConfigs = CrazeOrderConfigManager.Instance.GetStageConfigs();
        if (stageConfigs == null)
            return 0;

        for (var i = 0; i < stageConfigs.Count; i++)
        {
            if (value < stageConfigs[i].OrderNum)
                return i;
        }

        return stageConfigs.Count-1;
    }

    private int GetStageNum(int stage, int dir = 0)
    {
        stage += dir;
        if (stage < 0)
            return 0;

        var stageConfigs = CrazeOrderConfigManager.Instance.GetStageConfigs();
        if (stage >= stageConfigs.Count)
            return stageConfigs[stageConfigs.Count - 1].OrderNum;

        return stageConfigs[stage].OrderNum;
    }

    private float CalculateStageSlider(int num)
    {
        int stage = CalculateStage(num);
        float sliderValue = 0;
        if (stage > 0)
        {
            for (int i = 0; i <= stage - 1; i++)
            {
                sliderValue += _stageSliderValue[i];
            }
        }

        int stageNum = GetStageNum(stage, -1);
        sliderValue += _stageSliderValue[stage] * (1.0f * (CrazeOrderModel.Instance.AnimStage-stageNum) / (GetStageNum(stage, 0)-stageNum));

        return sliderValue;
    }
}