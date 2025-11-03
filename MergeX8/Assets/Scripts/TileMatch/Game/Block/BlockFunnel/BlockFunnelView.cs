using System;
using Framework;
using GamePool;
using TMPro;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockFunnelView : BlockView
    {
        private TextMeshPro _tileCount;
        private Animator _animator;
        private GameObject _animationObj;
        private GameObject _spawnObject;
        private GameObject _funnelBgObj;
        
        public BlockFunnelView(Block block) : base(block)
        {
        }
        
        public override void LoadView(Transform parent)
        {
            _obstaclePoolName = ObjectPoolName.TileMatchBlock_Glue_Funnel;
            
            base.LoadView(parent);

            _icon.gameObject.SetActive(false);
            
            _tileCount = _obstacle.transform.Find("Parent/CountText").GetComponent<TextMeshPro>();
            _tileCount.text = ((BlockFunnelModel)_block._blockModel)._residueCount.ToString();
            _animationObj = _obstacle.transform.Find("Animation").gameObject;
            _animationObj.gameObject.SetActive(false);
            
            _funnelBgObj = _obstacle.transform.Find("Parent/Background").gameObject;
            _funnelBgObj.gameObject.SetActive(false);
            
            _spawnObject = _obstacle.transform.Find("Animation/TileMaker/SpawnObject").gameObject;
            
            _animator = _obstacle.GetComponent<Animator>();

            RefreshView(_block.GetBlockState());
        }

        public void RefreshView(BlockState state)
        {
            switch (state)
            {
                case BlockState.Normal:
                {
                    if(_animationObj.gameObject.activeSelf)
                        return;
                    
                    _funnelBgObj.gameObject.SetActive(false);
                    _animationObj.gameObject.SetActive(true);
                    PlayAnimation("PrinterIdle");
                    break;
                }
                case BlockState.Overlap:
                {
                    _funnelBgObj.gameObject.SetActive(true);
                    _animationObj.gameObject.SetActive(false);
                    break;
                }
            }
        }
        public void RefreshView()
        {
            _tileCount.text = ((BlockFunnelModel)_block._blockModel)._residueCount.ToString();
        }

        public void PlayAnimation(string animName, Action action = null)
        {
            if (action == null)
            {
                _animator.Play(animName, 0, 0);
            }
            {
                CoroutineManager.Instance.StartCoroutine(CommonUtils.PlayAnimation(_animator, animName, "", action));
            }
        }

        public float GetSpawnZ()
        {
            return _spawnObject.transform.position.z;
        }
        
        public override void Shake()
        {
            PlayAnimation("TileMakerClick");
        }
    }
}