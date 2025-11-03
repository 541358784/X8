using System;
using System.IO;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.Video;

namespace asmr_new
{
    public class VideoHandler
    {
        private TableMoLevel _level;
        private Renderer _renderer;
        private FileStream _file;
        private VideoPlayer _videoPlayer;
        private AudioSource _audioSource;

        public bool IsPlaying => _videoPlayer != null && _videoPlayer.isPlaying;

        public double Time => _videoPlayer != null ? _videoPlayer.time : 0;

        private void InitVideoPlayer()
        {
            if (_videoPlayer == null)
                return;

            _videoPlayer.errorReceived += (source, message) =>
            {
                DebugUtil.LogError($"Video player {source} has error received: {message}");
            };

            try
            {
                string videoPath = $"{Application.persistentDataPath}/{_level.subID}.mp4";
                if (!File.Exists(videoPath))
                {
                    var path = $"Makeover/Levels/Level{_level.levelId}/Video/{_level.subID % 1001}";
                    var bytes = ResourcesManager.Instance.LoadResource<TextAsset>(path).bytes;
                    var filePath = Path.Combine(Application.persistentDataPath, _level.subID + ".mp4");
                    _file = File.Open(filePath, FileMode.OpenOrCreate);
                    _file.Write(bytes, 0, bytes.Length - 1);
                    _file.Flush();
                    _file.SetLength(bytes.Length);
                }
                _videoPlayer.url = videoPath;
                _videoPlayer.Pause();
                _renderer = _videoPlayer.GetComponent<Renderer>();
                _renderer.enabled = false;
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e.Message);
            }
        }

        private void InitAudioSource()
        {
            if (_audioSource == null)
                return;
            _audioSource.Pause();
            _audioSource.volume = SettingManager.Instance.SoundClose ? 0 : 1;
        }

        public VideoHandler(Component root, TableMoLevel level)
        {
            _level = level;
            _videoPlayer = root.GetComponentInChildren<VideoPlayer>();
            _audioSource = root.GetComponentInChildren<AudioSource>();
            InitVideoPlayer();
            InitAudioSource();
        }

        public void Play()
        {
            if (_videoPlayer != null)
                _videoPlayer.Play();
            if (_renderer != null && !_renderer.enabled)
                _renderer.enabled = true;
            if (_audioSource != null)
                _audioSource.Play();
        }

        public void Pause()
        {
            if (_videoPlayer != null)
                _videoPlayer.Pause();
            if (_audioSource != null)
                _audioSource.Pause();
        }

        public bool IsEnded()
        {
            if (_videoPlayer == null)
                return true;

            return !_videoPlayer.isPlaying && !_videoPlayer.isPaused &&
                   (_videoPlayer.time >= _videoPlayer.length ||
                    _videoPlayer.frame >= (long) _videoPlayer.frameCount - 1);
        }

        public void SetScale(float scale)
        {
            var trans = _videoPlayer.GetComponent<Transform>();
            var localScale = trans.localScale;
            localScale.x *= scale;
            localScale.y *= scale;
            trans.localScale = localScale;
        }

        public void Release()
        {
            var abName = $"module/asmr/levels/level{_level.levelId}/video/{_level.id % 1001}.ab";
            ResourcesManager.Instance.AssetBundleCache.Unload(abName, true);

            if (_videoPlayer != null)
                _videoPlayer.Stop();
            
            if (_file != null)
            {
                _file.Dispose();
                File.Delete(_file.Name);
                _file = null;
            }
            
            _videoPlayer = null;
            _audioSource = null;
            _level = null;
        }
    }
}