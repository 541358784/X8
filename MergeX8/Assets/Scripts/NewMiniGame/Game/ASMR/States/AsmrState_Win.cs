using ASMR;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Framework.UI;
using MiniGame;
using Scripts.UI;

namespace fsm_new
{
    public class AsmrState_Win : AsmrState_Base
    {
        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);
            
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameFinish, ASMRModel.Instance.AttData.chapterId.ToString(), ASMRModel.Instance.AttData.levelId.ToString());

            var rewardData = MiniGameModel.Instance.ClaimLevelReward(ASMRModel.Instance.AttData.chapterId, ASMRModel.Instance.AttData.levelId);

            UIAsmrWin.Open(rewardData);

            //先注释
            //BITool.SendGameEvent(BiEventMatchRush3D.Types.GameEventType.GameEventAsmrResult, ASMRModel.Instance.AttData.levelId.ToString(), "win");
        }
    }
}