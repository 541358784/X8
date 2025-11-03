using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldButtonOnClick : MonoBehaviour
{
    private Button button;
    public float shieldButTime = 1f;
    public bool isUse = true;

    private bool isOnClick = false;

    private float curTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        button = transform.GetComponent<Button>();
        if (button == null)
            return;

        button.onClick.AddListener(() =>
        {
            if (!isUse)
                return;
            isOnClick = true;
            curTime = Time.realtimeSinceStartup;
            button.interactable = false;
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (!isUse)
            return;
        if (!isOnClick)
            return;

        if (Time.realtimeSinceStartup - curTime < shieldButTime)
            return;

        button.interactable = true;
        isOnClick = false;
    }
}