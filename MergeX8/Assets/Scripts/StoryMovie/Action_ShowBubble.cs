using System.Collections.Generic;
using Deco.Node;
using Decoration;
using DG.Tweening;
using Framework;
using TMPro;
using UnityEngine;

namespace StoryMovie
{
    public class Action_ShowBubble : ActionBase
    {
        protected override void Init()
        {
        }

        protected override void Start()
        {
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.HIDE_NODE_BUY);
        }

        protected override void Stop()
        {
        }
        
        protected override void ParsingParam()
        {
        }
    }
}