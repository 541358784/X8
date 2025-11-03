
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class SROptions
    {
        private const string JungleAdventure = "丛林探险";

        [Category(JungleAdventure)]
        [DisplayName("重置")]
        public void RestJungleAdventure()
        {
            var activityId = JungleAdventureModel.Instance.JungleAdventure.ActivityId;
            JungleAdventureModel.Instance.JungleAdventure.Clear();
            JungleAdventureModel.Instance.JungleAdventure.ActivityId = activityId;
            
            CleanGuideList(new List<int>()
            {
                4450,
                4451,
                4452,
                4453,
                4500}
            );
        }
        
        [Category(JungleAdventure)]
        [DisplayName("当前分数")]
        public int JungleAdventureScore
        {
            get
            {
                return JungleAdventureModel.Instance.JungleAdventure.CurrentScore;
            }
            set
            {
                int oldScore = JungleAdventureModel.Instance.JungleAdventure.CurrentScore;
                JungleAdventureModel.Instance.JungleAdventure.CurrentScore = value;

                int diffScore = JungleAdventureModel.Instance.JungleAdventure.CurrentScore - oldScore;
                if(diffScore > 0)
                    JungleAdventureLeaderBoardModel.Instance.GetLeaderBoardStorage(JungleAdventureModel.Instance.JungleAdventure.ActivityId)?.CollectStar(diffScore);   
            }
        }
    }