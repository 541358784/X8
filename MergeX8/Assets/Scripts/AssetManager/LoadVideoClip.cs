using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class LoadVideoClip : MonoBehaviour
{
    public string videoClipPath = "";

    private VideoPlayer _videoPlayer;
    private RawImage _rawImage;

    private void Awake()
    {
        _videoPlayer = transform.GetComponent<VideoPlayer>();
        _rawImage = transform.GetComponentInChildren<RawImage>();

        //SetRawImageActive(false);
        PlayVideo();
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Vector3 localPos = _rawImage.transform.localPosition;
        _rawImage.transform.localPosition = new Vector3(10000, 10000, 10000);
        yield return new WaitForSeconds(1f);
        SetRawImageActive(true);
        _rawImage.transform.localPosition = localPos;
    }

    void PlayVideo()
    {
        VideoClip videoClip = PreLoadVideoManager.Instance().LoadVideo(videoClipPath);
        if (videoClip == null)
            return;

        _videoPlayer.clip = videoClip;
        _videoPlayer.Play();
    }

    private void SetRawImageActive(bool active)
    {
        if (_rawImage == null)
            return;

        _rawImage.gameObject.SetActive(active);
    }
}