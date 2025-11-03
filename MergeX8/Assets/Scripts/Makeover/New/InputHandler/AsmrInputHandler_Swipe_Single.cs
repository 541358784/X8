using UnityEngine;

namespace MiniGame
{
    public class AsmrInputHandler_Swipe_Single : AsmrInputHandler_Base
    {
        private Vector3 touchStartPos;
        private float swipeDistance = 0f; // 实时滑动距离
        public float totalDistance = 0f; // 总滑动距离

        public override void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    touchStartPos = touch.position;
                    swipeDistance = 0f;
                }
                else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    Vector3 touchCurrentPos = touch.position;
                    swipeDistance = (touchCurrentPos - touchStartPos).magnitude;

                    // 在这里执行实时计算滑动距离后的逻辑
                    //Debug.Log("实时滑动距离: " + swipeDistance);

                    totalDistance += swipeDistance; // 累加到总滑动距离

                    touchStartPos = touchCurrentPos; // 更新起始位置为当前位置
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    // 在这里执行滑动结束后的逻辑，例如输出总滑动距离
                    //Debug.Log("总滑动距离: " + totalDistance);
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;
                swipeDistance = 0f;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 touchCurrentPos = Input.mousePosition;
                swipeDistance = (touchCurrentPos - touchStartPos).magnitude;

                // 在这里执行实时计算滑动距离后的逻辑
                //Debug.Log("实时滑动距离: " + swipeDistance);

                totalDistance += swipeDistance; // 累加到总滑动距离

                touchStartPos = touchCurrentPos; // 更新起始位置为当前位置
            }
            else if (Input.GetMouseButtonUp(0))
            {
                // 在这里执行滑动结束后的逻辑，例如输出总滑动距离
                // Debug.Log("总滑动距离: " + totalDistance);
            }
        }


        // private bool _startVibration;

        public void FixedUpdate(float deltaTime)
        {
            // if (Input.touchCount > 0)
            // {
            //     AsmrLevel.VibrationShortFix();
            // }
        }
    }
}