using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace StoryMovie
{
    public class Action_PlayAnim : ActionBase
    {
        protected override void Init()
        {
        }

        protected override void Start()
        {
            if(_movieObject == null)
                return;
            
            InitPosition();
            PlayAnimation();
        }

        protected override void Stop()
        {
        }
        
        protected override void ParsingParam()
        {
            ParamToFloat();
        }
    }
}