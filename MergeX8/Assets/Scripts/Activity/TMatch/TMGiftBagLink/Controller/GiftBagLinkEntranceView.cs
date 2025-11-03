using DragonPlus;
using UnityEngine.UI;

namespace TMatch
{
    public class GiftBagLinkEntranceView :  UIEntranceBase<CrocodileActivityModel>
    {
        private bool _isOpen = false;
        private const string PREFAB_PATH = "Prefabs/Activity/TMatch/TMGiftBagLink/UIGiftBagLink";
        private LocalizeTextMeshProUGUI _time;
        
        protected override void Awake()
        {
            _time = transform.Find("TipsBG/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _isOpen = TMGiftBagLinkModel.Instance.IsOpened();
            InvokeRepeating("UpdateUI",0,1);
            
            transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnClick();
            });
        }
        
        protected void UpdateUI()
        {
            if(!gameObject.activeSelf)
                return;
            
            _time.SetText(TMGiftBagLinkModel.Instance.GetActivityLeftTimeString());
            gameObject.SetActive(TMGiftBagLinkModel.Instance.IsOpened());
        }
        
        protected override void OnClick()
        {
            base.OnClick();

            Open();
        }

        public static void Open()
        {
            UIManager.Instance.OpenWindow<UIGiftBagLinkController>(PREFAB_PATH, null);
        }
    }
}