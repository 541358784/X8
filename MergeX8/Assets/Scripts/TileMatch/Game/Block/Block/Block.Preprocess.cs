using System;
using System.Collections.Generic;

namespace TileMatch.Game.Block
{
    public partial class Block : IPreprocess
    {
        //准备预处理
        public virtual List<Block> PreparationPreprocess()
        {
            return new List<Block>() { this };
        }

        //执行预处理
        public virtual void ExecutePreprocess(List<Block> blocks)
        {
        }
    }
}