using System;
using System.Collections;
using DragonPlus;
using UnityEngine;

public class AudioMusicLogic : Manager<AudioMusicLogic>
{
    private int[] musicIds = new[] {22, 23, 24, 25, 26};
    private int playIndex = 0;
    private int delayTime = 4;

    public void PlayMusic(bool isRest = true)
    {
        if (isRest)
        {
            playIndex = 0;
            StopAllCoroutines();
        }

        StartCoroutine(PlayMusic(musicIds[playIndex]));
    }

    public void StopMusic()
    {
        StopAllCoroutines();
        AudioManager.Instance.StopAllMusic();
    }

    private IEnumerator PlayMusic(int id)
    {
        float time = AudioManager.Instance.PlayMusic(id);

        yield return new WaitForSeconds((int) (time + 0.1f) + delayTime);

        playIndex++;
        playIndex = playIndex >= musicIds.Length ? 0 : playIndex;

        PlayMusic(false);
    }
}