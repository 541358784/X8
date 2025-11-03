using UnityEngine;

namespace fsm_new
{
    public class AsmrInputHandler_Swipe_Single : AsmrInputHandler_Base
    {
        private Vector3 touchStartPos;
        private float swipeDistance = 0f; // 实时滑动距离
        public float totalDistance = 0f; // 总滑动距离

        private bool _touchStarted; //触摸开始

        public override void Update()
        {
            switch (Input.touchCount)
            {
                case > 0:
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Began)
                    {
                        touchStartPos = touch.position;
                        swipeDistance = 0f;

                        _touchStarted = true;
                    }
                    else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        if (!_touchStarted) return;

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

                    break;
                }
                default:
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        _touchStarted = true;

                        touchStartPos = Input.mousePosition;
                        swipeDistance = 0f;
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        if (!_touchStarted) return;

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

                    break;
                }
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