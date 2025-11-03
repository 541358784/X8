using System;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Store.Controller
{
    public class UIStoreItemBattlePass2 : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = transform.Find("Button").GetComponent<Button>();
            _button.onClick.AddListener(OnBtnBuy);
        }    
        
        protected  void Start()
        {
            InvokeRepeating("RefreshTime",0,1);
            InitCountDown();
        }

        public void RefreshTime()
        {
            if (!Activity.BattlePass_2.BattlePassModel.Instance.IsOpened() &&!Activity.BattlePass_2.BattlePassModel.Instance.IsPurchase())
            {
                gameObject.SetActive(false);
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
            }
        }

        private void OnBtnBuy()
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupBattlePass2BuyNew1,2);
        }
        
        private LocalizeTextMeshProUGUI _countDownText;
        public void InitCountDown()
        {
            _countDownText = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            InvokeRepeating("UpdateActivityTime", 0.01f, 1.0f);
        }
        void UpdateActivityTime()
        {
            if (!this)
                return;
            _countDownText.SetText(Activity.BattlePass_2.BattlePassModel.Instance.GetActivityLeftTimeString());
        }

        void OnDestroy()
        {
            CancelInvoke("UpdateActivityTime");
        }
    }
}