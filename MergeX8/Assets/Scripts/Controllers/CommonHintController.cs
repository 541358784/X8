using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;

public class CommonHintController : UIWindowController
{
    private LocalizeTextMeshProUGUI hintText = null;

    private Animator hintAnimator = null;

    public override void PrivateAwake()
    {
        hintText = GetItem<LocalizeTextMeshProUGUI>("Root/BgImage/Text");

        hintAnimator = transform.GetComponent<Animator>();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        StartCoroutine(CommonUtils.PlayAnimation(hintAnimator, "appear", null,
            () => { CloseWindowWithinUIMgr(true); }));

        if (objs == null || objs.Length == 0)
            return;

        hintText.SetText((string) objs[0]);
    }
}