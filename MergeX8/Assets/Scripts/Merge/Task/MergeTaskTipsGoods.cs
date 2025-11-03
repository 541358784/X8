using DragonPlus;
using DragonU3DSDK;
using UnityEngine;
using UnityEngine.UI;

public class MergeTaskTipsGoods : MonoBehaviour
{
    private Image icon;
    private LocalizeTextMeshProUGUI count;
    private GameObject doneGo;
    private Button button;
    private TableMergeItem tableMergeItem;
    private Animator animator;
    private GameObject warningGo;
    private Image _flashTip;

    public int id
    {
        get
        {
            if (tableMergeItem == null)
                return -1;
            return tableMergeItem.id;
        }
    }

    private void Awake()
    {
        icon = transform.GetComponentDefault<Image>("Icon");
        icon.gameObject.SetActive(true);

        button = transform.GetComponentDefault<Button>();
        button.onClick.AddListener(ShowTips);

        count = transform.GetComponentDefault<LocalizeTextMeshProUGUI>("Num");
        count.gameObject.SetActive(true);

        doneGo = transform.Find("Done").gameObject;

        warningGo = transform.Find("Mark").gameObject;
        _flashTip = transform.GetComponent<Image>(  "Shop");

        animator = transform.GetComponent<Animator>();
        SetWarningStatus(false);
        EventDispatcher.Instance.AddEventListener(EventEnum.FLASH_SALE_REFRESH,OnRefresh);

    }

    private void OnRefresh(BaseEvent obj)
    {
        SetShopTipStatus();
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.FLASH_SALE_REFRESH,OnRefresh);
    }

    public void SetShopTipStatus()
    {
        _flashTip.gameObject.SetActive(StoreModel.Instance.IsHaveFlashItem(id)&& !doneGo.activeSelf);
    }
    public void SetImageIcon(TableMergeItem tableMerge)
    {
        Sprite s = MergeConfigManager.Instance.mergeIcon.GetSprite(tableMerge.image);
        if (s == null)
        {
            DebugUtil.LogError("任务配置表所需物品名称出错:" + tableMerge.image);
            return;
        }

        tableMergeItem = tableMerge;
        if (icon == null)
            return;
        icon.gameObject.SetActive(true);
        icon.sprite = s;
        icon.rectTransform.anchoredPosition = new Vector2(0, tableMerge.offsetY);
    }

    private void ShowTips()
    {
        if (tableMergeItem == null)
            return;

        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ClickTaskNeedItem);
        MergeInfoView.Instance.OpenMergeInfo(tableMergeItem);
    }

    public void SetCountText(string txt)
    {
        count.SetText(txt);
    }

    public void SetCountActive(bool isActive)
    {
        count.gameObject.SetActive(isActive);
    }

    public void SetDoneStatus(bool status)
    {
        doneGo.SetActive(status);

        if (status)
        {
            _flashTip.gameObject.SetActive(false);
            SetWarningStatus(false);
        }
        else
        {
            SetShopTipStatus();
        }
        
    }

    public bool IsDone()
    {
        return doneGo.activeSelf;
    }
    
    public void SetWarningStatus(bool status)
    {
        if (warningGo == null)
            return;
        warningGo.SetActive(status);
    }

    public void PlayShake()
    {
        if (animator == null)
            return;

        animator.Play("shake", 0, 0f);
    }
}