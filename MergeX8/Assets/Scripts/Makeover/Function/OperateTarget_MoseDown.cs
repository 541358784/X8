using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ASMR
{
    public class OperateTarget_MoseDown : OperateTarget
    {
        public int TimeLineId = 0;
        public override void ExcuteTask()
        {
            if (TimeLineId >= 0)
            {
                this.gameObject.SetActive(false);
                ASMR.Model.Instance.ExcuteTimeLine(TimeLineId);
            }
        }
    }
}

