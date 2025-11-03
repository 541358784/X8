using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;

namespace OutsideGuide
{
    public class GuideArrow : GuideGraphicBase
    {
        public Image fingerImg;
        private Tweener tweener;
        private Sequence sequence;
        private Image m_Circle = null;

        private bool isInit;

        private Transform target;
        private Vector3 offsetPos;
        private RectTransform root;
        private Animator animator;

        protected override void Init()
        {
            if (isInit) return;
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingLayerName = "Guide";
            canvas.sortingOrder = 20;
            animator = GetComponent<Animator>();
            root = transform.Find($"Root").GetComponent<RectTransform>();
            m_Circle = transform.Find("Root/Circle").GetComponent<Image>();
            //小手白圈默认隐藏
            m_Circle.gameObject.SetActive(false);
            isInit = true;
        }

        public void SetCircleVisible(bool visible)
        {
            if (this.m_Circle == null) return;
            m_Circle.gameObject.SetActive(visible);
        }

        private void OnDestroy()
        {
            tweener?.Kill();
        }

        public override void Show()
        {
            
        }

        private void Show(GuideArrowType guideArrowType, LoopType moveType = LoopType.Yoyo, int param = 0)
        {
            tweener?.Kill();
            sequence?.Kill();
            bool useSpine = false;
            Vector3 moveOffset = new Vector3();
            switch (guideArrowType)
            {
                case GuideArrowType.FingerLeftDown:
                case GuideArrowType.FingerLeftUp:
                case GuideArrowType.FingerRightDown:
                case GuideArrowType.FingerRightUp:
                    useSpine = true;
                    break;
                case GuideArrowType.Up:
                    moveOffset = new Vector3(0, -50);
                    animator.enabled = false;
                    break;
                case GuideArrowType.Down:         
                    moveOffset = new Vector3(0, 50);
                    animator.enabled = false;
                    break;
            }

            var moveObj = root.Find($"{guideArrowType}");
            moveObj.gameObject.SetActive(true);
            if (!IsShow()) gameObject.SetActive(true);
            if (useSpine)
            {
                var skeleton = moveObj.GetComponentInChildren<SkeletonGraphic>();
                skeleton.AnimationState.SetAnimation(0, param == 0 ? "Click" : "LongPress", true);
                return;
            }
            moveObj.localPosition = Vector3.zero;
            Vector3 origPos = Vector3.zero;
            var targetPos = new Vector3(origPos.x + moveOffset.x, origPos.y + moveOffset.y, 0f);
            tweener = moveObj.DOLocalMove(targetPos, 0.6f).SetEase(Ease.Linear).SetLoops(-1, moveType);
        }

        public override void Hide()
        {
            sequence?.Kill();
            sequence = null;
            tweener?.Kill();
            tweener = null;
            target = null;
            if (!IsShow()) return;
            gameObject.SetActive(false);
        }

        public override bool IsShow()
        {
            if (gameObject == null)
            {
                return false;
            }
            return gameObject.activeSelf;
        }

        private void Update()
        {
            if (target == null) return;
            var origPos = GetTargetPosByLocal(target, root.GetComponent<RectTransform>());
            root.localPosition = new Vector3(origPos.x + offsetPos.x, origPos.y + offsetPos.y);
        }

        public void PointTo(Transform pos, Vector3 offset, GuideArrowType arrowType, int param = 0)
        {
            Init();
            tweener?.Kill();
            for (int i = 0; i < root.childCount; i++)
            {
                root.GetChild(i).gameObject.SetActive(false);
            }
            target = pos;
            offsetPos = offset;
            var origPos = GetTargetPosByLocal(pos, root.GetComponent<RectTransform>());
            root.localPosition = new Vector3(origPos.x + offset.x, origPos.y + offset.y);
            LoopType type =(int) arrowType >= 3 ? LoopType.Restart : LoopType.Yoyo;
            Show(arrowType, type, param);
        }

        public void PointToTarget(Transform pos1, Transform pos2)
        {
            Init();
            for (int i = 0; i < root.childCount; i++)
            {
                root.GetChild(i).gameObject.SetActive(false);
            }
            tweener?.Kill();
            root.localPosition = GetTargetPosByLocal(pos1, root.GetComponent<RectTransform>());
            LoopType type = LoopType.Restart;
            var moveObj = root.Find($"{GuideArrowType.FingerRightDown}");
            if (!IsShow()) gameObject.SetActive(true);
            moveObj.gameObject.SetActive(true);
            moveObj.transform.localPosition = Vector3.zero;
            var targetPos = GetTargetPosByLocal(pos2, root.GetComponent<RectTransform>());
            tweener = root.transform.DOLocalMove(targetPos, 1f).SetEase(Ease.Linear).SetLoops(-1, type);
        }
        private Vector3 GetTargetPosByLocal(Transform target,RectTransform self)
        {
            return self.parent.InverseTransformPoint(target.position);
        }
        private Vector3 GetTargetPosByLocal(Vector3 target,RectTransform self)
        {
            return self.parent.InverseTransformPoint(target);
        }
    }
}