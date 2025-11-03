using DragonPlus;
using GamePool;
using UnityEngine;

public class MergePromptManager : Manager<MergePromptManager>
{
    public MergeResourceController ShowPrompt(Vector3 position, float scale = 1f)
    {
        ShakeManager.Instance.ShakeLight();

        MergeResourceController controller =
            UIManager.Instance.OpenUI(UINameConst.MergeResource) as MergeResourceController;
        controller.gameObject.SetActive(false);

        controller.transform.position = position;
        controller.gameObject.transform.localScale = new Vector3(scale, scale, scale);
        controller.gameObject.SetActive(true);

        return controller;
    }

    public void HidePrompt()
    {
        MergeResourceController controller =
            UIManager.Instance.GetOpenedUIByPath(UINameConst.MergeResource) as MergeResourceController;
        if (controller != null)
        {
            controller.transform.SetParent(UIRoot.Instance.mUIRoot.transform);
            controller.transform.position = Vector3.zero;
            controller.gameObject.transform.localScale = Vector3.one;
        }

        UIManager.Instance.CloseUI(UINameConst.MergeResource);
    }

    public void ShowTextPrompt(Vector3 position, float scale = 1f)
    {
        string content = LocalizationManager.Instance.GetLocalizedString("UI_board_full");

        ShowTextPrompt(content, position, scale);
    }

    public void ShowRecharging(Vector3 position, float scale = 1f)
    {
        string content = LocalizationManager.Instance.GetLocalizedString("ui_recharging");

        ShowTextPrompt(content, position, scale);
    }
    
    public void ShowBagTextPrompt(Vector3 position, float scale = 1f)
    {
        string content = LocalizationManager.Instance.GetLocalizedString("UI_inventory_full");

        ShowTextPrompt(content, position, scale);
    }

    public void ShowBagFullTextPrompt(Vector3 position, float scale = 1f)
    {
        string content = LocalizationManager.Instance.GetLocalizedString("ui_tips_chest_drop_package");
        ShowTextPrompt(content, position, scale);
    }
    
    public void ShowBoxOpenNow(Vector3 position, float scale = 1f)
    {
        string content = LocalizationManager.Instance.GetLocalizedString("ui_opening");
        ShowTextPrompt(content, position, scale);
    }

    public void ShowTextPrompt(string content, Vector3 position, float scale = 1f)
    {
        ShakeManager.Instance.ShakeLight();

        MergeBoardFullController controller =
            UIManager.Instance.OpenUI(UINameConst.MergeBoardFull) as MergeBoardFullController;
        controller.gameObject.SetActive(false);

        controller.transform.position = position;
        controller.gameObject.transform.localScale = new Vector3(scale, scale, scale);
        controller.gameObject.SetActive(true);

        controller.PlayAnim(content, () => { UIManager.Instance.CloseUI(UINameConst.MergeBoardFull); });
    }

    public void ShowTextPromptMultiple(string content, Vector3 position, float scale = 1f)
    {
        ShakeManager.Instance.ShakeLight();
        
        MergeBoardFullController controller = UIRoot.Instance.CreateWindow("Merge/MergeBoardNum", componentType:(typeof(MergeBoardFullController))) as MergeBoardFullController;
        controller.gameObject.SetActive(false);
        if(controller.canvas == null)
            controller.InitCanvas();
        controller.canvas.sortingOrder = UIManager.Instance.GetMaxSortingOrder()+1;
        
        controller.transform.position = position;
        controller.gameObject.transform.localScale = new Vector3(scale, scale, scale);
        controller.gameObject.SetActive(true);

        controller.PlayAnim(content, () => { DestroyImmediate(controller.gameObject); });
    }
    
    public void ShowRankLevelTip(Vector3 position)
    {
        UIWindow uiWindow = UIManager.Instance.GetOpenedUIByPath(UINameConst.MergeMain);
        if (uiWindow == null || !uiWindow.isActiveAndEnabled)
        {
            uiWindow = UIManager.Instance.GetOpenedUIByPath(UINameConst.HappyGoMain);
        }
        
        if (uiWindow == null || !uiWindow.isActiveAndEnabled)
            return;
        
        
        var rewardTips = DragonU3DSDK.Asset.ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/LevelRanking/PopRewardTips");
        if(rewardTips == null)
            return;
        
        GameObject tips = GameObject.Instantiate(rewardTips);
        if(tips == null)
            return;

        tips.GetOrCreateComponent<MergeBoardFullController>().PlayAnim("+1", () =>
        {
            GameObject.Destroy(tips);
        });

        tips.GetOrCreateComponent<Canvas>().sortingOrder = uiWindow.canvas.sortingOrder + 1;

        tips.transform.parent = UIRoot.Instance.mUIRoot.transform;
        tips.transform.position = position;
        tips.gameObject.transform.localScale = Vector3.one;
        tips.gameObject.SetActive(true);
    }
}