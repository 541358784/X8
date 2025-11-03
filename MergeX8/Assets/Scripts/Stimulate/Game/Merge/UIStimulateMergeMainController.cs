using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Stimulate.Configs;
using Stimulate.Event;
using Stimulate.Model.Guide;
using UnityEngine;
using UnityEngine.UI;
using NotImplementedException = System.NotImplementedException;

namespace Stimulate.Model.Merge
{
    public class UIStimulateMergeMainController : UIWindowController
    {
        private TableStimulateMerge _stimulateMerge;
        private TableStimulateNodes _nodeConfig;
        private bool _canExit = true;
        private StimulateMergeBoard _mergeBoard;
        private Image _targetIcon;
        private GameObject _finishObject;
        private LocalizeTextMeshProUGUI _numText;

        private int _mergeCount = 0;
        public StimulateMergeBoard mergeBoard
        {
            get { return _mergeBoard; }
        }
        
        public static UIStimulateMergeMainController Instance;
        
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
                StimulateGameLogic.Instance.ExitGame(_nodeConfig);
            });
            
            EventDispatcher.Instance.AddEventListener(ConstEvent.Event_Refresh_Merge, Event_Refresh_Merge);
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);

            _nodeConfig = (TableStimulateNodes)objs[0];
            
            MergeManager.Instance.Refresh(MergeBoardEnum.Stimulate);
            InitMergeBoard();
            
            _mergeBoard = transform.Find("Root").GetComponentDefault<StimulateMergeBoard>("Board");
            
            StimulateGuideLogic.Instance.GuideLogic(false);
            
            _numText.SetText("0/1");
        }

        private void InitMergeBoard()
        {
            var storageMergeBoard = MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Stimulate);
            
            storageMergeBoard.Items.ForEach(a => a.Id = -1);

            _stimulateMerge = StimulateConfigManager.Instance._stimulateMerge.Find(a => a.id == _nodeConfig.param);
            if(_stimulateMerge == null)
                return;
            
            _targetIcon.sprite = UserData.GetResourceIcon(_stimulateMerge.targeMergeId);
            
            List<TableStimulateBoard> items = StimulateConfigManager.Instance.GetBoards(_stimulateMerge.boardId);
            for (var i = 0; i < items.Count; i++)
            {
                if(i >= storageMergeBoard.Items.Count)
                    break;
                
                if(items[i].itemId == 0)
                    continue;
                
                storageMergeBoard.Items[i].Id = items[i].itemId;
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
                    if (GuideSubSystem.Instance.isFinished(2011) && !GuideSubSystem.Instance.isFinished(2012))
                    {
                        GuideSubSystem.Instance.CacheGuideFinished.Remove(2011);
                    }
                    GuideSubSystem.Instance.CloseCurrent();
                    StimulateGameLogic.Instance.ExitGame(_nodeConfig);
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
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameAsmrChoose, _nodeConfig.id.ToString(), _stimulateMerge.id.ToString(), _mergeCount.ToString());

            int newMergeId = (int)e.datas[0];
            
            if(newMergeId != _stimulateMerge.targeMergeId)
                return;
            
            _numText.SetText("1/1");
            
            StimulateModel.Instance.OwnedNode(_nodeConfig.levelId, _nodeConfig.id);
            _finishObject.gameObject.SetActive(true);
            
            StimulateGameLogic.Instance.PlaySound("sfx_miserable_congratulate");
        }
    }
}