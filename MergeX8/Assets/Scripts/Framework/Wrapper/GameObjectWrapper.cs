using System;
using UnityEngine;

namespace Framework.Wrapper
{
    /// <summary>
    /// gameobject 和 transform 操作的封装类
    /// </summary>
    public class GameObjectWrapper
    {
        public GameObject _gameObject;

        public GameObjectWrapper(GameObject go)
        {
            if (go == null)
            {
                DragonU3DSDK.DebugUtil.LogError("GameObjectWrapper: gameObject is null");
            }

            _gameObject = go;
        }

        public string Name
        {
            get { return _gameObject != null ? _gameObject.name : null; }
            set
            {
                if (_gameObject != null) _gameObject.name = value;
            }
        }

        private Transform Transform => _gameObject != null ? _gameObject.transform : null;

        public int GetInstanceID()
        {
            return _gameObject != null ? _gameObject.GetInstanceID() : 0;
        }

        public string GetNameMark()
        {
            return _gameObject != null ? (_gameObject.name + _gameObject.transform.parent.name) : string.Empty;
        }

        public bool GetActive()
        {
            return _gameObject != null && _gameObject.activeSelf;
        }

        public void SetActive(bool active)
        {
            if (_gameObject != null) _gameObject.SetActive(active);
        }

        public bool IsActive()
        {
            return _gameObject != null && _gameObject.activeSelf;
        }

        public Vector3 GetPosition(Space relativeTo = Space.World)
        {
            return Transform != null
                ? relativeTo == Space.World ? Transform.position : Transform.localPosition
                : Vector3.zero;
        }

        public void SetPosition(Vector3 position, Space relativeTo = Space.World)
        {
            if (Transform == null) return;
            if (relativeTo == Space.World)
            {
                Transform.position = position;
            }
            else
            {
                Transform.localPosition = position;
            }
        }

        public Quaternion GetRotation(Space relativeTo = Space.World)
        {
            if (Transform == null)
            {
                return Quaternion.identity;
            }

            return relativeTo == Space.World ? Transform.rotation : Transform.localRotation;
        }

        public void SetRotationH(Vector3 dir)
        {
            Quaternion rotation;
            if (TransformUtil.ToRotationH(dir, out rotation))
            {
                SetRotation(rotation);
            }
        }

        public void SetRotation(Vector3 dir, Vector3 up)
        {
            Quaternion rotation;
            if (TransformUtil.ToRotation(dir, up, out rotation))
            {
                SetRotation(rotation);
            }
        }

        public void SetRotation(Quaternion rotation, Space relativeTo = Space.World)
        {
            try
            {
                if (Transform == null) return;
                if (relativeTo == Space.World)
                {
                    Transform.rotation = rotation;
                }
                else
                {
                    Transform.localRotation = rotation;
                }
            }
            catch (Exception e)
            {
                DragonU3DSDK.DebugUtil.LogError(e.ToString());
            }
        }

        public Vector3 GetEulerAngles(Space relativeTo = Space.World)
        {
            return GetRotation(relativeTo).eulerAngles;
        }

        public void SetEulerAngles(Vector3 eulerAngles, Space relativeTo = Space.World)
        {
            Quaternion rotation = Quaternion.Euler(eulerAngles);
            SetRotation(rotation, relativeTo);
        }

        public Vector3 GetRight()
        {
            return Transform != null ? Transform.right : Vector3.zero;
        }

        public Vector3 GetForward()
        {
            return Transform != null ? Transform.forward : Vector3.zero;
        }

        public Vector3 GetUp()
        {
            return Transform != null ? Transform.up : Vector3.zero;
        }

        public float GetAngleH()
        {
            return TransformUtil.GetAngleH(GetRotation());
        }

        public void SetAngleH(float angle)
        {
            SetRotation(Quaternion.AngleAxis(angle, Vector3.up));
        }

        public Vector3 GetScale(Space relativeTo = Space.Self)
        {
            if (Transform == null)
            {
                return Vector3.one;
            }

            return relativeTo == Space.World ? Transform.lossyScale : Transform.localScale;
        }

        public void SetScale(float scale)
        {
            if (scale > 0.0f)
            {
                SetScale(Vector3.one * scale);
            }
        }

