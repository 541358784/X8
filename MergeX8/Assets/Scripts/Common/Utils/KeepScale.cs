using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepScale : MonoBehaviour
{
    private Vector3 _keepScale = Vector3.one;

    private Vector3 _orgScale = Vector3.one;
    
    // Start is called before the first frame update
    void Start()
    {
        _orgScale = transform.localScale;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(transform.parent == null)
            return;

        var parentScale = transform.parent.localScale;
        
        _keepScale.x = _orgScale.x / parentScale.x;
        _keepScale.y = _orgScale.y / parentScale.y;
        _keepScale.z = _orgScale.z / parentScale.z;

        if (float.IsInfinity(_keepScale.x) || float.IsNaN(_keepScale.x))
            _keepScale.x = 0f;
        
        if (float.IsInfinity(_keepScale.y) || float.IsNaN(_keepScale.y))
            _keepScale.y = 0f;
        
        if (float.IsInfinity(_keepScale.z) || float.IsNaN(_keepScale.z))
            _keepScale.z = 0f;
        
        transform.localScale = _keepScale;
    }
}
