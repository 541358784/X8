using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

[ExecuteInEditMode]
public class IsometricObject : MonoBehaviour
{
    public float _floorHeight;

    [HideInInspector] public float _spriteLowerBound;
    private float _spriteHalfWidth;
    private static float  TAN_30 = 0.57735f;

#if UNITY_EDITOR
    public static bool HelperLine_Selected = true;
    public static bool HelperLine_UnSelected = true;

    private bool _selected = false;
#endif

    void Start()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            initComponents();
            StartCoroutine(lateUpate());
        }
#endif
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    IEnumerator lateUpate()
    {
        yield return new WaitForEndOfFrame();
        if (transform)
        {
            CaculatePosition();
        }
    }

    public void CaculatePosition()
    {
        updatePosition(transform.position);
    }

    protected void initComponents()
    {
        if(initComponent<SpriteRenderer>())
            return;
        
        if(initComponent<MeshRenderer>())
            return;
        
        if(initComponent<SkinnedMeshRenderer>())
            return;
        
        _spriteLowerBound = transform.position.y;
        _spriteHalfWidth = 1f;
    }

    protected bool initComponent<T>() where T : Renderer
    {
        bool isInit = false;
        
        var spriteRender = GetComponent<T>();
        if (spriteRender != null)
        {
            isInit = true;
            initLowerBoundAndHalfWidth(spriteRender);
        }

        var renderers = GetComponentsInChildren<T>();
        if (renderers != null && renderers.Length > 0)
        {
            isInit = true;
            foreach (var renderer in renderers)
            {
                initLowerBoundAndHalfWidth(renderer);
            }
        }

        return isInit;
    }
    
    private void initLowerBoundAndHalfWidth(Renderer render)
    {
        _spriteLowerBound = Mathf.Max(render.bounds.size.y * 0.5f, _spriteLowerBound);
        _spriteHalfWidth = Mathf.Max(render.bounds.size.x * 0.5f, _spriteHalfWidth);
    }

    protected void updatePosition(Vector2 newPosition)
    {
        if (transform)
        {
            transform.position = new Vector3(
                newPosition.x,
                newPosition.y,
                (newPosition.y - _spriteLowerBound + _floorHeight) * TAN_30);
        }
    }

#if UNITY_EDITOR

    void LateUpdate()
    {
        if(UnityEditor.SceneManagement.EditorSceneManager.IsPreviewSceneObject(gameObject))
            return;

        if (!Application.isPlaying)
        {
            CaculatePosition();
        }
    }

    private void OnSelectionChange()
    {
        _selected = !_selected;
    }

    private void OnDrawGizmosSelected()
    {
        if (HelperLine_Selected)
        {
            drawHelpLine(Color.green);
        }
    }

    void OnDrawGizmos()
    {
        if (HelperLine_UnSelected)
        {
            if (!_selected) drawHelpLine(Color.magenta);
        }
    }

    private void drawHelpLine(Color lineColor)
    {
        Vector3 floorHeightPos = new Vector3
        (
            transform.position.x,
            transform.position.y - _spriteLowerBound + _floorHeight,
            transform.position.z
        );
        Gizmos.color = lineColor;
        Gizmos.DrawLine(floorHeightPos + Vector3.left * _spriteHalfWidth,
            floorHeightPos + Vector3.right * _spriteHalfWidth);
    }

#endif
}