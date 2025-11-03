
using System;
using DG.Tweening;
using Framework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Decoration.Player
{
    public class PlayerStatusIdle : PlayerStatusBase
    {
        protected Coroutine _coroutine;
        
        protected float _leisureTime = 0;
        
        private readonly string[] _chiefLeisureName = new string[]
        {
            "idle",
            "look",
            "think",
            "hello",
            "notes",
            "telescope",
        };
        
        private readonly string[] _dogLeisureName = new string[]
        {
            "idle3",
            "idle4",
        };
        
        private readonly string[] _heroLeisureName = new string[]
        {
            "idle",
            "idle2",
        };
        
        public override void OnStart()
        {
            if (_animator == null)
                return;

            _leisureTime = Random.Range(10, 25);

            string animName = PlayerManager.Instance.GetAnimationName(_playerType, PlayerManager.ActionName.Idle);
            if(!animName.IsEmptyString())
                _animator.CrossFade(animName, _crossTime);

            _coroutine = CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(_leisureTime, () =>
            {
                SwitchPlayAnimStatus();
                _coroutine = null;
            }));
        }

        private void SwitchPlayAnimStatus()
        {
            string[] leisureName = _chiefLeisureName;
            if (_playerType == PlayerManager.PlayerType.Dog)
                leisureName = _dogLeisureName;
            else if(_playerType == PlayerManager.PlayerType.Hero)
                leisureName = _heroLeisureName;
                

            string leisure = leisureName[Random.Range(0, leisureName.Length)];
            PlayerManager.Instance.SwitchPlayerStatus(_playerType, PlayerManager.StatusType.PlayAnim, 0.1f, -1, leisure);
        }
        public override void OnStop()
        {
            if(_coroutine != null)
                CoroutineManager.Instance.StopCoroutine(_coroutine);
        }
    }
}