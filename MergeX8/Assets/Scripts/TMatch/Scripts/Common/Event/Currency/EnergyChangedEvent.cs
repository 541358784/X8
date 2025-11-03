using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMatch
{
    public class EnergyChangedEvent : BaseEvent
    {
        // 当前体力值，已经计算过变化量
        public int CurrentNumber { get; set; }

        // 本次操作变化的体力值
        public int ChangedNumber { get; set; }

        // 当前是否是无限体力
        public bool IsEnergyUnlimited { get; set; }

        public EnergyChangedEvent(bool isUnlimited, int curNum, int changedNum) : base(EventEnum.ENERGY_CHANGED)
        {
            IsEnergyUnlimited = isUnlimited;
            CurrentNumber = curNum;
            ChangedNumber = changedNum;
        }
    }
}