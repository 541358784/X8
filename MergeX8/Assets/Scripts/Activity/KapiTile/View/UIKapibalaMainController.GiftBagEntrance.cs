
using UnityEngine;
using UnityEngine.UI;

public partial class UIKapiTileMainController : UIWindowController
{
    private GiftBagEntrance GiftBagBtn;
    public void InitGiftBagEntrance()
    {
        GiftBagBtn = transform.Find("Root/GiftButton").gameObject.AddComponent<GiftBagEntrance>();
        GiftBagBtn.transform.Find("RedPoint").gameObject.SetActive(false);
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
                UIPopupKapiTileGiftBagController.Open();
            });
        }
    }
}