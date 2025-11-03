using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutsideGuide
{
    public class GuideSkip : GuideGraphicBase
    {
        protected override void Init()
        {
            
        }

        public override bool IsShow()
        {
            return gameObject.activeSelf;
        }

        public override void Show()
        {
            if(!IsShow()) gameObject.SetActive(true);
        }

        public override void Hide()
        {
            if(IsShow()) gameObject.SetActive(false);
        }

        public void OnSkip()
        {
            //TODO:发消息通知
        }
    }
}