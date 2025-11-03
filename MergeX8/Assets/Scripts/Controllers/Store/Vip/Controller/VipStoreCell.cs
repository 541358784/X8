using System;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Store.Vip.Controller
{
    public class VipStoreCell : MonoBehaviour
    {
        private TableVipStore _config;
        
        private int _index;
        private Button _btnBuy;
        private Button _btnHelp;
        private Image _icon;

        private Text _priceText;

        private LocalizeTextMeshProUGUI _contentText;
        private LocalizeTextMeshProUGUI _contentTextRare;
        
        private LocalizeTextMeshProUGUI _numberText;
        private LocalizeTextMeshProUGUI _numberTextRare;
        private Button _rvGO;
        private LocalizeTextMeshProUGUI _rvText;
        private Transform _coinGO, _diamonGO;
        private float clickTime;
        private Transform _tagGroup;
        private LocalizeTextMeshProUGUI _tagText;
        private LocalizeTextMeshProUGUI _rareText;
        private Transform _rareBG;
        private Transform _vipBG;
        private Transform _farmBg;
        private LocalizeTextMeshProUGUI _farmNumberText;
        private Transform _balloonRacing;

        private Transform _redPoint;

        private TableVipStoreSetting _settingConfig;
        public void Init(TableVipStore config, TableVipStoreSetting settingConfig)
        {
            _config = config;
            _settingConfig = settingConfig;
            
            InitUIComponent();
            InitUI();
        }

        private void InitUIComponent()
        {
            _redPoint = transform.Find("RedPoint");
            _redPoint.gameObject.SetActive(false);
            
            _icon = transform.Find("Icon").GetComponent<Image>();
            _priceText = transform.Find("ButtonBuy/Text").GetComponent<Text>();
            
            transform.Find("ContentText").gameObject.SetActive(false);
            _contentText = transform.Find("ContentTextVip").GetComponent<LocalizeTextMeshProUGUI>();
            _contentText.gameObject.SetActive(false);
            
            _contentTextRare = transform.Find("ContentTextRare").GetComponent<LocalizeTextMeshProUGUI>();
            _contentTextRare.gameObject.SetActive(false);
            
            _farmBg = transform.Find("FarmBG");
            _farmBg.gameObject.SetActive(false);
            
            _farmNumberText = transform.Find("NumberTextFarm").GetComponent<LocalizeTextMeshProUGUI>();
            _farmNumberText.gameObject.SetActive(false);
            
            transform.Find("NumberText").gameObject.SetActive(false);
            _numberText = transform.Find("NumberTextVip").GetComponent<LocalizeTextMeshProUGUI>();
            
            _numberTextRare = transform.Find("NumberTextRare").GetComponent<LocalizeTextMeshProUGUI>();
            _numberTextRare.gameObject.SetActive(false);
            
            _rareText = transform.Find("RareText").GetComponent<LocalizeTextMeshProUGUI>();
            _rareText.gameObject.SetActive(false);
            
            _rvGO = transform.Find("ButtonRv").GetComponent<Button>();
            _rvText = transform.Find("ButtonRv/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _rvGO.gameObject.SetActive(false);
            
            _coinGO = transform.Find("ButtonBuy/Coin");
            _coinGO.gameObject.SetActive(false);
            
            _diamonGO = transform.Find("ButtonBuy/Diamond");
            _diamonGO.gameObject.SetActive(false);
            
            _rareBG = transform.Find("RareBG");
            _rareBG.gameObject.SetActive(false);
            
            _vipBG = transform.Find("BG_Vip");
            _vipBG.gameObject.SetActive(true);
                
            _btnBuy = transform.Find("ButtonBuy").GetComponent<Button>();
            _btnBuy.onClick.AddListener(OnClickBuy);
            _btnBuy.gameObject.SetActive(true);
            
            _btnHelp = transform.Find("HelpButton").GetComponent<Button>();
            _btnHelp.onClick.AddListener(OnClickHelp);
            
            _tagGroup = transform.Find("TagGroup");
            _tagText = transform.Find("TagGroup/Text1").GetComponent<LocalizeTextMeshProUGUI>();
            _tagGroup.gameObject.SetActive(false);
            
            _balloonRacing = transform.Find("BalloonRacing");
            _balloonRacing.gameObject.SetActive(false);
            
            transform.Find("RabbitRacing").gameObject.SetActive(false);
            
        }

        private void Start()
        {
            CommonUtils.SetShieldButUnEnable(_btnBuy.gameObject);
        }

        public void Refresh()
        {
            InitUI();
        }
        
        private void InitUI()
        {
            _icon.sprite = UserData.GetResourceIcon(_config.buyItem);
            _diamonGO.gameObject.SetActive(_config.buyCost > 0);
            
            int buyCount = VipStoreModel.Instance.GetBuyCount(_config.storeid, _config.buyItem);
            int countLeft = _config.buyCount - buyCount;
            countLeft = Math.Max(countLeft, 0);

            _priceText.text = _config.buyCost.ToString();
            
            _btnBuy.interactable = true;
            
            _rareBG.gameObject.SetActive(_config.buyCost == 0);
            _numberTextRare.gameObject.SetActive(_config.buyCost == 0);
            _numberText.gameObject.SetActive(_config.buyCost >= 0);
            

            if (_config.getNum > 1)
            {
                _contentText.SetText(_config.getNum.ToString());
                _contentTextRare.SetText(_config.getNum.ToString());
                
                _contentText.gameObject.SetActive(_config.buyCost >= 0);
                _contentTextRare.gameObject.SetActive(_config.buyCost == 0);
            }
            else
            {
                _contentText.gameObject.SetActive(false);
                _contentTextRare.gameObject.SetActive(false);
            }
            
            string txt = LocalizationManager.Instance.GetLocalizedString("UI_info_text11");
            _numberText.SetText(string.Format(txt, countLeft));
            _numberTextRare.SetText(string.Format(txt, countLeft));
            
            if (countLeft == 0)
            {   
                _priceText.text = LocalizationManager.Instance.GetLocalizedString("button_soldout");
                _diamonGO.gameObject.SetActive(false);
                _btnBuy.interactable = false;
            }
            else
            {
                _priceText.text = _config.buyCost == 0 ? LocalizationManager.Instance.GetLocalizedString("UI_common_free") : _config.buyCost.ToString();
            }
            
            RefreshRedPoint();
            _btnHelp.gameObject.SetActive(!UserData.Instance.IsResource(_config.buyItem));
        }

        private void OnClickBuy()
        {
            if (_config.buyCost == 0)
            {
                BuyStoreItem(true);
                
                var reason = new GameBIManager.ItemChangeReasonArgs
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.VipStore,
                    data1 = _config.buyItem.ToString(),
                    data2 = VipStoreModel.Instance.GetVipLevelString(),
                };
                UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, _config.buyCost, reason);
            }
            else
            {
                if (UserData.Instance.CanAford(UserData.ResourceId.Diamond, _config.buyCost))
                {
                    var reason = new GameBIManager.ItemChangeReasonArgs
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.VipStore,
                        data1 = _config.buyItem.ToString(),
                        data2 = VipStoreModel.Instance.GetVipLevelString(),
                    };
                    UserData.Instance.ConsumeRes(UserData.ResourceId.Diamond, _config.buyCost, reason);
                    BuyStoreItem(false);
                }
                else
                {
                    CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                    {
                        DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_info_text14"),
                        HasCloseButton = true,
                    });
                }
            }
        }

        private void BuyStoreItem(bool isFree)
        {
            VipStoreModel.Instance.SaveBuyRecord(_config.storeid, _config.buyItem);
            InitUI();

            if (UserData.Instance.IsResource(_config.buyItem))
            {
                UserData.Instance.AddRes((int)UserData.ResourceId.Energy,_config.getNum,new GameBIManager.ItemChangeReasonArgs()
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.VipStore,
                    data1 = _config.buyItem.ToString(),
                    data2 = VipStoreModel.Instance.GetVipLevelString(),
                }, false);
               
                FlyGameObjectManager.Instance.FlyCurrency(CurrencyGroupManager.Instance.GetCurrencyUseController(),
                    UserData.ResourceId.Energy, _config.getNum, transform.position, 0.8f, true, true, 0.15f);
            }
            else
            {
                for (int i = 0; i < _config.getNum; i++)
                {
                    var mergeItem = MergeManager.Instance.GetEmptyItem();
                    mergeItem.Id = _config.buyItem;
                    mergeItem.State = 1;
                    MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.Main);

                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonVipStoreGet,
                        itemAId = mergeItem.Id,
                        isChange = true,
                        data1= isFree ? "0" : "1",
                        data2 = VipStoreModel.Instance.GetVipLevelString()
                    });
                    Vector3 endPos = Vector3.zero;
                    if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
                    {
                        endPos = MergeMainController.Instance.rewardBtnPos;
                    }
                    else
                    {
                        endPos = UIHomeMainController.mainController.MainPlayTransform.position;
                    }

                    FlyGameObjectManager.Instance.FlyObject(mergeItem.Id, transform.position, endPos, 1.2f, 2.0f, 1f,
                        () => { EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH); });
                }
            }
        }
        
        private void OnClickHelp()
        {
            MergeInfoView.Instance.OpenMergeInfo(_config.buyItem,_isShowProbability:true);
            UIPopupMergeInformationController controller =
                UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
            if (controller != null)
                controller.SetResourcesActive(false);
        }

        private void OnDestroy()
        {
            _contentText.gameObject.SetActive(false);
            _contentTextRare.gameObject.SetActive(false);
            
            _numberText.gameObject.SetActive(false);
            _numberTextRare.gameObject.SetActive(false);
            
            _vipBG.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _redPoint.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            RefreshRedPoint();
        }

        private void RefreshRedPoint()
        {
            if(_redPoint == null)
                return;
            
            _redPoint.gameObject.SetActive(false);
            if (VipStoreModel.Instance.GetCurrentConfig() == null || VipStoreModel.Instance.GetCurrentConfig().id < _settingConfig.id)
            {
                return;
            }
            
            if(_config.buyCost != 0)
                return;
            
            
            int buyCount = VipStoreModel.Instance.GetBuyCount(_config.storeid, _config.buyItem);
            int countLeft = _config.buyCount - buyCount;
            countLeft = Math.Max(countLeft, 0);
            if(countLeft == 0)
                 return;
            
            _redPoint.gameObject.SetActive(true);
        }
    }
}