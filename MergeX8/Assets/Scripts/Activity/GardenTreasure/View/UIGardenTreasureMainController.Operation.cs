using Activity.GardenTreasure.Model;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public partial class UIGardenTreasureMainController
{
    private LocalizeTextMeshProUGUI _timeText;
    
    private class ButtonGroup
    {
        private GameObject _gameObject;
        private GameObject _normalGroup;
        private GameObject _selectGroup;
        private LocalizeTextMeshProUGUI _normalText;
        private LocalizeTextMeshProUGUI _selectText;
        private GameObject _normalAdd;
        private GameObject _selectAdd;
        public GameObject gameObject
        {
            get { return _gameObject; }
        }
        
        public ButtonGroup(GameObject gameObject)
        {
            _gameObject = gameObject;

            _normalGroup = gameObject.transform.Find("Normal").gameObject;
            _selectGroup = gameObject.transform.Find("Selected").gameObject;

            _normalText = _normalGroup.transform.Find("Num/NumText").GetComponent<LocalizeTextMeshProUGUI>();
            _selectText = _selectGroup.transform.Find("Num/NumText").GetComponent<LocalizeTextMeshProUGUI>();

            _normalAdd = _normalGroup.transform.Find("Add").gameObject;
            _selectAdd = _selectGroup.transform.Find("Add").gameObject;
            _normalAdd.gameObject.SetActive(false);
            _selectAdd.gameObject.SetActive(false);
        }
        
        public void Update(int num)
        {
            _normalText.SetText(num.ToString());
            _selectText.SetText(num.ToString());
        }

        public void Select(bool isSelect)
        {
            _normalGroup.gameObject.SetActive(!isSelect);
            _selectGroup.gameObject.SetActive(isSelect);
        }
    }

    private ButtonGroup _shovelGroup;
    private ButtonGroup _bombGroup;
    public bool _isSelcetShovel = false;
    
    public Transform ShovelTransform
    {
        get => _shovelGroup.gameObject.transform;
    }
    
    public Transform BombTransform
    {
        get => _bombGroup.gameObject.transform;
    }

    private LocalizeTextMeshProUGUI RankText;

    public void UpdateRankText()
    {
        // RankText.gameObject.SetActive(RankStorage.IsInitFromServer());
        // RankText.SetText("No."+RankStorage.SortController().MyRank);
    }
    private void AwakeOperation()
    {
        transform.Find("Root/ButtonClose").GetComponent<Button>().onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        // transform.Find("Root/ButtonRank/Tip").gameObject.SetActive(false);
        // transform.Find("Root/ButtonRank").GetComponent<Button>().onClick.AddListener(() =>
        // {
        //     OpenRank();
        // });
        // RankText = transform.Find("Root/ButtonRank/Root/RankText").GetComponent<LocalizeTextMeshProUGUI>();
        // RankText.gameObject.SetActive(RankStorage.IsInitFromServer());
        // RankText.SetText("No."+RankStorage.SortController().MyRank);
        //InvokeRepeating("UpdateRankText",0,1);
        
        transform.Find("Root/ButtonShop").GetComponent<Button>().onClick.AddListener(() =>
        {
            OpenShop();
        });
        
        transform.Find("Root/ButtonHelp").GetComponent<Button>().onClick.AddListener(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIGardenTreasureHelp);
        });

        _timeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        
        InvokeRepeating("InvokeUpdate", 0, 1);

        _bombGroup = new ButtonGroup(transform.Find("Root/ButtonGroup/Small").gameObject);
        _bombGroup.gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            SelectButtonGroup(false);
        });
        
        _shovelGroup = new ButtonGroup(transform.Find("Root/ButtonGroup/Big").gameObject);
        _shovelGroup.gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            SelectButtonGroup(true);
        });

        SelectButtonGroup(true);
        UpdateValues();
    }

    private void InvokeUpdate()
    {
        _timeText.SetText(GardenTreasureModel.Instance.GetEndTimeString());
    }

    private void SelectButtonGroup(bool isShovel)
    {
        _isSelcetShovel = isShovel;
        _shovelGroup.Select(isShovel);
        _bombGroup.Select(!isShovel);

        if (isShovel)
        {
            CleanBoardTags();
        }
    }

    private void UpdateValues()
    {
        _bombGroup.Update(UserData.Instance.GetRes(UserData.ResourceId.GardenBomb));
        _shovelGroup.Update(UserData.Instance.GetRes(UserData.ResourceId.GardenShovel));
    }

    //private StorageCommonLeaderBoard RankStorage=>GardenTreasureLeaderBoardModel.Instance.GetLeaderBoardStorage(GardenTreasureModel.Instance.ActivityId);
    private void OpenRank()
    {
        //GardenTreasureLeaderBoardModel.Instance.OpenMainPopup(RankStorage); 
        // if (storage.IsInitFromServer())
        // {
        //     GardenTreasureLeaderBoardModel.Instance.OpenMainPopup(storage);   
        // }
        // else
        // {
        //     var tip = transform.Find("Root/ButtonRank/Tip");
        //     var tipText = tip.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        //     tipText.SetTermFormats(storage.LeastStarCount.ToString());
        //     tip.DOKill(false);
        //     tip.gameObject.SetActive(false);
        //     tip.gameObject.SetActive(true);
        //     DOVirtual.DelayedCall(2f, () => tip?.gameObject.SetActive(false)).SetTarget(tip);
        // }
    }

    private void OpenShop()
    {
        UIManager.Instance.OpenUI(UINameConst.UIPopupGardenTreasureGift, "1");
    }
}