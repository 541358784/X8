using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DragonPlus.Config.JungleAdventure;
using JungleAdventure;
using UnityEngine;

namespace Activity.JungleAdventure.Controller
{
    public partial class UIJungleAdventureMainController
    {
        private PathMap _pathMap;
        private bool _isMoving = false;
        
        private void Awake_Path()
        {
            _pathMap = transform.Find("Root/BGGroup/Path").GetComponent<PathMap>();
        }

        private Vector3 GetLocalPositionByStage(ref int index)
        {
            int stage = JungleAdventureModel.Instance.JungleAdventure.Stage;
            int currentScore = JungleAdventureModel.Instance.JungleAdventure.AnimScore;

            var config = JungleAdventureConfigManager.Instance.GetConfigByStage(stage);

            List<Vector2> pathPoint = GetPathPoint(stage);
            float length = GetPathLength(stage);

            return GetLocalPosition(config.Score, currentScore, pathPoint, length, ref index);
        }

        private List<Vector2> GetPathPoint(int stage)
        {
            if (_pathMap._points.Count <= stage)
                return _pathMap._points.Last().points;
            
            return _pathMap._points[stage].points;
        }

        private float GetPathLength(int stage)
        {
            if (_pathMap._pathLength.Count <= stage)
                return _pathMap._pathLength.Last();

            return _pathMap._pathLength[stage];
        }

        private Vector2 GetLocalPosition(int totalScore, int currentScore, List<Vector2> point, float length, ref int index)
        {
            if (currentScore == 0)
                return point.First();

            float ratio = 1.0f * currentScore / totalScore;

            return GetLocalPositionByLength(point, length*ratio, ref index);
        }

        private Vector3 GetLocalPositionByLength(List<Vector2> point, float length, ref int index)
        {
            index = 0;
            if (length == 0f)
                return point.First();
            
            float distance = 0;
            
            Vector2 startPos = point.First();
            for (int i = 1; i < point.Count; i++)
            {
                distance += Vector3.Distance(startPos, point[i]);

                index = i;
                if (distance >= length)
                    return point[i];
                
                startPos = point[i];
            }

            index = point.Count-1;
            return point.Last();
        }

        private async UniTask MovePath(int index, int stage, int totalScore, int currentScore, CancellationToken cancellationToken)
        {
            List<Vector2> pathPoint = GetPathPoint(stage);
            float length = GetPathLength(stage);
            
            float ratio = 1.0f * currentScore / totalScore; 

            int newIndex = 0;
            GetLocalPositionByLength(pathPoint, length*ratio, ref newIndex);

            PlayAnimation("huachuan");

            if (newIndex < index)
            {
                _isMoving = false;
                return;
            }
            
            while (index != newIndex)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _isMoving = false;
                    return;
                }

                if (index >= pathPoint.Count)
                {
                    _isMoving = false;
                    return;
                }
                
                _isMoving = true;
                
                var position = pathPoint[index++];
                _entity.transform.localPosition = position;
                
                if (index >= pathPoint.Count)
                {
                    _isMoving = false;
                    return;
                }
                
                LimitCameraPosition(position);
                UpdateEntityRotation(index, pathPoint, false);
                UpdateReward(stage, index, pathPoint.Count);
                RefreshSlider(1.0f*index/(pathPoint.Count-1));
                
                _currentPathIndex = index;
                await UniTask.WaitForFixedUpdate();
            }

            _isMoving = false;
        }
    }
}