using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Deco.World;
using DragonPlus;
using Screw.GameLogic;

namespace Decoration{

public class PinchMapComponent : MonoBehaviour
{
    public Transform MinPosition;
    public Transform MaxPosition;
    public Transform InitPosition;
    public Transform FinishPosition;
    
    private  float MinCameraSize = 14;
    private  float MaxCameraSize = 20;
    private  float OriginCameraSize = 16;


    private float _minCameraScale;
    private float _maxCameraScale;
    
    public float _reboundSpeed = 3.0f;

    private Vector2 _minCameraPosition;
    private Vector2 _maxCameraPosition;
    private Vector3 _screenMinPosition;
    private Vector3 _screenMaxPosition;

    private Touch _startTouch_1;
    private Touch _startTouch_2;

    private Vector2 _prevMousePos;
    private Vector2 _moveSpeed;
    private Vector2 _autoTargetPos;
    private float _autoScale;
    private bool _autoMoveFlag = false;
    private bool _autoMoveScaleFlag = false;
    private bool _autoScaleFlag = false;
    private bool _autoScalePositionFlag = false;
    private Action _completeCallback = null;

    private Vector3 _targetPosition;
    private float _targetScale;
    private Vector3 _originPosition;
    private float _originScale;
    private bool _focusEnable;
    public float _focusTime = 1.0f;
    private float _focusEscapeTime = 0.0f;

    private DecoWorld _world = null;

    //private DecorationMainController _worldMainUI;

    public float CurrentCameraScale
    {
        get { return DecoSceneRoot.Instance.mSceneCamera.orthographicSize / OriginCameraSize; }
    }

    public float OriginScale
    {
        get { return _originScale; }
    }

    public DecoWorld World
    {
        get => _world;
        set => _world = value;
    }

    private void Awake()
    {
        MinPosition = transform.Find("MinPosition");
        MaxPosition = transform.Find("MaxPosition");
        InitPosition = transform.Find("InitPosition");
        FinishPosition = transform.Find("FinishPosition");

        _minCameraPosition = MinPosition.position;
        _maxCameraPosition = MaxPosition.position;

        _minCameraScale = MinCameraSize / OriginCameraSize;
        _maxCameraScale = MaxCameraSize / OriginCameraSize;
        
        var uiRootTransform = UIRoot.Instance.mRoot.transform as RectTransform;
        var uiCamera = UIRoot.Instance.mUICamera;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(uiRootTransform, new Vector2(0, 0), uiCamera, out _screenMinPosition);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(uiRootTransform, new Vector2(Screen.width, Screen.height), uiCamera, out _screenMaxPosition);


        MinCameraSize = Decoration.DecorationConfigManager.Instance.GetGlobalConfigNumber(GlobalNumberConfigKey.deco_MinCameraSize);
        MaxCameraSize =  Decoration.DecorationConfigManager.Instance.GetGlobalConfigNumber(GlobalNumberConfigKey.deco_MaxCameraSize);
        OriginCameraSize = Decoration.DecorationConfigManager.Instance.GetGlobalConfigNumber(GlobalNumberConfigKey.deco_OriginCameraSize);
    }

    public void SetMaxCameraSize(float size)
    {
        MaxCameraSize = size;
    }
    
    private void initMainUI()
    {
        //_worldMainUI = UIManager.Instance.GetOpenedWindow<DecorationMainController>();
    }

