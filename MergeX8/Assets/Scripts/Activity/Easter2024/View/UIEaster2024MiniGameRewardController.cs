using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine.UI;

public class UIEaster2024MiniGameRewardController:UIWindowController
{
    private LocalizeTextMeshProUGUI ScoreText;
    private Button CloseBtn;
    private Action Callback;
    public override void PrivateAwake()
    {
        ScoreText = GetItem<LocalizeTextMeshProUGUI>("Root/Reward/TextNum");
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        var scoreCount = (int) objs[0];
        var resultId = (int) objs[1];
        Callback = objs[2] as Action;
        ScoreText.SetText(scoreCount.ToString());
        var effect = transform.Find("Root/Reward/FX_0" + (resultId + 1));
        effect.gameObject.SetActive(false);
        effect.gameObject.SetActive(true);
        AudioManager.Instance.PlaySound("sfx_easter2024_mini_win");
    }

    public void OnClickCloseBtn()
    {
        Callback();
        AnimCloseWindow();
    }
    public static UIEaster2024MiniGameRewardController Open(int scoreCount,int resultId,Action callback)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIEaster2024MiniGameReward, scoreCount,resultId,callback) as
            UIEaster2024MiniGameRewardController;
    }
}