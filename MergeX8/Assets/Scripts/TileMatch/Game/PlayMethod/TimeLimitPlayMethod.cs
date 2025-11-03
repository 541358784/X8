using System;
using System.Collections.Generic;
using System.Resources;
using DragonPlus;
using DragonU3DSDK.Asset;
using Framework;
using TileMatch.Event;
using UnityEngine;
using UnityEngine.UI;

namespace TileMatch.Game.PlayMethod
{
    public partial class TimeLimitPlayMethod : PlayMethodBase
    {
        private int _timeLimit = 0;
        private bool _isStart = false;
        private float _currentTime = 0;
        private bool _isPause = false;
        private int _orgTimeLimit = 0;

        
        public override void Init(params object[] param)
        {
            _timeLimit = (int)param[0];
            _orgTimeLimit = _timeLimit;

            InitAnim();
            UpdateTime();
        }

        public override void Start()
        {
            PlayAnim();
        }

        public override void Update()
        {
            if (!_isStart)
                return;
            
            if(_isPause)
                return;
            
            _currentTime += Time.deltaTime;
            UpdateSlider();
            if (_currentTime < 1)
                return;

            _currentTime -= 1;
            _timeLimit -= 1;

            UpdateTime();

            if (_timeLimit > 0)
                return;
            
            _isStart = false;
            TileMatchEventManager.Instance.SendEventImmediately(GameEventConst.GameEvent_Fail,FailTypeEnum.Time);
        }

        public override void Destroy()
        {
            _isStart = false;
        }

        public override void Pause()
        {
            _isPause = true;
        }

        public override void Recover()
        {
            _isPause = false;
        }

        private void UpdateTime()
        {
            _timeText.SetText(TimeUtils.GetTimeMinutesString(_timeLimit));

            string imageName = _sliderImageName[0];
            if (_timeLimit < 15)
                imageName = _sliderImageName[1];

            UpdateTileImage(imageName);
        }

        private void UpdateSlider()
        {
            _timeSlider.value = 1.0f * (_timeLimit-_currentTime) / _orgTimeLimit;
        }

        private void UpdateTileImage(string image)
        {
            if(_timeBgImage.sprite.name == image)
                return;

            _spine.AnimationState.SetAnimation(0, _timeLimit < 15 ? "Countdown" : "Keep", true);
            _spine.AnimationState.Update(0);
            
            _timeBgImage.sprite = ResourcesManager.Instance.GetSpriteVariant("SpriteAtlas/TileMatchAtlas", image);
            _timeBgImage.sprite.name = image;
        }
        public override int GetTime()
        {
            return _orgTimeLimit;
        }
        public override int GetLeftTime()
        {
            return _timeLimit;
        }
        public override void AddTime(int time)
        {
            _timeLimit += time;
            _currentTime = 0;
            _isStart = true;
            UpdateTime();
        }

        public override void StartShuffle()
        {
            Hide();
        }

        public override void StopShuffle()
        {
            Show();
        }        

        public override bool IsLock(Block.Block block)
        {
            return false;
        }
    }
}