using System.Collections.Generic;
using TileMatch.Game;
using UnityEngine.UI;

public class UIPopupDifficultyController : UIWindowController
{
    private List<Button> _buttons = new List<Button>();
    
    public override void PrivateAwake()
    {
        for (int i = 1; i <= 3; i++)
        {
            int index = i;
            transform.Find("Root/"+i.ToString()).GetComponent<Button>().onClick.AddListener(() =>
            {
                TileMatchGameManager.Instance.SelectDifficulty(index-1);
                CloseWindowWithinUIMgr(true);
            });
        }
    }
}