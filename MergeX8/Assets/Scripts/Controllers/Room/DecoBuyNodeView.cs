using System;
using System.Collections.Generic;
using Deco.Node;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;


public partial class DecoBuyNodeView : MonoBehaviour
{
    private class BuyNodeItem
    {
        public Button _helpButon;
        public Image _icon;
        public LocalizeTextMeshProUGUI _num;
        public LocalizeTextMeshProUGUI _name;

        public int _mergeId;
        public GameObject _itemObj;

        public BuyNodeItem(GameObject itemObj)
        {
            _itemObj = itemObj;

            _helpButon = itemObj.transform.Find("HelpButton").GetComponent<Button>();
            _helpButon.onClick.AddListener(() =>
            {
                MergeInfoView.Instance.OpenMergeInfo(_mergeId);
                UIPopupMergeInformationController controller =
                    UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
                if (controller != null)
                    controller.SetResourcesActive(false);
            });
            _icon = itemObj.transform.Find("Icon").GetComponent<Image>();
            _num = itemObj.transform.Find("Num").GetComponent<LocalizeTextMeshProUGUI>();
            _name = itemObj.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        }

        public void UpdateNode(int mergeId, int num)
        {
            if(_mergeId == mergeId)
                return;
            
            _mergeId = mergeId;

            bool isRes = UserData.Instance.IsResource(mergeId);
            _helpButon.gameObject.SetActive(!isRes);
            _icon.sprite = UserData.GetResourceIcon(mergeId, UserData.ResourceSubType.Big);
            //_name.gameObject.SetActive(false);
            _num.SetText(num.ToString());
            _icon.transform.localScale = Vector3.one;
            
            if(!isRes)
                _icon.transform.localScale = new Vector3(1.3f,1.3f,1.3f);
        }
    }
    
    
    private LocalizeTextMeshProUGUI _title;

    private Button _buyButton;

    private List<BuyNodeItem> _nodeItems = new List<BuyNodeItem>();

    private GameObject _itemGroup;
    private GameObject _cloneItem;

    private DecoNode _decoNode;
    public DecoNode DecoNode => _decoNode;

    private Image _costIcon;
    private LocalizeTextMeshProUGUI _costNum;
    public Image _buttonBg;
    private void Awake()
    {
        _title = transform.Find("Title/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        _buyButton = transform.Find("ButtonGroup/StartButton").GetComponent<Button>();
        _buyButton.onClick.AddListener(OnBuyNode);
        _itemGroup = transform.Find("ItemGroup").gameObject;
        _buttonBg = transform.Find("ButtonGroup/StartButton/UIBg").GetComponent<Image>();
        
        _cloneItem = transform.Find("ItemGroup/Item").gameObject;
        _cloneItem.gameObject.SetActive(false);

        _costIcon = transform.Find("ButtonGroup/StartButton/Icon").GetComponent<Image>();
        _costNum = transform.Find("ButtonGroup/StartButton/Num").GetComponent<LocalizeTextMeshProUGUI>();
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(_buyButton.transform.parent);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.BuyNode, _buyButton.transform as RectTransform, topLayer:topLayer);

        Awake_Days();
    }

