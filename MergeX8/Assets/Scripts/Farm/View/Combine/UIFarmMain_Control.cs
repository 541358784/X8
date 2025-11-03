using System;
using System.Collections.Generic;
using ABTest;
using Deco.Node;
using Decoration;
using DragonPlus;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Farm.Model;
using Filthy.Game;
using Gameplay;
using TMatch;
using UnityEngine;
using UnityEngine.UI;
using AudioManager = DragonPlus.AudioManager;
using SfxNameConst = DragonPlus.SfxNameConst;
using Utils = Makeover.Utils;

namespace Farm.View
{
    public class UIFarmMain_Control : MonoBehaviour, IInitContent
    {
        private Button _backButton;
        private Button _playButton;
        private Button _keepPetButton;
        private Button _warehouseButton;
        
        private Button _speedButton;
        private Button _specialSpeedButton;
        private Button _specialAddButton;

        private LocalizeTextMeshProUGUI _titleText;
        private LocalizeTextMeshProUGUI _timeText;
        private LocalizeTextMeshProUGUI _contentText;
        private LocalizeTextMeshProUGUI _speedText;
        
        private LocalizeTextMeshProUGUI _speedCostText;
        private Image _speedCostImage;
        
        private LocalizeTextMeshProUGUI _specialSpeedCostText;
        private LocalizeTextMeshProUGUI _specialSpeedText;
        private Image _specialSpeedCostImage;
        
        
        private GameObject _infoGameObject;
        private GameObject _tipsGameObject;

        private DecoNode _node;
        private long _ripeningTime;
        private int _speedCost;
        private int _speedCoef;
        private int _speedTime;
        private int _speedId;
        private int _cdTime;
        
        private int _propSpeedTime;
        private int _propId;
        
        private Button _butScrew;
        private Button _butTMatch;
        
        private FarmType _farmType = FarmType.None;
        
        private void Awake()
        {
            _infoGameObject = transform.Find("TaskBtn/Info").gameObject;
            _tipsGameObject = transform.Find("TaskBtn/TipsText").gameObject;
            
            _backButton = transform.Find("BackBtn").GetComponent<Button>();
            _backButton.onClick.AddListener(OnClickBack);
                
            _playButton = transform.Find("PlayBtn").GetComponent<Button>();
            _playButton.onClick.AddListener(OnClickPlay);
            FarmModel.Instance.MainPlayTransform = _playButton.transform;
            
            _keepPetButton = transform.Find("KeepPet").GetComponent<Button>();
            _keepPetButton.gameObject.AddComponent<Aux_KeepPet>();

            _butScrew = transform.Find("Screw").GetComponent<Button>();
            _butScrew.onClick.AddListener(OnClickScrew);
            
            _butTMatch = transform.Find("TMatch").GetComponent<Button>();
            _butTMatch.onClick.AddListener(OnClickTMatch);
            
            _warehouseButton = transform.Find("BagBtn").GetComponent<Button>();
            _warehouseButton.onClick.AddListener(OnClickWarehouse);
            FarmModel.Instance.WarehouseTransform = _warehouseButton.transform;
            
            _speedButton = transform.Find("TaskBtn/Info/SpeedBtn").GetComponent<Button>();
            _speedButton.onClick.AddListener(OnClickSpeed);
            _speedCostText = transform.Find("TaskBtn/Info/SpeedBtn/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _speedCostImage = transform.Find("TaskBtn/Info/SpeedBtn/Icon").GetComponent<Image>();
            _speedText = transform.Find("TaskBtn/Info/SpeedBtn/SellText").GetComponent<LocalizeTextMeshProUGUI>();
            
            _specialSpeedButton = transform.Find("TaskBtn/Info/SpecialSpeedBtn").GetComponent<Button>();
            _specialSpeedButton.onClick.AddListener(OnClickSpecialSpeed);
            _specialSpeedCostText = transform.Find("TaskBtn/Info/SpecialSpeedBtn/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _specialSpeedCostImage = transform.Find("TaskBtn/Info/SpecialSpeedBtn/Icon").GetComponent<Image>();
            _specialSpeedText = transform.Find("TaskBtn/Info/SpecialSpeedBtn/SellText").GetComponent<LocalizeTextMeshProUGUI>();
            
            _specialAddButton = transform.Find("TaskBtn/Info/SpecialSpeedBtn/Add").GetComponent<Button>();
            _specialAddButton.onClick.AddListener(OnClickAddButton);
            
            _titleText = transform.Find("TaskBtn/Info/TitleText").GetComponent<LocalizeTextMeshProUGUI>();
            _timeText = transform.Find("TaskBtn/Info/CdGroup/CdText").GetComponent<LocalizeTextMeshProUGUI>();
            _contentText = transform.Find("TaskBtn/Info/CdGroup/ContentText").GetComponent<LocalizeTextMeshProUGUI>();

            UpdateData();
            
            InvokeRepeating("InvokeRepeat", 0, 1);
            
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(_speedButton.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Farm_Speed, _speedButton.transform as RectTransform, topLayer:topLayer);

            EventDispatcher.Instance.AddEventListener(EventEnum.FARM_REFRESH_PRODUCT, Event_RefreshProduct);

            TeamEntrance = transform.Find("Guild").gameObject.AddComponent<TeamHomeEntrance>();
        }

        public TeamHomeEntrance TeamEntrance;
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.FARM_REFRESH_PRODUCT, Event_RefreshProduct);
        }

