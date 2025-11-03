using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIHappyGoHelpController : UIWindowController
{
    private Button _closeBtn;
    private RectTransform content;
    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/MiddleGroup/Scroll View/Viewport/Content/CloseButton");
        _closeBtn.onClick.AddListener(OnClose);
        content=transform.Find("Root/MiddleGroup/Scroll View/Viewport/Content") as RectTransform;
     
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        // CommonUtils.DelayedCall(1.7F, () =>
        // {
        //     int moveDes = 1288;
        //     float moveTime = 3f;
        //     if (CommonUtils.IsLE_16_10())
        //     {
        //         moveDes = 1641;
        //         moveTime = 4.5f;
        //     }
        //     
        //     content.DOLocalMoveY(moveDes,moveTime).OnComplete(() =>
        //     {
        //         _canClose = true;
        //     });
        // });
    }

    private void OnClose()
    {
    
        CloseWindowWithinUIMgr(true);
       
    }
    
}
