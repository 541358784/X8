using System;
using System.Linq;
using DragonPlus.Config.MiniGame;
using Scripts.UI;

namespace MiniGame
{
    public partial class MiniGameModel
    {
        public void LoadMiniGame()
        {
            int chapterId = CurrentChapter;
            if (chapterId > MiniGameConfigManager.Instance.MiniGameChapterList.Last().Id)
                chapterId = MiniGameConfigManager.Instance.MiniGameChapterList.Last().Id;
            
            var chapterConfig = MiniGameModel.Instance.GetChapterConfig(chapterId);
            if (chapterConfig == null)
                return;

            var chapterType = (ChapterType)chapterConfig.Type;

            switch (chapterType)
            {
                case ChapterType.New:
                {
                    UIChapter.Open(chapterId);
                    break;
                }
                case ChapterType.Normal:
                {
                    var storage = GetChapterStorage(chapterId);
                    if (!storage.StoryPlayed)
                    {
                        UIMiniGameStory.Open(chapterId);
                        storage.StoryPlayed = true;
                    }
                    else
                    {
                        UIChapter.Open(chapterId);
                    }
                    break;
                }
            }
        }
    }
}