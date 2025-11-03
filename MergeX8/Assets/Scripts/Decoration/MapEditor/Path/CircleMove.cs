using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMove : MonoBehaviour
{
    public float radius = 5f; // 圆形轨迹的半径
    public float speed = 1f; // 鱼的移动速度
    public float angle = 0;
    
    private Vector3 centerPoint; // 圆心位置

    private void Start()
    {
        centerPoint = transform.position;
    }

    private void Update()
    {
        float vAngle = angle + Time.time * speed;
        float x = centerPoint.x + Mathf.Cos(vAngle) * radius;
        float z = centerPoint.z + Mathf.Sin(vAngle) * radius;
        Vector3 newPosition = new Vector3(x, centerPoint.y, z);

        transform.position = newPosition;

        Vector3 direction = newPosition - centerPoint;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
