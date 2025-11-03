using System;
using System.Collections.Generic;
using Deco.Node;
using Decoration;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Farm.Model;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.View
{
    public class UIFarmMain_Buy : MonoBehaviour, IInitContent
    {
        private Image _costIcon;
        private LocalizeTextMeshProUGUI _costNum;
        private LocalizeTextMeshProUGUI _title;

        private Button _buyButton;

        private UIFarmMainController _content;
        private DecoNode _decoNode;

        private Image _buildIcon;
        
        private void Awake()
        {
            _title = transform.Find("Title/Text").GetComponent<LocalizeTextMeshProUGUI>();

            _buyButton = transform.Find("ButtonGroup/StartButton").GetComponent<Button>();
            _buyButton.onClick.AddListener(OnBuyNode);

            _costIcon = transform.Find("ButtonGroup/StartButton/Icon").GetComponent<Image>();
            _costNum = transform.Find("ButtonGroup/StartButton/Num").GetComponent<LocalizeTextMeshProUGUI>();
            
            _buildIcon = transform.Find("ItemGroup/Item/Icon").GetComponent<Image>();
            transform.Find("ItemGroup/Item/Num").gameObject.SetActive(false);
            
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(_buyButton.transform.parent);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.BuyNode, _buyButton.transform as RectTransform, topLayer:topLayer);
        }

        public void InitContent(object content)
        {
            _content = (UIFarmMainController)content;
        }

        public void UpdateData(params object[] param)
        {
            if (param == null || param.Length == 0)
                return;

            _decoNode = (DecoNode)param[0];

            _title.SetTerm(_decoNode._data._config.title);
            _costIcon.sprite = UserData.GetResourceIcon(_decoNode._data._config.costId, UserData.ResourceSubType.Big);
            _costNum.SetText(_decoNode._data._config.price.ToString());
            
            _buildIcon.sprite = CommonUtils.LoadDecoItemIconSprite(_decoNode._stage.Area.Id, _decoNode._data._config.icon);
            
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.BuyNode, _decoNode.Config.id.ToString());
        }

        private void OnBuyNode()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.BuyNode, _decoNode.Config.id.ToString());
            
            if (!_decoNode.IsOwned && _decoNode.CanAfford())
            {
                UserData.Instance.ConsumeRes((UserData.ResourceId)_decoNode._data._config.costId, _decoNode._data._config.price, new GameBIManager.ItemChangeReasonArgs()
                    { reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Decorate });

                _decoNode.Buy();

                FarmModel.Instance.HideUnLockTip(_decoNode);
                
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

            UserData.ResourceId resId = (UserData.ResourceId)_decoNode.Config.costId;
            
            Vector3 srcPos = FlyGameObjectManager.Instance.GetResourcePosition(resId);
            Vector3 targetPos = _costIcon.transform.position;

            UIRoot.Instance.EnableEventSystem = false;
            FlyEffectTail(srcPos, targetPos, flyTime, resId);
        }

        private void FlyEffectTail(Vector3 srcPos,Vector3 targetPos,float flyTime,UserData.ResourceId resId)
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
                    FlyEndLogic();
                    GameObject.Destroy(flyClone);
                }));
            });
        }
        
        private void FlyEndLogic()
        {
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Buy, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Farm_Top, false);
            
            if (!StorySubSystem.Instance.Trigger(StoryTrigger.BuyNodeSuccess, _decoNode.Id.ToString(),
                    isFinish =>
                    {
                        if (isFinish)
                        {
                            UIRoot.Instance.EnableEventSystem = false;
                            DecoManager.Instance.CurrentWorld.FocusNode(_decoNode, () => { _decoNode.Stage.Area.World.Select(_decoNode, fromFirstBuy: true); });
                        }
                    },
                    isGuide => { }))
            {
                UIRoot.Instance.EnableEventSystem = false;
                DecoManager.Instance.CurrentWorld.FocusNode(_decoNode,
                    () => { _decoNode.Stage.Area.World.Select(_decoNode, fromFirstBuy: true); });
            }
            else
            {
                UIRoot.Instance.EnableEventSystem = true;
            }
        }
    }
}