using System.Collections.Generic;
using Deco.Node;
using Decoration;
using DG.Tweening;
using Framework;
using TMPro;
using UnityEngine;

namespace StoryMovie
{
    public class Action_EndPreView : ActionBase
    {
        private int _nodeId = -1;
        protected override void Init()
        {
        }

        protected override void Start()
        {
            DecoNode decoNode = DecoManager.Instance.FindNode(_nodeId);
            if(decoNode == null)
                return;

            var itemId = decoNode.Config.defaultItem;
            decoNode.EndPreview(false);
        }

        protected override void Stop()
        {
        }
        
        protected override void ParsingParam()
        {
            int.TryParse(_config.actionParam, out _nodeId);
        }
    }
}