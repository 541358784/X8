//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月25日 星期三
//describe    :   
//-----------------------------------

using System;
using DragonPlus;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{
    /// <summary>
    /// 奖励动画预制
    /// </summary>
    public class TM_RewardAnimItem : MonoBehaviour
    {
        /// <summary>
        /// 奖励动画预制体
        /// </summary>
        private const string PREFAB_PATH = "Prefabs/Activity/TMatch/TMBP/TM_RewardItemAnim";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="info">数据</param>
        /// <param name="rectTransform">位置</param>
        public static void Create(TM_BPCreateRewardItemAnimEvent info, RectTransform rectTransform)
        {
            GameObject prefab = ResourcesManager.Instance.LoadResource<GameObject>(PREFAB_PATH);
            GameObject obj = Instantiate(prefab);

            obj.transform.SetParent(rectTransform.transform);
            obj.transform.localScale = Vector3.one;

            Vector2 screenPos =
                RectTransformUtility.WorldToScreenPoint(UIRoot.Instance.mUICamera, info.Target.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPos,
                UIRoot.Instance.mUICamera, out Vector2 uiPos);
            obj.transform.localPosition = uiPos;

            TM_RewardAnimItem item = obj.GetOrCreateComponent<TM_RewardAnimItem>();
            item.Init(info.ItemId, info.ItemNum, info.BpType);
        }

        /// <summary>
        /// 计时器
        /// </summary>
        private Timer timer;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(int itemId, int itemNum, TM_BpType type)
        {
            transform.Find("Root/Icon").GetComponent<Image>().sprite = ItemModel.Instance.GetItemSprite(itemId);
            transform.Find("Root/Count").GetComponent<LocalizeTextMeshProUGUI>()
                .SetText(CommonUtils.GetItemText(itemId, itemNum));
            transform.Find("Root/Count").GetComponent<LocalizeTextMeshProUGUI>()
                .SetColor(type == TM_BpType.Normal ? new Color(0.28f, 0.4f, 0.9f) : new Color(0.69f, 0.2f, 0.05f));

            DragonPlus.Config.TMatchShop.ItemConfig itemCfg = ItemModel.Instance.GetConfigById(itemId);
            transform.Find("Root/Icon/Limit").gameObject.SetActive((ItemType)itemCfg.type == ItemType.TMEnergyInfinity);
            transform.Find("Root/Icon/LimitGroup").gameObject.SetActive(
                (ItemType)itemCfg.type == ItemType.TMLightingInfinity ||
                (ItemType)itemCfg.type == ItemType.TMClockInfinity);

            timer = Timer.Register(2, OnTimer);
        }

        /// <summary>
        /// 计时器
        /// </summary>
        private void OnTimer()
        {
            GameObject.Destroy(gameObject);
        }

        private void OnDestroy()
        {
            Timer.Cancel(timer);
        }
    }
}