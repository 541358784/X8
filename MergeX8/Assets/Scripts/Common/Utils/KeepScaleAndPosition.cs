using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepScaleAndPosition : MonoBehaviour
{
    private Vector3 _localScale = Vector3.one;
    private Vector3 _parentScale = Vector3.one;
    private Vector3 _localPosition = Vector3.zero;

    private bool _init = false;
    // Start is called before the first frame update
    public void Awake()
    {
        if (_init)
            return;
        _init = true;
        UpdateScale();
    }

    public void UpdateScale()
    {
        if(transform.parent == null)
            return;
        _localScale = transform.localScale;
        _parentScale = transform.parent.localScale;
        _localPosition = transform.localPosition;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!_init)
            return;
        if(transform.parent == null)
            return;
        var parentScale = transform.parent.localScale;
        var localScale = Vector3.one;
        localScale.x = _localScale.x * _parentScale.x / parentScale.x;
        localScale.y = _localScale.y * _parentScale.y / parentScale.y;
        localScale.z = _localScale.z * _parentScale.z / parentScale.z;
        transform.localScale = localScale;
        var localPosition = Vector3.zero;
        localPosition.x = _localPosition.x * _parentScale.x/parentScale.x;
        localPosition.y = _localPosition.y * _parentScale.y/parentScale.y;
        localPosition.z = _localPosition.z * _parentScale.z/parentScale.z;
        transform.localPosition = localPosition;
    }
}