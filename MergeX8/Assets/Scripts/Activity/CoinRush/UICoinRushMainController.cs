using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class UICoinRushMainController:UIWindowController
{
    // public enum CoinRushMainShowType
    // {
    //     Open,
    //     // Failed,
    // }
    private Button _buttonClose;
    private Slider _progressSlider;
    private LocalizeTextMeshProUGUI _progressSliderText;
    private Transform _timeGroup;
    private LocalizeTextMeshProUGUI _timeGroupText;
    Transform _coinRushTaskDefaultItem;
    private RectTransform _content;
    private Dictionary<int, CoinRushTaskItem> _coinRushTaskDictionary;

    private Image _lastRewardImage;
    // private ScrollRect _scrollRect;
    // private SkeletonGraphic _skeletonGraphic;
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/CloseButton");
        _buttonClose.onClick.AddListener(OnCloseBtn);
        _progressSlider = GetItem<Slider>("Root/Slider");
        _progressSliderText = GetItem<LocalizeTextMeshProUGUI>("Root/Slider/Text");
        _timeGroup = GetItem<Transform>("Root/TimeGroup");
        _timeGroupText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _coinRushTaskDefaultItem = GetItem<Transform>("Root/Scroll View/Viewport/Content/Task");
        _coinRushTaskDefaultItem.gameObject.SetActive(false);
        _content = GetItem<RectTransform>("Root/Scroll View/Viewport/Content");
        _coinRushTaskDictionary = new Dictionary<int, CoinRushTaskItem>();
        _lastRewardImage = GetItem<Image>("Root/Slider/Icon");
        InvokeRepeating("UpdateTime", 0, 1);
    }
    protected override async void OnOpenWindow(params object[] objs)  
    {
        base.OnOpenWindow(objs);
        RefreshUI();
    }

    public void RefreshUI()
    {
        _lastRewardImage.sprite =
            UserData.GetResourceIcon(CoinRushModel.Instance.FinialRewards[0].id, UserData.ResourceSubType.Big);
        _progressSlider.value = (float) CoinRushModel.Instance.Level / CoinRushModel.Instance.MaxLevel;
        _progressSliderText.SetText(CoinRushModel.Instance.Level+"/"+CoinRushModel.Instance.MaxLevel);
        if (_coinRushTaskDictionary.Keys.Count != CoinRushModel.Instance.MaxLevel)
        {
            foreach (var coinRushTask in _coinRushTaskDictionary)
            {
                GameObject.Destroy(coinRushTask.Value.gameObject);
            }   
            _coinRushTaskDictionary.Clear();
            for (var i = 0; i < CoinRushModel.Instance.CoinRushTaskConfig.Count; i++)
            {
                var config = CoinRushModel.Instance.CoinRushTaskConfig[i];
                var coinRushTaskObj = GameObject.Instantiate(_coinRushTaskDefaultItem.gameObject, _coinRushTaskDefaultItem.parent);
                coinRushTaskObj.SetActive(true);
                var coinRushTask = coinRushTaskObj.AddComponent<CoinRushTaskItem>();
                coinRushTask.Init(config);
                _coinRushTaskDictionary.Add(config.Id,coinRushTask);
            }
        }

        var unCompletedTaskList = new List<CoinRushTaskItem>();
        var completedTaskList = new List<CoinRushTaskItem>();
        
        for (var i = 0; i < CoinRushModel.Instance.CoinRushTaskConfig.Count; i++)
        {
            var config = CoinRushModel.Instance.CoinRushTaskConfig[i];
            var coinRushTask = _coinRushTaskDictionary[config.Id];
            coinRushTask.RefreshView();
            if (CoinRushModel.Instance.AlreadyCollectLevels.Contains(config.Id))
            {
                completedTaskList.Add(coinRushTask);
            }
            else
            {
                unCompletedTaskList.Add(coinRushTask);
            }
        }

        for (var i = 0; i < unCompletedTaskList.Count; i++)
        {
            unCompletedTaskList[i].transform.SetAsLastSibling();
        }
        for (var i = 0; i < completedTaskList.Count; i++)
        {
            completedTaskList[i].transform.SetAsLastSibling();
        }

        _content.anchoredPosition = new Vector2(0, 0);
    }

    private void OnCloseBtn()
    {
        AnimCloseWindow();
    }

    public void UpdateTime()
    {
        if (!CoinRushModel.Instance.IsOpened())
        {
            _timeGroup.gameObject.SetActive(false);
            AnimCloseWindow();
            return;
        }
        _timeGroupText.SetText(CoinRushModel.Instance.GetActivityLeftTimeString());
    }


    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        AnimCloseWindow();
    }
}