    //void FixedUpdate()
    void Update()
    {
        if (_focusEnable)
        {
            _focusEscapeTime += Time.deltaTime;
            var camera = DecoSceneRoot.Instance.mSceneCamera;
            if (_focusEscapeTime >= _focusTime)
            {
                camera.orthographicSize = OriginCameraSize * _targetScale;
                camera.transform.position = _targetPosition;

                _focusEnable = false;
                _moveSpeed = Vector2.zero;
                InvokeCompleteCallback();
            }
            else
            {
                var percent = _focusEscapeTime / _focusTime;
                var value = AnimationConfig.curve.Evaluate(percent);
                camera.orthographicSize = OriginCameraSize * calculateValue(_originScale, _targetScale, value);
                camera.transform.position = new Vector3(calculateValue(_originPosition.x, _targetPosition.x, value), calculateValue(_originPosition.y, _targetPosition.y, value), _targetPosition.z);
            }

            return;
        }

        if(GuideSubSystem.Instance.IsShowingGuide()){
            return;
        }

        if (StoryMovieSubSystem.Instance.IsShowing)
            return;
        
        if(MainDecorationController.mainController==null || MainDecorationController.mainController.Status == DecoUIStatus.Decoration)
            return;
        
        if (_world.IsPreviewMode) 
            return;

        if (ScrewGameLogic.Instance.context != null)
            return;
        
        if( SceneFsm.mInstance.GetCurrSceneType()!=StatusType.Home && SceneFsm.mInstance.GetCurrSceneType() != StatusType.BackHome && SceneFsm.mInstance.GetCurrSceneType() != StatusType.EnterFarm)
            return;
        
        if (CommonUtils.IsTouchUGUI())
          return;
        
        if(!DecoManager.Instance.EnableUpdate) 
            return;
        
        if(!UIRoot.Instance.EnableEventSystem)
            return;
        
        if (Application.isEditor)
        {
            var scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");

            if (Input.GetMouseButtonDown(0))
            {
                _prevMousePos = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                ClearFlags();
                var currentMousePos = Input.mousePosition;
                // var curScale = DecoSceneRoot.Instance.mSceneCamera.orthographicSize / _originCameraSize;

                var worldOffset = DecoSceneRoot.Instance.mSceneCamera.ScreenToWorldPoint(currentMousePos) - DecoSceneRoot.Instance.mSceneCamera.ScreenToWorldPoint(_prevMousePos);
                
                _moveSpeed = worldOffset / Time.deltaTime;

                move(_moveSpeed);

                _prevMousePos = currentMousePos;
            }
            else
            {
                _moveSpeed *= Mathf.Pow(0.01f, Time.deltaTime);
                if (Mathf.Abs(Vector3.Magnitude(_moveSpeed)) < 1)
                {
                    _moveSpeed = Vector3.zero;
                }

                move(_moveSpeed);
            }

            if (Mathf.Abs(scrollWheelInput) > float.Epsilon)
            {
                ClearFlags();

                var curSize = DecoSceneRoot.Instance.mSceneCamera.orthographicSize;
                curSize /= 1 + scrollWheelInput;
                touchScale(curSize);
            }
        }
        else
        {
            if (Input.touchCount == 2)
            {
                ClearFlags();
                var touch0 = Input.GetTouch(0);
                var touch1 = Input.GetTouch(1);
                if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
                {
                    _startTouch_1 = touch0;
                    _startTouch_2 = touch1;
                }
                else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
                {
                    var currentDist = Vector2.Distance(touch0.position, touch1.position);
                    var prevDist = Vector2.Distance(touch0.position - touch0.deltaPosition, touch1.position - touch1.deltaPosition);

                    var curSize = DecoSceneRoot.Instance.mSceneCamera.orthographicSize;
                    curSize /= currentDist / prevDist;
                    touchScale(curSize);
                }
            }
            else if (Input.touchCount == 1)
            {
                ClearFlags();

                _startTouch_1 = Input.GetTouch(0);
                if (_startTouch_1.phase == TouchPhase.Moved)
                {
                    var currentMousePos = _startTouch_1.position;
                    var prePos = _startTouch_1.position - _startTouch_1.deltaPosition;
                    var worldOffset = DecoSceneRoot.Instance.mSceneCamera.ScreenToWorldPoint(currentMousePos) - DecoSceneRoot.Instance.mSceneCamera.ScreenToWorldPoint(prePos);
                    _moveSpeed = worldOffset / Time.deltaTime;
                    move(_moveSpeed);
                }
            }
            else
            {
                _moveSpeed *= Mathf.Pow(0.0005f, Time.deltaTime);
                if (Mathf.Abs(Vector3.Magnitude(_moveSpeed)) < 1)
                {
                    _moveSpeed = Vector3.zero;
                }

                move(_moveSpeed);
            }

            // else // 检测回弹
            // {
            //     CheckScaleRebound();
            // }
        }

    }

