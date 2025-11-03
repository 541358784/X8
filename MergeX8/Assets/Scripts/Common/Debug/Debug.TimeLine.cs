// using System.ComponentModel;
// using DragonU3DSDK.Asset;
// using UnityEngine.Playables;
//
//
// public partial class SROptions
// {
//     private const string TimeLine = "TimeLine";
//
//     private bool _isTimeLine = false;
//     [Category(TimeLine)]
//     [DisplayName("切换TimeLine")]
//     public void SwitchTimeLine()
//     {
//         if (UIHomeMainController.mainController == null)
//             return;
//
//         var playableDirector = UIHomeMainController.mainController.transform.Find("Root/hip").GetComponent<PlayableDirector>();
//         playableDirector.gameObject.SetActive(true);
//         
//         string name = _isTimeLine ? "Prefabs/Home/timeLine_work1" : "Prefabs/Home/timeLine_work2";
//         var loadResource = ResourcesManager.Instance.LoadResource<PlayableAsset>(name);
//         if(loadResource == null)
//              return;
//         _isTimeLine = !_isTimeLine;
//         playableDirector.playableAsset = loadResource;
//         playableDirector.Play();
//     }
// }