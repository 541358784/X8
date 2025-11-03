using System.Collections;
using Game;
using UnityEngine.UI;

public class Aux_Mail : Aux_ItemBase
{
    private IEnumerator iEnumerator = null;
    private Image redPoint;

    protected override void Awake()
    {
        base.Awake();

        redPoint = transform.Find("RedPoint").GetComponent<Image>();
        InvokeRepeating("UpdateUI", 0, 1);
        EventDispatcher.Instance.AddEventListener(EventEnum.GLOBAL_MAIL_UPDATED, OnMailUpdate);
    }


    public override void UpdateUI()
    {
        // gameObject.SetActive(MailDataModel.Instance.HasNoReadMails());
        redPoint.gameObject.SetActive(MailDataModel.Instance.HasNoReadMails());
        // if (gameObject.activeSelf)
        // {
        //     StopDelayWork();
        //     iEnumerator = CommonUtils.DelayWork(1, () => { UpdateUI(); });
        //     StartCoroutine(iEnumerator);
        // }
    }

    private void StopDelayWork()
    {
        if (iEnumerator == null)
            return;

        StopCoroutine(iEnumerator);
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();

        UIPopupMailListController.Open();

    }

    private void OnDestroy()
    {
        StopDelayWork();
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GLOBAL_MAIL_UPDATED, OnMailUpdate);
    }

    private void OnMailUpdate(BaseEvent obj)
    {
        UpdateUI();
    }
}