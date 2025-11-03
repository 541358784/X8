using System;
using System.Collections.Generic;
using Deco.Node;
using Decoration;
using DragonPlus;
using DragonU3DSDK.Asset;
using Gameplay;
using Manager;
using UnityEngine;
using UnityEngine.UI;


public class TaskDecorItemCell : MonoBehaviour
{
    private class DecorItem
    {
        public Button _helpButon;
        public Button _preViewButon;
        public Image _icon;
        public LocalizeTextMeshProUGUI _num;
        //public LocalizeTextMeshProUGUI _name;

        public int _resId;
        public GameObject _itemObj;

        public DecorItem(GameObject itemObj)
        {
            _itemObj = itemObj;

            _helpButon = itemObj.transform.Find("HelpButton").GetComponent<Button>();
            _helpButon.onClick.AddListener(() =>
            {
                MergeInfoView.Instance.OpenMergeInfo(_resId);
                UIPopupMergeInformationController controller =
                    UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
                if (controller != null)
                    controller.SetResourcesActive(false);
            });
            _icon = itemObj.transform.Find("Icon").GetComponent<Image>();
            _num = itemObj.transform.Find("Num").GetComponent<LocalizeTextMeshProUGUI>();
            //_name = itemObj.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            
            _preViewButon = itemObj.transform.Find("AnimButton").GetComponent<Button>();
            _preViewButon.onClick.AddListener(() =>
            {
            });
        }

        public void UpdateItem(DecoNode node)
        {
            _helpButon.gameObject.SetActive(false);
            _preViewButon.gameObject.SetActive(false);
            
            _icon.sprite =  CommonUtils.LoadDecoItemIconSprite(node._stage.Area.Id, node._data._config.icon);
            _num.SetTerm(node._data._config.title);
        }
    }
    
    
    public DecoNode _node;
    private GameObject _content;

    private GameObject _item = null;
    
    private Image _costIcon;
    private LocalizeTextMeshProUGUI _costNum;
    private Button _showButton;
    private List<DecorItem> _decorItems = new List<DecorItem>();
    public Image _buttonBg;
    private void Awake()
    {
        _content = gameObject;
        
        _item = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Home/UITaskIconCell");
        
        _costIcon = transform.Find("Button/Icon").GetComponent<Image>();
        _costNum = transform.Find("Button/Num").GetComponent<LocalizeTextMeshProUGUI>();
        
        _showButton = transform.Find("Button").GetComponent<Button>();
        _showButton.onClick.AddListener(ShowNode);

        _buttonBg = transform.Find("Button/UIBg").GetComponent<Image>();
    }

    public void InitData(DecoNode node)
    {
        _node = node;

        InitView();
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(_showButton.transform);

        string targetParam = "";
        bool isReplace = true;
        if (_node._data._config.costId == (int)UserData.ResourceId.RareDecoCoin ||
            _node._data._config.costId == (int)UserData.ResourceId.Seal ||  _node._data._config.costId == (int)UserData.ResourceId.Dolphin
            ||_node._data._config.costId == (int)UserData.ResourceId.Capybara)
        {
            targetParam = _node._data._config.costId.ToString();
            isReplace = false;
        }
            
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.GoDeco, _showButton.transform as RectTransform, moveToTarget:null, targetParam, isReplace, topLayer:topLayer);
    }

    private void InitView()
    {
        if(_node == null)
            return;
        
        _costIcon.sprite = UserData.GetResourceIcon(_node._data._config.costId, UserData.ResourceSubType.Big);
        _costNum.SetText(_node._data._config.price.ToString());
        
        if (_node._data._config.rewardId == null || _node._data._config.rewardId.Length == 0)
            return;
        
        var obj = GameObject.Instantiate(_item, _content.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        _decorItems.Add(new DecorItem(obj));
        _decorItems[_decorItems.Count-1].UpdateItem(_node);

        // bool canAford = UserData.Instance.CanAford((UserData.ResourceId)_node._data._config.costId, _node._data._config.price);
        _buttonBg.sprite  = ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, !CanAfford ? "Common_Button_Gray" : "Common_Button_Green");
    }
    public bool CanAfford => UserData.Instance.CanAford((UserData.ResourceId)_node._data._config.costId, _node._data._config.price);
    public void ShowNode()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.GoDeco, "");
        
        if (GameModeManager.Instance.GetGameMode() == GameModeManager.GameMode.MiniAndMerge)
        {
            if (GameModeManager.Instance.GetCurrenGameMode() == GameModeManager.CurrentGameMode.MiniGame)
            {
                AudioManager.Instance.PlayMusic(1, true);
                GameModeManager.Instance.SetCurrentGameMode(GameModeManager.CurrentGameMode.Deco);
                UIPopupTaskController.CloseView(() =>
                {
                    if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                    {
                        GameModeManager.Instance.RefreshGameStatus(false);
                        SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome, DecoOperationType.Buy, _node);
                    }
                    else
                    {
                        GameModeManager.Instance.RefreshGameStatus(true, () =>
                        {
                            DecoManager.Instance.SelectNode(_node);
                        });
                    }
                });
                
                return;
            }
        }
        
        if (!CanAfford)
        {
            UIPopupNoMoneyController.ShowUI(_node._data._config.costId);
            return;
        }
        if (AssetCheckManager.Instance.GetAreaResNeedToDownload(_node._stage.Area.Id).Count > 0)
        {
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                DescString = LocalizationManager.Instance.GetLocalizedString("UI_download_resources_tip"),
                HasCancelButton = false,
                HasCloseButton = false,
            });
            return;
        }
            
        _showButton.interactable = false;
        
        UIPopupTaskController.CloseView(() =>
        {
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
            {
                SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome, DecoOperationType.Buy, _node.Id);
            }
            else
            {
                DecoManager.Instance.SelectNode(_node);
            }
        });
    }
}