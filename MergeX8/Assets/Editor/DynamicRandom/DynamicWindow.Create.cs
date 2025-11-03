using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DynamicRandom.Editor
{
    public partial class DynamicWindow
    {
        private Color[] _colors = new[]
        {
            Color.red, 
            Color.blue, 
            Color.cyan, 
            Color.white, 
            Color.yellow, 
            Color.magenta, 
            new Color(255f/255f, 151f/255f, 235f/255f, 1),
            new Color(132f/255f, 160f/255f, 200f/255f, 1),
            new Color(132f/255f, 160f/255f, 25f/255f, 1),
            new Color(50f/255f, 255f/255f, 190f/255f, 1),
        };
        
        public class ShapeData
        {
            public Vector2Int _size;
            public List<Vector2Int> _position = new List<Vector2Int>();
        }

        private List<ShapeData> _shapeDatas = new List<ShapeData>();
        private void CreateShape()
        {
            _shapeDatas.Clear();
            
            if (_gridSize.x <= 0 || _gridSize.y <= 0)
            {
                UnityEditor.EditorUtility.DisplayDialog("错误", "没有设置格子 行-列", "确定");
                return;
            }

            if (_shapeList.Count == 0)
            {
                UnityEditor.EditorUtility.DisplayDialog("错误", "没有设置形状", "确定");
                return;
            }

            for (int i = 0; i < _shapeList.Count; i++)
            {
                if (_shapeList[i].x <= 0 || _shapeList[i].y <= 0)
                {
                    UnityEditor.EditorUtility.DisplayDialog("错误", $"形状{i+1} 行列设置错误", "确定");
                    return;
                }
            }

            PlaceShape();
        }

        private void PlaceShape()
        {
            int[,] board = new int[_gridSize.x, _gridSize.y];

            int loopNumLimit = 1000000;
            int loopTotalNum = 0;
            for(int i = 0; i < _shapeList.Count; i ++)
            {
                var size = _shapeList[i];
                
                bool placed = false;
                int interiorLoopNum = 0;
                while (!placed)
                {
                    int row = Random.Range(0, _gridSize.x);
                    int col = Random.Range(0, _gridSize.y);
                
                    if (CanPlaceShape(board, size, row, col))
                    {
                        PlaceShape(board, size, row, col, i+1);
                        placed = true;
                    }
                    else
                    {
                        interiorLoopNum++;
                        loopTotalNum++;
                        if(interiorLoopNum >= loopNumLimit)
                            break;
                    }
                }

                if (interiorLoopNum >= loopNumLimit)
                {
                    UnityEditor.EditorUtility.DisplayDialog("错误", $"放置形状失败,请重新尝试 生成总次数:{loopTotalNum}", "确定");
                    _shapeDatas.Clear();
                    return;
                }
            }

            PrintBoard(board);
            bool reslut = UnityEditor.EditorUtility.DisplayDialog("成功", $"生成成功 确定复制  生成总次数:{loopTotalNum}", "确定");
            if (reslut)
            {
                string copyString = "";
                copyString += "" + _gridSize.x + _gridSize.y+",";
                for (var i = 0; i < _shapeDatas.Count; i++)
                {
                    ShapeData data = _shapeDatas[i];
                    copyString += "" + data._size.x+data._size.y+";";
                    for (var j = 0; j < data._position.Count; j++)
                    {
                        var position = data._position[j];
                        copyString += position.x + ":" + position.y;
                        if (j < data._position.Count - 1)
                            copyString += ";";
                    }
                    
                    if(i < _shapeDatas.Count-1)
                        copyString += ",";
                }

                GUIUtility.systemCopyBuffer = copyString;
            }
        }

        private bool CanPlaceShape(int[,] board, Vector2Int size, int startRow, int startCol)
        {
            int shapeRows = size.x;
            int shapeCols = size.y;

            if (startRow + shapeRows > board.GetLength(0) || startCol + shapeCols > board.GetLength(1))
                return false;

            for (int i = 0; i < shapeRows; i++)
            {
                for (int j = 0; j < shapeCols; j++)
                {
                    if (board[startRow + i, startCol + j] != 0)
                        return false;
                }
            }
            return true;
        }
        
        private void PlaceShape(int[,] board, Vector2Int size, int startRow, int startCol, int index)
        {
            int shapeRows = size.x;
            int shapeCols = size.y;

            ShapeData data = new ShapeData();
            data._size = size;
            
            for (int i = 0; i < shapeRows; i++)
            {
                for (int j = 0; j < shapeCols; j++)
                {
                    board[startRow + i, startCol + j] = index;
                    Vector2Int pos = new Vector2Int(startRow + i, startCol + j);
                    data._position.Add(pos);
                }
            }
            
            _shapeDatas.Add(data);
        }
        
        private void PrintBoard(int[,] board)
        {
            ClearConsole();
            
            for (int i = 0; i < board.GetLength(0); i++)
            {
                string str = "";
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    str += board[i, j] + "\t";
                }
                Debug.Log(str + "\n");
            }
        }

        private void ClearConsole()
        {
            var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }

        private void DrawShape()
        {
            if(_shapeDatas.Count == 0)
                return;

            Vector3 orgPosition = Vector3.zero;
            orgPosition.x = -1.0f*_boardSize * _gridSize.y / 2;
            orgPosition.y = 1.0f*_boardSize * _gridSize.x / 2;
            
            for (var i = 0; i < _shapeDatas.Count; i++)
            {
                ShapeData data = _shapeDatas[i];

                int colorIndex = i;
                colorIndex = i >= _colors.Length ? (i - _colors.Length) : colorIndex;
                Handles.color = _colors[colorIndex];
                for (var j = 0; j < data._position.Count; j++)
                { 
                    var position = data._position[j];
                    Vector3 centerPosition = orgPosition + new Vector3(position.y*_boardSize, -position.x*_boardSize);
                    Vector3 topLeft = centerPosition;//+ new Vector3(-_boardSize/2, _boardSize/2, 0);
    
                    Rect rect = new Rect(topLeft.x, topLeft.y, _boardSize, -_boardSize);
                    Handles.DrawSolidRectangleWithOutline(rect, Handles.color, Handles.color);
                }
            }
        }
    }
}