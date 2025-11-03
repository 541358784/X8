using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;
using DragonU3DSDK.Storage;
using Framework.UI;
using Framework.Utils;
using fsm_new;
using MiniGame;
using Newtonsoft.Json;
using Scripts.UI;

namespace ASMR
{
    public class UIAsmr : UIView
    {
        public class Data : UIData
        {
            public int LevelId;
        }

        private Data _data;

        private Transform _content;

        private Slider _stepProgressBar;
        private Slider _levelProgressBar;

        private GameObject _stepItemPrefab;

        private Button _shockButton;
        private Button _closeButton;

        private StorageASMRLevel _storage;
        private List<UIAsmrGroupCell> _groupCells = new();


        public static void Open(int levelId)
        {
            Framework.UI.UIManager.Instance.Open<UIAsmr>("NewMiniGame/UIMiniGame/Prefab/UIAsmr", new Data() { LevelId = levelId });
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _content = BindItem("Root/AsmrGroup/SliderSteps");
            _levelProgressBar = BindItem<Slider>("Root/AsmrGroup/SliderSteps");
            _stepProgressBar = BindItem<Slider>("Root/AsmrGroup/SliderSpeed");
            _stepItemPrefab = BindItem("Root/AsmrGroup/SliderSteps/StepItem01").gameObject;

            _shockButton = BindButtonEvent("Root/AsmrGroup/ButtonShock", OnBtnVibrateClick);
            _closeButton = BindButtonEvent("Root/AsmrGroup/ButtonQuit", OnClickClose);

            _stepProgressBar.gameObject.SetActive(false);
        }

        protected internal override void OnOpen<T>(UIData data)
        {
            base.OnOpen<T>(data);

            RefreshShockButton();
            _data = data as Data;

            _storage = ASMRModel.Instance.GetLevelStorageByLevelId(_data.LevelId);

            for (int i = 2; i < _content.childCount; i++)
            {
                _content.GetChild(i).gameObject.SetActive(false);
            }

            _closeButton.gameObject.SetActive(ASMRModel.Instance.AttData.chapterId > 1);
            
            var levelConfig = ASMRModel.Instance.AsmrLevelConfigs.Find(c => c.Id == _data.LevelId);
            if (levelConfig != null)
            {
                while (_content.childCount - 2 < levelConfig.GroupIds.Count) Object.Instantiate(_stepItemPrefab, _content, false);

                for (var index = 0; index < levelConfig.GroupIds.Count; index++)
                {
                    var obj = _content.GetChild(index + 2).gameObject;
                    obj.SetActive(true);

                    var groupId = levelConfig.GroupIds[index];
                    var groupConfig = ASMRModel.Instance.AsmrGroupConfigs.Find(c => c.Id == groupId);

                    var levelCellScript = BindElement<UIAsmrGroupCell>(obj);

                    levelCellScript.InitUI(0, groupConfig.StepIds.Count, groupConfig.Icon);
                    _groupCells.Add(levelCellScript);
                }
            }

            _groupCells[0].SetState(UIAsmrGroupCell.State.Actvie);
            _levelProgressBar.value = 0;

             EventBus.Register<EventAsmrGroupChange>(OnEventAsmrGroupChange);
             EventBus.Register<EventAsmrStepChange>(OnEventAsmrStepChange);
        }

        protected override void OnClose()
        {
            base.OnClose();
             EventBus.UnRegister<EventAsmrGroupChange>(OnEventAsmrGroupChange);
             EventBus.UnRegister<EventAsmrStepChange>(OnEventAsmrStepChange);
        }

        private void OnBtnVibrateClick()
        {
            SettingManager.Instance.ShakeClose = !SettingManager.Instance.ShakeClose;
            RefreshShockButton();
        }

        private void OnFastForwardButtonClick()
        {
            //先注释
            // var data = new UIPopupCommonTips.Data();
            // data.contentMsgKey = "UI_tips_skipover";
            // data.titleMsgKey = "UI_tips_Tittle";
            // data.closeBtnEnable = false;
            // data.noBtnEnable = true;
            // data.yesBtnEnable = true;
            // data.okBtnEnable = false;
            // data.yesAction = ASMRModel.Instance.FastFinishCurrentLevel;
            // UIPopupCommonTips.Open(data);
        }

        private void OnEventAsmrStepChange(EventAsmrStepChange e)
        {
            _groupCells[e.groupIndex].SetProgress(e.stepCount, e.total);
            _storage.MaxComplete = (int)Mathf.Max(_storage.MaxComplete, (e.groupIndex / (float)e.groupCount) * 100);
            _storage.MaxComplete = Mathf.Min(100, _storage.MaxComplete);
        }


        private void OnClickClose()
        {
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                DescString = LocalizationManager.Instance.GetLocalizedString("UI_quit_game_tips_text"),
                OKCallback = () =>
                {
                    BackToHome();
                },
                HasCancelButton = false,
                HasCloseButton = true,
            });
        }

        private void RefreshShockButton()
        {
            var offImage = _shockButton.transform.Find("ImgOff");
            var onImage = _shockButton.transform.Find("ImgOn");

            var isOn = SettingManager.Instance.ShakeClose;
            offImage.gameObject.SetActive(!isOn);
            onImage.gameObject.SetActive(isOn);
        }

        private void OnEventAsmrGroupChange(EventAsmrGroupChange e)
        {
            _groupCells[e.current - 1].SetState(UIAsmrGroupCell.State.Finished);

            if (e.current < _groupCells.Count)
            {
                _groupCells[e.current].SetState(UIAsmrGroupCell.State.Actvie);
            }

            _levelProgressBar.value = e.current / (float)(e.total - 1);
        }

        public void ShowProgressBar(bool show)
        {
            _stepProgressBar.gameObject.SetActive(show);
        }

        public void OnProgressChanged(float value)
        {
            _stepProgressBar.value = value;
        }

        private void BackToHome()
        {
            Close();
            ASMRModel.Instance.QuitGame();
            UIChapter.Open(ASMRModel.Instance.AttData.chapterId);
        }
    }
}