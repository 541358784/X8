using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public class StarsNum : ResBar
    {
        private static List<StarsNum> viewList = new List<StarsNum>();

        public static Transform GetTopView()
        {
            if (viewList.Count > 0) return viewList[viewList.Count - 1]._icon.transform;
            return null;
        }

        private void Awake()
        {
            viewList.Add(this);
            _resourceId = ResourceId.TMStar;
            _itemType = ItemType.TMStar;
            _icon = transform.Find("Icon").GetComponent<Image>();
            UpdateNumber(false);
            EventDispatcher.Instance.AddEventListener(EventEnum.CurrencyFlyAniEnd, CurrencyFlyAniEnd);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < viewList.Count; i++)
            {
                if (viewList[i] == this)
                {
                    viewList.Remove(this);
                    break;
                }
            }

            EventDispatcher.Instance.RemoveEventListener(EventEnum.CurrencyFlyAniEnd, CurrencyFlyAniEnd);
        }

        protected override int CurrentCount
        {
            get => CurrencyModel.Instance.GetRes(_resourceId);
        }

        protected override TextMeshProUGUI CountText
        {
            get => transform.Find("CountText").GetComponent<TextMeshProUGUI>();
        }

        protected override bool TypeTest(ResourceId resId)
        {
            return resId == _resourceId;
        }

        // private void CurrencyFlyAniEnd(BaseEvent evt) {
        //     CurrencyFlyAniEnd eventData = evt as CurrencyFlyAniEnd;
        //     if (eventData.itemType == ItemType.Star) {
        //         transform.GetComponent<Animator>()?.Play("shake");
        //         UpdateNumber(true);
        //     }
        // }
    }
}