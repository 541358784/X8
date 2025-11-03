using System.Collections.Generic;
using Framework;
using UnityEngine;

public class UIReuseable : MonoBehaviour
{
    static private Stack<GameObject> _cellPoolList = new Stack<GameObject>();

    private GameObject _childGameObject;
    private GameObject _childTemplateGameObject;
    private Rect _availableRect;
    [SerializeField] private bool _using;
    private System.Action<GameObject> _initChildCallback;

    private float _delayCheckDelta = 0.1f;

    public void Init(GameObject childTemplate, Rect availableRect, System.Action<GameObject> initChildCallback)
    {
        _availableRect = availableRect;
        _initChildCallback = initChildCallback;
        _using = false;
        _childTemplateGameObject = childTemplate;
    }

    private GameObject GetReusedCell(GameObject template, GameObject parent)
    {
        GameObject cell = null;
        if (_cellPoolList.Count == 0)
        {
            cell = GameObjectFactory.Clone(template, parent);
            cell.SetActive(true);
        }
        else
        {
            cell = _cellPoolList.Pop();
            cell.transform.SetParent(parent.transform);
        }

        var templateRectransform = parent.GetComponent<RectTransform>();
        var cellRectTransform = cell.GetComponent<RectTransform>();
        cellRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        cellRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        cellRectTransform.sizeDelta = templateRectransform.sizeDelta;
        cellRectTransform.localPosition = Vector2.zero;

        return cell;
    }

    private void FixedUpdate()
    {
        _delayCheckDelta -= Time.deltaTime;
        if (_delayCheckDelta > 0) return;

        if (_availableRect.Contains(transform.position))
        {
            if (!_using)
            {
                _using = true;
                _childGameObject = GetReusedCell(_childTemplateGameObject, gameObject);
                _initChildCallback(_childGameObject);
            }
        }
        else
        {
            if (_using)
            {
                _using = false;
                _cellPoolList.Push(_childGameObject);
            }
        }
    }

    static public void ClearPool()
    {
        _cellPoolList.Clear();
    }
}