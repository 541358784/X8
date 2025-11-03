using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldButtonManager : Manager<ShieldButtonManager>
{
    private const float shieldingTime = 1f;

    private class ButParam
    {
        public Button button;
        public float clickTime;
    }

    private Dictionary<Button, ButParam> butParams = new Dictionary<Button, ButParam>();
    private List<Button> clearButtons = new List<Button>();

    public void RegisteredButton(Button button, bool isRecover = true)
    {
        if (button == null)
            return;


        button.interactable = false;
        if (!isRecover)
            return;

        ButParam butParam = null;
        if (butParams.ContainsKey(button))
        {
            butParam = butParams[button];
        }
        else
        {
            butParam = new ButParam();
            butParams.Add(button, butParam);
        }

        butParam.clickTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        if (butParams == null || butParams.Count == 0)
            return;

        float curTime = Time.realtimeSinceStartup;
        foreach (var kv in butParams)
        {
            if (curTime - kv.Value.clickTime < shieldingTime)
                continue;

            kv.Value.button.interactable = true;
            clearButtons.Add(kv.Key);
        }

        clearButtons.ForEach(a => butParams.Remove(a));
        clearButtons.Clear();
    }

    public void Destroy()
    {
        butParams.Clear();
        clearButtons.Clear();
    }
}