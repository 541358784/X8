using System;
using UnityEngine;

public class FocusLogic : MonoBehaviour
{
    private GameObject styleNormal = null;
    private GameObject styleGray = null;
    private Animator animator;

    private void Awake()
    {
        styleNormal = transform.Find("Style").gameObject;
        styleNormal.SetActive(true);

        styleGray = transform.Find("StyleGray").gameObject;
        styleGray.SetActive(false);

        animator = transform.GetComponent<Animator>();
    }

    public void Focus(Vector3 position, bool isMax)
    {
        transform.localPosition = position;
        gameObject.SetActive(true);
        animator.Play("appear", 0, 0f);

        styleNormal.SetActive(!isMax);
        styleGray.SetActive(isMax);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}