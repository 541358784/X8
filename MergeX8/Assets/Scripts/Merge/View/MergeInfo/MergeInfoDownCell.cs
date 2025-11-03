using System;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class MergeInfoDownCell : MonoBehaviour
{
    private Button _clickSelf;
    private Button _clickinfo;
    private Image _image;
    private TableMergeItem _mergeItem;

    private void Awake()
    {
        _image = transform.Find("Icon").GetComponent<Image>();
        _clickSelf = transform.GetComponent<Button>();
        _clickSelf.onClick.AddListener(() =>
        {
            if(GuideSubSystem.Instance.IsTargetTypeGuide(GuideTargetType.CloseItemInfo))
                return;

            MergeInfoView.Instance.OpenMergeInfo(_mergeItem);
        });
        _clickinfo = transform.Find("TipsBtn").GetComponent<Button>();
        _clickinfo.onClick.AddListener(() =>
        {
            MergeInfoView.Instance.OpenMergeInfo(_mergeItem);
        });
    }

    public void Init(TableMergeItem mergeItem)
    {
        _mergeItem = mergeItem;
        _image.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeItem.image);
    }
}