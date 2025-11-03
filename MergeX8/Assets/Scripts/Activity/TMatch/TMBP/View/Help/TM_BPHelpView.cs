//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月24日 星期二
//describe    :   
//-----------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{
    /// <summary>
    /// tm帮助
    /// </summary>
    public class TM_BPHelpView : UIWindowController
    {
        private const string PREFAB_PATH = "Prefabs/Activity/TMatch/TMBP/TM_BPHelpView";
        
        public static TM_BPHelpView Open()
        {
            return UIManager.Instance.OpenWindow<TM_BPHelpView>(PREFAB_PATH);
        }

        private Animator animator;
        private Button button;

        public override void PrivateAwake()
        {
            animator = transform.GetComponent<Animator>();
            button = transform.GetComponent<Button>();

            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            animator.Play("BattlePassMainHelp_disappear");
            closed();
        }

        private void closed()
        {
            CloseWindowWithinUIMgr(true);
        }
        
        protected override void OnBackButtonCallBack()
        {
            base.OnBackButtonCallBack();
            OnClick();
        }
    }
}