        public void SetScale(Vector3 scale, Space relativeTo = Space.Self)
        {
            try
            {
                var transform = Transform;
                if (transform == null)
                {
                    return;
                }

                if (scale.x > 0.0f && scale.y > 0.0f && scale.z > 0.0f)
                {
                    if (relativeTo == Space.World)
                    {
                        if (Transform.parent == null)
                        {
                            Transform.localScale = scale;
                        }
                        else
                        {
                            Vector3 parentScale = Transform.parent.lossyScale;
                            if (parentScale.x > 0.0f &&
                                parentScale.y > 0.0f &&
                                parentScale.z > 0.0f)
                            {
                                Transform.localScale = new Vector3(scale.x / parentScale.x,
                                    scale.y / parentScale.y,
                                    scale.z / parentScale.z);
                            }
                        }
                    }
                    else
                    {
                        Transform.localScale = scale;
                    }
                }
            }
            catch (Exception e)
            {
                DragonU3DSDK.DebugUtil.LogError(e.ToString());
            }
        }

        public Matrix4x4 GetLocalToWorldMatrix()
        {
            return Transform != null ? Transform.localToWorldMatrix : Matrix4x4.identity;
        }

        public Matrix4x4 GetWorldToLocalMatrix()
        {
            return Transform != null ? Transform.worldToLocalMatrix : Matrix4x4.identity;
        }

        public Transform GetParent()
        {
            return Transform != null ? Transform.parent : null;
        }


        public void SetParent(GameObjectWrapper parent, bool worldPositionStays = true)
        {
            SetParent(parent?.Transform, worldPositionStays);
        }

        public void SetParent(Transform parent, bool worldPositionStays = true)
        {
            var transform = Transform;
            if (transform == null)
            {
                return;
            }

            Transform.SetParent(parent, worldPositionStays);
            if (!worldPositionStays)
            {
                Transform.localPosition = Vector3.zero;
                Transform.localRotation = Quaternion.identity;
            }
        }

        public int GetChildCount()
        {
            return Transform != null ? Transform.childCount : 0;
        }

        public void AttachChild(Transform child)
        {
            AttachChild(child, Vector3.zero, rotation: Quaternion.identity);
        }

        public void AttachChild(Transform child, Vector3 position)
        {
            AttachChild(child, position, rotation: Quaternion.identity);
        }

        public void AttachChild(Transform child, Vector3 position, Quaternion rotation) =>
            AttachChild(child, position, rotation, Vector3.one);

        public void AttachChild(Transform child, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            try
            {
                if (child != null && Transform != null)
                {
                    child.SetParent(Transform, false);
                    child.localPosition = position;
                    child.localRotation = rotation;
                    child.localScale = scale;
                }
            }
            catch (Exception e)
            {
                DragonU3DSDK.DebugUtil.LogError(e.ToString());
            }
        }

        public void DetachChild(Transform child)
        {
            if (child != null)
            {
                child.SetParent(null);
            }
        }

        public void AttachChild(GameObjectWrapper child)
        {
            AttachChild(child, position: Vector3.zero, rotation: Quaternion.identity, scale: Vector3.one);
        }

        public void AttachChild(GameObjectWrapper child, Vector3 position)
        {
            AttachChild(child, position, Quaternion.identity, Vector3.one);
        }

        public void AttachChild(GameObjectWrapper child, Vector3 position, Quaternion rotation)
        {
            AttachChild(child, position, rotation, Vector3.one);
        }

        public void AttachChild(GameObjectWrapper child, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            try
            {
                if (Transform == null)
                {
                    return;
                }

                child.SetParent(Transform, false);
                child.SetPosition(position, Space.Self);
                child.SetRotation(rotation, Space.Self);
                child.SetScale(scale);
            }
            catch (Exception e)
            {
                DragonU3DSDK.DebugUtil.LogError(e.ToString());
            }
        }

        public void DetachChild(GameObjectWrapper child)
        {
            try
            {
                if (child != null)
                {
                    DetachChild(child.Transform);
                }
            }
            catch (Exception e)
            {
                DragonU3DSDK.DebugUtil.LogError(e.ToString());
            }
        }

        public virtual void SetLayer(int layer)
        {
            if (_gameObject != null) _gameObject.layer = layer;
        }

        public int GetLayer()
        {
            return _gameObject != null ? _gameObject.layer : 0;
        }

        public GameObject GameObject
        {
            get { return _gameObject; }
        }

        public Vector3 TransformPoint(Vector3 position)
        {
            return _gameObject.transform.TransformPoint(position);
        }

        public Vector3 InverseTransformVector(Vector3 position)
        {
            return _gameObject.transform.InverseTransformVector(position);
        }

        public Vector3 InverseTransformPoint(Vector3 position)
        {
            return _gameObject.transform.InverseTransformPoint(position);
        }

        public Vector3 TransformVector(Vector3 position)
        {
            return _gameObject.transform.TransformVector(position);
        }
    }
}