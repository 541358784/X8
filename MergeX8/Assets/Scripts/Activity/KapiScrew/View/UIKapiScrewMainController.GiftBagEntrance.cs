
using UnityEngine;
using UnityEngine.UI;

public partial class UIKapiScrewMainController : UIWindowController
{
    private GiftBagEntrance GiftBagBtn;
    public void InitGiftBagEntrance()
    {
        GiftBagBtn = transform.Find("Root/Aux_KapibalaOptionalGift").gameObject.AddComponent<GiftBagEntrance>();
        GiftBagBtn.transform.Find("RedPoint").gameObject.SetActive(false);
        GiftBagBtn.transform.Find("FreeTag").gameObject.SetActive(false);
        GiftBagBtn.Init();
    }

    public class GiftBagEntrance : MonoBehaviour
    {
        private Button Btn;
        public void Init()
        {
            Btn = GetComponent<Button>();
            Btn.onClick.AddListener(() =>
            {
                UIPopupKapiScrewShopController.Open();
            });
            
        }
    }
}