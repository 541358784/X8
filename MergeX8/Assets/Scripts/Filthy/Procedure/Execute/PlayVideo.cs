using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Filthy.Game;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using NotImplementedException = System.NotImplementedException;

namespace Filthy.Procedure
{
    public class PlayVideo : IExecute
    {
        public Transform _root { get; set; }
        public ProcedureBase _procedureBase { get; set; }

        private VideoPlayer _videoPlayer;

        private string _videoName;
        private bool _isLoop;

        private Button _button;
        
        public void Init(Transform root, ProcedureBase procedureBase)
        {
            _root = root;
            _procedureBase = procedureBase;
            
            _videoPlayer = root.transform.Find("Root/Video").GetComponent<VideoPlayer>();

            
            var paramArray = procedureBase._config.ExecuteParam.Split(',');

            _videoName = paramArray[0];
            _isLoop = paramArray[1] == "1" ? true : false;
        }
        
        public bool Execute()
        {  
            _button = _root.transform.Find("Root/SkipButton").GetComponent<Button>();
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() =>
            {
                FilthyGameLogic.Instance._procedureLogic.SkipProcedure(_procedureBase);
            });
            _button.gameObject.SetActive(true);
            _videoPlayer.Stop();
            _videoPlayer.time = 0;
            _videoPlayer.clip = Resources.Load<VideoClip>(_videoName);
            _videoPlayer.isLooping = _isLoop;
            _videoPlayer.Play();
            return true;
        }
    }
}