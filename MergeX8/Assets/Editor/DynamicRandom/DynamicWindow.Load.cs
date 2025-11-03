using UnityEngine;

namespace DynamicRandom.Editor
{
    public partial class DynamicWindow
    {
        private void LoadData(string data)
        {
            if (data.IsEmptyString())
            {
                UnityEditor.EditorUtility.DisplayDialog("错误", "数据为Null", "确定");
                return;
            }

            var info = data.Split(',');
            if (info == null || info.Length == 0)
            {
                UnityEditor.EditorUtility.DisplayDialog("错误", "数据错误", "确定");
                return;
            }

            int size = 0;
            if(!int.TryParse(info[0], out size))
            {
                UnityEditor.EditorUtility.DisplayDialog("错误", "数据错误", "确定");
                return;
            }

            _gridSize.x = size / 10;
            _gridSize.y = size % 10;

            _shapeDatas.Clear();
            _shapeList.Clear();
            for (int i = 1; i < info.Length; i++)
            {
                var shapeStr = info[i].Split(';');
                size = int.Parse(shapeStr[0]);

                ShapeData shapeData = new ShapeData();
                shapeData._size.x = size / 10;
                shapeData._size.y = size % 10;

                _shapeList.Add(shapeData._size);
                    
                for (int j = 1; j < shapeStr.Length; j++)
                {
                    var shapeString = shapeStr[j].Split(':');
                    
                    Vector2Int gridxy = new Vector2Int(int.Parse(shapeString[0]), int.Parse(shapeString[1]));
                    shapeData._position.Add(gridxy);
                }
                
                _shapeDatas.Add(shapeData);
            }
            
            
            UnityEditor.EditorUtility.DisplayDialog("成功", "加载数据成功", "确定");
        }
    }
}