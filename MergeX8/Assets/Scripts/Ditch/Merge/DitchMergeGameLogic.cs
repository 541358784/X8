using System;

namespace Ditch.Merge
{
    public class DitchMergeGameLogic: Singleton<DitchMergeGameLogic>
    {
        public void EnterGame(int id, Action<bool> action)
        {
            ExitGame(-1);
            
            UIManager.Instance.OpenUI(UINameConst.UIDitchMergeMain, id, action);
        }
        
        public bool ExitGame(int id, bool isInit = false)
        {
            UIManager.Instance.CloseUI(UINameConst.UIDitchMergeMain, true);
            return false;
        }
    }
}