using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DG.Tweening;
using Framework;
using GamePool;
using Spine.Unity;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace TileMatch.Game.PlayMethod
{
    public class PurdahMethodLogic : MonoBehaviour
    {
        private int _brokenNum = 0;
        private List<Block.Block> _blocks = new List<Block.Block>();
        private Dictionary<int, List<Block.Block>> _trimBlocks = new Dictionary<int, List<Block.Block>> ();
        private int _minIndex = int.MaxValue;
        private int _maxIndex = int.MinValue;
        private int _layoutId = 0;
        private TextMeshPro _brokenText;
        private bool _isBroken = false;

        private Animator _animator;
        private SkeletonAnimation _skeletonAnimation;
        
        public int LayoutId
        {
            get { return _layoutId; }
        }

        public List<Block.Block> Blocks
        {
            get
            {
                return _blocks;
            }
        }

        private int _poolIndex;
        private string[] _appearAnimName = new[] { "Appear2_3", "Appear4_5", "Appear6_7" };
        private string[] _overAnimName = new[] { "Over2_3", "Over4_5", "Over6_7" };
        private void Awake()
        {
            _brokenText = transform.Find("plane/SkeletonUtility-SkeletonRoot/root/bone/Zong/1a2/1b2/1c2/1d2/1e2/1f2/1g2/1h2/1i2/1j2/4/Text/CounterParent/Counter").GetComponent<TextMeshPro>();

            _animator = transform.GetComponent<Animator>();
            _skeletonAnimation = transform.Find("plane").GetComponent<SkeletonAnimation>();
        }

        public void Init(string param, List<Block.Block> blocks, int layoutId, int poolIndex)
        {
            _brokenNum = int.Parse(param);
            _blocks = blocks;
            _layoutId = layoutId;
            _isBroken = false;
            _poolIndex = poolIndex;
            
            _minIndex = int.MaxValue;
            _maxIndex = int.MinValue;
            
            _trimBlocks.Clear();
            
            _brokenText.SetText(_brokenNum.ToString());
            
            foreach (var block in blocks)
            {
                int index = block._blockModel._blockData.index_X;
                _minIndex = Math.Min(index, _minIndex);
                _maxIndex = Math.Max(index, _maxIndex);
                
                if(!_trimBlocks.ContainsKey(index))
                    _trimBlocks.Add(index, new List<Block.Block>());
                
                _trimBlocks[index].Add(block);
            }
            
            _skeletonAnimation.AnimationState.SetAnimation(0, _appearAnimName[_poolIndex], false);
            _skeletonAnimation.AnimationState.Update(0);
            _animator.Play("Appear", 0, 0);
        }

        private void OnDestroy()
        {
            _blocks.Clear();
            _blocks = null;
            
            _trimBlocks.Clear();
            _trimBlocks = null;
        }

        public bool Broken(List<Block.Block> blocks, Action action, bool isMagic = false)
        {
            if (_isBroken)
                return false;

            int brokenNum = blocks.Count;

            _brokenNum -= brokenNum;
            _brokenNum = Math.Max(_brokenNum, 0);
            
            _isBroken = _brokenNum == 0;

            
            
            var moveEndPosition = _brokenText.transform.position;
            moveEndPosition.z = -50;
            if (isMagic)
            {
                FlyBrokenEffect(blocks[0], _brokenNum, moveEndPosition, 0);
            }
            else
            {
                for(int i = 0; i < blocks.Count; i++)
                {
                    FlyBrokenEffect(blocks[i], _brokenNum, moveEndPosition, i);
                }
            }
            
            if (!_isBroken)
                return false;

            StartCoroutine(BrokenAnim(action));
            
            return true;
        }

        private void FlyBrokenEffect(Block.Block block, int brokenNum, Vector3 moveEndPosition, int index)
        {
            var flyEffect = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.TileMatchBlock_PurdahFly);

            var position = block._blockView._root.transform.position;
            position.z = -50f;
                
            flyEffect.transform.position = position;

            flyEffect.transform.DOMove(moveEndPosition, 0.5f).OnComplete(() =>
            {
                if (index == 0)
                {
                    _skeletonAnimation.AnimationState.SetAnimation(0, _appearAnimName[_poolIndex], false);
                    _skeletonAnimation.AnimationState.Update(0);
                    _animator.Play("Appear", 0, 0);
                }
                _brokenText.SetText(brokenNum.ToString());

                var fxEffect = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.TileMatchBlock_PurdahFx);
                fxEffect.transform.position = moveEndPosition;

                CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1, () =>
                {
                    GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.TileMatchBlock_PurdahFly, flyEffect);
                    GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.TileMatchBlock_PurdahFx, fxEffect);
                }));
            });
        }
        private IEnumerator BrokenAnim(Action action)
        {
            if(!_trimBlocks.ContainsKey(_minIndex))
                yield break;

            yield return new WaitForSeconds(1.1f);
            
            _skeletonAnimation.AnimationState.SetAnimation(0, _overAnimName[_poolIndex], false);
            _skeletonAnimation.AnimationState.Update(0);
            _animator.Play("Over", 0, 0);
            
            yield return new WaitForSeconds(0.35f);
            
            int index = 1;
            float moveTime = 0.05f;
            float moveMaxTime = 0;
            float moveY = _trimBlocks[_minIndex][0]._blockView._root.transform.localPosition.y-0.42f;
            
            for (int i = _minIndex + 1; i <= _maxIndex; i++)
            {
                if(!_trimBlocks.ContainsKey(i))
                    continue;

                foreach (var block in _trimBlocks[i])
                {
                    moveMaxTime = moveTime * index;
                    block._blockView._root.transform.DOLocalMoveY(moveY, moveMaxTime).OnComplete(() =>
                    {
                        block._blockView._root.gameObject.SetActive(false);
                    }).SetEase(Ease.Linear);
                }

                index++;
            }

            yield return new WaitForSeconds(moveMaxTime);
            
            foreach (var block in _trimBlocks[_minIndex])
            {
                block._blockView._root.gameObject.SetActive(false);
                
                var flyEffect = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.TileMatchBlock_PurdahFxBroken);

                var position = block._blockView._root.transform.position;
                position.z = -50f;
                flyEffect.transform.position = position;
                CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(2, () =>
                {
                    GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.TileMatchBlock_PurdahFxBroken, flyEffect);
                }));
            }
            
            yield return new WaitForSeconds(1f);
            
            action?.Invoke();
        }

    }
}