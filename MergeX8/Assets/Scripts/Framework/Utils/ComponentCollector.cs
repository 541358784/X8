using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class ComponentCollector
    {
        public Transform[] Transforms;
        public Renderer[] Renderers;
        public Renderer FirstRenderer;
        public BoxCollider[] BoxColliders;
        public ParticleSystem[] ParticleSystems;
        public MeshFilter[] MeshFilters;
        public MeshFilter FirstMeshFilter;


        public ComponentCollector(GameObject go)
        {
            Transforms = CollectAll<Transform>(go);
            Renderers = CollectAll<Renderer>(go);
            BoxColliders = CollectAll<BoxCollider>(go);
            ParticleSystems = CollectAll<ParticleSystem>(go);
            MeshFilters = CollectAll<MeshFilter>(go);

            if (Renderers?.Length > 0)
            {
                FirstRenderer = Renderers?[0];
            }

            if (MeshFilters?.Length > 0)
            {
                FirstMeshFilter = MeshFilters?[0];
            }
        }


        private void Collect<T>(Transform t, ref List<T> components, bool recrusive)
        {
            var c = t.GetComponent<T>();
            if (c != null && !c.Equals(null))
            {
                components.Add(c);
            }

            if (recrusive)
            {
                for (int i = 0; i < t.childCount; i++)
                {
                    Collect(t.GetChild(i), ref components, recrusive);
                }
            }
        }

        private T[] CollectAll<T>(GameObject go)
        {
            var components = new List<T>();
            Collect(go.transform, ref components, true);
            return components.ToArray();
        }
    }
}