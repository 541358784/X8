#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using OnePath;

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpriteRenderer))]
public class OnePathEditor : MonoBehaviour
{
    public float _chunkWidth = 0.1f;
    public float _chunkHeight = 0.1f;
    public int _gridRow = 50;
    public int _gridCol = 30;
    public float _orgPosX = 0;
    public float _orgPosY = 0;
    public  float _threshold = 0.03f;
    
    public LineRenderer _lineRender;
    
    private Vector3 _orgPosition = Vector3.zero;
    private Vector2Int _worldOrgPosition = Vector2Int.zero;

    private SpriteRenderer _sprite;

    private OnePathConfig _config = new OnePathConfig();
    private List<Vector2> _ignorePixel = new List<Vector2>();
    
    private void Awake()
    {
        _sprite = transform.GetComponent<SpriteRenderer>();
    }

    public void Save()
    {
        _sprite = transform.GetComponent<SpriteRenderer>();
        
        _ignorePixel.Clear();
        _config._chunks.Clear();

        _config._width = _chunkWidth;
        _config._height = _chunkHeight;
        _config._gridRow = _gridRow;
        _config._gridCol = _gridCol;
        _config._x = _orgPosX;
        _config._y = _orgPosY;
            
        _orgPosition.x = -1.0f * _gridCol / 2 * _chunkWidth + _orgPosX;
        _orgPosition.y = 1.0f * _gridRow / 2 * _chunkHeight + _orgPosY;

        _worldOrgPosition.x = (int)Math.Ceiling(_sprite.sprite.texture.width/2 + _orgPosX*100 - (_gridCol / 2 * _chunkWidth) * 100)-2;
        _worldOrgPosition.y = (int)Math.Ceiling(_gridRow * _chunkHeight * 100 + _sprite.sprite.texture.height/2 + _orgPosY*100 - _gridRow / 2 * _chunkHeight * 100);
        
        if(_lineRender != null)
            _lineRender.positionCount = 0;

        int index = 0;
        for (int i = 0; i < _gridRow * _gridCol; i++)
        {
            var chunk = new OnePathChunk();
            chunk.index = i;
            chunk.row = i / _gridCol;
            chunk.col = i % _gridCol;

            FillChunk(chunk);
            
            if(chunk._pixels.Count == 0)
                continue;
            
            _config._chunks.Add(chunk);

            if (_lineRender != null)
            {
                chunk._pixels.ForEach(a =>
                {
                    int currentPositionCount = _lineRender.positionCount;
                    _lineRender.positionCount = currentPositionCount + 1;
                    _lineRender.SetPosition(currentPositionCount, new Vector3(a.x, a.y, 0));
                });
            }

            // index++;
            // if(index >=3)
            //     break;
        }
        
        var configStr = JsonConvert.SerializeObject(_config, Formatting.Indented);
        File.WriteAllText($"{Application.dataPath}/Export/Configs/OnePath/{_sprite.name}.json", configStr);
        AssetDatabase.Refresh();
    }

    private void FillChunk(OnePathChunk chunk)
    {
        Vector3 centerPos = new Vector3(_orgPosition.x+chunk.col * _chunkWidth + _chunkWidth/2, _orgPosition.y-chunk.row * _chunkHeight-_chunkHeight/2);
        
        chunk.minX = centerPos.x - _chunkWidth / 2 ;
        chunk.minY = centerPos.y - _chunkHeight / 2;
        
        int initY = _worldOrgPosition.y-(int)((chunk.row * _chunkHeight)*100);
        int initX = _worldOrgPosition.x+(int)((chunk.col * _chunkWidth)*100);;
        int initW = (int)(_chunkWidth * 100);
        int initH = (int)(_chunkHeight * 100);

        Vector2 oldPoint = new Vector2(-10000, -1000);
        //Rect gridRect = new Rect(chunk.minX, chunk.minY, _chunkWidth, _chunkHeight);
        // 纹理遍历
        for (int y = initY+5; y > initY-initH-5; y--)
        {
            for (int x = initX-2; x < initX+initW+2; x++)
            {
                if (x < 0 || x >= _sprite.sprite.texture.width || y < 0 || y >= _sprite.sprite.texture.height)
                    continue;

                Color pixelColor = _sprite.sprite.texture.GetPixel(x, y);
                if(pixelColor.a <= 0)
                    continue;
                
                var pixelPoint = new Vector2(x, y);
                if(_ignorePixel.Contains(pixelPoint))
                    continue;
                
                if(!IsEdge(_sprite.sprite.texture, x,y))
                    continue;

                Vector2 point = ConvertToPathPoint(x,y);
                _ignorePixel.Add(pixelPoint);

                if(Vector2.Distance(oldPoint, point) < _threshold)
                    continue;

                oldPoint = point;
                
                OnePathPixel pixel = new OnePathPixel();
                pixel.x = point.x;
                pixel.y = point.y;
                // pixel.worldX = point.x;
                // pixel.worldY = point.y;
                chunk._pixels.Add(pixel);
            }
        }
    }
    
