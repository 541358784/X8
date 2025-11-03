using System;
using DragonPlus;
using DragonU3DSDK.Asset;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class RewardItem : MonoBehaviour
{
    public Image _icon;
    public LocalizeTextMeshProUGUI _textCount;

    private void Awake()
    {
        _icon = transform.Find("Root/Image").GetComponent<Image>();
        _textCount = transform.Find("Root/TextBackground/Text").GetComponent<LocalizeTextMeshProUGUI>();
    }

    public void SetData(ResData resData)
    {
        var item = GlobalConfigManager.Instance.GetTableItem((int) resData.id);
        _textCount.gameObject.SetActive(resData.id != (int) UserData.ResourceId.NoAds);
        if (resData.id == (int) UserData.ResourceId.Infinity_Energy)
        {
            _textCount.SetText(TimeUtils.GetTimeString(resData.count, true));
        }
        else
        {
            _textCount.SetText(resData.count.ToString());
        }

        _icon.gameObject.SetActive(true);
        _icon.sprite = UserData.GetResourceIcon(resData.id, UserData.ResourceSubType.Big);
        //_icon.SetNativeSize();
    }

    public void HideOrShow(bool hideOrShow)
    {
        gameObject.SetActive(hideOrShow);
    }
}