        public void InitContent(object content)
        {
        }

        private void Event_RefreshProduct(BaseEvent e)
        {
            if(!AnimControlManager.Instance.IsShow(AnimKey.Farm_Contrl))
               return;
               
               RefreshSpecialSpeed();
        }
        
        public void UpdateData(params object[] param)
        {
            _node = null;
            
            if (Utils.IsOpen && FilthyGameLogic.Instance.IsOpenFilthy())
            {
                _butScrew.gameObject.SetActive(true);   
                _butTMatch.gameObject.SetActive(false);
            }
            else
            {
                _butScrew.gameObject.SetActive(false);   
                _butTMatch.gameObject.SetActive(TMatchModel.Instance.IsUnlock() && ABTestManager.Instance.IsOpenTMatch());
            }
            
            if (param == null || param.Length == 0)
            {
                _infoGameObject.gameObject.SetActive(false);
                _tipsGameObject.gameObject.SetActive(true);
                
                return;
            }

            _infoGameObject.gameObject.SetActive(true);
            _tipsGameObject.gameObject.SetActive(false);
            _specialSpeedButton.gameObject.SetActive(false);
            
            _node = (DecoNode)param[0];
            _farmType = FarmConfigManager.Instance.GetDecoNodeType(_node.Id);
            
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.Farm_Speed, "");
            
            switch (_farmType)
            {
                case FarmType.Animal:
                {
                    AnimalControl();
                    break;
                }
                case FarmType.Ground:
                {
                    GroundControl();
                    break;
                }
                case FarmType.Machine:
                {
                    MachineControl();
                    break;
                }
                case FarmType.Tree:
                {
                    TreeControl();
                    break;
                }
            }

