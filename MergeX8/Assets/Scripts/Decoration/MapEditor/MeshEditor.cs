using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DragonPlus
{
    [ExecuteInEditMode]
    public class MeshEditor : MonoBehaviour
    {
        public Mesh mesh;
        private List<Vector3> _vectorList;
        private List<SpriteRenderer> _handleList;
        private bool _enable = false;
        private bool _inEditor = false;

        void OnEnable()
        {
            if (!_enable)
            {
                _enable = true;
#if UNITY_EDITOR
                Selection.selectionChanged += OnSelectionChanged;
#endif
            }
        }

        private void OnDisable()
        {
            if (_enable)
            {
#if UNITY_EDITOR
                Selection.selectionChanged -= OnSelectionChanged;
#endif
                _enable = false;
            }
            ExitEditor();
        }

        private void EnterEditor()
        {
#if UNITY_EDITOR
            if (!_inEditor)
            {
                var childCount = transform.childCount;
                for (var i = childCount - 1; i >= 0; --i)
                    DestroyImmediate(transform.GetChild(i).gameObject);

                _vectorList = new List<Vector3>();
                _handleList = new List<SpriteRenderer>();

                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Export/Textures/UI/Basic/CommonWhite.png");

                mesh = GetComponent<MeshFilter>().sharedMesh;
                var index = 0;
                foreach (var vert in mesh.vertices)
                {
                    var vertPos = transform.TransformPoint(vert);
                    var handle = new GameObject("vertex_" + index);
                    handle.transform.localPosition = vertPos;
                    handle.transform.parent = transform;
                    var spriteRenderer = handle.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = sprite;
                    spriteRenderer.color = Color.white;
                    spriteRenderer.sortingOrder = 1;
                    _vectorList.Add(vert);
                    _handleList.Add(spriteRenderer);
                    ++index;
                }
                _inEditor = true;
            }
#endif
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < _vectorList.Count; i++)
            {
                Gizmos.DrawSphere(transform.localToWorldMatrix.MultiplyPoint(_vectorList[i]), 0.1f);
            }
        }
#endif

        private void ExitEditor()
        {
            if (_inEditor)
            {
                _inEditor = false;
                var childCount = transform.childCount;
                for (var i = childCount - 1; i >= 0; --i)
                    DestroyImmediate(transform.GetChild(i).gameObject);
                _vectorList = null;
                _handleList = null;
            }
        }

        public void OnWillSave()
        {
            this.ExitEditor();
        }

        private void Update()
        {
            if (_inEditor)
            {
                for (var i = 0; i < _handleList.Count; ++i)
                {
                    _vectorList[i] = _handleList[i].transform.localPosition;
                }
                mesh.vertices = _vectorList.ToArray();
                mesh.RecalculateBounds();
                // mesh.RecalculateNormals();
            }
        }

        private void OnSelectionChanged()
        {
#if UNITY_EDITOR
            if (Selection.activeGameObject == this.gameObject)
            {
                if (!_inEditor)
                    EnterEditor();
            }
            else
            {
                if (_inEditor)
                    ExitEditor();
            }
            // var opacity = Selection.activeGameObject == this.gameObject ? 255 : 0;
            // foreach (var spriteRenderer in _handleList)
            // {
            //     var color = spriteRenderer.color;
            //     color.a = opacity;
            //     spriteRenderer.color = color;
            // }
#endif
        }

        public SpriteRenderer NearVertex(Vector2 worldPosition, float minDistance = 1.0f)
        {
            SpriteRenderer minVertex = null;
            foreach (var spriteRenderer in _handleList)
            {
                var position = spriteRenderer.transform.position;
                var distance = Vector2.Distance(position, worldPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minVertex = spriteRenderer;
                }
            }
            return minVertex;
        }
    }
}