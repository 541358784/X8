using UnityEngine;
using UnityEditor;

// Adds a mesh collider to each game object that contains collider in its name
public class ModelAssetPostProcessor : AssetPostprocessor
{
    //void OnPostprocessModel(GameObject g)
    //{
    //    Apply(g.transform);
    //}

    void Apply(Transform t)
    {
        var mfs = t.GetComponentsInChildren<MeshFilter>();

        if (mfs != null)
        {
            Vector3 minVertice = Vector3.positiveInfinity;
            Vector3 maxVertice = Vector3.negativeInfinity;
            foreach (var mf in mfs)
            {
                GetMinAndMax(mf.transform.position, mf.sharedMesh.vertices, ref minVertice, ref maxVertice);
            }

            var child = new GameObject("Collider", typeof(BoxCollider));
            child.transform.SetParent(t, false);
            var bc = child.GetComponent<BoxCollider>();
            bc.center = new Vector3((maxVertice.x + minVertice.x) / 2, (maxVertice.y + minVertice.y) / 2,
                (maxVertice.z + minVertice.z) / 2);
            bc.size = new Vector3(maxVertice.x - minVertice.x, maxVertice.y - minVertice.y,
                maxVertice.z - minVertice.z);
        }
    }

    private static void GetMinAndMax(Vector3 pos, Vector3[] points, ref Vector3 minVertice, ref Vector3 maxVertice)
    {
        foreach (var vertice in points)
        {
            if (vertice.x + pos.x < minVertice.x)
            {
                minVertice.x = vertice.x + pos.x;
            }

            if (vertice.y + pos.y < minVertice.y)
            {
                minVertice.y = vertice.y + pos.y;
            }

            if (vertice.z + pos.z < minVertice.z)
            {
                minVertice.z = vertice.z + pos.z;
            }

            if (vertice.x + pos.x > maxVertice.x)
            {
                maxVertice.x = vertice.x + pos.x;
            }

            if (vertice.y + pos.y > maxVertice.y)
            {
                maxVertice.y = vertice.y + pos.y;
            }

            if (vertice.z + pos.z > maxVertice.z)
            {
                maxVertice.z = vertice.z + pos.z;
            }
        }
    }
}