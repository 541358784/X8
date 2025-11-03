using System;
using DragonPlus;
using UnityEngine;


public class MergeBoardFullController : UIWindowController
{
    private Animator animator;

    private LocalizeTextMeshProUGUI contentText;

    public override void PrivateAwake()
    {
        animator = transform.GetComponent<Animator>();

        contentText = GetItem<LocalizeTextMeshProUGUI>("Text");
    }

    public void PlayAnim(string content, Action endCall)
    {
        if (animator == null)
        {
            endCall?.Invoke();
            return;
        }

        contentText?.SetTerm(content);
        StartCoroutine(CommonUtils.PlayAnimation(animator, "appear", "", endCall));
    }
}