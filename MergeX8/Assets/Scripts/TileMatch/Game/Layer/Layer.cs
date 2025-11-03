using System;
using System.Collections.Generic;
using LayoutData;
using TileMatch.Game.Block;
using UnityEngine;

namespace TileMatch.Game.Layer
{
    public class Layer : ILoadView, IDestroy
    {
        public LayerModel _layerModel;
        public LayerView _layerView;

        public List<Block.Block> _blocks;

        public Layer(LayoutData.Layout layout, LayoutData.Layer layerData, int index)
        {
            _layerModel = new LayerModel(this, layout, layerData, index);
            _layerView = new LayerView(this);
        }
        
        public void LoadView(Transform parent)
        {
            _layerView.LoadView(parent);

            InitBlock();
        }
        
        public void Destroy()
        {
            if (_blocks != null)
            {
                _blocks.ForEach(a=>a.Destroy());
                _blocks.Clear();
            }
            _blocks = null;
            
            _layerModel.Destroy();
            _layerModel = null;
            
            _layerView.Destroy();
            _layerView = null;
        }
        
        public void InitBlock()
        {
            _blocks = new List<Block.Block>();

            for (int i = 0; i < _layerModel._layerData.layerBlocks.Count; i++)
            {
                LayerBlock layerBlock = _layerModel._layerData.layerBlocks[i];

                Block.Block block = BlockFactory.CreateBlock(this, layerBlock, i+1, _layerView._root.transform);
                
                if (block != null)
                {
                    _blocks.Add(block);
                }
            }
        }
    }
}