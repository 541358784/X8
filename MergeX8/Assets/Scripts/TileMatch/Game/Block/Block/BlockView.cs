using System;
using DG.Tweening;
using DragonPlus.Config.TileMatch;
using DragonU3DSDK.Asset;
using GamePool;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockView : ILoadView, IDestroy, IShake, IBlockAnim, IObstacleHandle
    {
        public Block _block;
        
        public Transform _parent;
        public GameObject _root;

        public BoxCollider2D _collider2D;
        protected SpriteRenderer _icon;
        protected SpriteRenderer _maskGray;
        protected SpriteRenderer _maskDark;

        protected GameObject _obstacleRoot;
        public GameObject _obstacle;
        protected string _obstaclePoolName;
        
        protected Color _grayColor;
        protected Color _drakColor;
        
        public BlockView(Block block)
        {
            _block = block;
        }

        public virtual void LoadView(Transform parent)
        {
            _parent = parent;
            
            _root = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.TileMatchBlock);
            CommonUtils.AddChild(parent, _root.transform);

#if UNITY_EDITOR
            _root.name = _block._blockModel._blockData.id.ToString();
#endif
            _root.transform.localPosition = _block._blockModel.localPosition;

            _collider2D = _root.GetComponent<BoxCollider2D>();
            _icon = _root.transform.Find("Icon").GetComponent<SpriteRenderer>();
            _icon.gameObject.SetActive(true);
            
            _maskGray = _root.transform.Find("Mask").GetComponent<SpriteRenderer>();
            _maskDark = _root.transform.Find("MaskDark").GetComponent<SpriteRenderer>();
            _obstacleRoot =  _root.transform.Find("Obstacle").gameObject;

            _maskGray.DOKill();
            _maskDark.DOKill();

            _grayColor = _maskGray.color;
            _drakColor = _maskDark.color;
            
            RefreshIcon();
            RefreshMask();
            LoadObstacle(_obstacleRoot.transform);
        }
        
        public virtual void RefreshIcon()
        {
            var config = TileMatchConfigManager.Instance.BlockNameList.Find(a => a.id == _block.BlockId);
            if(_icon.sprite.name == config.imageName)
                return;

            _icon.sprite = null;
            _icon.sprite = ResourcesManager.Instance.GetSpriteVariant("SpriteAtlas/TileMatchAtlas", config.imageName);
            _icon.sprite.name = config.imageName;
        }

        private void RefreshMask()
        {
            var config = TileMatchConfigManager.Instance.BlockTypeList.Find(a => a.id == _block._blockModel._blockData.blockType);

            RefreshMask(config.blockMaskName);
        }

        public void RefreshNormalMask()
        {
            RefreshMask("normalMask");
        }

        private void RefreshMask(string maskName)
        {
            if(_maskGray.sprite.name == maskName)
                return;

            _maskGray.sprite = null;
            _maskGray.sprite = ResourcesManager.Instance.GetSpriteVariant("SpriteAtlas/TileMatchAtlas", maskName);
            _maskGray.sprite.name = maskName;

            _maskDark.sprite = null;
            _maskDark.sprite = ResourcesManager.Instance.GetSpriteVariant("SpriteAtlas/TileMatchAtlas", maskName);
            _maskDark.sprite.name = maskName;
        }
        public void DoDarkFade(float alpha, bool isAnim)
        {
            DOFade(_maskDark, alpha, isAnim);
        }

        public void DoGrayFade(float alpha, bool isAnim)
        {
            DOFade(_maskGray, alpha, isAnim);
        }

        private void DOFade(SpriteRenderer spriteRenderer, float alpha, bool isAnim)
        {
            spriteRenderer.DOKill();
            if (isAnim)
            {
                spriteRenderer.DOFade(alpha, 0.2f).SetEase(Ease.Linear);
            }
            else
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r ,spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            }
        }

        public void DoDarkColor(Color color, bool isAnim)
        {
            DoColor(_maskDark, color, isAnim);
        }

        public Color DarkColor
        {
            get { return _drakColor; }
        }
        public void DoGrayColor(Color color, bool isAnim)
        {
            DoColor(_maskGray, color, isAnim);
        }
        
        private void DoColor(SpriteRenderer spriteRenderer, Color color, bool isAnim)
        {
            spriteRenderer.DOKill();
            if (isAnim)
            {
                spriteRenderer.DOColor(color, 0.2f).SetEase(Ease.Linear);
            }
            else
            {
                spriteRenderer.color = color;
            }
        }
        
        public void SetColliderEnable(bool isEnable)
        {
            _collider2D.enabled = isEnable;
        }

        public virtual void Destroy()
        {
            UnLoadObstacle();
            
            if (_root != null)
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.TileMatchBlock, _root);

            _root = null;

            _collider2D = null;
            _icon = null;
            _maskGray = null;
            _maskDark = null;
            _block = null;
            _parent = null;
        }

        public virtual void Shake()
        {
        }

        public Tweener ShakeTweener()
        {
            return null;
        }

        public virtual void BreakAnim(Action action = null)
        {
            action?.Invoke();
        }

        public virtual void LoadObstacle(Transform parent)
        {
            if(_obstaclePoolName.IsEmptyString())
                return;
            
            _obstacle = GamePool.ObjectPoolManager.Instance.Spawn(_obstaclePoolName);
            CommonUtils.AddChild(parent, _obstacle.transform);
        }

        public virtual void UnLoadObstacle()
        {
            if(_obstacle == null)
                return;
            
            GamePool.ObjectPoolManager.Instance.DeSpawn(_obstaclePoolName, _obstacle);
            _obstacle = null;
        }

        public virtual void HideObstacle()
        {
            if(_obstacle == null)
                return;
            
            _obstacle.gameObject.SetActive(false);
        }

        public virtual void ShowObstacle()
        {   
            if(_obstacle == null)
                return;
            
            _obstacle.gameObject.SetActive(true);
        }
    }
}