using System.Collections.Generic;
using Deco.Node;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Gameplay;
using TMPro;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

namespace StoryMovie
{
    public class Action_PlayVideo : ActionBase
    {
        protected override void Init()
        {
        }

        protected override void Start()
        {
            GameObject prefab = ResourcesManager.Instance.LoadResource<GameObject>(_config.actionParam);
            //string[] spStr = _config.actionParam.Split('/');
            CGVideoManager.Instance.TryStartCG(null, () =>
            {
                AudioManager.Instance.ResumeAllMusic();
                // StorySubSystem.Instance.Trigger(StoryTrigger.VideoEnd, spStr[spStr.Length-1], b =>
                // {
                // });
            }, prefab, CanSkip:false);
        }

        protected override void Stop()
        {
        }
        
        protected override void ParsingParam()
        {
        }
    }
}