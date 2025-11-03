using System;
using Deco.Node;
using Decoration;
using DragonPlus;
using DragonU3DSDK.Asset;
using Gameplay;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupTaskController
{
    private Transform _wishingTimeGroup;
    private LocalizeTextMeshProUGUI _wishingTimeText;
    private Button _wishingButton;
    private LocalizeTextMeshProUGUI _wishingCostText;

    public const int DecoNodeId = 999999;
    public const string CoolTimeKey = "WishingCoolKey";
    private DecoNode _decoNode;

    private Image _buttonBg;
    private void Awake_Wishing()
    {
        _wishingTimeGroup = transform.Find("Root/Task2/Scroll View/Viewport/Content/UITaskCell/TimeGroup");
        _wishingTimeText = _wishingTimeGroup.Find("TimeText").GetComponent<LocalizeTextMeshProUGUI>();
                                         
        _wishingButton = transform.Find("Root/Task2/Scroll View/Viewport/Content/UITaskCell/TaskGroup/UITeskGroupCell/Button").GetComponent<Button>();
        _wishingButton.onClick.AddListener(OnClickWishing);
        _wishingCostText = _wishingButton.transform.Find("Num").GetComponent<LocalizeTextMeshProUGUI>();
        
        _buttonBg = transform.Find("Root/Task2/Scroll View/Viewport/Content/UITaskCell/TaskGroup/UITeskGroupCell/Button/UIBg").GetComponent<Image>();
        
        InvokeRepeating("InvokeUpdateCoolTime", 0, 1);
    }

    private void Init_Wishing()
    {
        _decoNode = DecoManager.Instance.FindNode(DecoNodeId);
        
        _wishingCostText.SetText(_decoNode.Config.price.ToString());

        UpdateWishingButton();
    }

    private void OnClickWishing()
    {
        if(_decoNode == null || _decoNode.Config == null)
            return;
        
        if(!UserData.Instance.CanAford(_decoNode.Config.costId, _decoNode.Config.price))
            return;
        
        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, CoolTimeKey, CommonUtils.GetTimeStamp());
        UpdateWishingButton();
        
        UserData.Instance.ConsumeRes((UserData.ResourceId)_decoNode.Config.costId, _decoNode.Config.price, new GameBIManager.ItemChangeReasonArgs(){});
        
        
        if (GameModeManager.Instance.GetGameMode() == GameModeManager.GameMode.MiniAndMerge)
        {
            if (GameModeManager.Instance.GetCurrenGameMode() == GameModeManager.CurrentGameMode.MiniGame)
            {
                AudioManager.Instance.PlayMusic(1, true);
                GameModeManager.Instance.SetCurrentGameMode(GameModeManager.CurrentGameMode.Deco);
                UIPopupTaskController.CloseView(() =>
                {
                    if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                    {
                        GameModeManager.Instance.RefreshGameStatus(false);
                        SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome, DecoOperationType.Buy, _decoNode);
                    }
                    else
                    {
                        GameModeManager.Instance.RefreshGameStatus(true, () =>
                        {
                            DecoManager.Instance.SelectNode(_decoNode);
                        });
                    }
                });
                
                return;
            }
        }
        
        UIPopupTaskController.CloseView(() =>
        {
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
            {
                SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome, DecoOperationType.Buy, _decoNode.Id);
            }
            else
            {
                DecoManager.Instance.SelectNode(_decoNode);
            }
        });
        
    }

    private void InvokeUpdateCoolTime()
    {
        UpdateWishingButton();
        
        if(CanWishing())
             return;
        
        var startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var cdTime = CoolingTimeManager.Instance.GetCDTime(CoolingTimeManager.CDType.OtherDay, CoolTimeKey);
        var localMidnight = startTime.AddMilliseconds(cdTime).ToUniversalTime().Date;
        localMidnight = localMidnight.AddDays(1);
        
        var millisecond = (long)(localMidnight - startTime).TotalMilliseconds;
        
        _wishingTimeText.SetText(CommonUtils.FormatLongToTimeStr(millisecond-(long)(DateTime.UtcNow.ToUniversalTime()-startTime).TotalMilliseconds));
    }
    
    private void UpdateWishingButton()
    {
        _wishingButton.interactable = CanWishing() && UserData.Instance.CanAford(_decoNode.Config.costId, _decoNode.Config.price);
        _wishingTimeGroup.gameObject.SetActive(!CanWishing());

        var name = !_wishingButton.interactable ? "Common_Button_Gray" : "Common_Button_Green";
        if(_buttonBg.sprite != null && _buttonBg.sprite.name == name)
            return;
        
        _buttonBg.sprite  = ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, name);
    }
    
    private bool CanWishing()
    {
        return !CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, CoolTimeKey);
    }

    public static bool IsShowWishingRedPoint()
    {
        if (!DecoManager.Instance.IsOwnedNode(DecoNodeId))
            return false;
        
        var decoNode = DecoManager.Instance.FindNode(DecoNodeId);
        if (decoNode == null)
            return false;
        
       return !CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, CoolTimeKey) && UserData.Instance.CanAford(decoNode.Config.costId, decoNode.Config.price);
    }
}