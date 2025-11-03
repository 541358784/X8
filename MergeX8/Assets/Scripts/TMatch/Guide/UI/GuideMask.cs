using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DragonPlus;

namespace OutsideGuide
{
    public class GuideMask : GuideGraphicBase//, IPointerDownHandler,IPointerUpHandler,IBeginDragHandler,IEndDragHandler,IDragHandler
    {
        public float smoothDuration = 0.2f;

        private GameObject selfGO;
        private Vector4 center;
        private Material material;
        public float currentRadius { get; set; }
        public Action<GuideTouchMaskEnum,EventTriggerType,BaseEventData,Transform> OnTouchMaskEvent { get; set; }

        private Dictionary<Transform, RectTransform> circleByObj = new Dictionary<Transform, RectTransform>();
        private Dictionary<Transform, Vector4> tracePos = new Dictionary<Transform, Vector4>();
        private Dictionary<Transform, float> targetRadius = new Dictionary<Transform, float>();

        private Canvas _canvas = null;
        public Canvas Canvas
        {
            get
            {
                if (_canvas == null) _canvas = GetComponentInParent<Canvas>();
                return _canvas;
            }
        }

        private Image m_Mask = null;
        private Material m_Material = null;

        private List<GameObject> m_Circles = new List<GameObject>();

        private Transform maskGroup;

        private Tweener maskTween = null;
        private bool m_IsTransparent = false;

        private List<Transform> targets;

        private float timer = 0;
        private List<bool> traces;
        private List<Vector3> offsetGroup;

        private Camera _gameCamera;
        private Camera _uiCamera;
        private float defaultRect = 150;
        private Tweener scaleTween;
        public Dictionary<Transform, GuideTarget> targetCanvas = new Dictionary<Transform, GuideTarget>();
        private bool isStartTrace;
        float yVelocity = 0f;

        private const int HOLE_SIZE = 4;
        private Vector4[] defaultPosArray = new Vector4[HOLE_SIZE];
        private float[] defaultRadius = new float[HOLE_SIZE];

        private bool IsTransparent
        {
            set
            {
                m_IsTransparent = value;
                if (m_Material == null) return;
                maskTween?.Kill();
                maskTween = m_Material.DOFade(m_IsTransparent ? 0 : 1, 0.5f);
            }
        }

        public List<Vector2> worldCenter = new List<Vector2>();
        private bool isInit;

        protected override void Init()
        {
            if (isInit) return;
            selfGO = this.gameObject;
            maskGroup = transform.Find($"MaskGroup");
            for (int i = 0; i < maskGroup.childCount; i++)
            {
                maskGroup.GetChild(i).gameObject.SetActive(false);
            }
            m_Mask = transform.Find("Mask").GetComponent<Image>();
            m_Material = m_Mask.GetComponent<Image>().material;

            SetShowCircles(null);
            isInit = true;
            
            CommonUtils.AddEventTrigger(m_Mask.gameObject, EventTriggerType.PointerClick, OnPointerClick);
            CommonUtils.AddEventTrigger(m_Mask.gameObject, EventTriggerType.PointerDown, OnPointerDown);
            CommonUtils.AddEventTrigger(m_Mask.gameObject, EventTriggerType.PointerUp, OnPointerUp);
            CommonUtils.AddEventTrigger(m_Mask.gameObject, EventTriggerType.Drag, OnDrag);
            CommonUtils.AddEventTrigger(m_Mask.gameObject, EventTriggerType.BeginDrag, OnBeginDrag);
            CommonUtils.AddEventTrigger(m_Mask.gameObject, EventTriggerType.EndDrag, OnEndDrag);
        }
        private void OnDestroy()
        {
            maskTween?.Kill();
            scaleTween?.Kill();
        }

