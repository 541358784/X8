// using System;
// using System.Collections;
// using System.Collections.Generic;
// using DaVikingCode.RectanglePacking;
// using DragonU3DSDK.Asset;
// using UnityEngine;
// using Object = System.Object;
//
// public class DARender : MonoBehaviour
// {
//     class RenderGroup
//     {
//         public RenderGroup(GameObject prefab, int index, int textureSize)
//         {
//             var newGroup = GameObject.Instantiate(prefab, prefab.transform.parent);
//             newGroup.SetActive(true);
//             newGroup.transform.localPosition = new Vector3(index * 100, 0f, 0f);
//
//             var camera = newGroup.transform.Find("Camera").GetComponent<Camera>();
//             var renderRoot = newGroup.transform.Find("RenderRoot");
//             _camera = camera;
//             _renderRoot = renderRoot;
//         }
//
//         private Camera _camera;
//         private Transform _renderRoot;
//
//         private List<Sprite> _spriteList = new List<Sprite>();
//
//         public void Clear()
//         {
//             _camera.gameObject.SetActive(false);
//         }
//
//         public void RenderTo(RenderTexture renderTexture)
//         {
//             _camera.targetTexture = renderTexture;
//             _camera.enabled = true;
//             RenderTexture.active = renderTexture;
//             _camera.Render();
//
//             _camera.enabled = false;
//             _camera.targetTexture = null;
//             RenderTexture.active = null;
//
//             _renderRoot.gameObject.RemoveAllChildren();
//             for (int i = 0; i < _spriteList.Count; i++)
//             {
//                 Resources.UnloadAsset(_spriteList[i]);
//             }
//
//             _spriteList.Clear();
//
//             Resources.UnloadUnusedAssets();
//         }
//
//         public void RenderSpriteAtRect(Sprite sp, IntegerRectangle rect)
//         {
//             var centerX = rect.x + rect.width / 2f;
//             var centerY = rect.y + rect.height / 2f;
//             var pos = new Vector2(centerX, centerY) / 100f;
//
//             var spObj = new GameObject();
//             var spRender = spObj.AddComponent<SpriteRenderer>();
//             spObj.transform.SetParent(_renderRoot);
//             spObj.transform.localPosition = pos;
//             spObj.transform.localScale = Vector3.one;
//             spRender.sprite = sp;
//             spObj.layer = LayerMask.NameToLayer("DA");
//
//             _spriteList.Add(sp);
//         }
//     }
//
//     public static DARender Instance;
//     private List<RenderGroup> _renderGroups = new List<RenderGroup>();
//
//     private void Awake()
//     {
//         Instance = this;
//     }
//
// #if UNITY_EDITOR
//     private void OnApplicationQuit()
//     {
//         RuntimeAtlasManager.Instance.ClearTemp();
//     }
// #endif
//
//     public void RenderSpriteAtRect(Sprite sp, IntegerRectangle rect, int groupIndex, int textureSize)
//     {
//         if (_renderGroups.Count <= groupIndex)
//         {
//             _renderGroups.Add(new RenderGroup(transform.Find("Render").gameObject, groupIndex, textureSize));
//         }
//
//         _renderGroups[groupIndex].RenderSpriteAtRect(sp, rect);
//     }
//
//     public void RenderTo(RenderTexture renderTexture, int groupIndex)
//     {
//         _renderGroups[groupIndex].RenderTo(renderTexture);
//     }
//
//
//     public void Clear()
//     {
//         foreach (var group in _renderGroups)
//         {
//             group.Clear();
//         }
//     }
// }