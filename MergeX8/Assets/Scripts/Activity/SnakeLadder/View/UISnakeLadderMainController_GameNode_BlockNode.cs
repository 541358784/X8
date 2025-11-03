using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public partial class UISnakeLadderMainController
{
    public class BlockNode:MonoBehaviour
    {
        private int MultiValue = 1;
        public void SetScoreMultiValue(int multiValue)
        {
            var blockType = (SnakeLadderBlockType) BlockConfig.BlockType;
            if (blockType == SnakeLadderBlockType.Start || blockType == SnakeLadderBlockType.End ||
                blockType == SnakeLadderBlockType.Snake || blockType == SnakeLadderBlockType.Ladder)
                return;
            MultiValue = multiValue;
            if (MultiValue > 1)
            {
                Double.gameObject.SetActive(true);
                Double.SetText("x"+MultiValue);
            }
            else
            {
                Double.gameObject.SetActive(false);
            }
        }
        
        private SnakeLadderBlockConfig BlockConfig;
        private Transform LadderBack;
        private Transform SnakeBack;
        private Transform NormalBack;
        private LocalizeTextMeshProUGUI Double;
        private Transform Score;
        private LocalizeTextMeshProUGUI ScoreText;
        private Transform Item;
        private Image ItemSprite;
        private LocalizeTextMeshProUGUI ItemText;
        public void Init(SnakeLadderBlockConfig config)
        {
            BlockConfig = config;
            var blockType = (SnakeLadderBlockType) BlockConfig.BlockType;
            if (blockType == SnakeLadderBlockType.Start || blockType == SnakeLadderBlockType.End)
                return;
            LadderBack = transform.Find("BG/Yellow");
            SnakeBack = transform.Find("BG/Purple");
            NormalBack = transform.Find("BG/Blue");
            Double = transform.Find("Content/Double").GetComponent<LocalizeTextMeshProUGUI>();
            Double.gameObject.SetActive(false);
            Score = transform.Find("Content/Num");
            ScoreText = transform.Find("Content/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
            Item = transform.Find("Content/Item");
            ItemSprite = transform.Find("Content/Item/Icon").GetComponent<Image>();
            ItemText = transform.Find("Content/Item/Text").GetComponent<LocalizeTextMeshProUGUI>();
            if (blockType == SnakeLadderBlockType.Snake)
            {
                Score.gameObject.SetActive(false);
                Item.gameObject.SetActive(false);
                LadderBack.gameObject.SetActive(false);
                SnakeBack.gameObject.SetActive(true);
                NormalBack.gameObject.SetActive(false);
            }
            else if (blockType == SnakeLadderBlockType.Ladder)
            {
                Score.gameObject.SetActive(false);
                Item.gameObject.SetActive(false);
                LadderBack.gameObject.SetActive(true);
                SnakeBack.gameObject.SetActive(false);
                NormalBack.gameObject.SetActive(false);
            }
            else if (blockType == SnakeLadderBlockType.Score)
            {
                Score.gameObject.SetActive(true);
                ScoreText.SetText(BlockConfig.Score.ToString());
                Item.gameObject.SetActive(false);
                LadderBack.gameObject.SetActive(false);
                SnakeBack.gameObject.SetActive(false);
                NormalBack.gameObject.SetActive(true);   
            }
            else if (blockType == SnakeLadderBlockType.Reward)
            {
                Score.gameObject.SetActive(false);
                Item.gameObject.SetActive(true);
                ItemSprite.sprite = UserData.GetResourceIcon(BlockConfig.RewardId[0],UserData.ResourceSubType.Big);
                ItemText.SetText(BlockConfig.RewardNum[0].ToString());
                LadderBack.gameObject.SetActive(false);
                SnakeBack.gameObject.SetActive(false);
                NormalBack.gameObject.SetActive(true); 
            }
        }

        public void SetLadderTop()
        {
            LadderBack.gameObject.SetActive(true);
            SnakeBack.gameObject.SetActive(false);
            NormalBack.gameObject.SetActive(false);
        }

        public void SetSnakeTail()
        {
            LadderBack.gameObject.SetActive(false);
            SnakeBack.gameObject.SetActive(true);
            NormalBack.gameObject.SetActive(false);
        }
    }
}