        /// <summary>
        /// 查找可用的节点
        /// </summary>
        /// <param name="maskType"></param>
        /// <returns></returns>
        private GameObject FindCanUseItem(GuideMaskType maskType = GuideMaskType.Circle)
        {
            var type = maskType;
            if (maskType == GuideMaskType.ChangeOrder) type = GuideMaskType.Circle;
            for (int i = 0; i < maskGroup.childCount; i++)
            {
                Transform child = maskGroup.GetChild(i);
                if (!child.gameObject.name.Equals(type.ToString())) continue;
                if (!child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(true);
                    return child.gameObject;
                }
            }

            GameObject go = Instantiate(maskGroup.Find(type.ToString()).gameObject, maskGroup, false);
            go.SetActive(true);
            return go;
        }

        public override void Show()
        {
            if (selfGO == null || selfGO.activeSelf) return;
            selfGO.SetActive(true);
        }

        public override void Hide()
        {
            isStartTrace = false;
            ResetMaskGroup();
            ResetTargets();
            targetCanvas.Clear();
            targets = null;
            m_Circles?.ForEach(go => go?.SetActive(false));
            m_Circles?.Clear();
            circleByObj.Clear();
            OffHole();
            //m_Material.SetInt("_CenterCount", 0);
            maskTween?.Kill();
            maskTween = m_Material.DOFade(0, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (selfGO == null || !selfGO.activeSelf) return;
                selfGO.SetActive(false);
            });
        }

        private void ResetMaskGroup()
        {
            if (maskGroup == null) return;
            for (int i = 0; i < maskGroup.childCount; i++)
            {
                maskGroup.GetChild(i).gameObject.SetActive(false);
            }
        }
        private void ResetTargets()
        {
            foreach (var targetCanva in targetCanvas)
            {
                if (targetCanva.Value == null) continue;
                Destroy(targetCanva.Value);
            }
        }

        public override bool IsShow()
        {
            return selfGO.activeSelf;
        }

        private void Update()
        {
            TraceTarget();
        }
        /// <summary>
        /// 追踪目标
        /// </summary>
        private void TraceTarget()
        {
            if (!isStartTrace) return;
            if (circleByObj.Count <= 0) return;
            if (targets == null || targets.Count <= 0) return;
            for (int i = 0; i < targets.Count; i++)
            {
                var target = targets[i];
                if (target == null) continue;
                if (traces != null && !traces[i]) continue;
                if (!tracePos.ContainsKey(target)) continue;
                var pos = GetTargetPosByLocal(target, circleByObj[target]);
                circleByObj[target].localPosition = new Vector2(pos.x + offsetGroup[i].x, pos.y + offsetGroup[i].y);
                var v = circleByObj[target].localPosition;
                var t = tracePos[target];
                tracePos[target] = new Vector4(v.x, v.y, t.z, t.w);
            }

            var posList = new List<Vector4>(tracePos.Values).ToArray();
            SetShowCircles(posList, new List<float>(targetRadius.Values).ToArray());
        }

        /// <summary>
        /// 设置孔状态
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="radius"></param>
        private void SetShowCircles(Vector4[] pos, float[] radius = null)
        {
            if (m_Material == null) return;
            if (pos == null || pos.Length <= 0)
            {
                OffHole();
                return;
            }
            
            if (pos[0].z != 0 && pos[0].w != 0)
            {
                SwtichRect();
            }
            else
            {
                SwtichCircle();
            }
            
            m_Material.SetVectorArray("_Centers", pos);
            m_Material.SetFloatArray("_Rs", radius ?? defaultRadius);
            int count = pos.Length;
            m_Material.SetInt("_Count", count);
        }

        public void ChangeTarget(List<Transform> points, List<float> radius, List<int> maskTypes, List<string> offsetGroups,bool isTransparent = false,List<int> Trace = null)
        {
            Init();

            IsTransparent = isTransparent;
            offsetGroup = new List<Vector3>();
            traces = new List<bool>();
            circleByObj.Clear();
            m_Circles.Clear();
            targetRadius.Clear();
            tracePos.Clear();
            targets = points;
            CheckHole(points, radius, maskTypes, offsetGroups, Trace);
            timer = Time.time;
            Show();
        }


