using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DragonPlus.Config.JungleAdventure;
using Unity.Mathematics;
using UnityEngine;

namespace Activity.JungleAdventure.Controller
{
    public partial class UIJungleAdventureMainController
    {
        private Transform _entity;
        private Transform _entityShip;
        
        private int _currentPathIndex = 0;
        
        private float _currentYRotation = 0f; // 添加一个变量跟踪当前Y轴旋转角度

        private Transform _entityPlayer;
        private Transform _entityDog;
        private Animator _playerAnimator;
        private Animator _dogAnimator;

        private Vector3 _playerFinishPosition = new Vector3(0.75537f,85.4169f,-31.37f);
        private Vector3 _playerFinishRotate = new Vector3(9.644f,202.81f,4.30f);
        private Vector3 _playerFinishScale = new Vector3(2.5f, 2.5f, 1);


        private Vector3 _dogFinishPosition = new Vector3(-0.0121f,85.4229f,-31.378f);
        private Vector3 _dogFinishRotate = new Vector3(10.55f,177.968f,359.876f);
        private Vector3 _dogFinishScale = new Vector3(2.5f, 2.5f, 1);
        
        
            
        private void Awake_Entity()
        {
            _entity = transform.Find("Root/BGGroup/Ship");
            _entityShip = transform.Find("Root/BGGroup/Ship/chuan");
            _entityPlayer = transform.Find("Root/BGGroup/Ship/nvzhu@skin");
            _entityDog = transform.Find("Root/BGGroup/Ship/dog@skin");
            
            _playerAnimator = _entityPlayer.GetComponent<Animator>();
            _dogAnimator = _entityDog.GetComponent<Animator>();
        }

        private void InitEntityPosition()
        {
            Vector2 localPosition = GetLocalPositionByStage(ref _currentPathIndex);

            List<Vector2> pathPoint = GetPathPoint(JungleAdventureModel.Instance.JungleAdventure.Stage);
            
            _entity.transform.localPosition = localPosition;
            UpdateEntityRotation(_currentPathIndex, pathPoint, true);
            LimitCameraPosition(localPosition);
        }

