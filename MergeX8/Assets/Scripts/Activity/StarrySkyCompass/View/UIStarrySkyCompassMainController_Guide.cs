using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public partial class UIStarrySkyCompassMainController
{
    private bool GuideSpin = false;
    public void CheckSpinGuide()
    {
        var position = GuideTriggerPosition.StarrySkyCompassSpin1;
        var target = GuideTargetType.StarrySkyCompassSpin1;
        if (!GuideSubSystem.Instance.isFinished(position))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(StartBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(target, StartBtn.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(position, null))
            {
                GuideSpin = true;
            }
        }
    }

    public async Task CheckArrowGuide()
    {
        var position = GuideTriggerPosition.StarrySkyCompassArrow;
        if (!GuideSubSystem.Instance.isFinished(position))
        {
            if (GuideSubSystem.Instance.Trigger(position, null))
            {
                var task = new TaskCompletionSource<bool>();
                Action<BaseEvent> EventCallback = null;
                EventCallback = (evt) =>
                {
                    var guideConfig = evt.datas[0] as TableGuide;
                    if (guideConfig.triggerPosition == (int) position)
                    {
                        task.SetResult(true);
                        EventDispatcher.Instance.RemoveEventListener(EventEnum.GuideFinish,EventCallback);
                    }
                };
                EventDispatcher.Instance.AddEventListener(EventEnum.GuideFinish,EventCallback);
                await task.Task;
            }
        }
    }
    
    public void CheckSpinGuide2()
    {
        var position = GuideTriggerPosition.StarrySkyCompassSpin2;
        var target = GuideTargetType.StarrySkyCompassSpin2;
        if (!GuideSubSystem.Instance.isFinished(position))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(StartBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(target, StartBtn.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(position, null))
            {
                GuideSpin = true;
            }
        }
    }
    
    public async Task CheckHappyGuide1()
    {
        var position = GuideTriggerPosition.StarrySkyCompassHappy1;
        if (!GuideSubSystem.Instance.isFinished(position))
        {
            if (GuideSubSystem.Instance.Trigger(position, null))
            {
                var task = new TaskCompletionSource<bool>();
                Action<BaseEvent> EventCallback = null;
                EventCallback = (evt) =>
                {
                    var guideConfig = evt.datas[0] as TableGuide;
                    if (guideConfig.triggerPosition == (int) position)
                    {
                        task.SetResult(true);
                        EventDispatcher.Instance.RemoveEventListener(EventEnum.GuideFinish,EventCallback);
                    }
                };
                EventDispatcher.Instance.AddEventListener(EventEnum.GuideFinish,EventCallback);
                await task.Task;
                await CheckHappyGuide2();
            }
        }
    }
    public async Task CheckHappyGuide2()
    {
        var position = GuideTriggerPosition.StarrySkyCompassHappy2;
        if (!GuideSubSystem.Instance.isFinished(position))
        {
            if (GuideSubSystem.Instance.Trigger(position, null))
            {
                var task = new TaskCompletionSource<bool>();
                Action<BaseEvent> EventCallback = null;
                EventCallback = (evt) =>
                {
                    var guideConfig = evt.datas[0] as TableGuide;
                    if (guideConfig.triggerPosition == (int) position)
                    {
                        task.SetResult(true);
                        EventDispatcher.Instance.RemoveEventListener(EventEnum.GuideFinish,EventCallback);
                    }
                };
                EventDispatcher.Instance.AddEventListener(EventEnum.GuideFinish,EventCallback);
                await task.Task;
            }
        }
    }
}