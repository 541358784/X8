using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

public class OnePathGenerator : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;

    public LineRenderer lineRenderer;
    
    public LineRenderer userLineRenderer; // 用户绘制轨迹的LineRenderer

    public Camera _camera;
    
    private bool isDrawing = false;
    private float threshold = 0.002f; // 定义距离阈值
    
    
    private List<Vector2> FindEdgePoints(Texture2D texture)
    {
        List<Vector2> edgePoints = new List<Vector2>();

        // 纹理遍历
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color pixelColor = texture.GetPixel(x, y);

                // 检测黑色像素
                if (pixelColor == Color.black)
                {
                    // // 这里使用了一个简单的策略：检查周围的像素是否有白色，如果有则认为当前像素是边缘
                     if (IsEdge(texture, x, y))
                     {
                        // 转换坐标为世界坐标或局部坐标
                        Vector2 worldPoint = ConvertToPathPoint(x, y, texture.width, texture.height);
                        edgePoints.Add(worldPoint);
                    }
                }
            }
        }

        return edgePoints;
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

    private Vector2 ConvertToPathPoint(int x, int y, int width, int height)
    {
        // 获取精灵的边界
        Bounds bounds = SpriteRenderer.bounds;

        // 计算精灵的纹理像素尺寸
        float pixelsPerUnit = SpriteRenderer.sprite.pixelsPerUnit;
        float spriteWidthInPixels = SpriteRenderer.sprite.rect.width;
        float spriteHeightInPixels = SpriteRenderer.sprite.rect.height;

        // 将纹理坐标转换为相对于精灵矩形的坐标
        float relativeX = (float)x / width * spriteWidthInPixels / pixelsPerUnit;
        float relativeY = (float)y / height * spriteHeightInPixels / pixelsPerUnit;

        // 转换坐标为世界坐标
        Vector2 worldPoint = new Vector2(bounds.min.x + relativeX, bounds.min.y + relativeY);
        return worldPoint;
    }


    void Start()
    {
        List<Vector2> path = FindEdgePoints(SpriteRenderer.sprite.texture);

        // 设置LineRenderer属性
        SetupLineRenderer(); 
        
        // 绘制路径
        DrawPath(path);
        
        userLineRenderer.positionCount = 0;
        userLineRenderer.startWidth = 0.1f;
        userLineRenderer.endWidth = 0.1f;
    }
    
    
    void SetupLineRenderer()
    {
        lineRenderer.startWidth = 0.1f; // 起始宽度
        lineRenderer.endWidth = 0.1f;   // 结束宽度
    }
    
    void DrawPath(List<Vector2> points)
    {
        // 设定顶点数量
        lineRenderer.positionCount = points.Count;

        // 设置每个顶点的位置
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 pointPosition = new Vector3(points[i].x, points[i].y, 0f); // 可能需要调整z坐标，或者其他转换
            lineRenderer.SetPosition(i, pointPosition);
        }
    }

    
    
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 startDrawPoint = GetWorldPositionFromMouse();
            if (IsPointCloseToPath(startDrawPoint))
            {
                StartDrawing(startDrawPoint);
            }
        }

        if (isDrawing && Input.GetMouseButton(0))
        {
            DrawUserPath(GetWorldPositionFromMouse());
        }

        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            isDrawing = false;
            // TODO: 检查用户绘制的轨迹是否完成
        }
    }
    
    void StartDrawing(Vector3 startPoint)
    {
        isDrawing = true;
        userLineRenderer.positionCount = 1;
        userLineRenderer.SetPosition(0, startPoint);
    }

    void DrawUserPath(Vector3 currentPoint)
    {
        if (IsPointCloseToPath(currentPoint))
        {
            int currentPositionCount = userLineRenderer.positionCount;
            userLineRenderer.positionCount = currentPositionCount + 1;
            userLineRenderer.SetPosition(currentPositionCount, currentPoint);
        }
    }

    bool IsPointCloseToPath(Vector3 point)
    {
        float minDistanceSqr = threshold;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 pathPoint = lineRenderer.GetPosition(i);
            float distSqr = (point - pathPoint).sqrMagnitude;

            if (distSqr <= minDistanceSqr)
            {
                return true;
            }
        }
        return false;
    }

    Vector3 GetWorldPositionFromMouse()
    {
        var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        return mousePosition;
    }
}
