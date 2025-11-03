using System.Collections.Generic;
using SomeWhere;
using SomeWhereTileMatch;
using TileMatch.Game.Block;

namespace TileMatch.Game.Shuffle
{
    //保证当前牌面有3张可消除的牌
    public class ShuffleBlockStrategy_Default
    {
        public static void Shuffle(List<Block.Block> blocks)
        {
            Dictionary<int, List<Block.Block>> dicBlocks = new Dictionary<int, List<Block.Block>>();
            List<Block.Block> availableBlocks = new List<Block.Block>();
            List<Block.Block> topBlocks = new List<Block.Block>();

            foreach (var block in blocks)
            {
                if(!block.CanShuffle())
                    continue;
                
                availableBlocks.Add(block);
                if (!dicBlocks.ContainsKey(block.BlockId))
                    dicBlocks[block.BlockId] = new List<Block.Block>();
                
                dicBlocks[block.BlockId].Add(block);
                    
                if(block.GetBlockState() == BlockState.Normal)
                    topBlocks.Add(block);
            }

            List<int> blockIds = new List<int>();
            foreach (var kv in dicBlocks)
            {
                if(kv.Value.Count >= 3)
                    blockIds.Add(kv.Key);
            }

            int fullBlockId = -1;
            if (blockIds.Count > 0)
                fullBlockId = blockIds.RandomPickOne();

            if (fullBlockId > 0 && topBlocks.Count >= 3)
            {
                List<Block.Block> fullBlocks = dicBlocks[fullBlockId];

                for (int i = 0; i < 3; i++)
                {
                    var randomTopBlock = fullBlocks.RandomPickOne();
                    var fullBlock = topBlocks.RandomPickOne();
                    
                    fullBlock.Shuffle(randomTopBlock);
                    availableBlocks.Remove(fullBlock);
                    fullBlocks.Remove(randomTopBlock);
                    topBlocks.Remove(fullBlock);
                }
            }

            availableBlocks.Shuffle();
            foreach (var block in availableBlocks)
            {
                var shuffleBlock = availableBlocks.RandomPickOne();
                block.Shuffle(shuffleBlock);
            }
        }
    }
}