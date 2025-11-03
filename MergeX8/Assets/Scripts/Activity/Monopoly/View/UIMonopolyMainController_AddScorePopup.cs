using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;

public partial class UIMonopolyMainController
{
    public void PopupAddScore(Vector3 position, int score)
    {
        var sortingLayerId = canvas.sortingLayerID;
        var sortingOrder = canvas.sortingOrder;
        var addScore = Instantiate(DefaultAddScore, DefaultAddScore.parent);
        addScore.gameObject.SetActive(true);
        addScore.position = position;
        var addScoreCanvas = addScore.gameObject.AddComponent<Canvas>();
        addScoreCanvas.overrideSorting = true;
        addScoreCanvas.sortingLayerID = sortingLayerId;
        addScoreCanvas.sortingOrder = sortingOrder + 2;
        addScore.gameObject.AddComponent<GraphicRaycaster>();
        var addScorePopup = addScore.gameObject.AddComponent<AddScorePopup>();
        addScorePopup.PopupAddScore(score,this);
        XUtility.WaitSeconds(2f, () =>
        {
            if (addScore)
            {
                Destroy(addScore.gameObject);
            }
        });
    }

    public class AddScorePopup : MonoBehaviour
    {
        private LocalizeTextMeshProUGUI NumText;
        private Animator Animator;

        private void Awake()
        {
            NumText = transform.Find("Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
            Animator = transform.GetComponent<Animator>();
        }

        public void ResetView()
        {
            NumText.SetText("+"+Score);
        }
        public UIMonopolyMainController MainUI;
        private int Score;

        public async void PopupAddScore(int score,UIMonopolyMainController mainUI)
        {
            MainUI = mainUI;
            Score = score;
            ResetView();
            Animator.PlayAnimation("open");
        }
    }
}