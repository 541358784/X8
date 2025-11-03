using Decoration;
using Decoration.DynamicMap;
using Farm.Model;

namespace TileMatch.fsm
{
    public class SceneFsmTileMatch : IFsmState
    {
        public StatusType Type => StatusType.Login;
        
        public void Enter(params object[] objs)
        {
            int levelId = (int)objs[0];
            FarmModel.Instance.AnimShow(false);
            UIHomeMainController.HideUI(false, true);
            PlayerManager.Instance.HidePlayer();
            DecoManager.Instance.CurrentWorld.HideByPosition();
            DynamicMapManager.Instance.PauseLogic = true;
            
            GamePool.ObjectPoolManager.Instance.InitPool();
            
            TileMatchRoot.Instance.LoadBoard(levelId);
        }

        public void Update(float deltaTime)
        {
        }

        public void LateUpdate(float deltaTime)
        {
        }

        public void Exit()
        {
            PlayerManager.Instance.RecoverPlayer();
            DecoManager.Instance.CurrentWorld.ShowByPosition();
            // DynamicMapManager.Instance.PauseLogic = false;
            TileMatchRoot.Instance._matchMainCamera.gameObject.SetActive(false);
            TileMatchRoot.Instance.DestroyBoard();
            GamePool.ObjectPoolManager.Instance.DestroyPool();
            XUtility.WaitFrames(1, () =>
            {
                global::UIManager.Instance.UpdateUIOrder();
            });

        }
    }
}