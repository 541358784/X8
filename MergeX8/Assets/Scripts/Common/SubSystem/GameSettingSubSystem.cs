using System;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Config;
using Framework;
using Framework.Wrapper;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIDebugPanelController : UIWindowController
{
    public class DebugButton : UIGameObjectWrapper
    {
        private Button _btn;
        private Text _txt;

        public DebugButton(GameObject go, string txtKey) : base(go)
        {
            _btn = GetItem<Button>(go);
            _txt = GetItem<Text>(txtKey);
        }

        public void SetOnButtonClick(UnityAction call)
        {
            _btn.onClick.RemoveAllListeners();
            _btn.onClick.AddListener(call);
        }

        public void SetText(string text)
        {
            _txt.text = text;
        }
    }

    private GameObject _goTemplLevel1;
    private GameObject _goTemplLevel2;
    private GameObject _goLevel2;
    private Text _txtConsole;
    private InputField _inputParam1;
    private InputField _inputParam2;

    private List<string> _types = new List<string>();
    private List<DebugButton> _level2Buttons = new List<DebugButton>();
    private List<DebugButton> _level1Buttons = new List<DebugButton>();

    private uint cmdCount = 0;

    public override void PrivateAwake()
    {
        _goTemplLevel1 = GetItem("ButtonList/Scroll View1/Viewport/Content/Button");
        _goTemplLevel2 = GetItem("ButtonList/Scroll View2/Viewport/Content/Button");
        _goLevel2 = GetItem("ButtonList/Scroll View2");
        _goLevel2.SetActive(false);
        _txtConsole = GetItem<Text>("Console/Text");
        _txtConsole.text = "";
        _inputParam1 = GetItem<InputField>("ButtonList/Scroll View2/InputField1");
        _inputParam2 = GetItem<InputField>("ButtonList/Scroll View2/InputField2");
        BindEvent("ButtonClose", gameObject, delegate(GameObject o) { CloseWindowWithinUIMgr(true); });
    }

    private void Start()
    {
        _LoadConfig();
        _ReBuildButtons();
    }

    private void _LoadConfig()
    {
    }


    private void _ReBuildButtons()
    {
        int i = 0;
        foreach (var name in _types)
        {
            DebugButton button;
            if (i < _level1Buttons.Count)
            {
                button = _level1Buttons[i];
            }
            else
            {
                button = new DebugButton(GameObjectFactory.Clone(_goTemplLevel1), "Text");
                button.SetActive(true);
                _level1Buttons.Add(button);
            }

            button.SetText(name);
            button.SetOnButtonClick(delegate { });

            i++;
        }

        _RemoveElementFrom(_level1Buttons, i);
    }


    private void _RemoveElementFrom(List<DebugButton> list, int index)
    {
        for (int i = index; i < list.Count; i++)
        {
            var button = list[i];
            if (button != null)
            {
                GameObjectFactory.Destroy(button.GameObject);
            }
        }

        if (index < list.Count)
        {
            list.RemoveRange(index, list.Count - index);
        }
    }
}