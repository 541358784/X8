/*
 * @file ContactUsMyCellController
 * 联系我们 - 自己说的话
 * @author lu
 */

using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class ContactUsMyCellController : MonoBehaviour
{
    UIPopupContactUsController Parent { get; set; }
    UserComplainMessage ItemData { get; set; }
    Text MessageText { get; set; }
    Text TimeText { get; set; }

    // private Image _icon;
    // private Image _headFrame;
    
    private RectTransform _headIconRoot;
    private HeadIconNode HeadIcon;
    
    // Use this for initialization
    void Start()
    {
        MessageText = transform.Find("Image/Image/Text").GetComponent<Text>();
        TimeText = transform.Find("Image/Image/Time").GetComponent<Text>();
        
        ReloadFromData();
    }

    public void SetData(UIPopupContactUsController parent, UserComplainMessage itemData)
    {
        Parent = parent;
        ItemData = itemData;

        transform.Find("HeadImage/PersonImage")?.gameObject.SetActive(false);
        var _headIconRootNode = transform.Find("HeadImage");
        if (_headIconRootNode)
            _headIconRoot = _headIconRootNode as RectTransform;
        if (_headIconRoot == null)
            return;
        UpdateHeadIcon(null);
    }
    private void UpdateHeadIcon(BaseEvent e)
    {
        if (_headIconRoot)
        {
            if (HeadIcon)
            {
                HeadIcon.SetAvatarViewState(HeadIconUtils.GetMyViewState());
            }
            else
            {
                HeadIcon = HeadIconNode.BuildHeadIconNode(_headIconRoot,HeadIconUtils.GetMyViewState());
            }
        }
    }  

    public void ReloadFromData()
    {
        MessageText.text = ItemData.Message;
        TimeText.text = DragonU3DSDK.Utils.GetTimeStampDateString(ItemData.CreatedAt);
        Parent.GotoScrollviewEnd();
    }
}