        private async UniTaskVoid SetEntityFinishStatus(bool isEnd)
        {
            if (JungleAdventureModel.Instance.JungleAdventure.Stage < JungleAdventureConfigManager.Instance.GetConfigs().Count)
                return;

            _entityPlayer.transform.parent = _entity.transform.parent;
            _entityPlayer.transform.localPosition = _playerFinishPosition;
            _entityPlayer.transform.rotation = Quaternion.Euler(_playerFinishRotate);
            _entityPlayer.transform.localScale = _playerFinishScale;
            _playerAnimator.Play("hello", 0, 0);
            
            
            _entityDog.transform.parent = _entity.transform.parent;
            _entityDog.transform.localPosition = _dogFinishPosition;
            _entityDog.transform.rotation = Quaternion.Euler(_dogFinishRotate);
            _entityDog.transform.localScale = _dogFinishScale;
            _dogAnimator.Play("idle", 0, 0);

            if (isEnd)
            {
                await UniTask.WaitForSeconds(0.1f);

                var startPosition = _camera.transform.localPosition;
                var endPosition = _entityPlayer.transform.localPosition;
                endPosition.y += 1;
                await DOTween.To(() => startPosition, x => startPosition = x, endPosition, 2f).SetEase(Ease.Linear).OnUpdate(() =>
                {
                    LimitCameraPosition(startPosition);
                });
            }
            else
            {
                var endPosition = _entityPlayer.transform.localPosition;
                endPosition.y += 1;
                LimitCameraPosition(endPosition);
            }
        }
        
        
        private async UniTask MoveEntity(bool isFirst, CancellationToken cancellationToken)
        {
            if (JungleAdventureModel.Instance.JungleAdventure.AnimScore == JungleAdventureModel.Instance.JungleAdventure.CurrentScore)
            {
                if(JungleAdventureModel.Instance.IsActivityTimeEnd())
                    _playButton.gameObject.SetActive(true);
                
                PlayAnimation("huachuanidle", true);
                SetEntityFinishStatus(false);
                return;
            }

            if (JungleAdventureModel.Instance.JungleAdventure.Stage >= JungleAdventureConfigManager.Instance.GetConfigs().Count)
            {
                if(JungleAdventureModel.Instance.IsActivityTimeEnd())
                    _playButton.gameObject.SetActive(true);
                
                PlayAnimation("huachuanidle", true);
                SetEntityFinishStatus(false);
                return;
            }

            if (isFirst)
                await UniTask.WaitForSeconds(0.5f);

            if (cancellationToken.IsCancellationRequested)
               return;
            
            int stage = JungleAdventureModel.Instance.JungleAdventure.Stage;
            int currentScore = JungleAdventureModel.Instance.JungleAdventure.CurrentScore;

            var config = JungleAdventureConfigManager.Instance.GetConfigByStage(stage);
            currentScore = Math.Min(config.Score, JungleAdventureModel.Instance.JungleAdventure.CurrentScore);
            
            await MovePath(_currentPathIndex, stage, config.Score, currentScore, cancellationToken);
            
            JungleAdventureModel.Instance.JungleAdventure.AnimScore = currentScore;
            InitSlider();
            
            if (cancellationToken.IsCancellationRequested)
               return;
            
            if (JungleAdventureModel.Instance.JungleAdventure.CurrentScore >= config.Score)
            {
                JungleAdventureModel.Instance.JungleAdventure.CurrentScore -= config.Score;
                JungleAdventureModel.Instance.JungleAdventure.Stage += 1;
                RefreshSlider(0f);
                _currentPathIndex = 0;
                JungleAdventureModel.Instance.JungleAdventure.AnimScore = 0;
                InitSlider();
                if (JungleAdventureModel.Instance.JungleAdventure.Stage >= JungleAdventureConfigManager.Instance.GetConfigs().Count)
                {
                    JungleAdventureModel.Instance.JungleAdventure.AnimScore = currentScore;
                    RefreshSlider(1f);
                    InitSlider();
                    RefreshReward();
                    PlayAnimation("huachuanidle");

                    SetEntityFinishStatus(true);
                    return;
                }
            }

            await MoveEntity(false, cancellationToken);
        }
        
        private void UpdateEntityRotation(int index, List<Vector2> point, bool isInit)
        {
            var currentPosition = point[index];
            Vector2 direction = Vector2.zero;
            if (index == 0)
            {
                var nextPosition = point[index + 1];
                direction = nextPosition-currentPosition;
            }
            else
            {
                var nextPosition = point[index - 1];
                direction = currentPosition-nextPosition;
            }

            // 根据移动方向计算目标Y轴旋转
            float targetYRotation = 0;
            if (direction.x > 0) // 向右移动
            {
                targetYRotation = -15f;
            }
            else if (direction.x < 0) // 向左移动
            {
                targetYRotation = 15f;
            }

            // 创建前向旋转
            Quaternion forwardRotation = Quaternion.LookRotation(Vector3.forward, direction);
            
            if (isInit)
            {
                _entity.transform.rotation = forwardRotation;
                _entityShip.transform.localRotation = Quaternion.Euler(_entityShip.transform.localEulerAngles.x, targetYRotation, _entityShip.transform.localEulerAngles.z);
                _currentYRotation = targetYRotation;
            }
            else
            {
                // 平滑插值到目标角度
                _currentYRotation = Mathf.Lerp(_currentYRotation, targetYRotation, Time.deltaTime/1f);
                _entityShip.transform.localRotation = Quaternion.Euler(_entityShip.transform.localEulerAngles.x, _currentYRotation, _entityShip.transform.localEulerAngles.z);
                _entity.transform.rotation = Quaternion.Lerp(_entity.transform.rotation, forwardRotation, Time.deltaTime/1f);
            }
        }

        private void PlayAnimation(string animName, bool isNormal = false)
        {
            if (isNormal)
            {
                _playerAnimator.Play(animName, 0, 0);
            }
            else
            {
                _playerAnimator.Play(animName);
            }
        }
    }
}