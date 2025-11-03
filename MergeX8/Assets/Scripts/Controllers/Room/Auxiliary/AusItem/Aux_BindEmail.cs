using Gameplay.UI.BindEmail;
using UnityEngine.UI;

namespace Gameplay.UI.Room.Auxiliary.AusItem
{
    public class Aux_BindEmail : Aux_ItemBase
    {
        protected override void Awake()
        {
            base.Awake();
        
            InvokeRepeating("UpdateUI", 0, 1);
            
            transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                UIManager.Instance.OpenWindow(UINameConst.UIPopupBindEmail);
            });
        }
        
        public override void UpdateUI()
        {
            bool hideEmail = UIPopupBindEmailController.HideEmail();
            if (hideEmail)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(UIPopupBindEmailController.CanShowBindEmailButton());
            }
        }
    }
}