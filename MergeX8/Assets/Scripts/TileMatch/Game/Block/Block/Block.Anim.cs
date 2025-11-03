using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using Framework;
using GamePool;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public partial class Block
    {
        private Action<Block> _dropMoveAction;
        private Ease _easeType = Ease.Linear;
        public virtual void MoveToCatmullRom(Vector3 position, float scale = 1f, float speed = 20f, Action<Block> action = null)
        {
            if(!_blockView._root.gameObject.activeSelf)
                return;
            
            _blockView._root.transform.DOKill();
            
            _blockView._root.transform.position = new Vector3(_blockView._root.transform.position.x, _blockView._root.transform.position.y, position.z);

            // Vector3[] path = new Vector3[3];
            // float height = 0.2f;
            // path[0] = _blockView._root.transform.position;
            // path[2] = position;
            // path[1] = path[0];
            // float offsetX = path[2].x - path[0].x;
            // float offsetY = Mathf.Abs(path[2].y - path[0].y);
            // path[1].x += offsetX * 0.2f;
            //
            // 
            // float yOffset = Mathf.Clamp(Mathf.Abs((dis * 0.05f)), 0, height); // 根据水平偏移和高度参数来计算y轴的偏
            // if (yOffset < 0.1)
            //     yOffset = 0f;
            // path[1] += Vector3.up * yOffset;
            //
            float offsetY = TileMatchGameManager.Instance.GetSuperBannerOffsetY();
             float offsetX = Mathf.Abs(position.x - _blockView._root.transform.position.x);
            // float offsetY = Mathf.Abs(position.y - _blockView._root.transform.position.y);
            float jumpPower = (_blockView._root.transform.position.y - offsetY) / 2 + offsetX / 5;
            float dis = Vector3.Distance(position, _blockView._root.transform.position);
            float time = dis / speed;

            DOJump(_blockView._root.transform, position, jumpPower, 1, time).OnComplete(() =>
            {
                action?.Invoke(this);
            }).SetEase(_easeType);
            _blockView._root.transform.DOScale(scale, time);
            _blockView._root.transform.DORotate(Vector3.zero, time);
            // _blockView._root.transform.DOPath(path, time, PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() =>
            // {
            //     action?.Invoke(this);
            // });
        }
        
        public Sequence DOJump(
             Transform target,
            Vector3 endValue,
            float jumpPower,
            int numJumps,
            float duration,
            bool snapping = false)
        {
            if (numJumps < 1)
                numJumps = 1;
            float startPosY = 0.0f;
            float offsetY = -1f;
            bool offsetYSet = false;
            Sequence s = DOTween.Sequence();
            Tween yTween = (Tween) DOTween.To((DOGetter<Vector3>) (() => target.position), (DOSetter<Vector3>) (x => target.position = x), new Vector3(0.0f, jumpPower, 0.0f), duration / (float) (numJumps * 2)).SetOptions(AxisConstraint.Y, snapping).SetEase<Tweener>(Ease.OutQuad).SetRelative<Tweener>().SetLoops<Tweener>(numJumps * 2, LoopType.Yoyo).OnStart<Tweener>((TweenCallback) (() => startPosY = target.position.y));
            s.Append((Tween) DOTween.To((DOGetter<Vector3>) (() => target.position), (DOSetter<Vector3>) (x => target.position = x), new Vector3(endValue.x, 0.0f, 0.0f), duration).SetOptions(AxisConstraint.X, snapping).SetEase<Tweener>(Ease.Linear)).Join((Tween) DOTween.To((DOGetter<Vector3>) (() => target.position), (DOSetter<Vector3>) (x => target.position = x), new Vector3(0.0f, 0.0f, endValue.z), duration).SetOptions(AxisConstraint.Z, snapping).SetEase<Tweener>(Ease.Linear)).Join(yTween).SetTarget<Sequence>((object) target).SetEase<Sequence>(DOTween.defaultEaseType);
            yTween.OnUpdate<Tween>((TweenCallback) (() =>
            {
                if (!offsetYSet)
                {
                    offsetYSet = true;
                    offsetY = s.isRelative ? endValue.y : endValue.y - startPosY;
                }
                Vector3 position = target.position;
                position.y += DOVirtual.EasedValue(0.0f, offsetY, yTween.ElapsedPercentage(), Ease.Linear);
                target.position = position;
            }));
            return s;
        }
        
        public virtual void MoveTo(Vector3 position, float scale = 1f, float speed = 20f, Action<Block> action = null)
        {
            if(!_blockView._root.gameObject.activeSelf)
                return;
            
            _blockView._root.transform.position = new Vector3(_blockView._root.transform.position.x, _blockView._root.transform.position.y, position.z);

            _blockView._root.transform.DOKill();
            
            float distance = Vector3.Distance(position, _blockView._root.transform.position);
            float moveTime = distance / speed;

            _blockView._root.transform.DORotate(Vector3.zero, moveTime);
            _blockView._root.transform.DOScale(scale, moveTime);
            _blockView._root.transform.DOMove(position, moveTime).OnComplete(() =>
            {
                action?.Invoke(this);
            }).SetEase(_easeType);
        }
        public virtual void MoveToParabola(Vector3 position, float scale = 1f, float speed = 20f, Action<Block> action = null)
        {
            _blockView._root.transform.position = new Vector3(_blockView._root.transform.position.x, _blockView._root.transform.position.y, position.z);

            _blockView._root.transform.DOKill();
            
            float height = 1f;
            Vector3[] path = new Vector3[3];
            path[0] = _blockView._root.transform.position;
            path[2] = position;
            
            float dis = Vector3.Distance(path[0], path[2]);
            path[1] = (path[2] + path[0])*0.5f+ (Vector3.up * height);;
            path[1] += Vector3.up*height;
    
            float time = dis / speed;
            
            var tween = DOTween.Sequence();
            tween.Insert(0, _blockView._root.transform.DOPath(path, time, PathType.CatmullRom).SetEase(_easeType));
            tween.Insert(0, _blockView._root.transform.DOScale(scale*1.2f, time * 0.5f));
            tween.Insert(time * 0.5f, _blockView._root.transform.DOScale(scale, time * 0.5f));
            tween.OnComplete(()=>
            {
                action?.Invoke(this);
            });
        }
        public virtual void DropMove(Vector3 position, float scale, float speed, Action<Block> action)
        {
            _dropMoveAction = action;
            _blockState = BlockState.Droping;
            MoveToCatmullRom(position, scale, speed, (b) =>
            {
                _blockState = BlockState.InBanner;
                _dropMoveAction?.Invoke(this);
                _dropMoveAction = null;
            });
            RefreshView();
        }

        public virtual void DropMoveLine(Vector3 position, float scale, float speed, Action<Block> action)
        {
            _dropMoveAction = action;
            _blockState = BlockState.Droping;
            MoveTo(position, scale, speed, (b) =>
            {
                _blockState = BlockState.InBanner;
                _dropMoveAction?.Invoke(this);
                _dropMoveAction = null;
            });
            RefreshView();
        }
        
        public virtual void DropMove(Vector3 position, float scale, float speed)
        {
            MoveTo(position, scale, speed, (b) =>
            {
                _blockState = BlockState.InBanner;
                _dropMoveAction?.Invoke(this);
                _dropMoveAction = null;
            });
        }

        public virtual void MoveSuperBanner(Vector3 position, float scale = 1f, float speed = 20f, Action<Block> action = null)
        {
            _blockView._root.transform.position = new Vector3(_blockView._root.transform.position.x, _blockView._root.transform.position.y, -5f);
            position.z = -45f;
            MoveTo(position, scale, speed, (b) =>
            {
                _blockView._root.transform.position = position;
                action?.Invoke(this);
            });
        }

        public virtual void BackMoveSuperBanner(Vector3 position, float speed = 20f, Action<Block> action = null)
        {
            _blockView._root.transform.position = new Vector3(_blockView._root.transform.position.x, _blockView._root.transform.position.y, -5f);
            position.z = -45f;
            MoveToParabola(position, 1, speed, (b) =>
            {
                _blockView._root.transform.position = position;
                action?.Invoke(this);
            });
        }
        public virtual void BackMove(float speed = 20f, Action<Block> action = null)
        {
            _blockState = BlockState.Droping;
            Vector3 position = _blockModel.position;
            position.z = _blockView._root.transform.position.z;
            MoveToParabola(position, speed:speed,action:(b) =>
            {
                _blockView._root.transform.position = _blockModel.position;
                SetState(BlockState.Normal);
                action?.Invoke(this);
            });
        }
        
        public virtual void TweenHide()
        {
            _blockState = BlockState.TweeningHide;
            var seq = DOTween.Sequence();
            // 先放大
            seq.Append(_blockView._root.transform.DOScale(1.2f, 0.2f));
            seq.Append(_blockView._root.transform.DOScale(0.5f, 0.08f));
            seq.onComplete = () =>
            {
                GameObject effect = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.TileMatchBlock_Blast);
                effect.transform.position = _blockView._root.transform.position;
                
                _blockView._root.gameObject.SetActive(false);

                CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1f, () =>
                {
                    GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.TileMatchBlock_Blast, effect);
                }));
            };
        }

        public virtual void RemoveAnim(Action action)
        {
            _blockModel.IsCollect = true;
            action?.Invoke();
        }
    }
}