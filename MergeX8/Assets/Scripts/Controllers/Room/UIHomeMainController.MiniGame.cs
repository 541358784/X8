using System.Linq;
using Decoration;
using DragonPlus;
using DragonU3DSDK.Asset;
using Manager;
using MiniGame;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public partial class UIHomeMainController
{
    private Button _miniGameBtn;
    private Transform _miniRedPoint;
    private Image _miniSlider;
    private LocalizeTextMeshProUGUI _miniProgress;
    private Transform _spineRoot;

    private int _chapterId = -1;
    private GameObject _spineEntry;

    private bool _isAllFinish = false;
    
    private void Awake_MiniGame()
    {
        _miniGameBtn = transform.Find("Root/BottomGroup/Middle/MiniGame").GetComponent<Button>();
        _miniGameBtn.onClick.AddListener(OnClickMiniGame);

        _miniRedPoint = transform.Find("Root/BottomGroup/Middle/MiniGame/RedPoint");

        _miniSlider = transform.Find("Root/BottomGroup/Middle/MiniGame/Slider/Fill").GetComponent<Image>();

        _miniProgress = transform.Find("Root/BottomGroup/Middle/MiniGame/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();

        _spineRoot = transform.Find("Root/BottomGroup/Middle/MiniGame/SpineRoot");
        
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MiniGame_Button, _miniGameBtn.transform as RectTransform, targetParam:("Click_MiniGame"), topLayer:_miniGameBtn.transform);
            
        RefreshMiniGame();
        
        EventDispatcher.Instance.AddEventListener(EventMiniGame.MINIGAME_SETSHOWSTATUS, RefreshMiniGameStatus);

        _isAllFinish = MiniGameModel.Instance.IsMiniGameAllFinish();
    }

    private void OnClickMiniGame()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MiniGame_Button, "Click_MiniGame");
        
        EventDispatcher.Instance.DispatchEvent(EventMiniGame.MINIGAME_SETSHOWSTATUS, true);
    }

    private void RefreshMiniGame()
    {
        _isAllFinish = MiniGameModel.Instance.IsMiniGameAllFinish();
        RefreshMiniGameIcon();
        
        var chapterId = MiniGameModel.Instance.CurrentChapter;
        
        var storage = MiniGameModel.Instance.GetChapterStorage(chapterId);
        if(storage == null)
            return;
        
        var levelIdList = MiniGameModel.Instance.GetChapterLevels(chapterId);
        if(levelIdList == null)
            return;
        
        var totalCount = levelIdList.Count;
        var finishedCount = storage.LevelsDic.Values.Count(a=>a.Claimed);
        _miniSlider.fillAmount = finishedCount / (float)totalCount;
        _miniProgress.SetText(finishedCount +"/" + totalCount);
    }

    private void RefreshMiniGameStatus(BaseEvent e)
    {
        bool isMiniShow = (bool)e.datas[0];
        bool isImmediately = false;
        if(e.datas.Count() >= 2)
            isImmediately= (bool)e.datas[1];
        
        AnimControlManager.Instance.AnimShow(AnimKey.Main_Group, !isMiniShow, isImmediately);
        AnimControlManager.Instance.AnimShow(AnimKey.Main_Bottom, !isMiniShow, isImmediately);
        
        RefreshMiniGame();
    }

    private void RefreshMiniGameIcon()
    {
        var isFinishAllLevel = MiniGameModel.Instance.IsMiniGameAllFinish();
        if (isFinishAllLevel)
        {
            if (_spineEntry != null)
            {
                GameObject.DestroyImmediate(_spineEntry);
                _spineEntry = null;
            }
        }
        else
        {
            LoadMiniGameIcon(_spineRoot);
        }
    }

    private void UpdateMiniGame()
    {
        if (GameModeManager.Instance.GetGameMode() == GameModeManager.GameMode.MiniAndMerge)
        {
            if(_isAllFinish)
                _miniGameBtn.gameObject.SetActive(false);
            else
                _miniGameBtn.gameObject.SetActive(GameModeManager.Instance.GetCurrenGameMode() == GameModeManager.CurrentGameMode.MiniGame);
            
            _miniRedPoint.gameObject.SetActive(DecoManager.Instance.CanBuyOrGet_Mini());
        }
        else
        {
            _miniGameBtn.gameObject.SetActive(false);
        }
    }
    
    private GameObject LoadMiniGameIcon(Transform parent)
    {
        var chapterId = MiniGameModel.Instance.CurrentChapter;
        if (_chapterId == chapterId)
            return null;
        
        var config = MiniGameModel.Instance.GetChapterConfig(chapterId);
        if (config == null) 
            return null;

        _chapterId = chapterId;
        if (_spineEntry != null)
        {
            GameObject.DestroyImmediate(_spineEntry);
            _spineEntry = null;
        }
        
        var iconName = config.HomeBtn;
        var prefabPath = $"NewMiniGame/UIEntry/Prefab/{iconName}";

        var originalPrefab = ResourcesManager.Instance.LoadResource<GameObject>(prefabPath);

        var clonePrefab =Instantiate(originalPrefab, parent);
        _spineEntry = clonePrefab;
        
        var spine = clonePrefab.transform.GetComponent<SkeletonGraphic>();
        spine?.MatchRectTransformWithBounds();

        return clonePrefab;
    }
}