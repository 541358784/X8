using DragonPlus;
using DragonPlus.Config.TMatchShop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public class WeeklyChallengeItemViewParam : ItemViewParam
    {
        public ItemData buffData;
    }

    public class WeeklyChallengeItemView : UIItemView
    {
        [ComponentBinder("Double/NumberText")] private LocalizeTextMeshProUGUI doubleNumberText;
        [ComponentBinder("Countless/Image")] private Image countlessImage;
        [ComponentBinder("Double")] public Transform doubleGroup;
        [ComponentBinder("Coin")] private Transform coin;
        [ComponentBinder("Coin/NumberText")] private LocalizeTextMeshProUGUI coinNumberText;

        private Color grayColor = new Color(0.5f, 0.5f, 0.5f);
        private Color infiniteTextColor;
        private Color doubleNumberTextColor;
        private Color coinNumberTextColor;

        public override void Refresh(ItemViewParam data)
        {
            base.Refresh(data);

            WeeklyChallengeItemViewParam derivedData = data as WeeklyChallengeItemViewParam;

            //记录
            infiniteTextColor = infiniteText.transform.GetComponent<TextMeshProUGUI>().color;
            doubleNumberTextColor = doubleNumberText.transform.GetComponent<TextMeshProUGUI>().color;
            coinNumberTextColor = coinNumberText.transform.GetComponent<TextMeshProUGUI>().color;

            DragonPlus.Config.TMatchShop.ItemConfig cfg = TMatchShopConfigManager.Instance.GetItem(data.data.id);
            text.gameObject.SetActive(false);
            coin.gameObject.SetActive(false);
            doubleGroup.gameObject.SetActive(false);

            //显示隐藏
            if (cfg.GetItemInfinityIconType() == ItemInfinityIconType.None || cfg.GetItemInfinityIconType() == ItemInfinityIconType.NoTag)
            {
                coin.gameObject.SetActive(true);
            }

            //数据
            string textValue = cfg.infinity ? CommonUtils.FormatPropItemTime((long)(cfg.infiniityTime * 1000)) : data.data.cnt.ToString();
            coinNumberText.SetText(textValue);

            //如果是buff
            if (derivedData.buffData != null)
            {
                DragonPlus.Config.TMatchShop.ItemConfig buffCfg = TMatchShopConfigManager.Instance.GetItem(derivedData.buffData.id);
                coin.gameObject.SetActive(false);
                doubleGroup.gameObject.SetActive(true);
                tagImage.gameObject.SetActive(true);

                infiniteText.SetText(CommonUtils.FormatPropItemTime((long)(buffCfg.infiniityTime * 1000)));
            }
        }

        public void SetColor(bool gray)
        {
            icon.color = gray ? grayColor : Color.white;
            infiniteTag.GetComponent<Image>().color = gray ? grayColor : Color.white;
            text.transform.GetComponent<TextMeshProUGUI>().color = gray ? grayColor : Color.white;
            tagImage.GetComponent<Image>().color = gray ? grayColor : Color.white;
            infiniteText.transform.GetComponent<TextMeshProUGUI>().color = gray ? infiniteTextColor * new Color(0.5f, 0.5f, 0.5f) : infiniteTextColor;
            doubleGroup.GetComponent<Image>().color = gray ? grayColor : Color.white;
            doubleNumberText.transform.GetComponent<TextMeshProUGUI>().color = gray ? doubleNumberTextColor * new Color(0.5f, 0.5f, 0.5f) : doubleNumberTextColor;
            countLess.GetComponent<Image>().color = gray ? grayColor : Color.white;
            countlessImage.color = gray ? grayColor : Color.white;
            coin.GetComponent<Image>().color = gray ? grayColor : Color.white;
            coinNumberText.transform.GetComponent<TextMeshProUGUI>().color = gray ? coinNumberTextColor * new Color(0.5f, 0.5f, 0.5f) : coinNumberTextColor;
        }
    }
}