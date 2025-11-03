using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.TileMatch;
using DragonU3DSDK.Asset;
using Framework;
using GamePool;
using UnityEngine;
using UnityEngine.UI;

namespace TileMatch.Game.Block
{
    public class BlockGoldView : BlockView
    {
        private List<Animator> _stageAnimators = new List<Animator>();
        private List<GameObject> _stageBase = new List<GameObject>();
        private Animator _animator;
        private Coroutine _coroutine;
        
        public BlockGoldView(Block block) : base(block)
        {
        }
        
        public override void LoadView(Transform parent)
        {
            _obstaclePoolName = ObjectPoolName.TileMatchBlock_Gold;
            
            base.LoadView(parent);

            var config = TileMatchConfigManager.Instance.BlockNameList.Find(a => a.id == _block.BlockId);
            
            for (int i = 1; i <= 3; i++)
            {
                var goldObj = _obstacle.transform.Find("goldpaper" + i);
                _stageAnimators.Add(goldObj.GetComponent<Animator>());

                var baseObj = _obstacle.transform.Find("BaseAnimator/BaseGrp/Base"+i);
                _stageBase.Add(baseObj.gameObject);
                
                if (config.goldImage != null && config.goldImage.Length == 2)
                {
                    goldObj.transform.Find("Node/Piece").GetComponent<SpriteRenderer>().sprite = 
                        ResourcesManager.Instance.GetSpriteVariant("SpriteAtlas/TileMatchAtlas", config.goldImage[0]+i);
                    
                    baseObj.GetComponent<SpriteRenderer>().sprite = 
                        ResourcesManager.Instance.GetSpriteVariant("SpriteAtlas/TileMatchAtlas", config.goldImage[1]+i);
                }
            }

            _animator = _obstacle.transform.Find("BaseAnimator").GetComponent<Animator>();
            InitView();
        }

        private void InitView()
        {
            _stageAnimators.ForEach(a=>a.gameObject.SetActive(false));
            _stageBase.ForEach(a=>a.gameObject.SetActive(false));
            _stageBase[0].gameObject.SetActive(true);
        }

        public override void BreakAnim(Action action = null)
        { 
            int brokenNum = ((BlockGoldModel)_block._blockModel)._brokenNum;

            AudioManager.Instance.PlaySound(27+TileMatchRoot.AudioDistance);
            
            _stageAnimators.ForEach(a=>a.gameObject.SetActive(false));
            _stageBase.ForEach(a=>a.gameObject.SetActive(false));

            int stageIndex = 0;
            switch (brokenNum)
            {
                case 2:
                {
                    stageIndex = 0;
                    break;
                }
                case 1:
                {
                    stageIndex = 1;
                    break;
                }
                case 0:
                {
                    stageIndex = 2;
                    break;
                }
            }
            
            _stageAnimators[stageIndex].gameObject.SetActive(true);
            
            if(stageIndex+1 < _stageBase.Count)
                _stageBase[stageIndex+1].gameObject.SetActive(true);
            
            action?.Invoke();
        }

        public void PlayAddCoinEffect()
        {
            var effect = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.TileMatchBlock_GoldpaperFX);
            Vector3 position = _obstacleRoot.transform.position;
            position.z = -40;
            effect.transform.position = position;

            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1.5f, () =>
            {
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.TileMatchBlock_GoldpaperFX, effect);
            }));
        }
        
        public void StartShuffle()
        {
        }

        public void StopShuffle()
        {
        }
    }
}