        /// <summary>
        /// 验证挖洞
        /// </summary>
        /// <param name="points">点位</param>
        /// <param name="radius">半径缩放</param>
        /// <param name="maskTypes">挖孔类型</param>
        /// <param name="offsetGroups">孔偏移</param>
        /// <param name="Trace">是否追踪</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void CheckHole(List<Transform> points, List<float> radius, List<int> maskTypes, List<string> offsetGroups, List<int> Trace)
        {
            if (points == null) return;
            for (int i = 0; i < points.Count; i++)
            {
                CreateHole(points[i], radius, maskTypes[i], CalculateOffset(offsetGroups, i), i);
                CalculateTrace(Trace, i);
            }

            if (tracePos.Count <= 0) return;
            scaleTween?.Kill();
            float scale = 0f; //这里做缩圈动画
            SetShowCircles(new List<Vector4>(tracePos.Values).ToArray(), new List<float>(targetRadius.Values).ToArray());
            scaleTween = DOTween.To(x => scale = x, 2, 1, 0.3f).OnUpdate(() =>
            {
                var scaleList = new List<float>(targetRadius.Values);
                for (int i = 0; i < scaleList.Count; i++)
                {
                    scaleList[i] *= scale;
                }

                SetShowCircles(new List<Vector4>(tracePos.Values).ToArray(), scaleList.ToArray());
            }).OnComplete(() =>
            {
                SetShowCircles(new List<Vector4>(tracePos.Values).ToArray(), new List<float>(targetRadius.Values).ToArray());
                isStartTrace = true;
            });
        }

