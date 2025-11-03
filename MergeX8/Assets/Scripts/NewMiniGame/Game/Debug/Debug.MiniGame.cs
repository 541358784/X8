using System.ComponentModel;
using ASMR;
using DragonU3DSDK.Storage;
using Manager;
using MiniGame;
using UnityEngine;

public partial class SROptions
{
    private int _levelIdMiniGame = 1;

    [Category("1 新小游戏")]
    [DisplayName("是否开启小游戏")]
    public bool IsOpenMiniGame
    {
        get
        {
            if (PlayerPrefs.HasKey("OpenMiniGame"))
            {
                if (PlayerPrefs.GetString("OpenMiniGame") == "true")
                    return true;

                return false;
            }

            return MiniGameModel.Instance.IsOpen();
        }
        set
        {
            if(value)
                PlayerPrefs.SetString("OpenMiniGame", "true");
            else
            {
                PlayerPrefs.SetString("OpenMiniGame", "false");
            }
        }
    }

    [Category("1 新小游戏")]
    [DisplayName("游戏模式")]
    public GameModeManager.GameMode GetGameMode
    {
        get
        {
            return GameModeManager.Instance.GetGameMode();
        }
        set
        {
            GameModeManager.Instance.SetGameMode(value);
        }
    }
    
    [Category("1 新小游戏")]
    [DisplayName("当前游戏模式")]
    public GameModeManager.CurrentGameMode GetCurrentGameMode
    {
        get
        {
            return GameModeManager.Instance.GetCurrenGameMode();
        }
        set
        {
            GameModeManager.Instance.SetCurrentGameMode(value);
        }
    }
    
    
    [Category("1 新小游戏")]
    [DisplayName("完成关卡")]
    public void FinishCurrentASMRLevel()
    {
        ASMRModel.Instance.FastFinishCurrentLevel();
    }
    
    [Category("1 新小游戏")]
    [DisplayName("指定解锁到关卡ID")]
    [Sort(1)]
    public int LevelIdMiniGame
    {
        get { return _levelIdMiniGame; }
        set { _levelIdMiniGame = value; }
    }

    [Category("1 新小游戏")]
    [DisplayName("清档")]
    public void MiniGame_Clear()
    {
        StorageManager.Instance.GetStorage<StorageASMR>().Clear();
        StorageManager.Instance.GetStorage<StorageMiniGameVersion>().Clear();
    }
    
    [Category("1 新小游戏")]
    [DisplayName("解锁到指定章节ID")]
    public void MiniGame_UnlockToChapter()
    {
        var maxId = MiniGameModel.Instance.GetMaxLevelId();
        if (LevelIdMiniGame > maxId)
        {
            return;
        }
        foreach (var kv in StorageManager.Instance.GetStorage<StorageMiniGameVersion>().Chapters)
        {
            kv.Value.LevelsDic.Clear();
            kv.Value.Claimed = kv.Value.Id <= LevelIdMiniGame;
            kv.Value.StoryPlayed = kv.Value.Id <= LevelIdMiniGame;
        }

        StorageManager.Instance.GetStorage<StorageMiniGameVersion>().CurrentChapter = LevelIdMiniGame;
    }
}