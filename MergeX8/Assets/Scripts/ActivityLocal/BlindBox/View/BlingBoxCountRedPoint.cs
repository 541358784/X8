using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public class BlingBoxRedPoint : MonoBehaviour
{
    private LocalizeTextMeshProUGUI Text;
    private bool IsAwake = false;
    private bool UseBox = false;
    private bool UseGroup = false;
    private void Awake()
    {
        IsAwake = true;
        Text = transform.Find("Label").GetComponent<LocalizeTextMeshProUGUI>();
        Text.gameObject.SetActive(false);
    }

    public void Init(bool useBox,bool useGroup)
    {
        if (!IsAwake)
            Awake();
        UseBox = useBox;
        UseGroup = useGroup;
        UpdateView();
        InvokeRepeating("UpdateView",0,1);
    }

    public void UpdateView()
    {
        var show = false;
        if (UseBox)
        {
            var count = BlindBoxModel.Instance.GetBlindBoxCountAll();
            if (count > 0)
                show = true;
        }

        if (UseGroup)
        {
            var groupGCount = BlindBoxModel.Instance.GetCanCollectGroupCountAll();
            if (groupGCount > 0)
                show = true;
        }
        gameObject.SetActive(show);
    }
}
public class BlingBoxThemeRedPoint : MonoBehaviour
{
    private StorageBlindBox Storage;
    private LocalizeTextMeshProUGUI Text;
    private bool IsAwake = false;
    private bool UseBox = false;
    private bool UseGroup = false;
    private bool UseText = false;
    private void Awake()
    {
        IsAwake = true;
        Text = transform.Find("Label").GetComponent<LocalizeTextMeshProUGUI>();
        Text.gameObject.SetActive(UseText);
    }

    public void Init(StorageBlindBox storage,bool useBox,bool useGroup,bool useText = false)
    {
        Storage = storage;
        if (!IsAwake)
            Awake();
        UseBox = useBox;
        UseGroup = useGroup;
        UseText = useText;
        Text.gameObject.SetActive(UseText);
        UpdateView();
        InvokeRepeating("UpdateView",0,1);
    }

    public void UpdateView()
    {
        var num = 0;
        var show = false;
        if (UseBox)
        {
            var count = Storage.BlindBoxCount;
            if (count > 0)
                show = true;
            num += count;
        }

        if (UseGroup)
        {
            var groupGCount = BlindBoxModel.Instance.GetCanCollectGroupCount(Storage);
            if (groupGCount > 0)
                show = true;
            num += groupGCount;
        }
        gameObject.SetActive(show);
        if (UseText)
        {
            Text.SetText(num.ToString());
        }
    }
}