// using UnityEngine;
// using System;
//
// namespace MiniGame
// {
//     public class AsmrInputHandler_ClickUp : AsmrInputHandler_Base
//     {
//         private Vector3 startPosition;
//         private float dragThreshold = 0.1f;
//         private Action<Vector3> clickCallback;
//         private Camera camera;
//
//         public AsmrInputHandler_ClickUp(Action<Vector3> callback, Camera camera)
//         {
//             clickCallback = callback;
//             this.camera = camera;
//         }
//
//         public override void Update()
//         {
//             if (Input.GetMouseButtonDown(0) || (Input.touchSupported && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
//             {
//                 startPosition = GetInputPosition();
//             }
//             else if (Input.GetMouseButtonUp(0) || (Input.touchSupported && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
//             {
//                 Vector3 currentInputPosition = GetInputPosition();
//                 float distance = Vector3.Distance(startPosition, currentInputPosition);
//                 if (distance <= dragThreshold)
//                 {
//                     Vector3 worldPosition = camera.ScreenToWorldPoint(startPosition);
//                     clickCallback?.Invoke(worldPosition);
//                     MiniModel.Instance.PlaySound("yx_common_tap");
//                 }
//             }
//         }
//
//         private Vector3 GetInputPosition()
//         {
//             if (Input.touchSupported && Input.touchCount > 0)
//             {
//                 return Input.GetTouch(0).position;
//             }
//             else
//             {
//                 return Input.mousePosition;
//             }
//         }
//     }
// }