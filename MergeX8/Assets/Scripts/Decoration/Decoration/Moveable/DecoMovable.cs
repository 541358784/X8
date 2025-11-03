using System;
using System.Collections.Generic;
using System.IO;
using Deco.Graphic;
using DragonU3DSDK;
using SomeWhere;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Deco.Moveable
{
    public abstract class DecoMovable<T> : DecoGraphicHost<T> where T : DecoMovableGraphic
    {
        public enum LiveStatus
        {
            In,
            Living,
            Out,
        }

        static protected int s_instanceId;

        protected PathSegment _currentSegment;
        protected PathMap _pathMap;
        protected List<PathSegment> _segmentList;
        protected bool _reverseDirection;
        protected float _currentJurney;
        protected LiveStatus _liveStatus;
        protected int _instanceId;
        protected float _moveSpeed = 1f;

        public bool IsLiving => _liveStatus == LiveStatus.Living;
        public abstract void Update(float deltaTime);

        public int InstancceId => _instanceId;

        protected void updateMoving(float deltaTime)
        {
            if (_currentSegment == null || _currentSegment.p1 == null || _currentSegment.p2 == null) return;

            var nextPos = _pathMap.GetBezierPointWithDistance(_currentSegment, Graphic.Position, ref _currentJurney, deltaTime / 4f * _moveSpeed, _reverseDirection);
            var isFront = Vector2.Dot(Graphic.Up, nextPos - Graphic.Position) > 0;
            var isLeft = Vector2.Dot(Graphic.Front, nextPos - Graphic.Position) < 0;

            Graphic.SetPosition(nextPos);

            //前后方向变动后需要切换不同动画,左右动画依赖XScale=-1调整,无需切换动画
            if (Graphic.SetDirection(isLeft, isFront))
            {
                Graphic.PlayMoving();
            }

            // if (_currentJurney >= 1f)
            // {
            //     var current = _currentSegment.GetEnd(_reverseDirection);
            //     var randomWeight = Random.Range(0, 100);
            //     if (current.StayAvailable())
            //     {
            //         ReachTargetPoint(current);
            //         performPointAction(current.pointType);
            //     }
            //     else
            //     {
            //         if (!moveNext())
            //         {
            //             onMoveNextFailed();
            //         }
            //     }
            // }
        }

        protected abstract void ReachTargetPoint(PathPoint point);
        protected abstract void OnMoveToNewSeg();
        protected abstract void OnLeaveSeg(PathSegment segment);

        protected virtual void onMoveNextFailed()
        {
        }

        protected bool moveNext(bool exceptionRequiredForce = false)
        {
            var beginPoint = _currentSegment.GetEnd(_reverseDirection);
            var exceptPoint = _currentSegment.GetBegin(_reverseDirection);

            var preSegment = _currentSegment;
            if (selectNextSegment(beginPoint, exceptPoint, exceptionRequiredForce))
            {
                _currentJurney = 0;
                _currentSegment.GetEnd(_reverseDirection).Order();
                OnLeaveSeg(preSegment);
                OnMoveToNewSeg();


                return true;
            }

            return false;
        }
        List<PathSegment> alternativeList=new List<PathSegment>();
        protected bool selectNextSegment(PathPoint beginPoint, PathPoint exceptPoint,
            bool exceptionRequiredForce = false)
        {
            try
            {
                alternativeList =
                    _segmentList.FindAll(segment => movableTest(segment, beginPoint, exceptPoint));
                if (alternativeList != null && alternativeList.Count > 0)
                {
                    _currentSegment = alternativeList[Random.Range(0, alternativeList.Count)];
                    _reverseDirection = beginPoint == _currentSegment.p2;

                    return true;
                }
                else
                {
                    if (!exceptionRequiredForce)
                    {
                        alternativeList =
                            _segmentList.FindAll(segment => movableTest(segment, beginPoint, null));
                        if (alternativeList != null && alternativeList.Count > 0)
                        {
                            _currentSegment = alternativeList[Random.Range(0, alternativeList.Count)];
                            _reverseDirection = beginPoint == _currentSegment.p2;

                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e.ToString());
            }

            return false;
        }

        protected abstract bool movableTest(PathSegment segment, PathPoint beginPoint, PathPoint exceptPoint);
    }
}