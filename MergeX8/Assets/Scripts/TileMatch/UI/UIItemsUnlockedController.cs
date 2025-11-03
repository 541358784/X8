using System;
using System.Collections.Generic;
using DragonPlus;
using Gameplay;
using UnityEngine.UI;

public class UIItemsUnlockedController : UIWindowController
{
    private Action _action;
    private Image _icon;
    private LocalizeTextMeshProUGUI _text;
    private UserData.ResourceId _resourceId;

    private Dictionary<UserData.ResourceId, string> _localKey = new Dictionary<UserData.ResourceId, string>()
    {
        {UserData.ResourceId.Prop_Back, "UI_guide_level_bonus_1_tips"},
        {UserData.ResourceId.Prop_SuperBack, "UI_guide_level_bonus_3_tips"},
        {UserData.ResourceId.Prop_Magic, "UI_guide_level_bonus_4_tips"},
        {UserData.ResourceId.Prop_Shuffle, "UI_guide_level_bonus_2_tips"},
        {UserData.ResourceId.Prop_Extend, "UI_guide_level_bonus_5_tips"},
    };
    
    
    public override void PrivateAwake()
    {
        _icon = transform.Find("Root/Icon").GetComponent<Image>();
        _text = transform.Find("Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        transform.Find("Root/ContinueButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            AnimCloseWindow(_action);
        });
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);

        _resourceId = (UserData.ResourceId)objs[0];
        _action = (Action)objs[1];

        _icon.sprite = UserData.GetResourceIcon(_resourceId);
        _text.SetTerm(_localKey[_resourceId]);
    }
}