using System.Collections;
using Framework;
using UnityEngine;

public class GuideArrowController:MonoBehaviour
{
    public void Init()
    {
        gameObject.SetActive(false);
    }
    private Coroutine CurrentCoroutine;

    private Canvas canvas;
    private Canvas Canvas
    {
        get
        {
            if (canvas == null)
            {
                canvas = transform.GetComponent<Canvas>();
            }
            return canvas;
        }
    }

    public void SetSortingOrder(int sortingOrder)
    {
        if (Canvas)
            Canvas.sortingOrder = sortingOrder;
    }
    public void ShowForTime(float time = 5f)
    {
        if (CurrentCoroutine != null)
            CoroutineManager.Instance.StopCoroutine(CurrentCoroutine);
        CurrentCoroutine = CoroutineManager.Instance.StartCoroutine(ShowForTimeEnumerator(time));
    }

    public void Hide()
    {
        if (CurrentCoroutine != null)
            CoroutineManager.Instance.StopCoroutine(CurrentCoroutine);
        gameObject.SetActive(false);
    }
    private IEnumerator ShowForTimeEnumerator(float time)
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}