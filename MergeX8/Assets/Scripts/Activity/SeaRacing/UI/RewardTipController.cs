
using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.UI;
using Framework;
using Gameplay;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SeaRacingTipController : MonoBehaviour
{
    public static SeaRacingTipController ShowTip(Vector3 pos,List<ResData> resDatas,Transform parent,int overrideSortingOrder = 999,bool autoClose = true)
    {
        var obj = DragonU3DSDK.Asset.ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/SeaRacing/SeaRacingTip");
        var popItem= Instantiate(obj, parent).AddComponent<SeaRacingTipController>();
        if (overrideSortingOrder > 0)
        {
            var canvas = popItem.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = overrideSortingOrder;
        }
        popItem.Init(resDatas,pos);
        popItem.gameObject.SetActive(true);
        if (autoClose)
            popItem.StartAutoClosePopup();
        return popItem;
    }
    private Transform _rewardItem;
    private List<Transform> _itemList;
    public  void Awake()
    {
        
        _rewardItem = transform.Find("Icon");
        _rewardItem.gameObject.SetActive(false);
        _itemList = new List<Transform>();
    }
    private Coroutine con;
    public void StartAutoClosePopup()
    {
        con=StartCoroutine(AutoClosePopup());
    }
    private IEnumerator AutoClosePopup()
    {
        yield return new WaitForSeconds(3f); // 等待3秒钟

        HidePopup(); // 3秒后关闭提示框
    }
    public void HidePopup()
    {
        if (con!=null)
        {
            StopCoroutine(con);
            con = null;
        }
        Destroy(gameObject);
        // _popItem.gameObject.SetActive(false);
    }
    void Update()
    {
        // 检测点击任意位置关闭
        if (Input.GetMouseButtonDown(0))
        {
            
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            if (results.Count == 0)
                return ;
            foreach (var result in results)
            {
                if (result.gameObject.transform.parent.gameObject == gameObject)
                    return ;
            }
            HidePopup();
        }
    }
    public void Init(List<ResData> resDatas,Vector3 popupPosition)
    {
        for (int i = 0; i < resDatas.Count; i++)
        {
           var item= Instantiate(_rewardItem, _rewardItem.parent);
           item.gameObject.SetActive(true);
           _itemList.Add(item);
           InitItem(item,resDatas[i].id,resDatas[i].count);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        transform.position = (Vector2) popupPosition;
    }


    private void InitItem(Transform item, int itemID, int ItemCount)
    {
        item.GetComponent<Image>().sprite = UserData.GetResourceIcon(itemID);
        string tex = ItemCount.ToString();
        if (itemID == (int) UserData.ResourceId.Infinity_Energy)
        {
            tex = TimeUtils.GetTimeString(ItemCount, true);
        }

        item.Find("Num").GetComponent<LocalizeTextMeshProUGUI>().SetText(tex);
        item.gameObject.SetActive(true);
    }
}
