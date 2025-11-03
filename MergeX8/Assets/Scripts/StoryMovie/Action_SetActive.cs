using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace StoryMovie
{
    public class Action_SetActive : ActionBase
    {
        private bool isActive = true;
        protected override void Init()
        {
        }

        protected override void Start()
        {
            if(_movieObject == null)
                return;
            
            _movieObject.gameObject.SetActive(isActive);
        }

        protected override void Stop()
        {
        }
        
        protected override void ParsingParam()
        {
            int activeValue = 0;
            int.TryParse(_config.actionParam, out activeValue);

            isActive = activeValue == 1;
        }
    }
}