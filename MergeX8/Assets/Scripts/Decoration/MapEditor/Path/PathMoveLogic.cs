using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace SomeWhere
{
    public partial class PathMoveLogic : MonoBehaviour
    {
        public string _pathId;
        public bool _is3DWorld;
        
        private RT_PathInfo _pathInfo;
        private int _segmentIndex;
        private int _pointIndex;
        private RT_PathSegment _curSegment;

        private float _waitTime = 0;

        private Vector3 _oldPosition = Vector3.zero;
        private Vector3 _newPosition = Vector3.zero;
        
        private SkeletonAnimation _skeletonAnimation = null;
        private Animator _animator;

        private float _localZ = 0;
        private void Awake()
        {
            InitPath();

            _localZ = transform.position.z;
        }

        private void InitPath()
        {
            _pathInfo = PathMapConfigManager.Instance.GetPathInfo(_pathId);
            _pointIndex = 0;
            _segmentIndex = 0;
            _curSegment = GetPathSegment(_segmentIndex);

            InitSkeleton();
            InitAnimator();
            
            if(_curSegment == null)
                return;
            
            var oldPoint = GetPoint(_curSegment, _pointIndex);
            transform.position = RtToVector3(ref _oldPosition, oldPoint);
            PlayAnimation(_curSegment._defaultAnimName);
            SetRotate(_curSegment._rotateAngle);
        }

        private void InitSkeleton()
        {
            _skeletonAnimation = transform.GetComponent<SkeletonAnimation>();
            if (_skeletonAnimation == null)
                _skeletonAnimation = transform.GetComponentInChildren<SkeletonAnimation>();
        }

        private void InitAnimator()
        {
            _animator = transform.GetComponent<Animator>();
            if (_animator == null)
                _animator = transform.GetComponentInChildren<Animator>();
        }
        
        private void Update()
        {
            if(_curSegment == null)
                return;

            if (IsWaitTime())
            {
                
                PlayAnimation(_curSegment._defaultAnimName);
                return;
            }

            if (_pointIndex == 0)
            {
                PlayAnimation(_curSegment._playAnimName);
                SetRotate(_curSegment._rotateAngle);
            }
             
             UpdatePoint();
             var oldPoint = GetPoint(_curSegment, _pointIndex++);
             
             UpdatePoint();
             var newPoint = GetPoint(_curSegment, _pointIndex);

             var curPos = transform.position;
             transform.position = RtToVector3(ref _oldPosition, oldPoint);
             _oldPosition.z = _localZ;
             transform.position = _oldPosition;

             RtToVector3(ref _newPosition, newPoint);
             _newPosition.z = _localZ;

             //
             
             // if (_is3DWorld)
             // {
             //     var direction = transform.position - curPos;
             //     Quaternion targetRotation = Quaternion.LookRotation(direction);
             //     transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime*5 );
             // }
             // else
             //     transform.rotation = Quaternion.Euler(0,0,oldPoint._angle);
             
             if (_curSegment._autoRotation)
             {
                 if (_is3DWorld)
                 {
                     var direction = _oldPosition - curPos;
                     Quaternion targetRotation = Quaternion.LookRotation(direction);
                     //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime*5 );
                     transform.rotation = targetRotation;
                 }
                 else
                 {
                     transform.rotation = Quaternion.Euler(0,0,oldPoint._angle);
                 }
             }
             else
             {
                 transform.rotation = Quaternion.Euler(Vector3.zero);
             }
        }

        private bool IsWaitTime()
        {
            if (_pointIndex != 0)
                return false;

            if (_curSegment._waitTime <= 0)
                return false;
            
            _waitTime += Time.deltaTime;
            
            if(_waitTime < _curSegment._waitTime)
                return true;

            _waitTime = 0;
            return false;
        }
        
        private RT_PathSegment GetPathSegment(int index)
        {
            if (_pathInfo == null || _pathInfo._segmentLists == null)
                return null;

            if (index < _pathInfo._segmentLists.Count)
                return _pathInfo._segmentLists[index];

            return null;
        }

        private RT_Point GetPoint(RT_PathSegment segment, int index)
        {
            if (segment == null || segment._points == null)
                return null;
            
            if (index < segment._points.Count)
                return segment._points[index];

            return null;
        }

        private bool IsSegmentEnd(List<RT_PathSegment> segments, int index)
        {
            return index < 0 || segments.Count <= index;
        }
        
        private bool IsPointEnd(List<RT_Point> points, int index)
        {
            return index < 0 || points.Count <= index;
        }

        private Vector3 RtToVector3(ref Vector3 pos, RT_Point rtPos)
        {
            pos.x = rtPos._x;
            pos.y = rtPos._y;
            pos.z = rtPos._z;

            return pos;
        }
        
        private Vector2 RtToVector3(ref Vector2 pos, RT_Point rtPos)
        {
            pos.x = rtPos._x;
            pos.y = rtPos._y;

            return pos;
        }

        private void UpdatePoint()
        {
            if (!IsPointEnd(_curSegment._points, _pointIndex))
                return;
            
            if (IsSegmentEnd(_pathInfo._segmentLists, ++_segmentIndex))
            {
                _segmentIndex = 0;
            }

            _pointIndex = 0;
            _curSegment = GetPathSegment(_segmentIndex);
        }

        private void PlayAnimation(string animName)
        {
            if(animName.IsEmptyString())
                return;
            
            if (_skeletonAnimation != null)
            {
                if(_skeletonAnimation.AnimationState?.GetCurrent(0)?.Animation?.Name == animName)
                    return;
                
                _skeletonAnimation.AnimationState?.SetAnimation(0, animName, true);
                _skeletonAnimation.Update(0);
            }

            if (_animator != null)
            {
                _animator.Play(animName);
            }
        }

        private void SetRotate(RT_Point rotate)
        {
            if(transform.childCount == 0)
                return;
            
            Vector3 rotateValue = Vector3.zero;
            RtToVector3(ref rotateValue, rotate);

            for(int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).localRotation = Quaternion.Euler(rotateValue);
        }
    }
}