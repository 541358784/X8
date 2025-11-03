namespace TMatch
{


    public interface TMatchLevelController
    {
        TMatchLevelData LevelData { get; }
        TMatchLevelStateData LevelStateData { get; }
        void Build(FsmParamTMatch fsmParamTMatch);
        TMGameType GameType { get; }
        FsmParamTMatch Param();
        void OnWin();
        void OnFail(uint failReason);
    }
}