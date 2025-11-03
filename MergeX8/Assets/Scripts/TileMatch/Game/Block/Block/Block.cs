using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK;
using LayoutData;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TileMatch.Game.Block
{
    public enum BlockState
    {
        Inactive, //不活动的 不可见 不可以点击
        Overlap,// 暗色，不可点击
        Black,  //黑色 ，看不到图标，不可点击
        Normal,// 普通状态，可以点击
        
        InCollection,// 在收集区，可以点击
        InCollectionOverlap,// 在收集区，不可以点击
        Droping,// 掉落过程中
        InBanner,// 在横条区域里
        TweeningHide,// 在播放消失动画
        Hided,// 已被隐藏
    }

    public enum AreaType
    {
        Normal, //普通
        SuperBanner, //上方收集栏
    }
    
    public partial class Block : ILoadView, IDestroy, IRemoveAnim, IInitData, IGameCheck
    {
        //扩充字段
        public bool _isCanRemove = true;
        
        public BlockModel _blockModel;
        public BlockView _blockView;
        
        public Layer.Layer _layer;

        protected BlockState _blockState = BlockState.Normal;
        protected AreaType _areaType = AreaType.Normal;

        public AreaType AreaType
        {
            get { return _areaType; }
        }
        
        public int _superBannerIndex;
        public bool _isSuperBanner;
        public int _index;
        public int _id;
        private int _blockId; //牌面id
        
        public int BlockId
        {
            get { return _blockId; }
        }
        public Block(Layer.Layer layer, LayerBlock blockData, int index)
        {
            _id = blockData.id;
            _index = index;
            _blockId = blockData.blockId;
            
            InitData(layer, blockData);
        }

        public virtual void InitData(Layer.Layer layer, LayerBlock blockData)
        {
            _blockModel = new BlockModel(layer, blockData, this);
            _blockView = new BlockView(this);
        }
        
        public virtual void LoadView(Transform parent)
        {
            _blockView.LoadView(parent);
        }
        
        public virtual void Destroy()
        {
            _blockModel.Destroy();
            _blockModel = null;
            
            _blockView.Destroy();
            _blockView = null;
        }

        public virtual void InitState()
        {
            if (_blockModel._blockData.children == null || _blockModel._blockData.children.Count == 0)
            {
                SetState(BlockState.Normal);
                return;
            }
            
            if (_blockModel._layer != null && _blockModel._layer._layerModel._layoutData.isStackLayout)
            {
                SetState(BlockState.Black);
            }
            else
            {
                foreach (var childId in _blockModel._blockData.children)
                {
                    var childBlock = TileMatchGameManager.Instance.GetBlock(childId);
                    if(!childBlock.IsInActiveState())
                        continue;
                    
                    SetState(BlockState.Overlap);
                    return;
                }

                SetState(BlockState.Normal);
            }
        }

        
        public BlockState GetBlockState()
        {
            return _blockState;
        }
        
        public void SetBlockState(BlockState state)
        {
            _blockState = state;
        }
        
        public AreaType GetAreaType()
        {
            return _areaType;
        }
        
        public void SetAreaType(AreaType type)
        {
            _areaType = type;
        }
        
        //是否可以消除
        public virtual bool CanRemove()
        {
            return _isCanRemove && IsInRemoveState();
        }

        public bool IsInRemoveState()
        {
            return _blockState == BlockState.Normal || _blockState == BlockState.InCollection;
        }
        
        //块是否是激活的 在牌面上 未消除
        public virtual bool IsActive()
        {
            return IsInActiveState();
        }

        public bool IsInActiveState()
        {
            return _blockState == BlockState.Black || _blockState == BlockState.Normal || _blockState == BlockState.Overlap;
        }

        public virtual bool IsValidState()
        {
            if (_blockState == BlockState.Normal || _blockState == BlockState.Black || _blockState == BlockState.Overlap || _blockState == BlockState.InCollection || _blockState == BlockState.InCollectionOverlap)
                return true;
            
            return false;
        }
        //是否可以激活
        public virtual bool CanBeActive(List<int> filter, bool ignoreCanRemove = false)
        {
            return IsInActiveState();
        }

        //屏蔽remove block handle
        public virtual bool IgnoreRemoveBlockHandle(Block block)
        {
            if(block._blockModel._blockData.blockType == (int)BlockTypeEnum.Funnel || block.BlockId == GameConst.NOVAILDID)
                return true;

            return false;
        }
       
        public bool IsActiveBlock(int filterId = -1)
        {
            foreach (var childId in _blockModel._blockData.children)
            {
                if (filterId > 0)
                {
                    if(childId == filterId)
                        continue;
                }
                
                Block childBlock = TileMatchGameManager.Instance.GetBlock(childId);
                if(childBlock.IsInActiveState())
                    return false;
                
                List<Block> appendBlock = TileMatchGameManager.Instance.GetAppendBlock(childId);
                if(appendBlock == null)
                    continue;
                
                foreach (var block in appendBlock)
                {
                    if(block.IsInActiveState())
                        return false;
                }
            }

            return true;
        }
       
        
        public virtual void SetState(BlockState blockState, bool isRefresh = true, bool isAnim = false)
        {
            _blockState = blockState;

            if(!isRefresh)
                return;
            
            RefreshView(isAnim);
        }

        public virtual void RefreshView(bool isAnim = false)
        {
            switch (_blockState)
            {
                case BlockState.Normal:
                    {
                        _blockView.DoGrayFade(0, isAnim);
                        _blockView.DoDarkFade(0, isAnim);
                        
                        _blockView.SetColliderEnable(true);
                    }
                    break;    
                case BlockState.InCollection:
                    {
                        _blockView.DoGrayFade(0, isAnim);
                        _blockView.DoDarkFade(0, isAnim);
                        
                        _blockView.SetColliderEnable(true);
                    }
                    break;
                case BlockState.Overlap:
                    {
                        _blockView.DoGrayFade(GameConst.Block_Gray_Alpha, isAnim);
                        _blockView.DoDarkFade(0, isAnim);
                        _blockView.SetColliderEnable(false);
                    }
                    break;    
                case BlockState.InCollectionOverlap:
                    {
                        _blockView.DoGrayFade(GameConst.Block_Gray_Alpha, isAnim);
                        _blockView.DoDarkFade(0, isAnim);
                        
                        _blockView.SetColliderEnable(false);
                    }
                    break;
                case BlockState.Black:
                    {
                        Color color = _blockView.DarkColor;
                        color.a = GameConst.Block_Dark_Alpha;
                        _blockView.DoDarkColor(color, isAnim);
                        _blockView.DoGrayFade(0, isAnim);
                        _blockView.SetColliderEnable(false);
                    }
                    break;
                case BlockState.Droping:
                case BlockState.InBanner:
                    {
                        _blockView.DoGrayFade(0, isAnim);
                        _blockView.DoDarkFade(0, isAnim);
                        _blockView.SetColliderEnable(false);
                    }
                break;
                case BlockState.TweeningHide:
                    {
                        _blockView.DoGrayFade(0, isAnim);
                        _blockView.DoDarkFade(0, isAnim);
                        _blockView.SetColliderEnable(false);
                    }
                    break;
                case BlockState.Hided:
                    {
                        _blockView.DoGrayFade(0, isAnim);
                        _blockView.DoDarkFade(0, isAnim);
                        _blockView.SetColliderEnable(false);
                    }
                    break;
            }
        }

        public bool IsSameCollider(Collider2D collider2D)
        {
            return _blockView._collider2D == collider2D;
        }

        public virtual FailTypeEnum CheckFailure(List<Block> blocks)
        {
            return FailTypeEnum.None;
        }

        public virtual int GetBlockNum()
        {
            if (_id == GameConst.NOVAILDID)
                return 0;

            return 1;
        }
    }
}