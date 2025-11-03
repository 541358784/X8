
using System.Threading.Tasks;
using DG.Tweening;
using Framework;
using UnityEngine;


namespace TMatch
{
    public partial class UITMatchMainController
    {
        private bool lowSpace;
        private bool destoryLowSpace;

        private void InitLowSpace()
        {
            Vector3 pos = CameraManager.MainCamera.WorldToScreenPoint(TMatchEnvSystem.Instance.CollectorTopPos);
            pos = CameraManager.UICamera.ScreenToWorldPoint(pos);
            transform.Find("Root/TagGroup").position = pos;
            transform.Find("Root/TagGroup/BG").localPosition = new Vector3(0.0f, -27.5f, 0.0f);

            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_COLLECTOR_ITEMS_CHANGE, OnCollectorItemsChangeEvent);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_LOW_SPACE_TIP, OnLowSpaceEvt);
        }

        private void DestoryLowSpace()
        {
            destoryLowSpace = true;
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_COLLECTOR_ITEMS_CHANGE, OnCollectorItemsChangeEvent);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_LOW_SPACE_TIP, OnLowSpaceEvt);
        }

        private void OnCollectorItemsChangeEvent(BaseEvent evt)
        {
            TMatchCollectorItemChange realEvt = evt as TMatchCollectorItemChange;
            if (realEvt.cnt >= 6)
            {
                transform.Find("Root/TagGroup/BG").DOLocalMove(new Vector3(0.0f, 27.5f, 0.0f), 0.2f);
                lowSpace = true;
            }
            else
            {
                transform.Find("Root/TagGroup/BG").DOLocalMove(new Vector3(0.0f, -27.5f, 0.0f), 0.2f);
                lowSpace = false;
            }
        }

        private async void OnLowSpaceEvt(BaseEvent evt)
        {
            transform.Find("Root/TagGroup/BG").DOLocalMove(new Vector3(0.0f, 27.5f, 0.0f), 0.2f);
            await Task.Delay(1000);
            if (destoryLowSpace) return;
            if (!lowSpace) transform.Find("Root/TagGroup/BG").DOLocalMove(new Vector3(0.0f, -27.5f, 0.0f), 0.2f);
        }
    }
}