using System.Collections;
using NoiseCrimeStudios.Core.Features.Easing;
using UnityEngine;

namespace Framework
{
    public static class TransformUtil
    {
        public static bool ToRotationH(Vector3 dir, out Quaternion rotation)
        {
            dir.y = 0.0f;
            if (dir.sqrMagnitude > Constants.DOUBLE_EPSILON)
            {
                dir.Normalize();
                rotation = Quaternion.LookRotation(dir, Vector3.up);
                return true;
            }

            rotation = Quaternion.identity;
            return false;
        }

        public static bool ToRotation(Vector3 dir, Vector3 up, out Quaternion rotation)
        {
            if (dir.sqrMagnitude > Constants.DOUBLE_EPSILON &&
                up.sqrMagnitude > Constants.DOUBLE_EPSILON)
            {
                rotation = Quaternion.LookRotation(dir, up);
                return true;
            }

            rotation = Quaternion.identity;
            return false;
        }

        public static float GetAngleH(Quaternion rotation)
        {
            var angle = 0.0f;
            var axis = Vector3.zero;
            rotation.ToAngleAxis(out angle, out axis);
            return angle * axis.y;
        }

        public static void CopyTransform(Transform src, Transform dst)
        {
            dst.parent = src.parent;
            dst.position = src.position;
            dst.rotation = src.rotation;
            dst.localPosition = src.localPosition;
            dst.localRotation = src.localRotation;
            dst.localScale = src.localScale;
        }

        public static IEnumerator Easing(Transform o, Transform startPos, Transform endPos, float duration)
        {
            var current = 0f;
            while ((current += Time.deltaTime) < duration)
            {
                var start = startPos.position;
                var change = endPos.position - start;
                var x = EasingEquationsDouble.Linear(current, start.x, change.x, duration);
                var y = EasingEquationsDouble.Linear(current, start.y, change.y, duration);
                var z = EasingEquationsDouble.Linear(current, start.z, change.z, duration);
                o.position = new Vector3((float) x, (float) y, (float) z);
                yield return null;
            }

            o.transform.position = endPos.position;
        }
    }
}