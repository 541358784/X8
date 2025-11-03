using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DragonPlus.Config.MiniGame;
using MiniGame;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupTaskController
{
    private Transform _miniItem;
    private Transform _miniContent;
    private ScrollRect _scrollRect;
    private bool _isLocation = false;
    
    private List<UIChapterCell> _miniGameItems = new List<UIChapterCell>();
    private void Awake_MiniGame()
    {
        _scrollRect = transform.Find("Root/MiniGame/Scroll View").GetComponent<ScrollRect>();
        _miniItem = transform.Find("Root/MiniGame/Scroll View/Viewport/Content/ChapterItem");
        _miniItem.gameObject.SetActive(false);
        _miniContent = transform.Find("Root/MiniGame/Scroll View/Viewport/Content");
    }
    
    private void InitMiniGameView()
    {
        var chapters = MiniGameConfigManager.Instance.MiniGameChapterList;
        for (var i = 0; i <= chapters.Count; i++)
        {
            var obj = GameObject.Instantiate(_miniItem, _miniItem.transform.parent);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.gameObject.SetActive(true);

            var subView = new UIChapterCell();
            subView.Create(obj.gameObject, new ChapterItemCellData
            {
                chapterId = i+1,
                isComingSoon = i >= chapters.Count,
            });
            
            _miniGameItems.Add(subView);
        }
    }

    private void FixedUpdate()
    {
        _miniGameItems.ForEach(a=>a.FixedUpdate());
    }

    private async UniTask InitLocation()
    {
        if(_isLocation)
            return;
        _isLocation = true;
        await UniTask.WaitForEndOfFrame();
        
        var chapterId = MiniGameModel.Instance.GetMinUnFinishedChapterId();
        var index = _miniGameItems.FindIndex(a => a.Storage.Id == chapterId);
        if (index < 0)
            index = 0;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);

        _scrollRect.verticalNormalizedPosition = 1f-1.0f * index / (MiniGameModel.Instance.ChapterConfigList.Count);
    }
}