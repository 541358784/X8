// using System;
// using System.Collections.Generic;
// using Framework;
// using OnePath.Model;
// using UnityEngine;
//
// namespace OnePath.View
// {
//     public partial class OnePathControl : MonoBehaviour
//     {
//         private LineRenderer _lineRenderer;
//         private OnePathConfig _onePathConfig;
//
//         private Dictionary<int, OnePathChunk> _onePathChunks = new Dictionary<int, OnePathChunk>();
//         private void Awake()
//         {
//             _lineRenderer = transform.Find("Line").GetComponent<LineRenderer>();
//         }
//
//         private void Start()
//         {
//             _onePathConfig = OnePathConfigManager.Instance.GetConfig(OnePathModel.Instance._config.levelId);
//             foreach (var onePathChunk in _onePathConfig._chunks)
//             {
//                 if(_onePathChunks.ContainsKey(onePathChunk.index))
//                     continue;
//                 
//                 _onePathChunks.Add(onePathChunk.index, onePathChunk);
//             }
//             
//             InitLogic();
//         }
//     }
// }