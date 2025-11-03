
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Asset;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public class ItemData
    {
        public int id;
        public int cnt;
    }

    public class ItemViewParam : UIViewParam
    {
        public ItemData data;
    }

    [AssetAddress("TMatch/Prefabs/UICommonItem")]
    public class UIItemView : UIView
    {
        protected override bool IsChildView => true;

        [ComponentBinder("Icon")] public Image icon;
        [ComponentBinder("NumberText")] public LocalizeTextMeshProUGUI text;
        [ComponentBinder("InfiniteTag")] public Transform infiniteTag;

        [ComponentBinder("TagImage/NumberText")]
        public LocalizeTextMeshProUGUI infiniteText;

        [ComponentBinder("TagImage")] public Transform tagImage;
        [ComponentBinder("Countless")] public Transform countLess;

        public ItemData ItemData;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);

            ItemViewParam data = param as ItemViewParam;
            Refresh(data);
        }

        public virtual void Refresh(ItemViewParam data)
        {
            ItemData = data.data;
            DragonPlus.Config.TMatchShop.ItemConfig cfg = TMatchShopConfigManager.Instance.GetItem(data.data.id);
            icon.sprite = ItemModel.Instance.GetItemSprite(cfg.id);

            infiniteTag.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
            tagImage.gameObject.SetActive(false);
            countLess.gameObject.SetActive(false);

            //显示隐藏
            if (cfg.GetItemType() == ItemType.TMEnergyInfinity)
            {
                infiniteTag.gameObject.SetActive(true);
            }

            if (cfg.GetItemInfinityIconType() == ItemInfinityIconType.None || cfg.GetItemInfinityIconType() == ItemInfinityIconType.NoTag)
            {
                text.gameObject.SetActive(true);
            }

            if (cfg.GetItemInfinityIconType() == ItemInfinityIconType.TagAndInfinity)
            {
                tagImage.gameObject.SetActive(true);
                countLess.gameObject.SetActive(true);
            }

            //数据
            string textValue = cfg.infinity ? CommonUtils.FormatPropItemTime((long)(cfg.infiniityTime * 1000)) : data.data.cnt.ToString();
            text.SetText(textValue);
            infiniteText.SetText(textValue);
        }

        public void Fly()
        {
            FlySystem.Instance.FlyItem(ItemData.id, ItemData.cnt, transform.position, FlySystem.Instance.GetTargetTransform(ItemData.id).position, null);
        }
    }
}