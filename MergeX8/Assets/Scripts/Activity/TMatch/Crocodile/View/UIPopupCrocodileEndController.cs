using System;
using System.Threading.Tasks;
using TMatch;
using UnityEngine.UI;

namespace Activity.TMatch.Crocodile.View
{
    [AssetAddress("Prefabs/Activity/TMatch/Crocodile/UIPopupCrocodileEnd")]
    public class UIPopupCrocodileEndController: UIPopup
    {
        public override void OnViewOpen(UIViewParam data)
        {
            base.OnViewOpen(data);
            
            transform.Find($"Root/StayButton").GetComponent<Button>().onClick.AddListener(CloseOnClick);
            transform.Find($"Root/ButtonClose").GetComponent<Button>().onClick.AddListener(CloseOnClick);
            transform.Find($"Root/QuitButton").GetComponent<Button>().onClick.AddListener(QuitOnClick);
        }
        
        public override Task OnViewClose()
        {
            return base.OnViewClose();
        }
        
        private void CloseOnClick()
        {
            UIViewSystem.Instance.Close<UIPopupCrocodileEndController>();
        }

        private void QuitOnClick()
        {
            CloseOnClick();

            Action action = () =>
            {
                UIViewSystem.Instance.Open<UITMatchLevelInterruptController>();

            };
            UICrocodileMainController.Open(action);

        }
    }
}