    private bool IsEdge(Texture2D texture, int x, int y)
    {
        // 检查周围8个像素中是否存在非黑色像素
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; // 跳过自身
                
                int checkX = x + i;
                int checkY = y + j;

                // 确保检查的点在纹理范围内
                if (checkX >= 0 && checkX < texture.width && checkY >= 0 && checkY < texture.height)
                {
                    Color checkColor = texture.GetPixel(checkX, checkY);
                    if (checkColor != Color.black)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    
    private Vector2 ConvertToPathPoint(int x, int y)
    {
        // 获取精灵的边界
        Bounds bounds = _sprite.bounds;

        int width = _sprite.sprite.texture.width;
        int height = _sprite.sprite.texture.height;
        
        // 计算精灵的纹理像素尺寸
        float pixelsPerUnit = _sprite.sprite.pixelsPerUnit;
        float spriteWidthInPixels = _sprite.sprite.rect.width;
        float spriteHeightInPixels = _sprite.sprite.rect.height;

        // 将纹理坐标转换为相对于精灵矩形的坐标
        float relativeX = (float)x / width * spriteWidthInPixels / pixelsPerUnit;
        float relativeY = (float)y / height * spriteHeightInPixels / pixelsPerUnit;

        // 转换坐标为世界坐标
        Vector2 worldPoint = new Vector2(bounds.min.x + relativeX, bounds.min.y + relativeY);
        return worldPoint;
    }
    
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            float height = _gridRow * _chunkHeight;
            float width = _gridCol * _chunkWidth;

            _orgPosition.x = _orgPosX;
            _orgPosition.y = _orgPosY;
            //Gizmos.DrawWireCube(_orgPosition,  new Vector3(width, height));

            OnDrawRowCol();
        }

        private void OnDrawRowCol()
        {
            Gizmos.color = Color.green;

            Vector3 orgPosition = Vector3.zero;
            orgPosition.x = -1.0f * _gridCol / 2 * _chunkWidth + _orgPosition.x;
            orgPosition.y = 1.0f * _gridRow / 2 * _chunkHeight + _orgPosition.y;

            Vector3 endPosition = Vector3.zero;
            endPosition.x = 1.0f * _gridCol / 2 * _chunkWidth + _orgPosition.x;
            endPosition.y = -1.0f * _gridRow / 2 * _chunkHeight + _orgPosition.y;

            for (var y = 0; y <= _gridRow; y++)
            {
                Gizmos.DrawLine(new Vector3(orgPosition.x, orgPosition.y - y * _chunkHeight, 0), new Vector3(endPosition.x, orgPosition.y - y * _chunkHeight, 0));
            }
            
            Gizmos.color = Color.green;
            for (var x = 0; x <= _gridCol; x++)
            {
                Gizmos.DrawLine(new Vector3(orgPosition.x + x * _chunkWidth, orgPosition.y, 0), new Vector3(orgPosition.x + x * _chunkWidth, endPosition.y, 0));
            }

            GUIStyle style = new GUIStyle();
            style.fontSize = 6; // 设置字体大小
            
            for (var x = 0; x < _gridCol; x++)
            {
                for (var y = 0; y < _gridRow; y++)
                {
                    Vector3 position = Vector3.zero;
                    position.x = orgPosition.x + x * _chunkWidth;
                    position.y = orgPosition.y - y * _chunkHeight;
                    position.z = 0;
                    
                    int index = y * _gridCol + x;
                    
                    Handles.Label(position, index.ToString(), style);
                }
            }
        }
}
#endif
