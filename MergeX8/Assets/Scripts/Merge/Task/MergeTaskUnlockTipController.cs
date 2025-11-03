using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public class MergeTaskUnlockTipController:MonoBehaviour
{
    private LocalizeTextMeshProUGUI LevelText;
    private List<int> UnlockLevelList = new List<int>() {4, 5, 10};
    private int CurLevel => StorageManager.Instance.GetStorage<StorageHome>().Level;
    private bool IsEnable => CurLevel >= 3 && CurLevel < UnlockLevelList.Last();
    private void Awake()
    {
        LevelText = transform.Find("StarSlider/Text").GetComponent<LocalizeTextMeshProUGUI>();
    }

    public void Refresh()
    {
        gameObject.SetActive(IsEnable);
        if (gameObject.activeSelf)
        {
            for (var i = 0; i < UnlockLevelList.Count; i++)
            {
                if (CurLevel < UnlockLevelList[i])
                {
                    LevelText.SetText(UnlockLevelList[i].ToString());
                    break;
                }
            }
        }
    }
}