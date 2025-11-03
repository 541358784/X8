// namespace DigTrench.UI
// {

using System;
using System.Collections.Generic;
using ConnectLine;
using DragonPlus;
using DragonPlus.Config.DigTrench;
using DragonPlus.Config.FishEatFish;
using DragonPlus.Config.Makeover;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay.UI.MiniGame;
using Makeover;
using OnePath;
using OnePathSpace;
using Psychology;
using Stimulate.Configs;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupGameChooseController:UIWindowController
{
    public class DigTrenchSelectGroup : MiniGameSelectGroup
    {
        public override UIPopupGameTabulationController.MiniGameTypeTab SelectType()
        {
            return UIPopupGameTabulationController.MiniGameTypeTab.DigTrench;
        }
    }
    public class OnePathSelectGroup : MiniGameSelectGroup
    {
        public override UIPopupGameTabulationController.MiniGameTypeTab SelectType()
        {
            return UIPopupGameTabulationController.MiniGameTypeTab.OnePath;
        }
    }
    public class ConnectLineSelectGroup : MiniGameSelectGroup
    {
        public override UIPopupGameTabulationController.MiniGameTypeTab SelectType()
        {
            return UIPopupGameTabulationController.MiniGameTypeTab.ConnectLine;
        }
    }
    public class FishEatFishSelectGroup : MiniGameSelectGroup
    {
        public override UIPopupGameTabulationController.MiniGameTypeTab SelectType()
        {
            return UIPopupGameTabulationController.MiniGameTypeTab.FishEatFish;
        }
    }
    
    public class StimulateSelectGroup : MiniGameSelectGroup
    {
        public override UIPopupGameTabulationController.MiniGameTypeTab SelectType()
        {
            return UIPopupGameTabulationController.MiniGameTypeTab.Stimulate;
        }
    }
    
    public class PsychologySelectGroup : MiniGameSelectGroup
    {
        public override UIPopupGameTabulationController.MiniGameTypeTab SelectType()
        {
            return UIPopupGameTabulationController.MiniGameTypeTab.Psychology;
        }
    }
    
    
    public abstract class MiniGameSelectGroup : MonoBehaviour
    {
        public abstract UIPopupGameTabulationController.MiniGameTypeTab SelectType();
        private Transform NormalGroup => transform.Find("Normal");
        private Transform SelectGroup => transform.Find("Selected");
        private Button ChooseBtn;
        private UIPopupGameChooseController Controller;

        private void Awake()
        {
            ChooseBtn = transform.GetComponent<Button>();
            ChooseBtn.onClick.AddListener(OnClickChooseBtn);
        }

        public void BindController(UIPopupGameChooseController controller)
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
        BtnList.Add(transform.Find("Root/Choose/DigTrench").gameObject.AddComponent<DigTrenchSelectGroup>());
        BtnList.Add(transform.Find("Root/Choose/FishEatFish").gameObject.AddComponent<FishEatFishSelectGroup>());
        BtnList.Add(transform.Find("Root/Choose/OnePath").gameObject.AddComponent<OnePathSelectGroup>());
        BtnList.Add(transform.Find("Root/Choose/ConnectLine").gameObject.AddComponent<ConnectLineSelectGroup>());
        BtnList.Add(transform.Find("Root/Choose/Stimulate").gameObject.AddComponent<StimulateSelectGroup>());
        BtnList.Add(transform.Find("Root/Choose/Psychology").gameObject.AddComponent<PsychologySelectGroup>());
        foreach (var btn in BtnList)
        {
            btn.BindController(this);
        }
        UpdateSelectType(UIPopupGameTabulationController.MiniGameTypeTab.None);

        if (Makeover.Utils.IsUseNewMiniGame())
        {
            //transform.Find("Root/Choose/Psychology").gameObject.SetActive(Utils.IsOn(UIPopupMiniGameController.MiniGameType.Psychology));
            transform.Find("Root/Choose/Psychology").gameObject.SetActive(true);
        }
    }

    public void UpdateSelectType(UIPopupGameTabulationController.MiniGameTypeTab newType)
    {
        SelectType = newType;
        StartBtn.interactable = !(SelectType == UIPopupGameTabulationController.MiniGameTypeTab.None ||
                                  SelectType == UIPopupGameTabulationController.MiniGameTypeTab.MakeOver);
        for (var i=0;i<BtnList.Count;i++)
        {
            BtnList[i].RefreshViewState(SelectType);
        }
    }
    public void OnClickStartBtn()
    {
        if (SelectType == UIPopupGameTabulationController.MiniGameTypeTab.None ||
            SelectType == UIPopupGameTabulationController.MiniGameTypeTab.MakeOver)
            return;
        StorageManager.Instance.GetStorage<StorageHome>().MiniGameDefaultType = (int) SelectType;
        
        if (Utils.IsUseNewMiniGame())
        {
            UIPopupMiniGameController.MiniGameType miniGameType = UIPopupMiniGameController.MiniGameType.None;
            switch (SelectType)
            {
                case UIPopupGameTabulationController.MiniGameTypeTab.ConnectLine:
                {
                    miniGameType = UIPopupMiniGameController.MiniGameType.ConnectLine;
                    break;
                }
                case UIPopupGameTabulationController.MiniGameTypeTab.DigTrench:
                {
                    miniGameType = UIPopupMiniGameController.MiniGameType.DigTrench;
                    break;
                }
                case UIPopupGameTabulationController.MiniGameTypeTab.Psychology:
                {
                    miniGameType = UIPopupMiniGameController.MiniGameType.Psychology;
                    break;
                }
            }
            StorageManager.Instance.GetStorage<StorageHome>().MiniGame.DefaultType = (int)miniGameType;
        }
        
        UILoadingTransitionController.Show(null);
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameChoose,
            data1:((int)SelectType).ToString());
        AnimCloseWindow(() =>
        {
            if (SelectType == UIPopupGameTabulationController.MiniGameTypeTab.DigTrench)
            {
                SceneFsm.mInstance.ChangeState(StatusType.DigTrench, DigTrenchConfigManager.Instance.DigTrenchLevelList[0], true);
            }
            else if (SelectType == UIPopupGameTabulationController.MiniGameTypeTab.FishEatFish)
            {
                SceneFsm.mInstance.ChangeState(StatusType.FishEatFish, FishEatFishConfigManager.Instance.FishEatFishLevelList[0],true);
            }
            else if (SelectType == UIPopupGameTabulationController.MiniGameTypeTab.OnePath)
            {
                SceneFsm.mInstance.ChangeState(StatusType.EnterOnePath, OnePathConfigManager.Instance._configs[0], true);
            }
            else if (SelectType == UIPopupGameTabulationController.MiniGameTypeTab.ConnectLine)
            {
                SceneFsm.mInstance.ChangeState(StatusType.EnterConnectLine, ConnectLineConfigManager.Instance._configs[0], true);
            }
            else if (SelectType == UIPopupGameTabulationController.MiniGameTypeTab.Stimulate)
            {
                SceneFsm.mInstance.ChangeState(StatusType.EnterStimulate, StimulateConfigManager.Instance._stimulateSetting[0], true);
            }
            else if (SelectType == UIPopupGameTabulationController.MiniGameTypeTab.Psychology)
            {
                SceneFsm.mInstance.ChangeState(StatusType.EnterPsychology, PsychologyConfigManager.Instance._configs[0], true);
            }
        });
    }
}