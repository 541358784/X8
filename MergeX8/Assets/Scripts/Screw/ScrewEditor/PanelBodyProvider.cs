#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;


namespace Screw
{
    public class PanelBodyProvider : MonoBehaviour
    {
        private List<Vector2> points = new List<Vector2>();
        // private List<Vector2> simplifiedPoints = new List<Vector2>();

        public void UpdatePolygonCollider2D(float tolerance = 0.05f)
        {
            List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

            for (int i = 0; i < transform.childCount; i++)
            {
                var spriteRenderer = transform.GetChild(i).GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && spriteRenderer.gameObject.name != "Shadow")
                {
                    spriteRenderers.Add(spriteRenderer);
                }
            }
            
            PolygonCollider2D polygonCollider = GetComponent<PolygonCollider2D>();

            var count = 0;
            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                var spriteRenderer = spriteRenderers[i];
                var shapeCount = spriteRenderer.sprite.GetPhysicsShapeCount();
                for (int j = 0; j < shapeCount; j++)
                {
                    count++;
                }
            }
            polygonCollider.pathCount = count;

            count = 0;
            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                var spriteRenderer = spriteRenderers[i];
                var shapeCount = spriteRenderer.sprite.GetPhysicsShapeCount();
                for (int j = 0; j < shapeCount; j++)
                {
                    spriteRenderer.sprite.GetPhysicsShape(j, points);
                    List<Vector2> simplifiedPoints = new List<Vector2>();
                    if (i != 0)
                    {
                        LineUtility.Simplify(points, tolerance, simplifiedPoints);
                        for (int k = 0; k < simplifiedPoints.Count; k++)
                        {
                            simplifiedPoints[k] = simplifiedPoints[k] * spriteRenderer.transform.localScale +
                                                  (Vector2) spriteRenderer.transform.localPosition;
                        }
                    }
                    else
                    {
                        simplifiedPoints = points;
                        for (int k = 0; k < simplifiedPoints.Count; k++)
                        {
                            simplifiedPoints[k] = simplifiedPoints[k] * spriteRenderer.transform.localScale +
                                                  (Vector2) spriteRenderer.transform.localPosition;
                        }
                    }
                    polygonCollider.SetPath(count, simplifiedPoints);
                    count++;
                }
            }
        }
    }
}
#endif