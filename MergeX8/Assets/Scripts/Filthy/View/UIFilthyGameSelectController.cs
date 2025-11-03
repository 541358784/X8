using System.Collections.Generic;
using ConnectLine;
using DragonPlus;
using DragonPlus.Config.DigTrench;
using DragonPlus.Config.FishEatFish;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay.UI.MiniGame;
using OnePath;
using Psychology;
using Stimulate.Configs;
using UnityEngine;
using UnityEngine.UI;

namespace Filthy.View
{
    public class UIFilthyGameSelectController : UIWindowController
    {
        public class ScrewSelectGroup : MiniGameSelectGroup
        {
            public override UIPopupGameTabulationController.MiniGameTypeTab SelectType()
            {
                return UIPopupGameTabulationController.MiniGameTypeTab.Screw;
            }
        }

        public class MergeSelectGroup : MiniGameSelectGroup
        {
            public override UIPopupGameTabulationController.MiniGameTypeTab SelectType()
            {
                return UIPopupGameTabulationController.MiniGameTypeTab.Merge;
            }
        }

        public abstract class MiniGameSelectGroup : MonoBehaviour
        {
            public abstract UIPopupGameTabulationController.MiniGameTypeTab SelectType();
            private Transform NormalGroup => transform.Find("Normal");
            private Transform SelectGroup => transform.Find("Selected");
            private Button ChooseBtn;
            private UIFilthyGameSelectController Controller;

            private void Awake()
            {
                ChooseBtn = transform.GetComponent<Button>();
                ChooseBtn.onClick.AddListener(OnClickChooseBtn);
            }

            public void BindController(UIFilthyGameSelectController controller)
            {
                Controller = controller;
            }

            public void OnClickChooseBtn()
            {
                Controller.UpdateSelectType(SelectType());
            }

            public void RefreshViewState(UIPopupGameTabulationController.MiniGameTypeTab curSelectType)
            {
                SelectGroup.gameObject.SetActive(curSelectType == SelectType());
                NormalGroup.gameObject.SetActive(curSelectType != SelectType());
            }
        }

        private Button StartBtn;
        private UIPopupGameTabulationController.MiniGameTypeTab SelectType = UIPopupGameTabulationController.MiniGameTypeTab.None;
        private List<MiniGameSelectGroup> BtnList = new List<MiniGameSelectGroup>();

        public override void PrivateAwake()
        {
            StartBtn = GetItem<Button>("Root/Button");
            StartBtn.onClick.AddListener(OnClickStartBtn);
            BtnList.Add(transform.Find("Root/Choose/merge").gameObject.AddComponent<MergeSelectGroup>());
            BtnList.Add(transform.Find("Root/Choose/screw").gameObject.AddComponent<ScrewSelectGroup>());
            
            foreach (var btn in BtnList)
            {
                btn.BindController(this);
            }

            UpdateSelectType(UIPopupGameTabulationController.MiniGameTypeTab.None);
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFatgirlAsmr23);
        }

        public void UpdateSelectType(UIPopupGameTabulationController.MiniGameTypeTab newType)
        {
            SelectType = newType;
            StartBtn.interactable = !(SelectType == UIPopupGameTabulationController.MiniGameTypeTab.None ||
                                      SelectType == UIPopupGameTabulationController.MiniGameTypeTab.MakeOver);
            for (var i = 0; i < BtnList.Count; i++)
            {
                BtnList[i].RefreshViewState(SelectType);
            }
        }

        public void OnClickStartBtn()
        {
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameChoose,
                data1: ((int)SelectType).ToString());
            
            AnimCloseWindow(() =>
            {
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.Add("FilthyGameSelect", "FilthyGameSelect");
                
                switch (SelectType)
                {
                    case UIPopupGameTabulationController.MiniGameTypeTab.Merge:
                    {
                        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFatgirlAsmr24);
                        GuideSubSystem.Instance.ForceFinished(101);
                        GuideSubSystem.Instance.ForceFinished(4420);
                        SceneFsm.mInstance.TransitionGame();
                        break;
                    }
                    case UIPopupGameTabulationController.MiniGameTypeTab.Screw:
                    {
                        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFatgirlAsmr25);
                        SceneFsm.mInstance.EnterScrewHome();
                        break;
                    }
                }
            });
        }
    }
}