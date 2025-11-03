//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月21日 星期六
//describe    :   
//-----------------------------------

using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    /// <summary>
    /// 
    /// </summary>
    public class TM_BPOtherBuyEntrance : UIView
    {
        protected override bool IsChildView =>true;

        #region ui mem

        /// <summary>
        /// 购买按钮
        /// </summary>
        private Button btnBuy;

        /// <summary>
        /// 金币数量
        /// </summary>
        private LocalizeTextMeshProUGUI txtGoldNum;

        /// <summary>
        /// 道具预制
        /// </summary>
        private GameObject itemPrefab;

        /// <summary>
        /// 道具位置
        /// </summary>
        private Transform itemParent;

        /// <summary>
        /// 体力数量
        /// </summary>
        private LocalizeTextMeshProUGUI txtEnergyNum;

        /// <summary>
        /// 倒计时
        /// </summary>
        private LocalizeTextMeshProUGUI txtCountDown;
        

        /// <summary>
        /// 价格
        /// </summary>
        private Text txtPrice;

        #endregion

        /// <summary>
        /// 购买回调
        /// </summary>
        private Action<ShopLevel> buyCall;

        /// <summary>
        /// 奖励列表
        /// </summary>
        private List<GameItemInfo> itemlist;

        /// <summary>
        /// 计时器
        /// </summary>
        private Timer timer;

        /// <summary>
        /// 剩余时间
        /// </summary>
        private long remainTime;
        
        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitComponent()
        {
            txtGoldNum = transform.Find("Root/Gold/GoldNum").GetComponent<LocalizeTextMeshProUGUI>();
            txtEnergyNum= transform.Find("Root/Energy/EnergyMaxNum").GetComponent<LocalizeTextMeshProUGUI>();
            txtCountDown= transform.Find("Root/CountDown/TimeNum").GetComponent<LocalizeTextMeshProUGUI>();
            txtPrice = transform.Find("Root/Price/Text").GetComponent<Text>();

            itemPrefab = transform.Find("Root/ItemsGroup/Items").gameObject;
            itemPrefab.SetActive(false);

            itemParent = transform.Find("Root/ItemsGroup");
            
            btnBuy = transform.Find("Root/ButtonBuy").GetComponent<Button>();
            btnBuy.onClick.RemoveAllListeners();
            btnBuy.onClick.AddListener(ClickBuyBtn);
            
            EventDispatcher.Instance.AddEventListener(EventEnum.TM_BattlePassOnBuy, OnBuyEvent);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(Action<ShopLevel> buyEvent = null)
        {
            InitComponent();
            
            buyCall = buyEvent;

            InitReward();
            
            txtEnergyNum.SetText(TMBPModel.Instance.MaxEnergyNum.ToString());
            txtPrice.text = StoreModel.Instance.GetPrice(TMBPModel.Instance.ShopId1).ToString();
            
            timer = Timer.Register(1, OnTimer, null, true);
            OnTimer();
        }

        /// <summary>
        /// 计时器
        /// </summary>
        private void OnTimer()
        {
            remainTime = (long)TMBPModel.Instance.GetActivityLeftTime();
            if (remainTime == 0)
            {
                gameObject.SetActive(false);
                Timer.Cancel(timer);
            }
            else
                txtCountDown.SetText(CommonUtils.FormatLongToTimeStr((long)remainTime));
        }

        /// <summary>
        /// 初始化奖励
        /// </summary>
        private void InitReward()
        {
            itemlist = TMBPModel.Instance.GetBuyCanGetItemInfo();
            for (int i = 0; i < itemlist.Count; i++)
            {
                if (itemlist[i].ItemId == (int)ResourceId.TMCoin)
                {
                    txtGoldNum.SetText(itemlist[i].ItemNum.ToString());
                }
                else
                {
                    GameObject obj = GameObject.Instantiate(itemPrefab);
                    obj.transform.SetParent(itemParent);
                    obj.SetActive(true);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale = Vector3.one;

                    ItemConfig cfg = ItemModel.Instance.GetConfigById(itemlist[i].ItemId);
                    obj.transform.Find("Icon").GetComponent<Image>().sprite = ItemModel.Instance.GetItemSprite(cfg.id);
                    obj.transform.Find("NumberText").GetComponent<LocalizeTextMeshProUGUI>().SetText(
                        cfg.infinity ? CommonUtils.FormatPropItemTime((long) (cfg.infiniityTime * 1000)) : itemlist[i].ItemNum.ToString());
                    obj.transform.Find("InfiniteTag").gameObject.SetActive((ItemType)cfg.type == ItemType.TMEnergyInfinity);
                    obj.transform.Find("LimitGroup").gameObject.SetActive((ItemType)cfg.type == ItemType.TMLightingInfinity || (ItemType)cfg.type == ItemType.TMClockInfinity);
                }
            }
        }

        /// <summary>
        /// 购买事件
        /// </summary>
        /// <param name="obj"></param>
        private void OnBuyEvent(BaseEvent obj)
        {
            if (obj is TM_BPBuyEvent buyInfo)
            {
                var biValue = buyInfo.ShopLevel == ShopLevel.Normal ? "1" : "2";
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmBpbuyLifeUseup,data1:biValue);
                buyCall?.Invoke(buyInfo.ShopLevel);
            }
        }

        /// <summary>
        /// 点击购买按钮
        /// </summary>
        private void ClickBuyBtn()
        {
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmBpclickLifeUseup,data1:"1");
            TM_BPBuyView.Open();
        }

        private void OnDestroy()
        {
            Timer.Cancel(timer);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TM_BattlePassOnBuy, OnBuyEvent);
        }
    }
}