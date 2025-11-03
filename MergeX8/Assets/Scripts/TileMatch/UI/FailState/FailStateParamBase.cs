using Framework;
using TileMatch.Game;

namespace TileMatch.UI.FailState
{
    public class FailStateParamBase : FsmStateParamBase
    {
        public FailTypeEnum FailTypeEnum;
        public BlockTypeEnum BlockTypeEnum;
    }
}