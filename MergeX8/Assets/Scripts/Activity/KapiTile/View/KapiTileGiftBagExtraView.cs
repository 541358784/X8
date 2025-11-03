using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class KapiTileGiftBagExtraView : MonoBehaviour
{
    private Button Btn;
    private List<CommonRewardItem> Items = new List<CommonRewardItem>();
    public void Init()
    {
        var defaultItem = transform.Find("RewardGroup/Item");
        defaultItem.gameObject.SetActive(false);
        var rewards = CommonUtils.FormatReward(KapiTileModel.Instance.GlobalConfig.RewardIdShow,
            KapiTileModel.Instance.GlobalConfig.RewardNumShow);
        foreach (var item in Items)
        {
            Destroy(item.gameObject);
        }
        Items.Clear();
        for (var i = 0; i < rewards.Count; i++)
        {
            var rewardItem = Instantiate(defaultItem, defaultItem.parent).gameObject.AddComponent<CommonRewardItem>();
            rewardItem.gameObject.SetActive(true);
            rewardItem.Init(rewards[i]);
            Items.Add(rewardItem);
        }

        Btn = transform.Find("Button").GetComponent<Button>();
        Btn.onClick.AddListener(() =>
        {
            UIPopupKapiTileGiftBagController.Open();
        });
    }
}
            