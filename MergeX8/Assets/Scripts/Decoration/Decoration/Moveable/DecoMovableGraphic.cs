using Deco.Graphic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Tilemaps;
using Decoration;
using UnityEngine;
namespace Deco.Moveable
{
    public abstract class DecoMovableGraphic : DecoGraphic
    {
        protected IsometricMoving _isometricMoving;
        private RectTransform _canvasRectTransform;

        protected float _graphicWidth = 50;
        protected float _graphicHeight = 100;

        private float _visibleCheckTick = 0f;
        private bool _isVisible = true;

        internal virtual Vector2 Front => new Vector2(1f, 0f);
        internal virtual Vector2 Up => new Vector2(0f, -1);

        protected abstract void OnShowGraphic(bool show);

        internal Vector2 Position
        {
            get
            {
                if (_isometricMoving)
                {
                    return _isometricMoving.transform.position;
                }

                return Vector2.zero;
            }
        }

        public bool Visible
        {
            get
            {
                if (!gameObject) return false;

                var screenPos = DecoSceneRoot.Instance.mSceneCamera.WorldToScreenPoint(gameObject.transform.position);

                var leftOut = screenPos.x + _graphicWidth < 0;
                if (leftOut) return false;

                var rightOut = screenPos.x - _graphicWidth > Screen.width;
                if (rightOut) return false;

                var topOut = screenPos.y - _graphicHeight > Screen.height;
                if (topOut) return false;

                var bottomOut = screenPos.y + _graphicHeight < 0;
                if (bottomOut) return false;

                return true;
            }
        }

        internal void SetPosition(Vector2 position)
        {
            if (_isometricMoving)
            {
                _isometricMoving.SetPosition(position);
            }
        }

        internal void update(float deltaTime)
        {
            updateVisibleCheck();
        }

        internal void updateVisibleCheck()
        {
            if (_skeletonAnimations != null && _skeletonAnimations.Length > 0 && !_skeletonAnimations[0]) return;
            if (_visibleCheckTick <= 0.1f)
            {
                _visibleCheckTick += Time.deltaTime;
                return;
            }

            _visibleCheckTick = 0f;

            var isVisible = Visible;
            if (_isVisible != isVisible)
            {
                _isVisible = isVisible;

                ShowGraphic(isVisible);
                EnableAnimator(isVisible);

                OnShowGraphic(isVisible);
            }
        }

        public abstract bool SetDirection(bool isLeft, bool isFront);
        public abstract void PlayMoving();

        internal void leave()
        {
            if (_skeletonAnimations != null && _skeletonAnimations.Length > 0 && _skeletonAnimations[0] && _skeletonAnimations[0].gameObject) _skeletonAnimations[0].gameObject.SetActive(true);
            if (_isometricMoving && _isometricMoving.gameObject)
            {
                //MyMain.myGame.ObjectPoolMgr.RecycleGameObject(_isometricMoving.gameObject);
                _isometricMoving.gameObject.SetActive(false);
                GameObject.Destroy(_isometricMoving.gameObject);
            }
        }
    }
}