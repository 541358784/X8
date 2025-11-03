using System;
using DragonPlus;
using Framework;
using Gameplay.UI.EnergyTorrent;
using UnityEngine;
using UnityEngine.UI;

public class UIEnergyTorrentTipsController : UIWindowController
{
    private Animator _animator;
    private LocalizeTextMeshProUGUI _text;
    public override void PrivateAwake()
    {
        _text = GetItem<LocalizeTextMeshProUGUI>("Root/Text");
        _animator = GetComponent<Animator>();
        GetComponent<Canvas>().sortingOrder = 500;
      
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
    }
    public void PlayAnim(string content, Action endCall)
    {
        if (_animator == null)
        {
            endCall?.Invoke();
            return;
        }

        _text?.SetTerm(content);
        StartCoroutine(CommonUtils.PlayAnimation(_animator, "appear", "", endCall));
    }
    
}