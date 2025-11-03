namespace Merge.OneselfChose
{
    public class OneselfChoseLogic
    {
        public static void OpenChoseUI(int mergeId, int boardIndex)
        {
            var config = GameConfigManager.Instance.GetChoiceChest(mergeId);
            if(config == null)
                return;
            
            if(config.item == null || config.item.Length == 0)
                return;

            if (config.item.Length == 3)
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupOneOutOfThree,mergeId, boardIndex);
            }
            else if(config.item.Length == 5)
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupOneOutOfFive,mergeId, boardIndex);
            }
        }
    }
}