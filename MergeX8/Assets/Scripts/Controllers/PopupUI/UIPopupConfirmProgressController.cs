using System;
using DragonPlus;
using DragonU3DSDK;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupConfirmProgressController : UIWindowController
{
    GameObject _serverRoot;
    GameObject _localRoot;

    Button _buttonServer;
    Button _buttonLocal;
    Button _buttonClose;

    LocalizeTextMeshProUGUI _serverDiamondText;
    LocalizeTextMeshProUGUI _serverLevelText;
    LocalizeTextMeshProUGUI _serverLevelText2;
    LocalizeTextMeshProUGUI _serverTimeText;

    LocalizeTextMeshProUGUI _localDiamondText;
    LocalizeTextMeshProUGUI _localLevelText;
    LocalizeTextMeshProUGUI _localTimeText;
    LocalizeTextMeshProUGUI _localLevelText2;

    ChooseProgressData _serverData;
    ChooseProgressData _localData;

    DragonU3DSDK.Network.API.Protocol.Profile _serverProfile;
    bool _isPickedServerData;

    public override void PrivateAwake()
    {
        _serverRoot = GetItem("ChooseProgress/Progress1");
        _localRoot = GetItem("ChooseProgress/Progress2");

        _buttonClose = GetItem<Button>("ChooseProgress/BgPopupBoandBig/ButtonClose");
        _buttonClose.onClick.AddListener(OnCloseClick);

        _buttonLocal = GetItem<Button>("ChooseProgress/Progress2/Button");
        _buttonLocal.onClick.AddListener(OnLocalClick);
        _localDiamondText = GetItem<LocalizeTextMeshProUGUI>("ChooseProgress/Progress2/Diamonds/Textcontent/Text");
        _localLevelText = GetItem<LocalizeTextMeshProUGUI>("ChooseProgress/Progress2/Level");
        _localTimeText = GetItem<LocalizeTextMeshProUGUI>("ChooseProgress/Progress2/Time");
        _localLevelText2 = GetItem<LocalizeTextMeshProUGUI>("ChooseProgress/Progress2/Lv/Textcontent/Text");

        _buttonServer = GetItem<Button>("ChooseProgress/Progress1/Button");
        _buttonServer.onClick.AddListener(OnServerClick);
        _serverDiamondText = GetItem<LocalizeTextMeshProUGUI>("ChooseProgress/Progress1/Diamonds/Textcontent/Text");
        _serverLevelText = GetItem<LocalizeTextMeshProUGUI>("ChooseProgress/Progress1/Level");
        _serverTimeText = GetItem<LocalizeTextMeshProUGUI>("ChooseProgress/Progress1/Time");
        _serverLevelText2 = GetItem<LocalizeTextMeshProUGUI>("ChooseProgress/Progress1/Lv/Textcontent/Text");
    }

    public void SetData(DragonU3DSDK.Network.API.Protocol.Profile profile, bool isPickedServerData)
    {
        _isPickedServerData = isPickedServerData;
        try
        {
            _serverProfile = profile;
            var localJson = DragonU3DSDK.Storage.StorageManager.Instance.ToJson();
            var serverJson = profile.Json;
            _localData = new ChooseProgressData(localJson);
            _serverData = new ChooseProgressData(serverJson);
            ReloadUi();
        }
        catch (Exception e)
        {
            DebugUtil.LogError(e);
        }
    }

    void ReloadUi()
    {
        _serverRoot.SetActive(_isPickedServerData);
        _localRoot.SetActive(_isPickedServerData == false);
        string levelStr = LocalizationManager.Instance.GetLocalizedString("UI_common_text_LevelLabel");
        _localDiamondText.SetText(_localData.DiamondNum.ToString());
        _localLevelText.SetText(levelStr + (_localData.Level) + "  " +
                                LocalizationManager.Instance.GetLocalizedString("&key.UI_choose_progress_on_device"));
        _localTimeText.SetText(Utils.GetTimeStampDateString(_localData.Time));
        _localLevelText2.SetText(_localData.Level.ToString());

        _serverDiamondText.SetText(_serverData.DiamondNum.ToString());
        _serverLevelText.SetText(levelStr + (_serverData.Level) + "  " +
                                 LocalizationManager.Instance.GetLocalizedString("&key.UI_choose_progress_on_server"));
        _serverTimeText.SetText(Utils.GetTimeStampDateString(_serverData.Time));
        _serverLevelText2.SetText(_serverData.Level.ToString());
    }

    private void OnLocalClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventChooseProfileSelect, "client");
        DragonU3DSDK.Storage.StorageManager.Instance.ResolveProfileConfict(_serverProfile, false);
        CloseWindowWithinUIMgr(true);
    }

    private void OnServerClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventChooseProfileSelect, "server");
        DragonU3DSDK.Storage.StorageManager.Instance.ResolveProfileConfict(_serverProfile, true);
        CloseWindowWithinUIMgr(true);
    }

    private void OnCloseClick()
    {
        CloseWindowWithinUIMgr(true);
        UIManager.Instance.OpenUI(UINameConst.UIPopupChooseProgress)
            .GetComponent<UIPopupChooseProgressController>().SetData(_serverProfile);
    }
}