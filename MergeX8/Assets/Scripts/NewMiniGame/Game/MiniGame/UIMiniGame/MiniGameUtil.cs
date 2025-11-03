using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Scripts.UI
{
    public enum MiniGameLevelType
    {
        ASRM = 1,
        STORY = 2,
        Direct = 3,
    }

    public enum MiniGameCommonStatus
    {
        Lock, // 未解锁
        Processing, // 已解锁、正在进行
        Finished, // 已解锁、已完成
    }

    public enum MiniGameLockReason
    {
        None,
        NotFinishPre, // 前置未完成
        NotReachMainLevel, // 主线关卡未达到等级
    }

    public struct MiniGameStatus
    {
        public MiniGameCommonStatus Status;
        public MiniGameLockReason LockReason;
    }

    public static class MiniGameUtil
    {
        public static double GetEndTimeWithSignal(PlayableDirector director)
        {
            var timelineAsset = director.playableAsset as TimelineAsset;
            if (timelineAsset is null) return director.duration;

            foreach (var track in timelineAsset.GetOutputTracks())
            {
                if (track is not MarkerTrack markerTrack) continue;

                foreach (var marker in markerTrack.GetMarkers())
                {
                    if (marker is not SignalEmitter signalEmitter) continue;

                    MiniGameDebugLog($"[MiniGame] 获取到 story 的信号：{signalEmitter.asset.name}，时间：{marker.time}");
                    return marker.time;
                }
            }

            return director.duration;
        }

        public static void MiniGameDebugLog(string log)
        {
#if MINI_GAME_DEBUG
            DebugUtil.Log(log);
#endif
        }
    }
}