    public void SetNodeData(DecoNode nodeData, int areaId)
    {
        _decoNode = nodeData;
        
        _title.SetTerm(_decoNode._data._config.title);
        _costIcon.sprite = UserData.GetResourceIcon(_decoNode._data._config.costId, UserData.ResourceSubType.Big);
        _costNum.SetText(_decoNode._data._config.price.ToString());
        
        UpdateNodeItems();
        UpdateDays();
        
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.BuyNode, nodeData.Config.id.ToString());
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.BuyNode, nodeData.Config.costId.ToString());       
        
        // bool canAford = UserData.Instance.CanAford((UserData.ResourceId)nodeData._data._config.costId, nodeData._data._config.price);
        _buttonBg.sprite  = ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, !nodeData.CanAfford() ? "Common_Button_Gray" : "Common_Button_Green");

    }

    private void UpdateNodeItems()
    {
        _nodeItems.ForEach(a=>a._itemObj.SetActive(false));
        if (_decoNode._data._config.rewardId != null && _decoNode._data._config.rewardId.Length > _nodeItems.Count)
        {
            int count = _decoNode._data._config.rewardId.Length - _nodeItems.Count;
            for (int i = 0; i < count; i++)
            {
                var obj = GameObject.Instantiate(_cloneItem, _itemGroup.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;

                var nodeItem = new BuyNodeItem(obj);
                _nodeItems.Add(nodeItem);
            }
        }

        for (int i = 0; i < _decoNode._data._config.rewardId.Length; i++)
        {
            _nodeItems[i]._itemObj.transform.localScale = Vector3.one;
            if( _decoNode._data._config.rewardId.Length >= 3)
                _nodeItems[i]._itemObj.transform.localScale = new Vector3(0.85f,0.85f,0.85f);
            
            _nodeItems[i]._itemObj.SetActive(true);
            _nodeItems[i].UpdateNode(_decoNode._data._config.rewardId[i], _decoNode._data._config.rewardNumber[i]);
        }
    }

    public void OnBuyNode()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.BuyNode, _decoNode._data._config.id.ToString());
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.BuyNode, _decoNode._data._config.costId.ToString());
        
        if (!_decoNode.IsOwned && _decoNode.CanAfford())
        {
            UserData.Instance.ConsumeRes((UserData.ResourceId)_decoNode._data._config.costId, _decoNode._data._config.price,  new GameBIManager.ItemChangeReasonArgs()
                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Decorate});
            
            _decoNode.Buy();

            AudioManager.Instance.PlaySound(38);
            FlyEffectLogic();
        }
        else
        {
            UIPopupNoMoneyController.ShowUI(_decoNode._data._config.costId);
        }
    }
    
    private void FlyEffectLogic()
    {
        float flyTime = 0.8f;

        Action flyEndCall = () =>
        {
            AnimControlManager.Instance.AnimShow(AnimKey.Der_BuyNode, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, false);
            if (!StorySubSystem.Instance.Trigger(StoryTrigger.BuyNodeSuccess, _decoNode.Id.ToString(),
                    isFinish =>
                    {
                        if (isFinish)
                        {
                            UIRoot.Instance.EnableEventSystem = false;
                            DecoManager.Instance.CurrentWorld.FocusNode(_decoNode, () =>
                            {
                                _decoNode.Stage.Area.World.Select(_decoNode, fromFirstBuy: true);
                            });
                            
                        }
                    },
                    isGuide => { }))
            {
                UIRoot.Instance.EnableEventSystem = false;
                DecoManager.Instance.CurrentWorld.FocusNode(_decoNode,
                    () =>
                    {
                        _decoNode.Stage.Area.World.Select(_decoNode, fromFirstBuy: true);
                    });
            }
            else
            {
                UIRoot.Instance.EnableEventSystem = true;
            }
        };

        UserData.ResourceId resId = (UserData.ResourceId)_decoNode.Config.costId;
        if (resId == UserData.ResourceId.Seal || resId == UserData.ResourceId.Dolphin || resId == UserData.ResourceId.Capybara)
        {
            UIRoot.Instance.EnableEventSystem = false;
            flyEndCall();
        }
        else
        {
            Vector3 srcPos = FlyGameObjectManager.Instance.GetResourcePosition(resId);
            // CurrencyGroupManager.Instance.currencyController.GetIconTransform(resId).position;
            Vector3 targetPos = _costIcon.transform.position;

            UIRoot.Instance.EnableEventSystem = false;
            FlyEffectTail(srcPos, targetPos, flyTime,resId, () =>
            {
                flyEndCall();
            });
        }
    }
    
    
    public static void FlyEffectTail(Vector3 srcPos,Vector3 targetPos,float flyTime,UserData.ResourceId resId,  Action callback)
    {
        string effName = resId == UserData.ResourceId.Coin ? "vfx_trail_001" : "vfx_trail_002";
        var prefabs = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Effects/"+effName);
        if (prefabs == null)
            return;
        
        var flyClone = GameObject.Instantiate(prefabs, new Vector3(0,1000,0), Quaternion.identity, (Transform)MainDecorationController.mainController.transform);
        var padEf = flyClone.transform.Find("FlyTrail/Trail");
        if(CommonUtils.IsLE_16_10())
            padEf?.gameObject.SetActive(false);
        
        Vector2 control = TransformFly.getControlType(srcPos, targetPos, TransformFly.ControlType.Left);
        var flyUnit = new FlyUnit();
        flyUnit.Start(srcPos, targetPos, control, flyClone.transform, flyTime, 0, () =>
        {
            FlyGameObjectManager.Instance.PlayHintStarsEffect(targetPos);
            ShakeManager.Instance.ShakeLight();
            
            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.8f, () =>
            {
                callback?.Invoke();
                GameObject.Destroy(flyClone);
            }));
        });
    }
}