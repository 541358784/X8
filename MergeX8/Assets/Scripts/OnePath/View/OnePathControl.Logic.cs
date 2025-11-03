// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Framework;
// using OnePath.Model;
// using UnityEngine;
//
// namespace OnePath.View
// {
//     public partial class OnePathControl
//     {
//         private bool _canTouch = true;
//         
//         private int[][] _dir = new[]
//         {
//             new[] { 0, -1 },
//             new[] { 0, 1 },
//             new[] { -1, 0 },
//             new[] { 1, 0 },
//             new[] { -1, -1 },
//             new[] { 1, -1 },
//             new[] { 1, 1 },
//             new[] { -1, 1 }
//         };
//         
//         private int[][] _moreDir = new[]
//         {
//             new[] { 0, -1 },
//             new[] { 0, 1 },
//             new[] { -1, 0 },
//             new[] { 1, 0 },
//             new[] { -1, -1 },
//             new[] { 1, -1 },
//             new[] { 1, 1 },
//             new[] { -1, 1 },
//             
//             new[] { -2, -2},
//             new[] { -1, -2 },
//             new[] { 0, -2 },
//             new[] { 1, -2 },
//             new[] { 2, -2 },
//             new[] { -2, -1 },
//             new[] { -2, 0 },
//             new[] { -2, 1 },
//             new[] { -2, 2 },
//             new[] { 2, -1 },
//             new[] { 2, 0 },
//             new[] { 2, 1 },
//             new[] { 2, 2 },
//             new[] { -1, 2 },
//             new[] { 0, 2 },
//             new[] { 1, 2 },
//         };
//
//
//         private int[][] _topDir = new[]
//         {
//             new[] { -1, 1 },
//             new[] { 0, 1 },
//             new[] { 1, 1 },
//         };
//         
//         private int[][] _leftDir = new[]
//         {
//             new[] { -1, 1 },
//             new[] { -1, 0 },
//             new[] { -1, -1 },
//         };
//         
//         private int[][] _rightDir = new[]
//         {
//             new[] { 1, 1 },
//             new[] { 1, 0 },
//             new[] { 1, -1 },
//         };
//         
//         private int[][] _buttomDir = new[]
//         {
//             new[] { -1, -1 },
//             new[] { 0, -1 },
//             new[] { 1, -1 },
//         };
//         
//         private float threshold = 0.4f; // 定义距离阈值
//         private Rect _mouseRect = new Rect(0,0,0,0);
//
//         private List<int> dynamicIndexs = new List<int>();
//         private List<OnePathChunk> _ignoreChunk = new List<OnePathChunk>();
//         
//         private Vector3 _mousePosition = Vector3.zero;
//         
//         private int _dynamicIndex = -1;
//         private void InitLogic()
//         {
//             SetLineRenderer();
//             _dynamicIndex = -1;
//             _ignoreChunk.Clear();
//             dynamicIndexs.Clear();
//         }
//         
//
//         void SetLineRenderer()
//         {
//             _lineRenderer.startWidth = 0.1f;
//             _lineRenderer.endWidth = 0.1f; 
//             _lineRenderer.positionCount = 0;
//         }
//
//         void Update()
//         {
//             if(!_canTouch)
//                 return;
//             
//             if (Input.GetMouseButtonDown(0))
//             {
//                 InitLogic();
//             }
//             
//             if (Input.GetMouseButton(0))
//             {
//                 if (_dynamicIndex < 0)
//                 {
//                     CalculateChunksForAll();
//                 }
//                 else
//                 {
//                     CalculateChunksForSudoku();
//                 }
//             }
//            
//             if (Input.GetMouseButtonUp(0))
//             {
//             }
//         }
//         
//         Vector3 GetWorldPositionFromMouse()
//         {
//             var mousePosition = CameraManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
//             mousePosition.z = 0;
//             return mousePosition;
//         }
//
//         //首次点击 找到鼠标对应的格子
//         void CalculateChunksForAll()
//         {
//             Vector3 startDrawPoint = GetWorldPositionFromMouse();
//             _mouseRect.Set(startDrawPoint.x-threshold/4, startDrawPoint.y-threshold/4, threshold/2, threshold/2);
//
//             _mousePosition = startDrawPoint;
//             foreach (var chunk in OnePathModel.Instance._onePathConfig._chunks)
//             {
//                 if (!chunk.GetRect(OnePathModel.Instance._onePathConfig._width, OnePathModel.Instance._onePathConfig._height).Overlaps(_mouseRect))
//                     continue;
//
//                 if(_ignoreChunk.Contains(chunk))
//                     continue;
//                 
//                 _dynamicIndex = chunk.index;
//
//                 dynamicIndexs.Clear();
//                 if(!_ignoreChunk.Contains(chunk))
//                     _ignoreChunk.Add(chunk);
//
//                 Debug.Log("----------1 " + _dynamicIndex);
//                 dynamicIndexs.Add(_dynamicIndex);
//                 break;
//             }
//         }
//         
//         void CalculateChunksForSudoku()
//         {
//             Vector3 position = GetWorldPositionFromMouse();
//             if (Vector3.Distance(_mousePosition, position) < 0.001f)
//                 return;
//             
//             Vector2 direction = position - _mousePosition;
//
//             _mousePosition = position;
//             _mouseRect.Set(position.x-threshold/2, position.y-threshold/2, threshold, threshold);
//             
//             var mainChunk = _onePathChunks[_dynamicIndex];
//             if (!mainChunk.GetRect(OnePathModel.Instance._onePathConfig._width, OnePathModel.Instance._onePathConfig._height).Overlaps(_mouseRect))
//                 return;
//
//             var tempIndex = new List<int>(dynamicIndexs);
//             var diffDir = GetPrioritizedDirection(direction.normalized, _dir);
//             
//             foreach (var initIndex in tempIndex)
//             {
//                 int initRow = initIndex / _onePathConfig._gridCol;
//                 int initCol = initIndex % _onePathConfig._gridCol;
//
//                 int dirIndex = 0;
//                 foreach (var dir in diffDir)
//                 {
//                     int col = dir[0];
//                     int row = dir[1];
//
//                     row = initRow + row;
//                     col = initCol + col;
//
//                     int index = _onePathConfig.GetIndex(row, col);
//                     dirIndex++;
//                     if (!_onePathChunks.ContainsKey(index))
//                     {
//                         continue;
//                     }
//
//                     var sudokuChunk = _onePathChunks[index];
//                     if (_ignoreChunk.Contains(sudokuChunk))
//                     {
//                         continue;
//                     }
//                     Debug.Log("_dynamicIndex " + _dynamicIndex + "\t" +dir[0]+"\t"+dir[1]+"\t"+_mouseRect+"\t"+sudokuChunk.GetRect(OnePathModel.Instance._onePathConfig._width, OnePathModel.Instance._onePathConfig._height));
//
//                     if (!sudokuChunk.GetRect(OnePathModel.Instance._onePathConfig._width, OnePathModel.Instance._onePathConfig._height).Overlaps(_mouseRect))
//                     {
//                         continue;
//                     }
//
//                     Debug.Log("-------- dirIndex " + dirIndex + "\t" + index + "\t" +dir[0]+"\t"+dir[1]);
//                     UpdateDrawChunk(sudokuChunk,diffDir);
//                     break;
//                 }
//             }
//         }
//
//         void UpdateDrawChunk(OnePathChunk chunk, int[][] moveDir)
//         {
//             _dynamicIndex = chunk.index;
//             dynamicIndexs.Clear();
//             if(!_ignoreChunk.Contains(chunk))
//                 _ignoreChunk.Add(chunk);
//
//             dynamicIndexs.Add(_dynamicIndex);
//             SetLineRendererPosition(chunk);
//             
//             int initRow = chunk.index/_onePathConfig._gridCol;
//             int initCol = chunk.index % _onePathConfig._gridCol;
//             
//             foreach (var dir in moveDir)
//             {
//                 int col = dir[0];
//                 int row = dir[1];
//             
//                 row = initRow + row;
//                 col = initCol + col;
//             
//                 int index = _onePathConfig.GetIndex(row, col);
//                 
//                 dynamicIndexs.Add(index);
//                 if (!_onePathChunks.ContainsKey(index))
//                     continue;
//                 var sudokuChunk = _onePathChunks[index];
//                 if(_ignoreChunk.Contains(sudokuChunk))
//                     continue;
//                 
//                 _ignoreChunk.Add(sudokuChunk);
//             
//                 SetLineRendererPosition(sudokuChunk);
//             }
//         }
//
//         private void SetLineRendererPosition(OnePathChunk chunk)
//         {
//             if(chunk == null || chunk._pixels == null)
//                 return;
//             
//             Debug.Log("----------Render " + chunk.index);
//             for (int i = 0; i < chunk._pixels.Count; i++)
//             {
//                 int currentPositionCount = _lineRenderer.positionCount;
//                 _lineRenderer.positionCount = currentPositionCount + 1;
//                 _lineRenderer.SetPosition(currentPositionCount, new Vector3(chunk._pixels[i].x, chunk._pixels[i].y, -100));
//                 
//             }
//         }
//         
//         
//          private static float Rad2Deg = (float)(180.0 / Math.PI);
//         
//         // 根据鼠标移动方向获取最接近并符合优先级的预定义方向
//         public int[][] GetPrioritizedDirection(Vector2 mouseDirection, int[][] predefinedDirections, int validNum = -1)
//         {
//             validNum = validNum < 0 ? predefinedDirections.Length : validNum;
//             
//             // 将鼠标方向转换为角度
//             float mouseAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Rad2Deg;
//         
//             // 创建一个列表以存储方向和它们与鼠标方向之间的角度差
//             List<Tuple<int[], float>> directionDiffs = new List<Tuple<int[], float>>();
//         
//             foreach (int[] dir in predefinedDirections)
//             {
//                 // 计算预定义方向的角度
//                 float predefAngle = Mathf.Atan2(dir[1], dir[0]) * Rad2Deg;
//                 // 计算角度差
//                 float angleDiff = Mathf.DeltaAngle(predefAngle, mouseAngle);
//         
//                 // 添加到列表中
//                 directionDiffs.Add(new Tuple<int[], float>(dir, angleDiff));
//             }
//         
//             // 根据角度差对方向进行排序，考虑优先级（假设左方向是负x轴）
//             directionDiffs.Sort((a, b) =>
//             {
//                 // 如果鼠标方向主要向左（负x轴），则调整左方向的优先级
//                 if (mouseDirection.x < 0)
//                 {
//                     // 如果两个方向都在左边，或者都不在左边，比较它们的角度差
//                     if ((a.Item1[0] <= 0 && b.Item1[0] <= 0) || (a.Item1[0] > 0 && b.Item1[0] > 0))
//                     {
//                         return Math.Abs(a.Item2).CompareTo(Math.Abs(b.Item2));
//                     }
//                     // 否则，如果A在左边而B不在左边，那么A应该排在B前面
//                     else if (a.Item1[0] <= 0 && b.Item1[0] > 0)
//                     {
//                         return -1;
//                     }
//                     // 反之亦然
//                     else
//                     {
//                         return 1;
//                     }
//                 }
//                 // 如果鼠标方向不主要向左，就简单地比较角度差
//                 else
//                 {
//                     return Math.Abs(a.Item2).CompareTo(Math.Abs(b.Item2));
//                 }
//             });
//         
//             int[][] diff = new int[validNum][];
//             for (int i = 0; i < validNum; i++)
//                 diff[i] = new int[2];
//         
//             for (int i = 0; i < validNum; i++)
//             {
//                 diff[i][0] = directionDiffs[i].Item1[0];
//                 diff[i][1] = directionDiffs[i].Item1[1];
//             }
//             
//             return diff;
//         }
//
//
//         public int[][] GetPrioritizedDirection(Vector2 mouseDirection)
//         {
//             List<int[]> dir = new List<int[]>();
//
//             if (mouseDirection.y > 0)
//             {
//                 dir.AddRange(_topDir);
//             }
//             else if (mouseDirection.y < 0)
//             {
//                 dir.AddRange(_buttomDir);
//             }
//             
//             if (mouseDirection.x > 0)
//             {
//                 dir.AddRange(_rightDir);
//             }
//             else if (mouseDirection.x < 0)
//             {
//                 dir.AddRange(_leftDir);
//             }
//
//             return dir.ToArray();
//         }
//         
//     }
// }