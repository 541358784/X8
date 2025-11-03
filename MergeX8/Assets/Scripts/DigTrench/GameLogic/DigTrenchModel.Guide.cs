using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DigTrench
{
    public partial class DigTrenchModel
    {
        public class CheckAnyTouch : MonoBehaviour
        {
            private TaskCompletionSource<bool> _anyTouchTask;
            private void Awake()
            {
                _anyTouchTask = new TaskCompletionSource<bool>();
            }

            public Task AnyTouch()
            {
                return _anyTouchTask.Task;
            }
            private void Update()
            {
                if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)  || Input.GetMouseButtonDown(0))
                {
                    _anyTouchTask.TrySetResult(true);
                }
            }
        }

        private int eventSystemEnableFrameCount = 0;//手机上EventSystem启动时未抬起的触碰点会自动触发一次OnPointerDown,需要手动避免掉
        public async void PlayGuide(int guidePositionNum)
        {
            var guidePosition = (GuideTriggerPosition) guidePositionNum;
            if (!GuideSubSystem.Instance.isFinished(guidePosition))
            {
                var eventSystem = EventSystem.current;
                PointerEventData eventDataPointerUp = new PointerEventData(eventSystem);
                eventDataPointerUp.position = Input.mousePosition;
                _isInDrag = false;
                // OnPointerUp(eventDataPointerUp);
                GuideSubSystem.Instance.Trigger(guidePosition, null);
                eventSystem.enabled = false;
                await XUtility.WaitSeconds(0.6f);
                eventSystemEnableFrameCount = Time.frameCount;
                eventSystem.enabled = true;
                Debug.LogError("开启忽略第一次触碰机制");
                await AnyTouch();
                GuideSubSystem.Instance.FinishCurrent();
            }
        }

        public async Task AnyTouch()
        {
            var checkTouch = gameObject.AddComponent<CheckAnyTouch>();
            await checkTouch.AnyTouch();
            if (!this)
                return;
            Destroy(checkTouch);
        }
    }
}