/*
 * @file FpsButtonController
 * Debug - 入口
 * @author lu
 */

using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class FpsButtonController : MonoBehaviour
{
    Button RootButton { get; set; }
    public string CloseFPSSaveKey = "CloseFPSSaveKey";

    // Use this for initialization
    void Start()
    {
        RootButton = transform.GetComponent<Button>();
        RootButton.onClick.AddListener(OnClickRootButton);

        //ClientMgr.Instance.FpsButtonController = this;
        if (PlayerPrefs.HasKey(CloseFPSSaveKey) && PlayerPrefs.GetInt(CloseFPSSaveKey) == 0)
            DebugModel.Instance.CloseFPS = true;
        transform.gameObject.SetActive(Debug.isDebugBuild && !DebugModel.Instance.CloseDebug);
    }

    void OnClickRootButton()
    {
        DebugModel.Instance.CloseFPS = !DebugModel.Instance.CloseFPS;
        PlayerPrefs.SetInt(CloseFPSSaveKey, DebugModel.Instance.CloseFPS ? 0 : 1);
    }
}