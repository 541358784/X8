using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class ShowAllBoxColliderBounds
{
    static ShowAllBoxColliderBounds()
    {
        // 注册回调以在场景视图中绘制边界框
        SceneView.duringSceneGui += DrawColliderBounds;
    }
    private const string BoolKey = "ShouldShowAllBoxColliderBounds";

    [MenuItem("Tools/显示或隐藏2D碰撞体轮廓")]
    private static void ShouldShowAllBoxColliderBounds()
    {
        bool currentState = EditorPrefs.GetBool(BoolKey, false); // 默认为false
        bool newState = !currentState; // 切换状态
        EditorPrefs.SetBool(BoolKey, newState); // 将新状态存储到EditorPrefs
    }

    [MenuItem("MyMenu/显示或隐藏2D碰撞体轮廓", true)] // 这个方法是用来检查菜单项是否应该处于活动状态
    private static bool ValidateShouldShowAllBoxColliderBounds()
    {
        return true; // 返回true表示菜单项处于活动状态
    }
    private static bool Active => EditorPrefs.GetBool(BoolKey, false);
    private static void DrawColliderBounds(SceneView sceneView)
    {
        if (Active)
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null)
            {
                BoxCollider2D[] colliders = GameObject.FindObjectsOfType<BoxCollider2D>();

                // 绘制每个碰撞器的边界框
                foreach (BoxCollider2D collider in colliders)
                {
                    DrawBoxColliderBounds(collider);
                }
                var polygonColliders = GameObject.FindObjectsOfType<PolygonCollider2D>();
                foreach (PolygonCollider2D collider in polygonColliders)
                {
                    DrawPolygonColliderBounds(collider);
                }
                var circleColliders = GameObject.FindObjectsOfType<CircleCollider2D>();
                foreach (CircleCollider2D collider in circleColliders)
                {
                    DrawCircleColliderBounds(collider);
                }
            }
            else
            {
                var colliders = prefabStage.prefabContentsRoot.GetComponentsInChildren<BoxCollider2D>();
                foreach (BoxCollider2D collider in colliders)
                {
                    DrawBoxColliderBounds(collider);
                }
                var polygonColliders = prefabStage.prefabContentsRoot.GetComponentsInChildren<PolygonCollider2D>();
                foreach (PolygonCollider2D collider in polygonColliders)
                {
                    DrawPolygonColliderBounds(collider);
                }
                var circleColliders = prefabStage.prefabContentsRoot.GetComponentsInChildren<CircleCollider2D>();
                foreach (CircleCollider2D collider in circleColliders)
                {
                    DrawCircleColliderBounds(collider);
                }
            }
        }
    }

    private static void DrawBoxColliderBounds(BoxCollider2D collider)
    {
        // 计算边界框的四个角点
        Vector3[] points = new Vector3[5];
        Vector2 center = collider.transform.TransformPoint(collider.offset);
        Vector2 size = collider.size;

        points[0] = center + new Vector2(-size.x / 2 * collider.transform.lossyScale.x, -size.y / 2 * collider.transform.lossyScale.y);
        points[1] = center + new Vector2(-size.x / 2 * collider.transform.lossyScale.x, size.y / 2* collider.transform.lossyScale.y);
        points[2] = center + new Vector2(size.x / 2 * collider.transform.lossyScale.x, size.y / 2* collider.transform.lossyScale.y);
        points[3] = center + new Vector2(size.x / 2 * collider.transform.lossyScale.x, -size.y / 2* collider.transform.lossyScale.y);
        points[4] = points[0];
        if (collider.transform.rotation.z != 0)
        {
            // 定义旋转角度
            float angleInDegrees = -collider.transform.rotation.z;
            float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

            // 循环遍历每个点
            for (int i1 = 0; i1 < points.Length; i1++)
            {
                // 计算每个点相对于圆心的坐标
                Vector2 relativePosition = (Vector2)points[i1] - center;

                // 应用旋转矩阵
                float rotatedX = relativePosition.x * Mathf.Cos(angleInRadians) - relativePosition.y * Mathf.Sin(angleInRadians);
                float rotatedY = relativePosition.x * Mathf.Sin(angleInRadians) + relativePosition.y * Mathf.Cos(angleInRadians);

                // 将旋转后的相对坐标加回到圆心的坐标
                Vector2 rotatedPosition = new Vector2(rotatedX, rotatedY) + center;

                // 更新点的坐标
                points[i1] = rotatedPosition;
            }
        }
        // 设置绘制颜色并绘制边界框
        Handles.color = Color.cyan;
        Handles.DrawPolyLine(points);
    }
    private static void DrawPolygonColliderBounds(PolygonCollider2D collider)
    {
        Vector2 center = collider.transform.TransformPoint(collider.offset);
        var lossyScale = collider.transform.lossyScale;
        for (var i = 0; i < collider.pathCount; i++)
        {
            var path = collider.GetPath(i);
            Vector3[] points = new Vector3[path.Length+1];
            for (var j = 0; j < path.Length; j++)
            {
                var pathPoint = path[j];
                points[j] = center + new Vector2(pathPoint.x * lossyScale.x, pathPoint.y * lossyScale.y);
            }
            points[points.Length - 1] = points[0];
            if (collider.transform.rotation.z != 0)
            {
                // 定义旋转角度
                float angleInDegrees = collider.transform.rotation.eulerAngles.z;
                float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

                // 循环遍历每个点
                for (int i1 = 0; i1 < points.Length; i1++)
                {
                    // 计算每个点相对于圆心的坐标
                    Vector2 relativePosition = (Vector2)points[i1] - center;

                    // 应用旋转矩阵
                    float rotatedX = relativePosition.x * Mathf.Cos(angleInRadians) - relativePosition.y * Mathf.Sin(angleInRadians);
                    float rotatedY = relativePosition.x * Mathf.Sin(angleInRadians) + relativePosition.y * Mathf.Cos(angleInRadians);

                    // 将旋转后的相对坐标加回到圆心的坐标
                    Vector2 rotatedPosition = new Vector2(rotatedX, rotatedY) + center;

                    // 更新点的坐标
                    points[i1] = rotatedPosition;
                }
            }
            Handles.color = Color.cyan;
            Handles.DrawPolyLine(points);
        }
    }
    private static void DrawCircleColliderBounds(CircleCollider2D collider)
    {
        Vector2 center = collider.transform.TransformPoint(collider.offset);
        var radius = collider.radius;
        var scale = Math.Max(collider.transform.lossyScale.x, collider.transform.lossyScale.y);
        // Matrix4x4 matrix = Matrix4x4.TRS(center, Quaternion.identity, new Vector3(scale, scale, 1.0f));
        // Handles.matrix = matrix;
        // 设置绘制颜色并绘制边界框
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(center, Vector3.forward, radius*scale);
        // 恢复默认的变换矩阵
        // Handles.matrix = Matrix4x4.identity;
    }
}