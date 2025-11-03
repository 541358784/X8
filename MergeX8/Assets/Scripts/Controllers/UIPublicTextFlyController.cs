using System.Collections;
using DragonPlus;
using Framework;
using Framework.Wrapper;
using UnityEngine;
using UnityEngine.UI;

public class UIPublicTextFlyController : UIWindowController
{
    private GameObject _goTemplate;
    private Animator _anim;
    private Coroutine _coLateClose;
    private const float _lateCloseTime = 5f;
    private const int _maxMsgCount = 10;
    private int _curCount;

    public class FlyText : UIGameObjectWrapper
    {
        private Text _text;
        private Animator _anim;

        public FlyText(GameObject go) : base(go)
        {
            _text = GetItem<Text>("TxtType");
            _anim = GetItem<Animator>(go);
        }

        public void Fly(string text)
        {
            _text.text = LocalizationManager.Instance.GetLocalizedString(text);
            _anim.ApplyBuiltinRootMotion();
        }
    }

    static public void Popup(string msg)
    {
    }

    public override void PrivateAwake()
    {
        _goTemplate = GetItem("Animator");
        _goTemplate.SetActive(false);
        _curCount = 0;
    }

    private void _OnPopup(string msg)
    {
        if (_curCount >= _maxMsgCount)
        {
            return;
        }

        _curCount++;

        var goText = GameObjectFactory.Clone(_goTemplate);
        if (goText)
        {
            var flyText = new FlyText(goText);
            flyText.SetActive(true);
            flyText.Fly(msg);
        }

        if (_coLateClose != null)
        {
            StopCoroutine(_coLateClose);
        }

        _coLateClose = StartCoroutine(_LateClose());
    }

    private IEnumerator _LateClose()
    {
        yield return new WaitForSeconds(_lateCloseTime);
        CloseWindowWithinUIMgr(true);
    }
}