using System.Collections.Generic;
using Decoration;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace StoryMovie
{
    public class Action_Show : Action_PlayAnim
    {
        protected override void Init()
        {
            base.Init();
        }

        protected override void Start()
        {
            if(_movieObject == null)
                return;
            
            InitPosition();
            PlayAnimation();

            switch (_config.controlType)
            {
                //主角
                case 1:
                {
                    PlayerManager.Instance.AnimShowPlayer(PlayerManager.PlayerType.Chief, _config.movieTime);
                    break;
                }
                //小狗
                case 2:
                {
                    PlayerManager.Instance.AnimShowPlayer(PlayerManager.PlayerType.Dog, _config.movieTime);
                    break;
                }
                //男主
                case 4:
                {
                    PlayerManager.Instance.AnimShowPlayer(PlayerManager.PlayerType.Hero, _config.movieTime);
                    break;
                }
            }
        }

        protected override void Stop()
        {
        }


        protected override void ParsingParam()
        {
            ParamToVector3DList();
        }
    }
}