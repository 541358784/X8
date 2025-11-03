using System;
using System.Collections.Generic;
using Deco.Node;
using Decoration;
using DragonPlus;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Network.API;
using Farm.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Logic
{
    public class BubbleLogic : MonoBehaviour
    {
        public DecoNode _node { get; set; }
        public FarmType _type { get; set; }
        public bool _isInit { get; set; }
        public GameObject _root { get; set; }

        private GameObject _freeObj;
        private GameObject _producingObj;
        private GameObject _finishObj;

        private Image _freeImage;
        private Image _producingImage;
        private Image _finishImage;
        private Image _slider;
        //private LocalizeTextMeshProUGUI _coolTime;

        private long _ripeningTime;
        private long _startTime;
        private int _speedCost;
        private int _speedCoef;
        private int _speedTime;
        private long _cdTime;


        private GameObject _redPoint;
        
        private Button _button;
        
        private void Awake()
        {
            _freeObj = transform.Find("Root/Free").gameObject;
            _freeImage = transform.Find("Root/Free/Icon").GetComponent<Image>();
            _redPoint = transform.Find("Root/RedPoint").gameObject;
            _redPoint.gameObject.SetActive(false);
            
            _producingObj = transform.Find("Root/Producing").gameObject;
            _producingImage = transform.Find("Root/Producing/Icon").GetComponent<Image>();
            _slider = transform.Find("Root/Producing/Icon/SliderImage").GetComponent<Image>();
            //_coolTime = transform.Find("Root/Producing/Icon/CoolTime").GetComponent<LocalizeTextMeshProUGUI>();

            _finishObj = transform.Find("Root/Finish").gameObject;
            _finishImage = transform.Find("Root/Finish/Icon").GetComponent<Image>();

            InvokeRepeating("InvokeUpdate", 0, 1);

            _button = transform.GetComponent<Button>();
            _button.onClick.AddListener(() =>
            {
                if (GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.Farm_TouchGround))
                {
                    if (_node.Id == 901001)
                    {
                        var status = FarmModel.Instance.GetGroundProductStatus(_node);
                        if(status == FarmProductStatus.Finish)
                            FarmModel.Instance.ForceFinishSpeedGuide(true);
                    }
                }
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.Farm_TouchGround, _node.Id.ToString());
                FarmModel.Instance.TriggerFarmGuide();
                EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_TOUCH_NODE, _node);
            });
            
        }

        public void Init(Transform root, DecoNode node, FarmType type)
        {
            _root = root.gameObject;
            _node = node;
            _type = type;
            _isInit = true;
            
            UpdateStatus();
            CommonUtils.AddChild(UIRoot.Instance.mWorldUIRoot.transform, transform, false);

            if (FarmModel.Instance.IsGuideNode(node.Id))
            {
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(_button.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.Farm_TouchGround, _button.transform as RectTransform, topLayer:topLayer, targetParam:_node.Id.ToString());
            }
        }

        public void UpdateStatus()
        {
            InitBubbleUI();
            InitStatus();
            UpdateActive();
            UpdateCoolTime();
        }

        public void Select()
        {
        }

        public void UnSelect()
        {
        }

        private void InitBubbleUI()
        {
            string freeImage = "";
            string producingImage = "";
            string finishImage = "";

            switch (_type)
            {
                case FarmType.Animal:
                {
                    var config = FarmConfigManager.Instance.TableFarmAnimalList.Find(a => a.DecoNode == _node.Id);
                    if (config == null)
                        break;

                    freeImage = config.Image;
                    finishImage = FarmConfigManager.Instance.GetFarmProductIcon(config.ProductItem);
                    break;
                }
                case FarmType.Ground:
                {
                    var config = FarmConfigManager.Instance.TableFarmGroundList.Find(a => a.DecoNode == _node.Id);
                    if (config == null)
                        break;
                    freeImage = config.Image;

                    var storage = FarmModel.Instance.GetStorageGround(_node);
                    if (storage == null)
                        break;

                    var seedConfig = FarmConfigManager.Instance.TableFarmSeedList.Find(a => a.Id == storage.SeedId);
                    if (seedConfig == null)
                        break;

                    finishImage = seedConfig.Image;
                    break;
                }
                case FarmType.Machine:
                {
                    var config = FarmConfigManager.Instance.TableFarmMachineList.Find(a => a.DecoNode == _node.Id);
                    if (config == null)
                        break;
                    freeImage = config.Image;

                    var storage = FarmModel.Instance.GetStorageMachine(_node);
                    if (storage == null)
                        break;

                    var orderMachineConfig = FarmConfigManager.Instance.GetFarmMachineOrderConfig(storage.OrderId);
                    if (orderMachineConfig == null)
                        break;
                    finishImage = FarmConfigManager.Instance.GetFarmProductIcon(orderMachineConfig.ProductItem);
                    break;
                }
                case FarmType.Tree:
                {
                    var config = FarmConfigManager.Instance.TableFarmTreeList.Find(a => a.DecoNode == _node.Id);
                    if (config == null)
                        break;
                    freeImage = config.Image;

                    var storage = FarmModel.Instance.GetStorageTree(_node);
                    if (storage == null)
                        return;

                    finishImage = FarmConfigManager.Instance.GetFarmProductIcon(config.ProductItem);
                    break;
                }
            }

            if (_freeImage.sprite == null || _freeImage.sprite.name != freeImage)
                _freeImage.sprite = FarmModel.Instance.GetFarmIcon(freeImage);

            if (_finishImage.sprite == null || _finishImage.sprite.name != finishImage)
                _finishImage.sprite = FarmModel.Instance.GetFarmIcon(finishImage);
        }

        private void InvokeUpdate()
        {
            if(_node == null)
                return;
            
            if(!gameObject.activeSelf)
                return;
            
            InitStatus();
            UpdateCoolTime();
        }

        private void OnDisable()
        {
        }

        private void InitStatus()
        {
            var status = GetStatus();

            _freeObj.gameObject.SetActive(status == FarmProductStatus.Free);
            _producingObj.gameObject.SetActive(status == FarmProductStatus.Producing);
            _finishObj.gameObject.SetActive(status == FarmProductStatus.Finish);
            _redPoint.gameObject.SetActive(status == FarmProductStatus.Finish);
            
            if (status == FarmProductStatus.Producing)
                UpdateCoolTime();
        }

        private FarmProductStatus GetStatus()
        {
            FarmProductStatus status = FarmProductStatus.None;

            switch (_type)
            {
                case FarmType.Animal:
                {
                    status = FarmModel.Instance.GetAnimalProductStatus(_node);
                    break;
                }
                case FarmType.Ground:
                {
                    status = FarmModel.Instance.GetGroundProductStatus(_node);
                    break;
                }
                case FarmType.Machine:
                {
                    status = FarmModel.Instance.GetMachineProductStatus(_node);
                    break;
                }
                case FarmType.Tree:
                {
                    status = FarmModel.Instance.GetTreeProductStatus(_node);
                    break;
                }
            }

            return status;
        }

        private void UpdateCoolTime()
        {
            var status = GetStatus();
            if (status != FarmProductStatus.Producing)
                return;

            switch (_type)
            {
                case FarmType.Animal:
                {
                    var config = FarmConfigManager.Instance.TableFarmAnimalList.Find(a => a.DecoNode == _node.Id);
                    
                    var storage = FarmModel.Instance.GetStorageAnimal(_node);
                    if(storage == null)
                        return;

                    _ripeningTime = storage.RipeningTime;
                    _startTime= storage.StartTime;
                    _cdTime = storage.CdTime;
                    
                    _speedCost = config.SpeedCost;
                    _speedCoef = config.SpeedCoef;

                    _speedTime = (int)(config.RipeningTime * (config.SpeedCoef / 100f)*1000);
                    break;
                }
                case FarmType.Ground:
                {
                    var storage = FarmModel.Instance.GetStorageGround(_node);
                    if(storage == null)
                        return;

                    _ripeningTime = storage.RipeningTime;
                    _startTime= storage.StartTime;
                    _cdTime = storage.CdTime;
                    
                    var seedConfig = FarmConfigManager.Instance.TableFarmSeedList.Find(a => a.Id == storage.SeedId);
                    if(seedConfig == null)
                        return;
            
                    _speedCost = seedConfig.SpeedCost;
                    _speedCoef = seedConfig.SpeedCoef;
                    
                    _speedTime = (int)(seedConfig.RipeningTime * (seedConfig.SpeedCoef / 100f)*1000);
                    break;
                }
                case FarmType.Machine:
                {
                    var storage = FarmModel.Instance.GetStorageMachine(_node);
                    if(storage == null)
                        return;

                    _ripeningTime = storage.RipeningTime;
                    _startTime= storage.StartTime;
                    _cdTime = storage.CdTime;

                    var orderMachineConfig = FarmConfigManager.Instance.GetFarmMachineOrderConfig(storage.OrderId);
                    if(orderMachineConfig == null)
                        return;
                    _speedCost = orderMachineConfig.SpeedCost;
                    _speedCoef = orderMachineConfig.SpeedCoef;

                    _speedTime = (int)(orderMachineConfig.RipeningTime * (orderMachineConfig.SpeedCoef / 100f)*1000);
                    break;
                }
                case FarmType.Tree:
                {
                    var config = FarmConfigManager.Instance.TableFarmTreeList.Find(a => a.DecoNode == _node.Id);
                    var storage = FarmModel.Instance.GetStorageTree(_node);
                    if(storage == null)
                        return;
                    
                    _ripeningTime = storage.RipeningTime;
                    _startTime= storage.StartTime;
                    _cdTime = storage.CdTime;
                    
                    _speedCost = config.SpeedCost;
                    _speedCoef = config.SpeedCoef;

                    _speedTime = (int)(config.RipeningTime * (config.SpeedCoef / 100f)*1000);
                    break;
                }
            }

            long diff = _cdTime * 1000;
            long time = _ripeningTime - (long)APIManager.Instance.GetServerTime();
            if (time < 0)
                return;

            _slider.fillAmount = 1.0f - 1.0f * time / diff;
            //_coolTime.SetText(CommonUtils.FormatLongToTimeStr(time));
        }

        private void LateUpdate()
        {
            Follow();
        }

        private void Follow()
        {
            if(_node == null)
                return;
            
            var position = DecoSceneRoot.Instance.mSceneCamera.WorldToScreenPoint(_node.IconTipTransform.position);
            var screenPos = UIRoot.Instance.mUICamera.ScreenToWorldPoint(position);
            screenPos.z = 0;
            transform.position = screenPos;
        }

        private void UpdateActive()
        {
            if (_node == null || !_node.IsOwned || !FarmModel.Instance.IsUnLockNode(_node))
            {
                gameObject.SetActive(false);
                return;
            }
            
            gameObject.SetActive(true);
        }
    }
}