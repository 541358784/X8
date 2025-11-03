using System;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : Manager<TickManager>
{
    List<Action<float>> ticks = new List<Action<float>>();
    List<Action<float>> seconds = new List<Action<float>>();
    private float timeDelta = 0.0f;

    public void AddTick(Action<float> action)
    {
        if (action == null)
            return;
        ticks.Add(action);
    }

    public void RemoveTick(Action<float> action)
    {
        if (action == null)
            return;
        ticks.Remove(action);
    }

    public void AddSecond(Action<float> action)
    {
        if (action == null)
            return;
        seconds.Add(action);
    }

    public void RemoveSecond(Action<float> action)
    {
        if (action == null)
            return;
        seconds.Remove(action);
    }

    void Update()
    {
        var deltaTime = Time.deltaTime;
        for (var i = 0; i < ticks.Count; i++)
            ticks[i]?.Invoke(deltaTime);

        timeDelta += deltaTime;
        if (timeDelta >= 1.0f)
        {
            timeDelta -= 1.0f;
            for (var i = 0; i < seconds.Count; i++)
                seconds[i]?.Invoke(1.0f);
        }
    }
}