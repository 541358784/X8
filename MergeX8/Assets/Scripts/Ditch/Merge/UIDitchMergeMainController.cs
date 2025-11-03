using System;
using DragonPlus;
using DragonPlus.Config.Ditch;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Ditch.Merge
{
    public class UIDitchMergeMainController: UIWindowController
    {
        private TableDitchMerge _mergeConfig;
        private bool _canExit = true;
        private DitchMergeBoard _mergeBoard;
        private Image _targetIcon;
        private GameObject _finishObject;
        private LocalizeTextMeshProUGUI _numText;
        private Action<bool> _action;
        private int _mergeConfigId;
        
        private int _mergeCount = 0;
        public DitchMergeBoard mergeBoard
        {
            get { return _mergeBoard; }
        }
        
        public static UIDitchMergeMainController Instance;
        
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
                DitchMergeGameLogic.Instance.ExitGame(_mergeConfigId);
                
                _action?.Invoke(true);
            });
            
            EventDispatcher.Instance.AddEventListener(ConstEvent.Event_Refresh_Merge, Event_Refresh_Merge);
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);

            _mergeConfigId = (int)objs[0];
            _action = (Action<bool>)objs[1];
            
            MergeManager.Instance.Refresh(MergeBoardEnum.Ditch);
            InitMergeBoard();
            
            _mergeBoard = transform.Find("Root").GetComponentDefault<DitchMergeBoard>("Board");
            
            //FilthyGuideLogic.Instance.GuideLogic(false);
            
            _numText.SetText("0/1");
        }

        private void InitMergeBoard()
        {
            var storageMergeBoard = MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Ditch);
            
            storageMergeBoard.Items.ForEach(a => a.Id = -1);

            _mergeConfig = DitchConfigManager.Instance.TableDitchMergeList.Find(a => a.Id == _mergeConfigId);
            if(_mergeConfig == null)
                return;
            
            _targetIcon.sprite = UserData.GetResourceIcon(_mergeConfig.TargetMergeId);
            
            var items = DitchConfigManager.Instance.TableDitchBoardList.FindAll(a=>a.BoardId == _mergeConfig.BoardId);
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
                    DitchMergeGameLogic.Instance.ExitGame(_mergeConfigId);
                    _action?.Invoke(false);
                },
                HasCloseButton = true,
                HasCancelButton = true,
                IsHighSortingOrder = true,
                CullingMaskLayer = 28,
            });
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(ConstEvent.Event_Refresh_Merge, Event_Refresh_Merge);
        }

        private void Event_Refresh_Merge(BaseEvent e)
        {
            _mergeCount++;
            // if(_nodeConfig != null)
            //     GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameAsmrChoose, _nodeConfig.Id.ToString(), _mergeConfig.Id.ToString(), _mergeCount.ToString());

            int newMergeId = (int)e.datas[0];
            
            if(newMergeId != _mergeConfig.TargetMergeId)
                return;
            
            _numText.SetText("1/1");
            
            // if(_nodeConfig != null)
            //     FilthyModel.Instance.OwnedNode(_nodeConfig.LevelId, _nodeConfig.Id);
            
            _finishObject.gameObject.SetActive(true);
            
            //FilthyGameLogic.Instance.PlaySound("sfx_miserable_congratulate");
        }
    }
}