         /// <summary>
        /// 创建孔
        /// </summary>
        /// <param name="points"></param>
        /// <param name="radius"></param>
        /// <param name="maskTypes"></param>
        /// <param name="offsetGroups"></param>
        /// <param name="Trace"></param>
        /// <param name="i"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void CreateHole(Transform point, List<float> radius, int maskTypeValue, Vector3 offset, int index)
        {
            var maskType = (GuideMaskType) maskTypeValue;

            GameObject m_Circle = FindCanUseItem(maskType);
            RectTransform size = point.GetComponent<RectTransform>();
            RectTransform circleRect = m_Circle.GetComponent<RectTransform>();
            var pos = GetTargetPosByLocal(point, circleRect);
            circleRect.localPosition = new Vector3(pos.x + offset.x, pos.y + offset.y);
            float circleradius = 0;
            if (size == null)
            {
                circleRect.sizeDelta = new Vector2(defaultRect, defaultRect);
                if (maskType != GuideMaskType.ChangeOrder) tracePos.Add(point, new Vector4(circleRect.localPosition.x, circleRect.localPosition.y));
                circleradius = defaultRect;
            }
            else
            {
                switch (maskType)
                {
                    case GuideMaskType.Circle:
                        circleradius = Mathf.Min(size.rect.width, size.rect.height);
                        circleRect.sizeDelta = new Vector2(circleradius, circleradius);
                        if (maskType != GuideMaskType.ChangeOrder) tracePos.Add(point, new Vector4(circleRect.localPosition.x, circleRect.localPosition.y));
                        break;
                    case GuideMaskType.Rectangle:
                        circleRect.sizeDelta = new Vector2(size.rect.width, size.rect.height);
                        if (maskType != GuideMaskType.ChangeOrder)
                            tracePos.Add(point, new Vector4(circleRect.localPosition.x, circleRect.localPosition.y, size.rect.width / 2, size.rect.height / 2));
                        break;
                    case GuideMaskType.ChangeOrder:
                        GuideTarget canvas = point.GetComponent<GuideTarget>();
                        if (canvas != null)
                        {
                            if(!targetCanvas.ContainsKey(point)) targetCanvas.Add(point, canvas);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            float radiusScale = 1;
            if (radius != null && radius.Count > 0) radiusScale = radius[index];
            circleRect.sizeDelta = circleRect.sizeDelta * radiusScale;
            targetRadius.Add(point, radiusScale * circleradius / 2);
            m_Circles.Add(m_Circle);
            circleByObj.Add(point, circleRect);
        }
        /// <summary>
        /// 计算追踪
        /// </summary>
        /// <param name="offsetGroups"></param>
        /// <param name="index"></param>
        private void CalculateTrace(List<int> traceGroup, int index)
        {
            if (traceGroup == null || traceGroup.Count <= index)
            {
                traces.Add(true);
            }
            else
            {
                bool istrace = traceGroup[index] == 1;
                traces.Add(istrace);
            }
        }
        /// <summary>
        /// 计算偏移
        /// </summary>
        /// <param name="offsetGroups"></param>
        /// <param name="index"></param>
        private Vector3 CalculateOffset(List<string> offsetGroups, int index)
        {
            if (offsetGroups == null || offsetGroups.Count <= index)
            {
                offsetGroup.Add(Vector3.zero);
                return Vector3.zero;
            }
            else
            {
                var arr = offsetGroups[index].Split('|');
                float.TryParse(arr[0], out var x);
                float.TryParse(arr[1], out var y);
                Vector3 offset = new Vector3(x, y);
                offsetGroup.Add(offset);
                return offset;
            }
        }
        private Vector3 GetTargetPosByLocal(Transform target, RectTransform self)
        {
            return self.parent.InverseTransformPoint(target.position);
        }
        
        public void OnPointerClick(BaseEventData eventData)
        {
            OnTouch(eventData,EventTriggerType.PointerClick);
        }
        public void OnPointerDown(BaseEventData eventData)
        {
            OnTouch(eventData, EventTriggerType.PointerDown);
        }
        
        public void OnPointerUp(BaseEventData eventData)
        {
            OnTouch(eventData, EventTriggerType.PointerUp);
        }

        public void OnBeginDrag(BaseEventData eventData)
        {
            OnTouch(eventData, EventTriggerType.BeginDrag);
        }

        public void OnEndDrag(BaseEventData eventData)
        {
            OnTouch(eventData, EventTriggerType.EndDrag);
        }

        public void OnDrag(BaseEventData eventData)
        {
            OnTouch(eventData, EventTriggerType.Drag);
        }

        private void OnTouch(BaseEventData baseEventData,EventTriggerType mode)
        {
            if (Time.time - timer < 0.3f) return;
            bool within = RaycastThrough(baseEventData, out Transform target);
            OnTouchMaskEvent?.Invoke(within ? GuideTouchMaskEnum.Within : GuideTouchMaskEnum.Outside, mode, baseEventData, target);
        }
        
        public bool RaycastThrough(BaseEventData baseEventData, out Transform targetObj)
        {
            PointerEventData pointerEventData = (PointerEventData)baseEventData;
		
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            //获取当前射线检测到的所有结果
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);
            bool isHit = false;
            Transform touchObj = null;
            targetObj = touchObj;
            for (int i = 0; i < raycastResults.Count; i++)
            {
                isHit = raycastResults[i].gameObject.transform.IsChildOf(maskGroup);
                if(isHit)
                {
                    touchObj = raycastResults[i].gameObject.transform;
                    break;
                }
            }

            if (!isHit) return false;
            foreach (var item in circleByObj)
            {
                if (item.Value.transform != touchObj) continue;
                targetObj = item.Key;
                break;
            }
            return true;
        }
        
        void OffHole()
        {
            if (!m_Material)
                return;
            
            m_Material.DisableKeyword("CIRCLE");
            m_Material.DisableKeyword("RECT");
        }

        void SwtichCircle()
        {
            if (!m_Material)
                return;
            
            m_Material.EnableKeyword("CIRCLE");
            m_Material.DisableKeyword("RECT");
        }
        
        void SwtichRect()
        {
            if (!m_Material)
                return;
            
            m_Material.DisableKeyword("CIRCLE");
            m_Material.EnableKeyword("RECT");
        }
    }
}