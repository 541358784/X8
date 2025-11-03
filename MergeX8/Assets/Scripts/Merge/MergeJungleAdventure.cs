using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.JungleAdventure;
using DragonPlus.Config.WinStreak;
using UnityEngine;
using UnityEngine.UI;

public class MergeJungleAdventure : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _rankText;
    private LocalizeTextMeshProUGUI _scoreText;
    private Button _button;
    private Image _image;
    private Tweener _tweener;

    private bool _isInit = false;
    private void Awake()
    {
        _timeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _scoreText = transform.Find("Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _rankText = transform.Find("Root/RankText").GetComponent<LocalizeTextMeshProUGUI>();
        
        _button = transform.GetComponent<Button>();
        _button.onClick.AddListener(() =>
        {
            UIManager.Instance.OpenWindow(UINameConst.UIJungleAdventureMain);
        });
        
        _image = transform.Find("Root/Slider").GetComponent<Image>();
        
        InvokeRepeating("UpdateTime", 0, 1);
        
        
        EventDispatcher.Instance.AddEventListener(EventEnum.Event_Refre_JungleAdventure_Score, RefreshScore);
        EventDispatcher.Instance.AddEventListener(EventEnum.Event_Refre_JungleAdventure_UI, RefreshUI);

        _isInit = true;
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.Event_Refre_JungleAdventure_Score, RefreshScore);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.Event_Refre_JungleAdventure_UI, RefreshUI);
    }

    private void OnEnable()
    {
        InitUI();
    }

    private void InitUI()
    {
        if(!_isInit)
            return;
        
        if (!JungleAdventureModel.Instance.IsInitFromServer())
            return;
        
        _timeText.SetText(JungleAdventureModel.Instance.GetEndTimeString());

        if (JungleAdventureModel.Instance.IsFinish())
        {
            _image.fillAmount = 1f;
            _scoreText.SetText(JungleAdventureModel.Instance.JungleAdventure.CurrentScore.ToString());
        }
        else
        {
            int stage = JungleAdventureModel.Instance.JungleAdventure.Stage;
            int currentScore = JungleAdventureModel.Instance.JungleAdventure.CurrentScore;

            var config = JungleAdventureConfigManager.Instance.GetConfigByStage(stage);

            _image.fillAmount = 1.0f*currentScore / config.Score;
            _scoreText.SetText(JungleAdventureModel.Instance.JungleAdventure.CurrentScore.ToString() + "/" + config.Score);
        }
    }

    private void UpdateTime()
    {
        Refresh();
        
        _timeText.SetText(JungleAdventureModel.Instance.GetEndTimeString());

        _rankText.SetText("No.--");
        _rankText.gameObject.SetActive(false);
        var LeaderBoardStorage = JungleAdventureLeaderBoardModel.Instance.GetLeaderBoardStorage(JungleAdventureModel.Instance.ActivityId); 
        if(LeaderBoardStorage == null)
            return;
        
        if (LeaderBoardStorage.IsStorageWeekInitFromServer())
        {
            _rankText.gameObject.SetActive(true);
            _rankText.SetText("No."  + LeaderBoardStorage.SortController().MyRank);
        }
    }

    private void RefreshUI(BaseEvent e)
    {
        InitUI();
    }

    private void Refresh()
    {
        if (!JungleAdventureModel.Instance.IsOpened())
        {
            gameObject.SetActive(false);
            return;
        }
        
        if (!JungleAdventureModel.Instance.IsPreheatEnd())
        {
           gameObject.SetActive(false);
            return;
        }
        
        gameObject.SetActive(true);
    }
    private void RefreshScore(BaseEvent e)
    {
        if (JungleAdventureModel.Instance.IsFinish())
        {
            _image.fillAmount = 1f;
            _scoreText.SetText(JungleAdventureModel.Instance.JungleAdventure.CurrentScore.ToString());
            return;
        }

        PlayScoreAnim((int)e.datas[0], (int)e.datas[1]);
       
    }

    private async UniTask PlayScoreAnim(int oldValue, int newValue)
    {
        if (_tweener != null)
            _tweener.Kill();
        
        int stage = JungleAdventureModel.Instance.JungleAdventure.Stage;
        var config = JungleAdventureConfigManager.Instance.GetConfigByStage(stage);
        
        _tweener = DOTween.To(() => oldValue, x => oldValue = x, newValue, 1f).OnUpdate(() =>
        {
            _scoreText.SetText(oldValue.ToString() + "/" + config.Score);
            _image.fillAmount = 1.0f * oldValue / config.Score;
        });

        await _tweener;

        await UniTask.WaitForSeconds(1.5f);
        
        if (newValue >= config.Score)
        {
            if(UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupLimitOrder) != null)
                return;
            
            UIManager.Instance.OpenWindow(UINameConst.UIJungleAdventureMain);
        }
    }
}