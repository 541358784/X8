using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;
public class UIPopupOneOutOfFiveController: UIWindowController
{
    private Image _boxIcon;
    private Button _closeBtn;
    private Button _claimBtn;
    private List<OneselfChoseItem> _items;
    private TableMergeItem _selectItem;
    private TableMergeItem tableMergeItem;
    private int boxIndex;
    private LocalizeTextMeshProUGUI _titleText;
    private LocalizeTextMeshProUGUI _choiceText;
    public override void PrivateAwake()
    {
        _boxIcon = GetItem<Image>("Root/BoxGroup/Image3");
        _closeBtn = GetItem<Button>("Root/CloseButton");
        _claimBtn = GetItem<Button>("Root/ItemGroup/Button");
        _titleText = GetItem<LocalizeTextMeshProUGUI>("Root/TitleGroup/Text");
        _choiceText = GetItem<LocalizeTextMeshProUGUI>("Root/ItemGroup/Text");
        _claimBtn.gameObject.SetActive(false);
        _claimBtn.onClick.AddListener(OnClaim);
        _closeBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        _items = new List<OneselfChoseItem>();
        for (int i = 1; i < 6; i++)
        {
          var item=  transform.Find("Root/ItemGroup/Item" + i).gameObject.AddComponent<OneselfChoseItem>();
          _items.Add(item);
        }
    }

    private void OnClaim()
    {
        if (_selectItem == null)
            return;
        MergeManager.Instance.RemoveBoardItem(boxIndex,MergeBoardEnum.Main,"OneThree");
        AnimCloseWindow(() =>
        {
            AddRewardItem();
        });
    }

    public void AddRewardItem()
    {
        if (_selectItem == null)
            return;
        var mergeItem = MergeManager.Instance.GetEmptyItem();
        mergeItem.Id = _selectItem.id;
        mergeItem.State = 1;
        MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.Main);
        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonChoiseChestGet,
            itemAId = _selectItem.id,
            ItemALevel = _selectItem.level,
            itemBId = tableMergeItem.id,
            itemBLevel = tableMergeItem.level,
            isChange = true,
           
        });
        Vector3 endPos = Vector3.zero;
        if (MergeMainController.Instance != null && MergeMainController.Instance.gameObject.activeSelf)
        {
            endPos = MergeMainController.Instance.rewardBtnPos;
        }
        else
        {
            endPos = UIHomeMainController.mainController.MainPlayTransform.position;
        }

        FlyGameObjectManager.Instance.FlyObject(mergeItem.Id, transform.position, endPos, 1f, 2.0f, 1f, () =>
        {
            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
        });
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (objs != null && objs.Length > 0)
        {
            int mergeID = (int)objs[0];
            boxIndex =(int)objs[1];
            var choiceChest= GameConfigManager.Instance.GetChoiceChest(mergeID);
            tableMergeItem = GameConfigManager.Instance.GetItemConfig(mergeID);
            for (int i = 0; i < choiceChest.item.Length; i++)
            {
                var mergeItem= GameConfigManager.Instance.GetItemConfig(choiceChest.item[i]);
                _items[i].Init(mergeItem, SelectItem, i);
            }
            _boxIcon.sprite= MergeConfigManager.Instance.mergeIcon.GetSprite(tableMergeItem.image);
            _titleText.SetTerm(tableMergeItem.name_key);
        }
    }

    public void SelectItem(int index,TableMergeItem mergeItem)
    {
        _selectItem = mergeItem;
        for (int i = 0; i < _items.Count; i++)
        {
            if (i == index)
            {
                _items[i].SetStatus(OneselfChoseItem.ItemStatus.Select);
            }
            else
            {
                _items[i].SetStatus(OneselfChoseItem.ItemStatus.UnSelect);
            }
        }
        _claimBtn.gameObject.SetActive(true);
        _choiceText.gameObject.SetActive(false);
        
    }
}
