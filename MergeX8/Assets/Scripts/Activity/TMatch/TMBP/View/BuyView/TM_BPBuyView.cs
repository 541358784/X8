//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月21日 星期六
//describe    :   
//-----------------------------------

using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{
    public class BPBuyOpenData : UIWindowData
    {
        public bool TaskPop;
    }

    /// <summary>
    /// 购买bp
    /// </summary>
    public class TM_BPBuyView : UIWindowController
    {
        #region open ui

        /// <summary>
        /// 预制体路径
        /// </summary>
        private const string PREFAB_PATH = "Prefabs/Activity/TMatch/TMBP/TM_BPBuyView";

        /// <summary>
        /// 打开
        /// </summary>
        public static void Open(BPBuyOpenData data = null)
        {
            TMBPModel.Instance.JoinActivity();
            UIManager.Instance.OpenWindow<TM_BPBuyView>(PREFAB_PATH,data);
        }

        #endregion

        #region ui mem

        /// <summary>
        /// 价格
        /// </summary>
        private Text txtPrice1, txtPrice2;

        /// <summary>
        /// 倒计时
        /// </summary>
        private LocalizeTextMeshProUGUI txtCountDown;

        /// <summary>
        /// 奖励预制体
        /// </summary>
        private GameObject rewardPrefab;

        /// <summary>
        /// 奖励列表位置
        /// </summary>
        private Transform rewardParent;
        
        /// <summary>
        /// 添加等级
        /// </summary>
        private LocalizeTextMeshProUGUI txtAddlvNum;

        /// <summary>
        /// 添加等级的位置
        /// </summary>
        private Transform addLevelTrans;

        #endregion

        /// <summary>
        /// 计时器
        /// </summary>
        /// <returns></returns>
        private Timer timer;

        /// <summary>
        /// 剩余时间
        /// </summary>
        private ulong remainTime;

        /// <summary>
        /// 是否是任务链弹出的
        /// </summary>
        public bool IsTaskPop;

        /// <summary>
        /// 初始化组件
        /// </summary>
        public override void PrivateAwake()
        {
            SetClickListener("Root/CloseButton", () => CloseWindowWithinUIMgr());
            SetClickListener("Root/BuyButton1", () => ClickBuy(TMBPModel.Instance.ShopId1));
            SetClickListener("Root/BuyButton2", () => ClickBuy(TMBPModel.Instance.ShopId2));
            SetClickListener("Root/BuyButton2/AddLevel", ClickLevel);

            addLevelTrans = GetItem("Root/BuyButton2/AddLevel/Image").transform;
            txtPrice1 = GetItem<Text>("Root/BuyButton1/TextPrice");
            txtPrice2 = GetItem<Text>("Root/BuyButton2/TextPrice");
            txtCountDown = GetItem<LocalizeTextMeshProUGUI>("Root/CountDown/TextCountDown");
            
            txtAddlvNum = GetItem<LocalizeTextMeshProUGUI>("Root/BuyButton2/AddLevel/AddLevelNum");

            rewardParent = GetItem("Root/RewardParent").transform;
            rewardPrefab = GetItem("Root/RewardParent/RewardItem");
            rewardPrefab.SetActive(false);

            timer = Timer.Register(1, OnTimer, null, true);
            EventDispatcher.Instance.AddEventListener(EventEnum.TM_BattlePassOnBuy, OnBuyEvent);
        }
        
        /// <summary>
        /// 界面打开
        /// </summary>
        /// <param name="data"></param>
        protected override void OnOpenWindow(UIWindowData data)
        {
            base.OnOpenWindow(data);

            // GameBIManager.SendGameEventEx(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmPopGoldpass, TMBPModel.Instance.GetCurLevel());
            
            if (data != null && data is BPBuyOpenData openData)
            {
                IsTaskPop = openData.TaskPop;
            }

            TMBPModel.Instance.Data.LastPopBuyTime = APIManager.Instance.GetServerTime();
            
            txtPrice1.text = StoreModel.Instance.GetPrice(TMBPModel.Instance.ShopId1);
            txtPrice2.text = StoreModel.Instance.GetPrice(TMBPModel.Instance.ShopId2);
            txtAddlvNum.SetText(string.Format($"+{TMBPModel.Instance.AddLvNum}\nLv"));
           
            List<GameItemInfo> rewardList = TMBPModel.Instance.GetBuyCanGetItemInfo();
            for (int i = 0; i < rewardList.Count; i++)
            {
                GameObject obj = GameObject.Instantiate(rewardPrefab);
                obj.SetActive(true);
                obj.transform.SetParent(rewardParent);
                obj.transform.localScale = Vector3.one;
                
                DragonPlus.Config.TMatchShop.ItemConfig cfg = ItemModel.Instance.GetConfigById(rewardList[i].ItemId);
                
                obj.transform.Find("Icon").GetComponent<Image>().sprite =
                    ItemModel.Instance.GetItemSprite(rewardList[i].ItemId);
                obj.transform.Find("Num").GetComponent<LocalizeTextMeshProUGUI>().SetText(
                    cfg.infinity ? CommonUtils.FormatPropItemTime((long) (cfg.infiniityTime * 1000 * rewardList[i].ItemNum)) : rewardList[i].ItemNum.ToString());
                
                obj.transform.Find("InfiniteTag").gameObject.SetActive((ItemType)cfg.type == ItemType.TMEnergyInfinity);
                obj.transform.Find("LimitGroup").gameObject.SetActive((ItemType)cfg.type == ItemType.TMLightingInfinity || (ItemType)cfg.type == ItemType.TMClockInfinity);
            }

            OnTimer();
        }

        /// <summary>
        /// 计时器
        /// </summary>
        private void OnTimer()
        {
            remainTime = TMBPModel.Instance.GetActivityLeftTime();
            if (remainTime == 0)
                CloseWindowWithinUIMgr();
            else
                txtCountDown.SetText(CommonUtils.FormatLongToTimeStr((long)remainTime));
        }

        /// <summary>
        /// 购买事件
        /// </summary>
        private void OnBuyEvent(BaseEvent obj)
        {
            CloseWindowWithinUIMgr();
        }

        /// <summary>
        /// 点击购买
        /// </summary>
        private void ClickBuy(int shopId)
        {
            TMBPModel.Instance.Purchase(shopId);
        }

        /// <summary>
        /// 点击等级
        /// </summary>
        private void ClickLevel()
        {
            TM_BPTipBox.Open(addLevelTrans, LocalizationManager.Instance.GetLocalizedString("UI_battlepass_buy_extraLv10_tips"), new Vector2(17-70-70, 75));
        }

        /// <summary>
        /// 返回按钮
        /// </summary>
        protected override void OnBackButtonCallBack()
        {
            base.OnBackButtonCallBack();
            CloseWindowWithinUIMgr();
        }
        
        /// <summary>
        /// 销毁
        /// </summary>
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TM_BattlePassOnBuy, OnBuyEvent);
            Timer.Cancel(timer);
            if (IsTaskPop)
            {
                LobbyTaskSystem.Instance.FinishCurrentTask();
            }
        }
    }
}