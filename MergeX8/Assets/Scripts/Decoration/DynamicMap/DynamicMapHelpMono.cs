using Decoration.DynamicMap;
using UnityEditor;
using UnityEngine;

namespace DragonPlus
{
    public class DynamicMapHelpMono : MonoBehaviour
    {
        Vector3 _orgPosition = Vector3.zero;
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            float height = DynamicMapConfigManager.Instance._gridRow * DynamicMapConfigManager.Instance._chunkHeight;
            float width = DynamicMapConfigManager.Instance._gridCol * DynamicMapConfigManager.Instance._chunkWidth;

            _orgPosition.x = DynamicMapConfigManager.Instance._orgPosX;
            _orgPosition.y = DynamicMapConfigManager.Instance._orgPosY;
            Gizmos.DrawWireCube(_orgPosition,  new Vector3(width, height));

            OnDrawRowCol();
            
            // Vector3 orgPosition = Vector3.zero;
            // orgPosition.x = -1.0f * DynamicMapConfigManager.Instance._gridCol / 2 * DynamicMapConfigManager.Instance._chunkSize + DynamicMapConfigManager.Instance._chunkSize/2;
            // orgPosition.y = 1.0f * DynamicMapConfigManager.Instance._gridRow / 2 * DynamicMapConfigManager.Instance._chunkSize - DynamicMapConfigManager.Instance._chunkSize/2 ;
            // Gizmos.color = Color.red;
            // Gizmos.DrawLine(orgPosition, orgPosition + Vector3.right * 2);
            // var font = new GUIStyle();
            // font.normal.textColor = Color.white;
            // Handles.Label(transform.position, $"{dirIsFront}:{dirIsLeft}", font);
        }

        private void OnDrawRowCol()
        {
            Gizmos.color = Color.green;
            
            Vector3 orgPosition = Vector3.zero;
            orgPosition.x = -1.0f * DynamicMapConfigManager.Instance._gridCol / 2 * DynamicMapConfigManager.Instance._chunkWidth+_orgPosition.x;
            orgPosition.y = 1.0f * DynamicMapConfigManager.Instance._gridRow / 2 * DynamicMapConfigManager.Instance._chunkHeight+_orgPosition.y;
            
            Vector3 endPosition = Vector3.zero;
            endPosition.x = 1.0f * DynamicMapConfigManager.Instance._gridCol / 2 * DynamicMapConfigManager.Instance._chunkWidth+_orgPosition.x;
            endPosition.y = -1.0f * DynamicMapConfigManager.Instance._gridRow / 2 * DynamicMapConfigManager.Instance._chunkHeight+_orgPosition.y;
            
            for (var y = 0; y < DynamicMapConfigManager.Instance._gridRow; y++)
            {
                Gizmos.DrawLine(new Vector3(orgPosition.x, orgPosition.y - y*DynamicMapConfigManager.Instance._chunkHeight, 0), new Vector3(endPosition.x, orgPosition.y - y*DynamicMapConfigManager.Instance._chunkHeight, 0));
            }
            
            Gizmos.color = Color.green;
            for (var x = 0; x < DynamicMapConfigManager.Instance._gridCol; x++)
            {
                Gizmos.DrawLine(new Vector3(orgPosition.x+x*DynamicMapConfigManager.Instance._chunkWidth, orgPosition.y, 0), new Vector3(orgPosition.x+x*DynamicMapConfigManager.Instance._chunkWidth, endPosition.y, 0));
            }
        }
#endif
    }
}