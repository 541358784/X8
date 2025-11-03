using UnityEngine;
using System.Collections;
using Decoration;
using UnityEngine.UI;

public class ComingSoonTipComponent : FollowTargetBase
{
    private Animator _animator;
    private void Awake()
    {
        transform.GetComponent<Button>().onClick.AddListener(onButtonClicked);
        _animator = transform.Find("Tips").GetComponent<Animator>();
    }

    private void onButtonClicked()
    {
        _animator.Play("appear", -1, 0);
    }
}
