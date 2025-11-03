using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Activity.JungleAdventure.Controller
{
    public partial class UIJungleAdventureMainController : IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Camera _camera;
        private Vector3 _minPosition;
        private Vector3 _maxPosition;
        private RawImage _dragArea;
        
        // 拖拽相关变量
        private bool _isDragging;
        private Vector2 _dragStartPosition;
        private Vector2 _latDragPosition;
        private Vector2 _currentVelocity;
        private float _smoothTime = 0.2f;
        private float _dragSpeed = 1.0f;
        private float _minVelocity = 0.01f;
        private bool _isFirstDrag = true;
        private float _sensitive = 80f;
        private float _lastDragTime; // 添加最后一次拖拽的时间记录
        private const float MAX_INERTIA_INTERVAL = 0.1f; // 计算惯性的最大时间间隔
        
        const float referenceWidth = 768f;
        const float referenceHeight = 1720f;
        float referenceOrthographicSize = 0.049f;
        private float referenceAspectRatio = referenceWidth / referenceHeight;
        private void Awake_Camera()
        {
            _camera = transform.Find("Root/Camera").GetComponent<Camera>();
            _minPosition = transform.Find("Root/BGGroup/minPosition").localPosition;
            _maxPosition = transform.Find("Root/BGGroup/maxPosition").localPosition;
            _dragArea = transform.Find("Root/RawImage").GetComponent<RawImage>();
            
            _camera.orthographicSize = ((float)Screen.width / (float)Screen.height) * referenceOrthographicSize / referenceAspectRatio;
        }

        private void UpdateCamera()
        {
            if(_isMoving)
                return;
            
            ApplyInertia();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(_isMoving)
                return;
            
            // 检查是否点击在RawImage上
            if (eventData.pointerCurrentRaycast.gameObject != _dragArea.gameObject)
                return;

            _isDragging = true;
            _dragStartPosition = eventData.position;
            _latDragPosition = _dragStartPosition;
            _lastDragTime = Time.time;
            _currentVelocity = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;

            // 记录当前位置之前的位置，用于计算惯性
            _latDragPosition = _dragStartPosition;
            _lastDragTime = Time.time;
            
            Vector2 currentPos = eventData.position;
            ProcessDragDelta(currentPos);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;

            // 检查时间间隔
            float timeSinceLastDrag = Time.time - _lastDragTime;
            if (timeSinceLastDrag > MAX_INERTIA_INTERVAL)
            {
                // 如果间隔太长，不产生惯性
                _currentVelocity = Vector2.zero;
                return;
            }

            // 计算最终位置与上一次位置的距离
            Vector2 finalDelta = eventData.position - _latDragPosition;
            
            // 如果移动距离太小，不产生惯性效果
            if (finalDelta.magnitude < 5f)
            {
                _currentVelocity = Vector2.zero;
                return;
            }

            _currentVelocity = finalDelta / timeSinceLastDrag;

            // 限制最大速度
            float maxVelocity = 800f;
            _currentVelocity = Vector2.ClampMagnitude(_currentVelocity, maxVelocity);

            // 如果速度太小，直接设为零
            if (_currentVelocity.magnitude < _minVelocity * 10)
            {
                _currentVelocity = Vector2.zero;
            }
        }

        private void ProcessDragDelta(Vector2 currentPos)
        {
            // 使用当前位置减去起始位置计算移动距离
            Vector2 worldStart = _camera.ScreenToWorldPoint(_dragStartPosition);
            Vector2 worldCurrent = _camera.ScreenToWorldPoint(currentPos);
            Vector2 worldDelta = worldCurrent - worldStart;
            worldDelta *= -_sensitive;
            
            Vector3 newPosition = _camera.transform.localPosition + (Vector3)worldDelta;
            
            // 更新位置
            LimitCameraPosition(newPosition);
            
            // 更新起始位置为当前位置，为下一次移动做准备
            _dragStartPosition = currentPos;
        }

        private void ApplyInertia()
        {
            if (!_isDragging && _currentVelocity.magnitude > _minVelocity)
            {
                // 应用惯性
                Vector2 delta = _currentVelocity * Time.deltaTime;
                Vector3 newPosition = _camera.transform.localPosition - (Vector3)delta/20f;
                
                // 平滑减速，增加减速率
                _currentVelocity = Vector2.Lerp(_currentVelocity, Vector2.zero, Time.deltaTime / _smoothTime);
                
                // 限制相机位置
                LimitCameraPosition(newPosition);
            }
        }

        private void LimitCameraPosition(Vector2 localPosition)
        {
            _camera.transform.localPosition = LimitPosition(localPosition);
        }

        private Vector3 LimitPosition(Vector2 localPosition)
        {
            float localHalfHeight = _camera.orthographicSize / transform.lossyScale.y;
            float clampedY = Mathf.Clamp(localPosition.y, _minPosition.y + localHalfHeight, _maxPosition.y - localHalfHeight);
            Vector3 newPosition = new Vector3(_camera.transform.localPosition.x, clampedY, _camera.transform.localPosition.z);

            return newPosition;
        }
    }
}