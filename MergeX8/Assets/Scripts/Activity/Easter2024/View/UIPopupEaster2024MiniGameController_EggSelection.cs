using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupEaster2024MiniGameController
{
    public void AwakeEggSelection()
    {
        for (var i = 0; i < 9; i++)
        {
            var egg = transform.Find("Root/Game/" + (i+1)).gameObject.AddComponent<EggSelection>();
            egg.Init(i, OnClick);
            EggSelectionList.Add(egg);
        }
    }

    public void OnClick(int eggIndex)
    {
        var egg = EggSelectionList[eggIndex];
        if (egg.IsOpen)
            return;
        if (SelectIndex >= MiniGameConfig.ResultList.Count)
            return;
        var resultId = MiniGameConfig.ResultList[SelectIndex];
        SelectIndex++;
        egg.Open(resultId);
        ResultState[resultId]++;
        ShowCollectEffect(resultId,ResultState[resultId]);
        if (ResultState[resultId] >= WinNeedCount)
        {
            HandleWin(resultId);
        }
    }
    public class EggSelection:MonoBehaviour
    {
        private Button EggBtn;
        private int EggIndex;
        private Action<int> OnClick;
        public bool IsOpen = false;
        public int ResultId = -1;
        private Transform Cover;
        private List<Transform> ResultList = new List<Transform>();
        public Transform Effect;
        private Animator Animator;
        private Transform WinEffect;
        private void Awake()
        {
            Animator = transform.GetComponent<Animator>();
            EggBtn = transform.GetComponent<Button>();
            EggBtn.onClick.AddListener(OnClickEgg);
            for (var i = 0; i < 3; i++)
            {
                var resultIcon = transform.Find("IconGroup/Icon" + (i + 1));
                ResultList.Add(resultIcon);
                resultIcon.gameObject.SetActive(false);
            }
            Cover = transform.Find("Egg");
            Effect = transform.Find("FX_egg");
            Effect.gameObject.SetActive(false);
            WinEffect = transform.Find("VFX_BG_1");
            WinEffect?.gameObject.SetActive(false);
        }

        public void OnClickEgg()
        {
            OnClick(EggIndex);
        }
        public void Init(int eggIndex,Action<int> onClick)
        {
            EggIndex = eggIndex;
            OnClick = onClick;
            IsOpen = false;
        }

        public async void Open(int resultId)
        {
            if (IsOpen)
                return;
            IsOpen = true;
            ResultId = resultId;
            if (Animator)
            {
                Animator.PlayAnimation("open");
                await XUtility.WaitSeconds(0.35f);
            }
            Cover.gameObject.SetActive(false);
            ResultList[resultId].gameObject.SetActive(true);
            Effect.DOKill();
            Effect.gameObject.SetActive(false);
            Effect.gameObject.SetActive(true);
            DOVirtual.DelayedCall(2f, () => Effect.gameObject.SetActive(false)).SetTarget(Effect);
        }

        public void PerformWin()
        {
            EggBtn.interactable = false;
            WinEffect?.gameObject.SetActive(true);
        }

        public void PerformDark()
        {
            EggBtn.interactable = false;
        }

        public void PerformUnOpenDark()
        {
            EggBtn.interactable = false;
        }
    }
}