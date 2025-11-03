using LayoutData;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockFactory
    {
        public static Block CreateBlock(Layer.Layer layer, LayerBlock blockData, int index, Transform root)
        {
            Block block = null;
            switch ((BlockTypeEnum)blockData.blockType)
            {
                case BlockTypeEnum.Normal:
                {
                    block = CreateGlueLineBlock(layer, blockData, index);
                    if(block != null)
                        break;
                    
                    block = CreateFunnelLineBlock(layer, blockData, index);
                    if(block != null)
                        break;
                    
                    block = new Block(layer, blockData, index);
                    break;
                } 
                case BlockTypeEnum.Lock:
                {
                    block = new BlockLock(layer, blockData, index);
                    break;
                }
                case BlockTypeEnum.Glue:
                {
                    block = new BlockGlue(layer, blockData, index);
                    break;
                }
                case BlockTypeEnum.Funnel:
                {
                    block = new BockFunnel(layer, blockData, index);
                    break;
                }
                case BlockTypeEnum.Ice:
                {
                    block = new BlockIce(layer, blockData, index);
                    break;
                }
                case BlockTypeEnum.Grass:
                {
                    block = new BlockGrass(layer, blockData, index);
                    break;
                }
                case BlockTypeEnum.Bomb:
                {
                    block = new BlockBomb(layer, blockData, index);
                    break;
                }
                case BlockTypeEnum.Curtain:
                {
                    block = new BlockCurtain(layer, blockData, index);
                    break;
                }
                case BlockTypeEnum.Wrapper:
                {
                    block = new BlockWrapper(layer, blockData, index);
                    break;
                }
                case BlockTypeEnum.Gold:
                {
                    block = new Block(layer, blockData, index);
                    break;
                }
                case BlockTypeEnum.Frog:
                {
                    block = new Block(layer, blockData, index);
                    
                    TileMatchGameManager.Instance.AddFrogBlock(block);
                    break;
                }
                case BlockTypeEnum.Purdah:
                {
                    block = new BlockPurdah(layer, blockData, index);
                    
                    TileMatchGameManager.Instance.AddPurdahBlock(block);
                    break;
                }
                case BlockTypeEnum.Plane:
                {
                    block = new BlockPlane(layer, blockData, index);
                    break;
                }
                default:
                {
                    block = new Block(layer, blockData, index);
                    break;
                }
            }

            TileMatchGameManager.Instance.RegisterBlock(block);
            
            block.LoadView(root);
            
            return block;
        }

        private static Block CreateGlueLineBlock(Layer.Layer layer, LayerBlock blockData, int index)
        {
            var neighbors = blockData.neighbors.Find(a => a.neighborType == (int)NeighborEnum.Left);
            if (neighbors == null)
                return null;
            
            var block = layer._layerModel._layerData.layerBlocks.Find(a => a.id == neighbors.id);
            if (block == null)
                return null;
            
            if (block.blockType != (int)BlockTypeEnum.Glue)
                return null;
            
            return new BlockGlueLink(layer, blockData, index);
        }
        
        private static Block CreateFunnelLineBlock(Layer.Layer layer, LayerBlock blockData, int index)
        {
            if (blockData.blockId != GameConst.NOVAILDID)
                return null;
            
            var neighbors = blockData.neighbors.Find(a => a.neighborType == (int)NeighborEnum.Up);
            if (neighbors == null)
                return null;
            
            var block = layer._layerModel._layerData.layerBlocks.Find(a => a.id == neighbors.id);
            if (block == null)
                return null;
            
            if (block.blockType != (int)BlockTypeEnum.Funnel)
                return null;
            
            return new BlockFunnelLink(layer, blockData, index);
        }
    }
}