    private void move(Vector3 speed)
    {
        //DragonU3DSDK.DebugUtil.Log($"move-->speed={speed}");
        //if (GuideSubSystem.Instance.IsShowingGuide()) return;
        if (speed == Vector3.zero) return;

        var currentPos = DecoSceneRoot.Instance.mSceneCamera.transform.position;

        var newPos = currentPos - speed * Time.deltaTime;
        var nextPos = Vector3.Lerp(currentPos, newPos, Time.deltaTime * 50f);

        var curScale = DecoSceneRoot.Instance.mSceneCamera.orthographicSize / OriginCameraSize;
        var halfScreenSize = (_screenMaxPosition - _screenMinPosition) / 2 * curScale;

        var xMin = _minCameraPosition.x + halfScreenSize.x;
        var xMax = _maxCameraPosition.x - halfScreenSize.x;
        var yMin = _minCameraPosition.y + halfScreenSize.y;
        var yMax = _maxCameraPosition.y - halfScreenSize.y;

        if (nextPos.x < xMin) nextPos.x = xMin;
        if (nextPos.x > xMax) nextPos.x = xMax;
        if (nextPos.y < yMin) nextPos.y = yMin;
        if (nextPos.y > yMax) nextPos.y = yMax;

        DecoSceneRoot.Instance.mSceneCamera.transform.position = nextPos;
    }

    private void touchScale(float curSize)
    {
        if (GuideSubSystem.Instance.IsShowingGuide())
            return;
        
        if (DecoSceneRoot.Instance.mSceneCamera.orthographicSize == curSize)
            return;

        curSize = Mathf.Clamp(curSize, MinCameraSize, MaxCameraSize);
        DecoSceneRoot.Instance.mSceneCamera.orthographicSize = curSize;

        BoundLimit();
    }

    private void CheckScaleRebound()
    {
        var minSize = MinCameraSize;
        var maxSize = MaxCameraSize;
        var curSize = DecoSceneRoot.Instance.mSceneCamera.orthographicSize;
        if (curSize < minSize)
        {
            curSize = Mathf.Lerp(curSize, minSize, _reboundSpeed * Time.deltaTime);
            DecoSceneRoot.Instance.mSceneCamera.orthographicSize = curSize;
        }
        else if (curSize > maxSize)
        {
            curSize = Mathf.Lerp(curSize, maxSize, _reboundSpeed * Time.deltaTime);
            DecoSceneRoot.Instance.mSceneCamera.orthographicSize = curSize;
        }
    }

    // 通过公式计算出差值
    private float calculateValue(float origin, float target, float percent)
    {
        var diff = target - origin;
        return origin + diff * percent;
    }

    private void ClearFlags()
    {
        _autoMoveFlag = false;
        _autoMoveScaleFlag = false;
        _autoScaleFlag = false;
        _autoScalePositionFlag = false;
    }

    private void BoundLimit()
    {
        var curScale = DecoSceneRoot.Instance.mSceneCamera.orthographicSize / OriginCameraSize;

        var halfScreenSize = (_screenMaxPosition - _screenMinPosition) / 2 * curScale;
        var position = DecoSceneRoot.Instance.mSceneCamera.transform.position;

        position.x = Mathf.Clamp(position.x, _minCameraPosition.x + halfScreenSize.x, _maxCameraPosition.x - halfScreenSize.x);
        position.y = Mathf.Clamp(position.y, _minCameraPosition.y + halfScreenSize.y, _maxCameraPosition.y - halfScreenSize.y);
        DecoSceneRoot.Instance.mSceneCamera.transform.position = position;
    }

