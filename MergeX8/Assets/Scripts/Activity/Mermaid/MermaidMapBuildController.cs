using System;
using Deco.Node;
using Deco.World;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class MermaidMapBuildController : UIWindowController
{
    private LocalizeTextMeshProUGUI _coinText;
    private Button _closeButton;
    private Image _costIcon;
    private LocalizeTextMeshProUGUI _costText;
    private LocalizeTextMeshProUGUI _nameText;
    private Button _buttonGreen;
    private Button _buttonGrey;

    private DecoNode _decoNode;

    private GameObject _objNumGroup;
    private GameObject _objPurchased;
    public override void PrivateAwake()
    {
        _objNumGroup = transform.Find("Root/BuildGroup/NumGroup").gameObject;
        _objPurchased = transform.Find("Root/BuildGroup/Purchased").gameObject;
        
        _nameText = transform.Find("Root/BuildGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _costText = transform.Find("Root/BuildGroup/NumGroup/Num").GetComponent<LocalizeTextMeshProUGUI>();
        _coinText= transform.Find("Root/CurrencyGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _closeButton= transform.Find("Root/BuildGroup/ButtonClose").GetComponent<Button>();
        _costIcon = transform.Find("Root/BuildGroup/Icon").GetComponent<Image>();
        
        _closeButton.onClick.AddListener(() =>
        {
            EndPreview();
            
            AnimCloseWindow(() =>
            {
                AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, true);
                EventDispatcher.Instance.DispatchEventImmediately(EventEnum.HIDE_NODE_BUY);
            });
        });
        
        _buttonGreen= transform.Find("Root/BuildGroup/ButtonGreen").GetComponent<Button>();
        _buttonGreen.onClick.AddListener(() =>
        {
            BuyNode();
        });
        
        _buttonGrey= transform.Find("Root/BuildGroup/ButtonGrey").GetComponent<Button>();
        _buttonGrey.onClick.AddListener(() =>
        {
            UIPopupNoMoneyController.ShowUI(_decoNode._data._config.costId);
        });
    }
    
    protected override void OnOpenWindow(params object[] objs)
    {
        _decoNode = (DecoNode)objs[0];

        int resValue = UserData.Instance.GetRes(UserData.ResourceId.Mermaid);
        _coinText.SetText(resValue.ToString());
        _costText.SetText((_decoNode.Config.price*MermaidModel.Instance.GetExtendMultiple()).ToString());

        _objNumGroup.gameObject.SetActive(false);
        _objPurchased.gameObject.SetActive(false);
        if (_decoNode.IsOwned)
        {
            _buttonGreen.gameObject.SetActive(true);
            _buttonGrey.gameObject.SetActive(false);
            _objPurchased.gameObject.SetActive(true);
        }
        else
        {
            _buttonGreen.gameObject.SetActive(resValue >= _decoNode.Config.price*MermaidModel.Instance.GetExtendMultiple());
            _buttonGrey.gameObject.SetActive(resValue < _decoNode.Config.price*MermaidModel.Instance.GetExtendMultiple());
            _objNumGroup.gameObject.SetActive(true);
        }

        _decoNode.PreviewItem(_decoNode.Config.itemList[0]);
        
        ExchangeReward reward = MermaidModel.Instance.GetExchangeRewards().Find(a => a.RewardId.ToString() == _decoNode.Id + "1");

        var item = DecoManager.Instance.FindItem(reward.RewardId);
        if (item != null)
        {
            _costIcon.sprite =  ResourcesManager.Instance.GetSpriteVariant("MermaidAtlas", item.Config.buildingIcon);
            _nameText.SetTerm(item.Config.name);
        }  
    }

    private void BuyNode()
    {
        if(!canClickMask)
            return;
        
        if (_decoNode.IsOwned)
        {
            AnimCloseWindow(() =>
            {
                AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, true);
                EventDispatcher.Instance.DispatchEventImmediately(EventEnum.HIDE_NODE_BUY);
            });
            
            return;
        }
        
        if (!_decoNode.IsOwned && UserData.Instance.CanAford((UserData.ResourceId)_decoNode._data._config.costId, _decoNode._data._config.price*MermaidModel.Instance.GetExtendMultiple()))
        {
            canClickMask = false;
            UserData.Instance.ConsumeRes((UserData.ResourceId)_decoNode._data._config.costId, _decoNode._data._config.price*MermaidModel.Instance.GetExtendMultiple(),  new GameBIManager.ItemChangeReasonArgs()
                {reason = BiEventCooking.Types.ItemChangeReason.MermaidDeco});
            
            _decoNode.Buy();
            
            ExchangeReward reward = MermaidModel.Instance.GetExchangeRewards().Find(a => a.RewardId.ToString() == _decoNode.Id + "1");
            MermaidModel.Instance.RecordExchange(reward.RewardId);
            AudioManager.Instance.PlaySound(38);
            FlyEffectLogic();
        }
        else
        {
            UIPopupNoMoneyController.ShowUI(_decoNode._data._config.costId);
        }
    }

    public void EndPreview()
    {
        _decoNode.EndPreview(false);
    }
    private void FlyEffectLogic()
    {
        float flyTime = 0.8f;

        Action flyEndCall = () =>
        {
            if (!StorySubSystem.Instance.Trigger(StoryTrigger.BuyNodeSuccess, _decoNode.Id.ToString(),
                    isFinish =>
                    {
                        if (isFinish)
                        {
                            var tempNode = _decoNode;
                            _decoNode.EndPreview(false);
                            AnimCloseWindow(() =>
                            {
                                UIRoot.Instance.EnableEventSystem = false;
                                DecoManager.Instance.CurrentWorld.FocusNode(tempNode, () =>
                                {
                                    tempNode.Stage.Area.World.Select(tempNode, fromFirstBuy: true);
                                });
                            });
                        }
                    },
                    isGuide => { }))
            {
                AnimCloseWindow(() =>
                {
                    var tempNode = _decoNode;
                    _decoNode.EndPreview(false);
                    UIRoot.Instance.EnableEventSystem = false;
                    DecoManager.Instance.CurrentWorld.FocusNode(tempNode,
                        () =>
                        {
                            tempNode.Stage.Area.World.Select(tempNode, fromFirstBuy: true);
                        });
                });
            }
            else
            {
                UIRoot.Instance.EnableEventSystem = true;
            }
        };

        UserData.ResourceId resId = (UserData.ResourceId)_decoNode.Config.costId;
        
        Vector3 srcPos = _coinText.transform.position;
        Vector3 targetPos = _buttonGreen.transform.position;

        UIRoot.Instance.EnableEventSystem = false;
        FlyEffectTail(srcPos, targetPos, flyTime,resId, () =>
        {
            flyEndCall();
        });

        int newValue = UserData.Instance.GetRes(UserData.ResourceId.Mermaid);
        int oldValue = newValue + _decoNode.Config.price*MermaidModel.Instance.GetExtendMultiple();
        DOTween.To(() => oldValue, x => oldValue = x, newValue, flyTime).OnUpdate(() =>
        {
            _coinText.SetText(oldValue.ToString());
        });
    }
    
    private void FlyEffectTail(Vector3 srcPos,Vector3 targetPos,float flyTime,UserData.ResourceId resId,  Action callback)
    {
        string effName = "vfx_trail_003";
        var prefabs = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Effects/"+effName);
        if (prefabs == null)
            return;
        
        var flyClone = GameObject.Instantiate(prefabs, new Vector3(0,1000,0), Quaternion.identity, transform);
        var padEf = flyClone.transform.Find("FlyTrail/Trail");
        var image = flyClone.transform.Find("FlyTrail/Image");
        image.gameObject.SetActive(true);
        
        if(CommonUtils.IsLE_16_10())
            padEf?.gameObject.SetActive(false);
        
        Vector2 control = TransformFly.getControlType(srcPos, targetPos, TransformFly.ControlType.Left);
        var flyUnit = new FlyUnit();
        flyUnit.Start(srcPos, targetPos, control, flyClone.transform, flyTime, 0, () =>
        {
            FlyGameObjectManager.Instance.PlayHintStarsEffect(targetPos);
            ShakeManager.Instance.ShakeLight();
            
            image.gameObject.SetActive(false);
            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.8f, () =>
            {
                callback?.Invoke();
                GameObject.Destroy(flyClone);
            }));
        });
    }
}