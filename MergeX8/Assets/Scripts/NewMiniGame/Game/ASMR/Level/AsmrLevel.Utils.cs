using DragonU3DSDK.Storage;
using UnityEngine;
using ASMR;
using Framework.UI;
namespace fsm_new
{
    public partial class AsmrLevel
    {
        private static AndroidJavaObject _vib;

        public void ResetAllNormals()
        {
            gameObject.SetActive(false);
            gameObject_fake.gameObject.SetActive(true);

            InitMonos(gameObject_fake);
        }

        private void HideNode_Exit(AsmrStep step)
        {
            if (step == null) return;
            if (step.Config.HidePaths_Exit == null) return;

            if (step.Config.HidePaths_Exit.Count <= 0) return;

            foreach (var path in step.Config.HidePaths_Exit)
            {
                var node = transform.Find(path);
                if (node) node.gameObject.SetActive(false);
            }
        }
        
        private void IsShowChangeAnimation(AsmrStep step)
        {
            if (step == null) return;
            switch (step.Config.TransitionAnimationType)
            {
                case 1:
                    UIASMRFinish.Open();
                    break;
                case 2:
                    Framework.UI.UIManager.Instance.Close<UIASMRFinish>();
                    break;
            }
        }

        /// <summary>
        /// 结束扫光动画，关闭一些node
        /// </summary>
        public void HideNode_Finish()
        {
            if (Config == null) return;
            if (Config.HidePaths_Finish == null) return;

            if (Config.HidePaths_Finish.Count <= 0) return;

            foreach (var path in Config.HidePaths_Finish)
            {
                var node = gameObject_fake.transform.Find(path);
                if (node) node.gameObject.SetActive(false);
            }
        }

        public void ChangeState_Win()
        {
            _fsm.ChangeState<AsmrState_Win>(_fsm.CurrentState.Param);
        }
    }
}