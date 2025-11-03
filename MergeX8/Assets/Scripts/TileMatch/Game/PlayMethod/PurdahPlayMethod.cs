using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus.Config.TileMatch;
using GamePool;
using TileMatch.Game.Block;
using UnityEngine;

namespace TileMatch.Game.PlayMethod
{
    public class PurdahPlayMethod: PlayMethodBase
    {
        private List<Block.Block> _purdahBlocks = new List<Block.Block>();
        private Dictionary<int, List<Block.Block>> _purdahLayerBlocks = new Dictionary<int, List<Block.Block>>();

        private List<PurdahMethodLogic> _purdahMethodLogics = new List<PurdahMethodLogic>();
        
        // private Dictionary<string, int> _edgeInfo = new Dictionary<string, int>()
        // {
        //     { "左上", 0 },
        //     { "右上", 1 },
        //     { "左下", 2 },
        //     { "右下", 3 },
        //     { "上边", 4 },
        //     { "下边", 5 },
        //     { "左边", 6 },
        //     { "右边", 7 },
        //     { "中间", 8 },
        // };

        private string[] _poolObjNames = new[]
        {
            ObjectPoolName.TileMatchBlock_PurdahRoot_1,
            ObjectPoolName.TileMatchBlock_PurdahRoot_2,
            ObjectPoolName.TileMatchBlock_PurdahRoot_3,
        };

        private Dictionary<GameObject, string> _poolObjs = new Dictionary<GameObject, string>();
        
        public override void Init(params object[] param)
        {
            _purdahBlocks = (List<Block.Block>)param[0];

            int levelId = TileMatchGameManager.Instance.LevelId;
            var levelConfig = TileMatchConfigManager.Instance.TileLevelList.Find(a => a.id == levelId);
            var layoutGroup = TileMatchLayoutManager.Instance.GetLayoutGroup(levelConfig.layoutId);
            foreach (var layout in layoutGroup.layout)
            {
                if(!layout.isMaskLayout)
                    continue;
                
                if(!_purdahLayerBlocks.ContainsKey(layout.id))
                    _purdahLayerBlocks.Add(layout.id, new List<Block.Block>());
                
                foreach (var layer in layout.layers)
                {
                    List<Block.Block> topBlocks = new List<Block.Block>();
                    List<Block.Block> allBlocks = new List<Block.Block>();
                    int minIndex = int.MaxValue;
                    int maxIndex = int.MinValue;
                    foreach (var block in layer.layerBlocks)
                    {
                        int blockId = block.id;
                        Block.Block blockData = TileMatchGameManager.Instance.GetBlock(blockId);
                        if (block.blockParam == "0" || block.blockParam == "1" || block.blockParam == "4")
                        {
                            topBlocks.Add(blockData);
                        }

                        int index = blockData._blockModel._blockData.index_X;
                        minIndex = Math.Min(minIndex, index);
                        maxIndex = Math.Max(maxIndex, index);
                         
                        allBlocks.Add(blockData);
                        
                        GetAllParentBlock(blockData, _purdahLayerBlocks[layout.id]);
                    }

                    Vector3 centrePosition = Vector3.zero;
                    centrePosition.z = -5;
                    float minOffsetX = int.MaxValue;
                    float maxOffsetX = int.MinValue;
                    foreach (var topBlock in topBlocks)
                    {
                        minOffsetX = Mathf.Min(topBlock._blockModel.localPosition.x, minOffsetX);
                        maxOffsetX = Mathf.Max(topBlock._blockModel.localPosition.x, maxOffsetX);
                        centrePosition.y = topBlock._blockModel.localPosition.y;
                    }

                    centrePosition.x = minOffsetX + (maxOffsetX-minOffsetX)/2;
                    centrePosition.y += 0.25f;

                    int num = maxIndex - minIndex;
                    int poolIndex = 0;
                    string poolName = "";
                    if (num < 3)
                    {
                        poolName = _poolObjNames[0];
                        poolIndex = 0;
                    }
                    else if (num < 5)
                    {
                        poolName = _poolObjNames[1];
                        poolIndex = 1;
                    }
                    else
                    {
                        poolName = _poolObjNames[2];
                        poolIndex = 2;
                    }
                    
                    var spawn = GamePool.ObjectPoolManager.Instance.Spawn(poolName);
                    CommonUtils.AddChild(TileMatchGameManager.Instance.LayoutRoot, spawn.transform);
                    spawn.transform.localPosition = centrePosition;

                    _poolObjs.Add(spawn, poolName);
                    
                    PurdahMethodLogic logic = spawn.GetComponent<PurdahMethodLogic>();
                    if (logic == null)
                        logic = spawn.AddComponent<PurdahMethodLogic>();
                    
                    _purdahMethodLogics.Add(logic);
                    logic.Init(layout.layerParam, allBlocks, layout.id, poolIndex);
                }
            }
        }

        private void GetAllParentBlock(Block.Block block, List<Block.Block> blocks)
        {
            if(block != null && !blocks.Contains(block))
                blocks.Add(block);
            
            if(block._blockModel._blockData.parent == null || block._blockModel._blockData.parent.Count == 0)
                return;
            
            foreach (var id in block._blockModel._blockData.parent)
            {
                Block.Block blockData = TileMatchGameManager.Instance.GetBlock(id);
                GetAllParentBlock(blockData, blocks);
            }
        }
        public override void Destroy()
        {
            _purdahBlocks.Clear();
            _purdahLayerBlocks.Clear();
            
            foreach (var kv in _poolObjs)
            {
                GamePool.ObjectPoolManager.Instance.DeSpawn(kv.Value, kv.Key);
            }
            _poolObjs.Clear();
            
            _purdahMethodLogics.Clear();
        }

        public override void DisappearBlock(List<Block.Block> blocks, bool isMagic = false)
        {
            for(int i = 0; i < _purdahMethodLogics.Count; i++)
            {
                var logic = _purdahMethodLogics[i];
                if(!logic.Broken(blocks, () =>
                   {
                       if (_poolObjs.ContainsKey(logic.gameObject))
                       {
                           GamePool.ObjectPoolManager.Instance.DeSpawn(_poolObjs[logic.gameObject], logic.gameObject);
                       }
                       
                       foreach (var logicBlock in logic.Blocks)
                       {
                           logicBlock.SetState(BlockState.Hided);

                           if (logicBlock._blockModel._blockData.parent != null)
                           {
                               foreach (var id in logicBlock._blockModel._blockData.parent)
                               {
                                   var opBlock = TileMatchGameManager.Instance.GetBlock(id);
                                   if(opBlock != null)
                                       opBlock.BeforeRemoveBlock(new List<Block.Block>(){logicBlock});
                               }
                           }
                       }
                   }, isMagic))
                    continue;

                _purdahLayerBlocks.Remove(logic.LayoutId);
                _purdahMethodLogics.Remove(logic);

                i--;
            }
        }
        
        public override void Magic_AfterRemoveBlock(List<Block.Block> blocks)
        {
            DisappearBlock(blocks, true);
        }
        
        public override bool IsLock(Block.Block block)
        {
            foreach (var kv in _purdahLayerBlocks)
            {
                if (kv.Value.Contains(block))
                    return true;
            }

            return false;
        }
        
        public List<GameObject> GetPlayMethodGameObjects()
        {
            return _poolObjs.Keys.ToList();
        }
        
        public List<Block.Block> GetPurdahBlocks()
        {
            return _purdahBlocks;
        }
    }
}