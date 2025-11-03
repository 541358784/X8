using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.JungleAdventure.Controller
{
    public partial class UIJungleAdventureMainController : UIWindowController
    {
        private RawImage _rawImage;
        
        private CancellationTokenSource cts;
        public override void PrivateAwake()
        {
            _rawImage = transform.Find("Root/RawImage").GetComponent<RawImage>();
            
            transform.Find("Root/TopGroup/ButtonClose").GetComponent<Button>().onClick.AddListener(() =>
            {
                if(_isMoving)
                    return;
                
                if(_isGuideing)
                    return;
                
                AnimCloseWindow();
            });
            
            Awake_Path();
            Awake_Camera();
            Awake_Entity();
            Awake_Reward();
            AwakeUI();
            AwakeRank();
            Awake_Guide();
        }

        private void Start()
        {
            InitMiniReward();
            InitEntityPosition();
            InitSlider();
            InitUI();
            InitRewardStatus();

            cts = new CancellationTokenSource();
            MoveEntity(true, cts.Token);

            TriggerGuide();
        }

        private void Update()
        {
            UpdateCamera();
            UpdateRewardInput();
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.DispatchEvent(EventEnum.Event_Refre_JungleAdventure_UI);
            cts.Cancel();
            cts.Dispose();
        }
        
        private const string coolTimeKey = "UIJungleAdventureMainController";
        public static bool CanShow()
        {
            if (GuideSubSystem.Instance.IsShowingGuide())
                return false;
            
            if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.JungleAdventureStart))
                return false;
                    
            if (!JungleAdventureModel.Instance.IsOpened())
                return false;

            if (!JungleAdventureModel.Instance.IsPreheatEnd())
                return false;
            
            if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
                return false;
            
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey,CommonUtils.GetTimeStamp());
            UIManager.Instance.OpenWindow(UINameConst.UIJungleAdventureMain);
            return true;
        }
    }
}