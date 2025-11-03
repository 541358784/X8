using System;
using System.Collections.Generic;
using DragonU3DSDK;
using Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialMask : MonoBehaviour, ICanvasRaycastFilter
{
    public RectTransform[] _targets;

    private Material _material;

    // private float _diameter;
    private float _current = 0f;
    private float _yVelocity = 0f;

    private Vector2[] _targetScreenPoints;
    private float _radius;
    private float _width;
    private float _height;
    private Canvas _canvas;
    private Camera _camera;
    private Image _maskImage;
    private Vector3[] _worldCorners = new Vector3[4];
    private TableGuide _config;
    private readonly float MIN_RADIUS = 15f;
    private bool _isMainMerge = true;

    public Action<PointerEventData> OnMaskClick;


    private List<Transform> _topLayers = null;
    
    private double _updateTargetDelta;
    private static readonly int Radius = Shader.PropertyToID("_Radius");

    void Awake()
    {
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _camera = CameraManager.UICamera;
        _material = GetComponent<Image>().material;
        _maskImage = GetComponent<Image>();
    }

    public void SetConfig(TableGuide config, string targetParam)
    {
        _config = config;
        _updateTargetDelta = 1f;
        _radius = 0;
        _current = 0;
        _topLayers = null;
        _width = 0;
        _height = 0;
        
        if (_config == null)
        {
            DebugUtil.LogError("tutorial Mask setConfig config is null");
            return;
        }

        _targets = new RectTransform[1];

        for (int i = 0; i < _targets.Length; i++)
        {
            _targets[i] = GuideSubSystem.Instance.GetTarget((GuideTargetType) config.targetType, targetParam);
        }

        _topLayers = GuideSubSystem.Instance.GetTopLayers((GuideTargetType) config.targetType, targetParam);
        
        if (_targets.Length > 0 && _targets[0] != null)
        {
            _targets[0].GetWorldCorners(_worldCorners);
            _radius = Mathf.Max(Mathf.Max(_targets[0].rect.width, _targets[0].rect.height) / 2f, MIN_RADIUS);

            _width = _targets[0].rect.width;
            _height = _targets[0].rect.height;
            
            _radius *= 1.2f;
            if (config.radius > 0)
            {
                _radius *= config.radius;
                _width *= config.radius;
                _height *= config.radius;
            }

            updateCenterAndRange(true);
        }
        else
        {
            _material.SetFloat("_isRadius", 0);
            _material.SetVector("_Center", Vector4.zero);
            //_material.SetFloat("_Radius", 0f);
            _material.SetFloat("_Width", _width);
            _material.SetFloat("_Height", _height);
        }

        _maskImage.raycastTarget = _config.rayCast;
        var fullClear = _config.clearMask;
        if (fullClear)
        {
            _material.color = Color.clear;
        }
        else
        {
            _material.color = Color.white;
        }
    }

    private void updateCenterAndRange(bool animate)
    {
        if (_targets != null && _targets.Length > 0 && _targets[0] != null)
        {
            _targetScreenPoints = new Vector2[_targets.Length];

            for (int i = 0; i < _targetScreenPoints.Length; i++)
            {
                if (_targets[i] != null)
                {
                    _targets[i].GetWorldCorners(_worldCorners);
                    var x = (_worldCorners[0].x + _worldCorners[2].x) / 2;
                    var y = (_worldCorners[0].y + _worldCorners[2].y) / 2;
                    _targetScreenPoints[i] = _camera.WorldToScreenPoint(new Vector3(x, y, 0));
                }
            }


            var center = WordToCanvasPos(_canvas, _targetScreenPoints[0]);
            (_canvas.transform as RectTransform).GetWorldCorners(_worldCorners);
            _current = _radius * (animate ? 1.3f : 1f);

            if (_topLayers != null && _topLayers.Count > 0)
            {
                _material.SetVector("_Center", Vector2.zero);
                //_material.SetFloat("_Radius", 0);
                _material.SetFloat("_Width", 0);
                _material.SetFloat("_Height", 0);
            }
            else
            {
                _material.SetVector("_Center", center);
                //_material.SetFloat("_Radius", _current);
                if (_config.isRandiusMask)
                {
                    _material.SetFloat("_isRadius", 1);
                    _material.SetFloat("_Width", _radius);
                    _material.SetFloat("_Height", _radius); 
                }
                else
                {
                    _material.SetFloat("_isRadius", 0);
                    _material.SetFloat("_Width", _width);
                    _material.SetFloat("_Height", _height); 
                }
            }
        }
        else
        {
            if (_config.isRandiusMask)
            {
                _material.SetFloat("_isRadius", 1);
                _material.SetFloat("_Width", _radius);
                _material.SetFloat("_Height", _radius);
            }
            else
            {
                _material.SetFloat("_isRadius", 0);
                _material.SetVector("_Center", Vector2.zero);
                _material.SetFloat("_Width", _width);
                _material.SetFloat("_Height", _height);
            }
        }
    }

    void Update()
    {
        if (_topLayers != null && _topLayers.Count > 0)
            return;
        
        var value = Mathf.SmoothDamp(_current, _radius, ref _yVelocity, 0.3f);
        if (!Mathf.Approximately(value, _current))
        {
            _current = value;
            //_material.SetFloat(Radius, _current);
        }

        _updateTargetDelta -= Time.deltaTime;
        if (_updateTargetDelta <= 0)
        {
            _updateTargetDelta = 1f;
            updateCenterAndRange(false);
        }
    }

    Vector2 WordToCanvasPos(Canvas canvas, Vector3 screenPoint)
    {
        var position = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPoint, _camera,
            out position);
        return position;
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (_config == null) return true;
        if (_config.blockAll) return true;

        if (_targets != null && _targets.Length > 0 &&
            _targetScreenPoints != null && _targetScreenPoints.Length > 0)
        {
            foreach (var targetCenter in _targetScreenPoints)
            {
                var clickInCircle = Vector2.Distance(sp, targetCenter) < _radius;
                if (clickInCircle)
                {
                    //false标示没有点中遮罩，不响应点击，透过
                    return false;
                }
            }
        }

        return true;
    }

    public float GetRadius()
    {
        return  _radius;
    }
}