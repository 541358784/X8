using System;
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{
    public class UIGiftBagRewardCell : MonoBehaviour
    {
        private Image iconImage;
        private LocalizeTextMeshProUGUI lockNumText;
        private LocalizeTextMeshProUGUI normalNumText;
        private Button _infoButton;
        private GameObject _infinity;
        private void Awake()
        {
            iconImage = transform.Find("Icon").GetComponent<Image>();
            lockNumText = transform.Find("BlueText").GetComponent<LocalizeTextMeshProUGUI>();
            normalNumText = transform.Find("YellowText").GetComponent<LocalizeTextMeshProUGUI>();
            _infoButton = transform.Find("TipsBtn").GetComponent<Button>();
            _infoButton.gameObject.SetActive(false);
            _infinity = transform.Find("infinity").gameObject;
            _infinity.gameObject.SetActive(false);
        }

        public void InitData(int itemId, int num)
        {
            _infinity.gameObject.SetActive(false);
            DragonPlus.Config.TMatchShop.ItemConfig cfg = TMatchShopConfigManager.Instance.GetItem(itemId);
            iconImage.sprite = ItemModel.Instance.GetItemSprite(cfg.id);
            
            if (cfg.infinity)
            {
                lockNumText.SetText(CommonUtils.FormatPropItemTime((long)(cfg.infiniityTime * 1000)));
                normalNumText.SetText(CommonUtils.FormatPropItemTime((long)(cfg.infiniityTime * 1000)));
                
                if(cfg.type == (int)ItemType.TMEnergyInfinity)
                    _infinity.gameObject.SetActive(true);
            }
            else
            {
                lockNumText.SetText("x" + num.ToString());
                normalNumText.SetText("x" + num.ToString());
            }
        }

        public void UpdateStatus(bool isUnLock)
        {
            lockNumText.gameObject.SetActive(!isUnLock);
            normalNumText.gameObject.SetActive(isUnLock);
        }
    }
}