using DragonPlus;

public partial class UIKeepPetMainController
{
    public void PlayDogBGM()
    {
        AudioManager.Instance.PlayMusic("bgm_keep_pet_dog",true);
    }

    public void StopDogBGM()
    {
        XUtility.WaitSeconds(0.1f, () =>
        {
            AudioManager.Instance.PlayMusic(1, true);
        });
    }

    #region 饥饿音效
    public void StartLoopHungrySound()
    {
        if (!IsLoopHungrySound)
        {
            IsLoopHungrySound = true;
            InvokeRepeating("PlayHungrySound",0f,5f);   
        }

        StopLoopSleepSound();
    }

    private bool IsLoopHungrySound = false;
    public void PlayHungrySound()
    {
        AudioManager.Instance.PlaySoundById(138);
    }
    public void StopLoopHungrySound()
    {
        if (IsLoopHungrySound)
        {
            IsLoopHungrySound = false;
            CancelInvoke("PlayHungrySound");
        }
    }
    #endregion
    #region 睡觉音效
    public void StartLoopSleepSound()
    {
        if (!IsLoopSleepSound)
        {
            IsLoopSleepSound = true;
            InvokeRepeating("PlaySleepSound",0f,5f);   
        }
        StopLoopHungrySound();
    }
    private bool IsLoopSleepSound = false;
    public void PlaySleepSound()
    {
        AudioManager.Instance.PlaySoundById(143);
    }
    public void StopLoopSleepSound()
    {
        if (IsLoopSleepSound)
        {
            IsLoopSleepSound = false;
            CancelInvoke("PlaySleepSound");
        }
    }
    #endregion
    #region 搜寻完成音效
    public void StartSearchFinishBarkSound()
    {
        if (!IsLoopSearchFinishBarkSound)
        {
            IsLoopSearchFinishBarkSound = true;
            InvokeRepeating("PlaySearchFinishBarkSound",2.35f,3.67f);
        }
    }
    private bool IsLoopSearchFinishBarkSound = false;
    public void PlaySearchFinishBarkSound()
    {
        AudioManager.Instance.PlaySoundById(155);
    }
    public void StopSearchFinishBarkSound()
    {
        if (IsLoopSearchFinishBarkSound)
        {
            IsLoopSearchFinishBarkSound = false;
            CancelInvoke("PlaySearchFinishBarkSound");
        }
    }
    #endregion

    public void PlayHappySound()
    {
        XUtility.WaitSeconds(1.6f, () =>
        {
            if (this && DogSpine.AnimationState.GetCurrent(0).Animation.Name == "happy")
            {
                AudioManager.Instance.PlaySound(137);
            }
        });
    }

    public void PlayIdle2Sound()
    {
        XUtility.WaitSeconds(3.4f, () =>
        {
            if (this && DogSpine.AnimationState.GetCurrent(0).Animation.Name == "idle2")
                AudioManager.Instance.PlaySound(155);
        });
    }

    public void PlayFrisbeeSound()
    {
        AudioManager.Instance.PlaySoundById(142);
    }

    public void PlaySearchFinishSound()
    {
        AudioManager.Instance.PlaySoundById(141);
    }

    public void PlaySearchingSound()
    {
        XUtility.WaitSeconds(3.6f, () =>
        {
            if (this && DogSpine.AnimationState.GetCurrent(0).Animation.Name == "mission")
            {
                AudioManager.Instance.PlaySound(140);
            }
        });
    }

    public void PlayEatSound()
    {
        AudioManager.Instance.PlaySoundById(139);
    }
}