    private Vector3 BoundaryPosition(Vector3 originPosition, float scale)
    {
        var halfScreenSize = (_screenMaxPosition - _screenMinPosition) / 2 * scale;
        var position = new Vector3(originPosition.x, originPosition.y, originPosition.z);

        position.x = Mathf.Clamp(position.x, _minCameraPosition.x + halfScreenSize.x, _maxCameraPosition.x - halfScreenSize.x);
        position.y = Mathf.Clamp(position.y, _minCameraPosition.y + halfScreenSize.y, _maxCameraPosition.y - halfScreenSize.y);

        return position;
    }

    public void FocusPosition(Vector2 position, float scale, float focusTime = 1f, Action onFinish = null)
    {
        var camera = DecoSceneRoot.Instance.mSceneCamera;
        _originPosition = camera.transform.position;

        _originScale = this.CurrentCameraScale;
        _focusTime = focusTime;

        //_targetScale = Mathf.Clamp(scale, _minCameraScale, _maxCameraScale);
        _targetScale = scale;
        _targetPosition = BoundaryPosition(new Vector3(position.x, position.y, camera.transform.position.z), _targetScale);

        _focusEnable = true;
        _completeCallback = onFinish;
        _focusEscapeTime = 0;
    }

    public void UpdateTargetPosition(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    private void InvokeCompleteCallback()
    {
        var cb = _completeCallback;
        _completeCallback = null;

        cb?.Invoke();
    }

    public void FocusTargetPosition(Vector3 position, float scale = 1.0f, bool check_boundary = false)
    {
        _originScale = this.CurrentCameraScale;
        
        var camera = DecoSceneRoot.Instance.mSceneCamera;
        //scale = Mathf.Clamp(scale, _minCameraScale, _maxCameraScale);
        camera.orthographicSize = OriginCameraSize * scale;

        position.z = camera.transform.position.z;
        camera.transform.position = position;

        if (check_boundary)
            BoundLimit();
        
        _originPosition = camera.transform.position;

        _focusTime = 0;

        _targetScale = scale;
        _targetPosition = BoundaryPosition(new Vector3(position.x, position.y, camera.transform.position.z), _targetScale);

        _focusEnable = false;
        _focusEscapeTime = 0;
    }

    public void Reset()
    {
        _prevMousePos = Vector2.zero;
        _moveSpeed = Vector2.zero;
        _autoTargetPos = Vector2.zero;
        _autoScale = 0;
        _autoMoveFlag = false;
        _autoMoveScaleFlag = false;
        _autoScaleFlag = false;
        _autoScalePositionFlag = false;
        _targetPosition = Vector3.zero;
        _targetScale = 0f;
        _originPosition = Vector3.zero;
        _originScale = 0f;
        _focusEnable = false;
        _focusTime = 1.0f;
        _focusEscapeTime = 0.0f;

        DecoSceneRoot.Instance.mSceneCamera.transform.position = Vector3.zero;
        DecoSceneRoot.Instance.mSceneCamera.orthographicSize = DecorationConfigManager.Instance.GetGlobalConfigNumber("WolrdSceneCameraSize",500)*0.01f;; //_originCameraSize;
    }
    
    public void FocusDefaultCameraSize(Action onFinish)
    {
        FocusPosition(_targetPosition, _originScale, onFinish:onFinish);
    }
    
    public float GetRelativeCameraScale()
    {
        float curSize = DecoSceneRoot.Instance.mSceneCamera.orthographicSize - OriginCameraSize;
        float relativeSize = 0;
        
        if (curSize >= 0)
            relativeSize = MaxCameraSize - OriginCameraSize;
        else
            relativeSize = OriginCameraSize - MinCameraSize;
       
        return curSize / relativeSize;
    }
}
    
}