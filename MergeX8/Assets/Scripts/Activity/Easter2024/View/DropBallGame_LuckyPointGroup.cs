using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public partial class DropBallGame
{
    private bool InitLuckyPointGroupFlag = false;
    private LuckyPointGroup LuckyPointNode;
    public void InitLuckyPointGroup()
    {
        if (InitLuckyPointGroupFlag)
            return;
        InitLuckyPointGroupFlag = true;
        LuckyPointNode = MainUI.LuckyPointGroup.gameObject.AddComponent<LuckyPointGroup>();
        LuckyPointNode.Init(this);
    }
    public class LuckyPointGroup : MonoBehaviour
    {
        private int MiniGameNeedLuckyPointCount => Easter2024Model.Instance.MiniGameNeedLuckyPointCount;
        private int LuckyPointCount => Game.Storage.LuckyPointCount;
        private DropBallGame Game;
        private Button MiniGameBtn;

        private List<Animator> EffectList = new List<Animator>();
        // private LocalizeTextMeshProUGUI LuckyPointCountText;
        private void Awake()
        {
            MiniGameBtn = transform.Find("Button").GetComponent<Button>();
            MiniGameBtn.onClick.AddListener(OnClickMiniGameBtn);
            for (var i = 0; i < MiniGameNeedLuckyPointCount; i++)
            {
                var effect = transform.Find((i + 1).ToString()).GetComponent<Animator>();
                EffectList.Add(effect);
            }
            // LuckyPointCountText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            EventDispatcher.Instance.AddEvent<EventEaster2024LuckyPointCountChange>(OnLuckyPointCountChange);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventEaster2024LuckyPointCountChange>(OnLuckyPointCountChange);
        }

        public void OnLuckyPointCountChange(EventEaster2024LuckyPointCountChange evt)
        {
            UpdateUI(true);
        }
        public void Init(DropBallGame game)
        {
            Game = game;
            UpdateUI(false);
        }

        public void UpdateUI(bool withEffect = false)
        {
            // MiniGameBtn.interactable = LuckyPointCount >= MiniGameNeedLuckyPointCount;
            MiniGameBtn.gameObject.SetActive(LuckyPointCount >= MiniGameNeedLuckyPointCount);
            for (var i = 0; i < EffectList.Count; i++)
            {
                var curState = i < LuckyPointCount;
                if (withEffect && curState)
                {
                    EffectList[i].Play("appear");
                }
                else
                {
                    if (curState)
                    {
                        EffectList[i].Play("idle");
                    }
                    else
                    {
                        EffectList[i].Play("normal");
                    }
                }
            }
            // LuckyPointCountText.SetText(LuckyPointCount.ToString());
        }
        public void OnClickMiniGameBtn()
        {
            if (Game.IsPlaying())
                return;
            if (LuckyPointCount < MiniGameNeedLuckyPointCount)
                return;
            UIPopupEaster2024MiniGameController.Open(Game.Storage);
        }
    }
}