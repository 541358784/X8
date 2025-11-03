using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMatch
{


    public class ResChangeEvent : BaseEvent
    {
        public ResourceId CurrencyId { get; private set; }
        public ResHostUI FlyUI { get; private set; }
        public bool AnimateChange = true;

        /// <summary>
        /// 需要忽略的数字大小，输入正数才需要减去该数字，反之则加
        /// </summary>
        public int IgnoreNumber { get; private set; }

        public ResChangeEvent(ResourceId cId, ResHostUI flyUI = ResHostUI.None, int ignoreNumber = 0,
            bool animateChange = true) : base(EventEnum.CURRENCY_CHANGED)
        {
            CurrencyId = cId;
            FlyUI = flyUI;
            IgnoreNumber = ignoreNumber;
            AnimateChange = animateChange;
        }
    }
}