            if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.Farm_Speed))
                _speedCost = 0;
            
            _speedCostText.SetText(_speedCost.ToString());
        }

        private void InvokeRepeat()
        {
            if(_node == null)
                return;

            long time = _ripeningTime - (long)APIManager.Instance.GetServerTime();
            if (time < 0)
            {
                _node = null;
                _infoGameObject.gameObject.SetActive(false);
                _tipsGameObject.gameObject.SetActive(true);
                
                FarmModel.Instance.ForceFinishSpeedGuide();
                
                FarmModel.Instance.TriggerFarmGuide();
                return;
            }
            
            _timeText.SetText(CommonUtils.FormatLongToTimeStr(time));
        }

        private void OnClickBack()
        {
            if(!DecoManager.Instance.IsWorldReady)
                return;
            
            FarmModel.Instance.LeaveWorld();
        }

        private void OnClickPlay()
        {
            UIRoot.Instance.EnableEventSystem = false;
            ShakeManager.Instance.ShakeSelection();
            AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        
            SceneFsm.mInstance.TransitionGame();
        }
        
        private void OnClickWarehouse()
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupFarmBag);
        }
        private void OnClickSpeed()
        {
            if (!UserData.Instance.CanAford(UserData.ResourceId.Diamond, _speedCost))
            {
                BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "FarmSpeed", "", "diamond_lack_bag",true, _speedCost);
                return;
            }
            
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.Farm_Speed);
            
            UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, _speedCost, new GameBIManager.ItemChangeReasonArgs()
            {
                reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.SpeedUpFarm,
                data1 = _speedId.ToString(),
            });
            
            _ripeningTime -= _speedTime;
            FarmModel.Instance.Speed(_node, _speedTime);

            InvokeRepeat();
            
            FarmModel.Instance.TriggerFarmGuide();
        }
        
        private void OnClickSpecialSpeed()
        {
            if(!FarmModel.Instance.HavEnoughProduct(_propId, 1))
                return;
            
            FarmModel.Instance.ConsumeProductItem(_propId, 1);
            
            _ripeningTime -= _propSpeedTime;
            FarmModel.Instance.Speed(_node, _propSpeedTime);

            InvokeRepeat();
            
            FarmModel.Instance.TriggerFarmGuide();
        }

        private void OnClickAddButton()
        {
            UIStoreController.OpenUI("",ShowArea.flash_sale);
        }
        
        private void AnimalControl()
        {
            var config = FarmConfigManager.Instance.TableFarmAnimalList.Find(a => a.DecoNode == _node.Id);
            _titleText.SetTerm(config.NameKey);
                    
            var storage = FarmModel.Instance.GetStorageAnimal(_node);
            if(storage == null)
                return;

            _ripeningTime = storage.RipeningTime;

            _speedCost = config.SpeedCost;
            _speedCoef = config.SpeedCoef;
            _speedId = config.Id;

            _speedTime = (int)(config.RipeningTime * (config.SpeedCoef / 100f)*1000);
            _contentText.SetTerm("UI_farminfo_animalAcc");
            _speedText.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("UI_farminfo_speedup"), CommonUtils.FormatLongToTimeStr(_speedTime)));
            InvokeRepeat();
            
            PropControl(UserData.ResourceId.Farm_Clock, config.RipeningTime);
        }

        private void MachineControl()
        {
            var config = FarmConfigManager.Instance.TableFarmMachineList.Find(a => a.DecoNode == _node.Id);
            _titleText.SetTerm(config.NameKey);
                    
            var storage = FarmModel.Instance.GetStorageMachine(_node);
            if(storage == null)
                return;

            _ripeningTime = storage.RipeningTime;

            var orderMachineConfig = FarmConfigManager.Instance.GetFarmMachineOrderConfig(storage.OrderId);
            if(orderMachineConfig == null)
                return;
            _speedCost = orderMachineConfig.SpeedCost;
            _speedCoef = orderMachineConfig.SpeedCoef;
            _speedId = orderMachineConfig.Id;

            _speedTime = (int)(orderMachineConfig.RipeningTime * (orderMachineConfig.SpeedCoef / 100f)*1000);
            _contentText.SetTerm("UI_farminfo_machineAcc");
            _speedText.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("UI_farminfo_speedup"), CommonUtils.FormatLongToTimeStr(_speedTime)));
            InvokeRepeat();
            
            PropControl(UserData.ResourceId.Farm_Gear, orderMachineConfig.RipeningTime);
        }

        private void GroundControl()
        {
            var config = FarmConfigManager.Instance.TableFarmGroundList.Find(a => a.DecoNode == _node.Id);

            var storage = FarmModel.Instance.GetStorageGround(_node);
            if(storage == null)
                return;

            _ripeningTime = storage.RipeningTime;
                    
            var seedConfig = FarmConfigManager.Instance.TableFarmSeedList.Find(a => a.Id == storage.SeedId);
            if(seedConfig == null)
                return;
            
            _titleText.SetTerm(seedConfig.NameKey);

            _speedCost = seedConfig.SpeedCost;
            _speedCoef = seedConfig.SpeedCoef;
            _speedId = seedConfig.Id;

            _speedTime = (int)(seedConfig.RipeningTime * (seedConfig.SpeedCoef / 100f)*1000);
            _contentText.SetTerm("UI_farminfo_groundAcc");
            _speedText.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("UI_farminfo_speedup"), CommonUtils.FormatLongToTimeStr(_speedTime)));
            InvokeRepeat();
            
            PropControl(UserData.ResourceId.Farm_SFertilizer, seedConfig.RipeningTime);
        }

        private void TreeControl()
        {
            var config = FarmConfigManager.Instance.TableFarmTreeList.Find(a => a.DecoNode == _node.Id);
            _titleText.SetTerm(config.NameKey);
                    
            var storage = FarmModel.Instance.GetStorageTree(_node);
            if(storage == null)
                return;
                    
            _ripeningTime = storage.RipeningTime;
                    
            _speedCost = config.SpeedCost;
            _speedCoef = config.SpeedCoef;
            _speedId = config.Id;

            _speedTime = (int)(config.RipeningTime * (config.SpeedCoef / 100f)*1000);
            _contentText.SetTerm("UI_farminfo_treeAcc");
            _speedText.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("UI_farminfo_speedup"), CommonUtils.FormatLongToTimeStr(_speedTime)));
            InvokeRepeat();

            PropControl(UserData.ResourceId.Farm_SKettle, config.RipeningTime);
        }

        private void PropControl(UserData.ResourceId id, int speedTime)
        {
            _propId = (int)id;

            var isShow = RefreshSpecialSpeed();
            
            if(!isShow)
                return;

            var config = FarmConfigManager.Instance.TableFarmPropList.Find(a => a.Id == _propId);
            if(config == null)
                return;
            
            _propSpeedTime =  (int)(speedTime * (config.SpeedCoef / 100f)*1000);
            
            _specialSpeedCostText.SetText("1");
            _specialSpeedText.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("UI_farminfo_speedup"), CommonUtils.FormatLongToTimeStr(_propSpeedTime)));
           
            if(_specialSpeedCostImage.sprite == null || _specialSpeedCostImage.sprite.name != config.Image)
                _specialSpeedCostImage.sprite =  FarmModel.Instance.GetFarmIcon(config.Image);
        }

        private bool RefreshSpecialSpeed()
        {
            int propNum = FarmModel.Instance.GetProductItemNum(_propId);

            bool isShow = propNum > 0;
            
            _specialSpeedButton.gameObject.SetActive(isShow);
            if (isShow)
            {
                _specialAddButton.gameObject.SetActive(false);
                _specialSpeedCostText.gameObject.SetActive(true);
            }
            else
            {
                isShow = StoreModel.Instance.CanBuyFarmSale(_propId);
                
                _specialSpeedButton.gameObject.SetActive(isShow);
                
                _specialAddButton.gameObject.SetActive(true);
                _specialSpeedCostText.gameObject.SetActive(false);
            }

            return isShow;
        }
        
        private void OnClickScrew()
        {
            SceneFsm.mInstance.EnterScrewHome();
        }

        private void OnClickTMatch()
        {
            UIRoot.Instance.EnableEventSystem = false;
            TMatchModel.Instance.Enter();
        }

        public bool IsSelectNode()
        {
            return _node != null;
        }
    }
}