#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SomeWhere
{
    public partial class PathPoint
    {
        private void OnDrawGizmos()
        {
            DrawDebug();
        }

        protected virtual void DrawDebug()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(Position, 0.1f);
        }
    }
}
#endif