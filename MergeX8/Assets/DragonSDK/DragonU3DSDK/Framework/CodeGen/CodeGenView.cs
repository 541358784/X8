using UnityEngine;

public abstract class CodeGenView
{
    public GameObject gameObject;
    public Transform transform;

    public abstract void FindChildren(Transform transform, bool resetPosition = true);
}