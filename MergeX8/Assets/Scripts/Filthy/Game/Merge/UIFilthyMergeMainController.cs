using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.Filthy;
using DragonU3DSDK.Network.API.Protocol;
using Filthy.Event;
using Filthy.Model;
using Gameplay;
using Stimulate.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Filthy.Game
{
    public class UIFilthyMergeMainController : UIWindowController
    {
        private FilthyMerge _filthyMerge;
        private FilthyNodes _nodeConfig;
        private bool _canExit = true;
        private FilthyMergeBoard _mergeBoard;
        private Image _targetIcon;
        private GameObject _finishObject;
        private LocalizeTextMeshProUGUI _numText;
        private Action action;
        private int _mergeId;
        
        private int _mergeCount = 0;
        public FilthyMergeBoard mergeBoard
        {
            get { return _mergeBoard; }
        }
        
        public static UIFilthyMergeMainController Instance;
        
        public override void PrivateAwake()
        {
            Instance = this;
            
            transform.Find("Root/ButtonClose").GetComponent<Button>().onClick.AddListener(OnExitButton);
            _numText = transform.Find("Root/Num").GetComponent<LocalizeTextMeshProUGUI>();
            
            _targetIcon = transform.Find("Root/targetIcon").GetComponent<Image>();
            
            _finishObject = transform.Find("Root/Finish").gameObject;
            _finishObject.gameObject.SetActive(false);
            _finishObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                if(_nodeConfig != null)
                    FilthyGameLogic.Instance.ExitGame(_nodeConfig);
                
                action?.Invoke();
            });
            
            EventDispatcher.Instance.AddEventListener(ConstEvent.Event_Refresh_Merge, Event_Refresh_Merge);
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);

            _nodeConfig = (FilthyNodes)objs[0];

            if (objs.Length > 1)
            {
                _mergeId = (int)objs[1];
                action = (Action)objs[2];
            }
            else
            {
                _mergeId = _nodeConfig.Param;
            }
            
            MergeManager.Instance.Refresh(MergeBoardEnum.Filthy);
            InitMergeBoard();
            
            _mergeBoard = transform.Find("Root").GetComponentDefault<FilthyMergeBoard>("Board");
            
            FilthyGuideLogic.Instance.GuideLogic(false);
            
            _numText.SetText("0/1");
        }

        private void InitMergeBoard()
        {
            var storageMergeBoard = MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Filthy);
            
            storageMergeBoard.Items.ForEach(a => a.Id = -1);

            _filthyMerge = FilthyConfigManager.Instance.FilthyMergeList.Find(a => a.Id == _mergeId);
            if(_filthyMerge == null)
                return;
            
            _targetIcon.sprite = UserData.GetResourceIcon(_filthyMerge.TargeMergeId);
            
            var items = FilthyConfigManager.Instance.FilthyBoardList.FindAll(a=>a.BoardId == _filthyMerge.BoardId);
            for (var i = 0; i < items.Count; i++)
            {
                if(i >= storageMergeBoard.Items.Count)
                    break;
                
                if(items[i].ItemId == 0)
                    continue;
                
                storageMergeBoard.Items[i].Id = items[i].ItemId;
            }
        }
        private void OnExitButton()
        {
            if(!_canExit)
                return;
            
            CommonUtils.OpenCommon1ConfirmWindow(new NoticeUIData
            {
                DescString = LocalizationManager.Instance.GetLocalizedString("ui_minigame_11"),
                OKCallback = () =>
                {
                    //引导特殊处理
                    GuideSubSystem.Instance.ForceFinished(4401);
                    GuideSubSystem.Instance.ForceFinished(4402);
                    GuideSubSystem.Instance.CloseCurrent();
                    
                    if(_nodeConfig != null)
                        FilthyGameLogic.Instance.ExitGame(_nodeConfig);
                    
                    action?.Invoke();
                },
                HasCloseButton = true,
                HasCancelButton = true,
                IsHighSortingOrder = true,
            });
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(ConstEvent.Event_Refresh_Merge, Event_Refresh_Merge);
        }

        private void Event_Refresh_Merge(BaseEvent e)
        {
            _mergeCount++;
            if(_nodeConfig != null)
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameAsmrChoose, _nodeConfig.Id.ToString(), _filthyMerge.Id.ToString(), _mergeCount.ToString());

            int newMergeId = (int)e.datas[0];
            
            if(newMergeId != _filthyMerge.TargeMergeId)
                return;
            
            _numText.SetText("1/1");
            
            if(_nodeConfig != null)
                FilthyModel.Instance.OwnedNode(_nodeConfig.LevelId, _nodeConfig.Id);
            
            _finishObject.gameObject.SetActive(true);
            
            FilthyGameLogic.Instance.PlaySound("sfx_miserable_congratulate");
        }
    }
}