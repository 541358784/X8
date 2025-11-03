using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

public class PreLoadVideoManager
{
    private static PreLoadVideoManager _instance = null;
    private Dictionary<string, VideoClip> _videoClips = new Dictionary<string, VideoClip>();

    private readonly string _videoPath = "Video/{0}";

    private PreLoadVideoManager()
    {
    }

    public static PreLoadVideoManager Instance()
    {
        if (_instance == null)
            _instance = new PreLoadVideoManager();

        return _instance;
    }

    public void PreLoadVideo(string videoName)
    {
        string videoClipPath = string.Format(_videoPath, videoName);
        if (_videoClips.ContainsKey(videoClipPath))
            return;

        VideoClip videoClip = Resources.Load<VideoClip>(videoClipPath);
        if (videoClip == null)
            return;

        _videoClips.Add(videoClipPath, videoClip);
    }

    public VideoClip LoadVideo(string videoClipPath)
    {
        if (_videoClips.ContainsKey(videoClipPath))
            return _videoClips[videoClipPath];

        return Resources.Load<VideoClip>(videoClipPath);
    }
}