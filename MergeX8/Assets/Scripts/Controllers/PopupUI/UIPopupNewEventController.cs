using System.Collections;
using System.Collections.Generic;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Tsp;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;
using BiEvent = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupNewEventController : UIWindowController
{
    private LocalizeTextMeshProUGUI descText;

    private Image showImage = null;

    public override void PrivateAwake()
    {
        Button close = GetItem<Button>("Root/BgPopupBoandBig/ButtonClose");
        close.onClick.AddListener(() => { CloseWindowWithinUIMgr(true); });

        Button update = GetItem<Button>("Root/ButtonGroup/ButtonUpdate");
        update.onClick.AddListener(() =>
        {
            Global.OpenAppStore();
            CloseWindowWithinUIMgr(true);
        });

        showImage = GetItem<Image>("Root/MiddleGroup/Mask/Icon");

        Texture2D texture = NewEventModel.Instance.wwwImage;
        if (texture != null)
            showImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.0f, 0.0f));


        descText = GetItem<LocalizeTextMeshProUGUI>("Root/MiddleGroup/TextGroup/Text1");
        descText.SetText(NewEventModel.Instance.GetCustomizeMsg());
    }

    private void OnDestroy()
    {
        NewEventModel.Instance.LastPopWindowTime = CommonUtils.GetTimeStamp();
    }
}