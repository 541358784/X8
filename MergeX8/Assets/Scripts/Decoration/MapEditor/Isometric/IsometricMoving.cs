using Spine.Unity;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;

#endif

[ExecuteInEditMode]
public class IsometricMoving : IsometricObject
{
    [HideInInspector] public bool dirIsFront;
    [HideInInspector] public bool dirIsLeft;

    void LateUpdate()
    {
        if (transform)
        {
            updatePosition(transform.position);
        }
    }

    public void SetPosition(Vector2 newPosition)
    {
        updatePosition(newPosition);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var font = new GUIStyle();
        font.normal.textColor = Color.white;
        Handles.Label(transform.position, $"{dirIsFront}:{dirIsLeft}", font);
    }
#endif
}