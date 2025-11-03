using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Framework;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Deco.World;
using Deco.Area;


namespace Decoration
{
    using StorageDecoration = DragonU3DSDK.Storage.Decoration.StorageDecoration;
    using StorageWorld = DragonU3DSDK.Storage.Decoration.StorageWorld;
    using StorageArea = DragonU3DSDK.Storage.Decoration.StorageArea;
    using StorageItem = DragonU3DSDK.Storage.Decoration.StorageItem;
    using StorageNode = DragonU3DSDK.Storage.Decoration.StorageNode;
    using StorageStage = DragonU3DSDK.Storage.Decoration.StorageStage;
    public struct AssetGroup
    {
        public string groupName;
        public string resPath;

        public AssetGroup(string groupName, string resPath)
        {
            this.groupName = groupName;
            this.resPath = resPath;
        }
    }

    public class AssetCheckManager : GlobalSystem<AssetCheckManager>
    {
        private List<DownloadInfo> _allTask = new List<DownloadInfo>();
        private int[] _activityArea = new[] { 888, 999 };
        
        StorageDecoration _storage;
        public StorageDecoration GetStorage()
        {
            var _storage = StorageManager.Instance.GetStorage<StorageDecoration>();
            if (_storage == null)
            {
                _storage = new StorageDecoration();
            }
            return _storage;
        }
        public Dictionary<string, string> GetNeedDownloadFiles()
        {
            //获取已解锁的区域Id
            var unlockedAreaIdList = new List<int>();
            var storage = GetStorage();
            foreach (var worldKv in storage.WorldMap)
            {
                var world = worldKv.Value;
                var worldId = worldKv.Key;
                if (world.State == (int)DecoWorld.Status.Unlock)
                {
                    foreach (var kv in world.AreasData)
                    {
                        var areaId = kv.Key;
                        var areaData = kv.Value;

                        if (areaData.State != (int)DecoArea.Status.Lock || worldId > 1)
                        {
                            unlockedAreaIdList.Add(areaId);
                        }
                    }
                }

                TableWorlds tableWorlds = DecorationConfigManager.Instance.WorldConfigs.Find(a=>a.id == worldId);
                if(tableWorlds == null)
                    continue;
                
                foreach (var id in tableWorlds.areaIds)
                {
                    StorageArea areaData = null;
                    if (world.AreasData.ContainsKey(id))
                        areaData = world.AreasData[id];

                    if(areaData == null)
                        continue;

                    if (areaData.State != (int)DecoArea.Status.Complete)
                        continue;
                    
                    TableAreas tableAreas = DecorationConfigManager.Instance.AreaConfigList.Find(a => a.id == id);
                    if (tableAreas == null || tableAreas.nextAreaId <= 0)
                        continue;
                    
                    if(!unlockedAreaIdList.Contains(tableAreas.nextAreaId))
                        unlockedAreaIdList.Add(tableAreas.nextAreaId);
                }
            }
            
            foreach (var areaId in _activityArea)
            {
                if(!unlockedAreaIdList.Contains(areaId))
                    unlockedAreaIdList.Add(areaId);
            }

            //按区域获取许下载的资源列表
            var needDownloadFiles = new Dictionary<string, string>();
            foreach (var areaId in unlockedAreaIdList)
            {
                var files = GetAreaResNeedToDownload(areaId);
                if (files == null) continue;
                if (files.Count > 0) needDownloadFiles.Merge(files);
            }

            return needDownloadFiles;
        }

        public Dictionary<string, string> GetMapResNeedToDownload(int localMapId)
        {
            var resGroupList = GetResRequired(localMapId);
            return ResDownloadFitter(resGroupList);
        }

        public Dictionary<string, string> GetAreaResNeedToDownload(int areaId)
        {
            var resGroupList = GetResRequired(areaId);
            if (resGroupList == null) return new Dictionary<string, string>();

            return ResDownloadFitter(resGroupList);
        }

        public void UpdateProgressFromDownloadInfoList(List<DownloadInfo> downloadTaskList, Action<float, string> onProgressUpdate)
        {
            var taskCount = downloadTaskList.Count;
            if (taskCount > 0)
            {
                var downloadedBytes = 0f;
                var totalBytes = 0f;
                var allSizeGot = true;
                for (int i = 0; i < taskCount; i++)
                {
                    if (downloadTaskList[i].downloadSize >= downloadTaskList[i].downloadedSize) //确保get httphead之后，才开始算进度
                    {
                        totalBytes += downloadTaskList[i].downloadSize;
                        downloadedBytes += downloadTaskList[i].downloadedSize;
                    }

                    if (downloadTaskList[i].downloadSize <= 0) allSizeGot = false;
                }

                if (totalBytes > 0)
                {
                    var downloadStr = string.Format("{0:N1}M", downloadedBytes / 1024 / 1024);
                    var totalStr = string.Format("{0:N1}M", totalBytes / 1024 / 1024);
                    var extralInfo = $"{downloadStr}/{totalStr}";
                    if (!allSizeGot) extralInfo = string.Empty; //LocalizationManager.Instance.GetLocalizedString("UI_loading_count");

                    onProgressUpdate?.Invoke(downloadedBytes / totalBytes, extralInfo);
                }
            }
        }

        public Dictionary<string, string> PathMd5Dictionary = new Dictionary<string, string>();

        public string GetMd5(string path)
        {
            if (PathMd5Dictionary.ContainsKey(path))
                return PathMd5Dictionary[path];
            string localFilePath = string.Format("{0}/{1}", FilePathTools.persistentDataPath_Platform, path);
            var localMd5 = AssetUtils.BuildFileMd5(localFilePath);
            PathMd5Dictionary.Add(path,localMd5);
            Debug.LogError("MD5更新 path="+path+" Md5="+localMd5);
            return PathMd5Dictionary[path];
        }
        public Dictionary<string, string> ResDownloadFitter(List<AssetGroup> resGroupList, bool checkMd5 = false)
        {
            var needDownloadFiles = new Dictionary<string, string>();

            foreach (var group in resGroupList)
            {
                var files = VersionManager.Instance.GetUpdateFilesDict(group.groupName, group.resPath);

                //是否存在
                if (!FilePathTools.IsFileExists(group.resPath))
                {
                    if (!files.ContainsKey(group.resPath))
                    {
                        if (!VersionManager.Instance.OfflineMode)
                        {
                            var remoteVersionFile = VersionManager.Instance.GetRemoteVersion();
                            var remoteName = remoteVersionFile.GetAssetBundleByKeyAndName(group.groupName, group.resPath);
                            var remoteMd5 = remoteVersionFile.GetAssetBundleMd5(group.groupName, remoteName);
                            if (!string.IsNullOrEmpty(remoteMd5))
                            {
                                files.Add(group.resPath, remoteMd5);
                            }
                            else
                            {
                                DebugUtil.LogError($"资源检查Md5为null, remoteName:{remoteName} group.resPath:{group.resPath}");
                            }
                        }
                    }
                }
                else if(checkMd5)
                {
                    // string localFilePath = string.Format("{0}/{1}", FilePathTools.persistentDataPath_Platform, group.resPath);
                    // var localMd5 = AssetUtils.BuildFileMd5(localFilePath);
                    var localMd5 = GetMd5(group.resPath);
                    if (!files.ContainsKey(group.resPath))
                    {
                        if (!VersionManager.Instance.OfflineMode)
                        {
                            var remoteVersionFile = VersionManager.Instance.GetRemoteVersion();
                            var remoteName =
                                remoteVersionFile.GetAssetBundleByKeyAndName(group.groupName, group.resPath);
                            var remoteMd5 = remoteVersionFile.GetAssetBundleMd5(group.groupName, remoteName);
                            if (!string.IsNullOrEmpty(remoteMd5))
                            {
                                if (localMd5 != remoteMd5)
                                {
                                    files.Add(group.resPath, remoteMd5);
                                }
                            }
                        }
                    }
                }

                if (files.Count > 0) needDownloadFiles.Merge(files);
            }

            return needDownloadFiles;
        }
        public List<DownloadInfo> DownloadAreaFiles(int areaId, Action<bool> onFinished)
        {
            var files = GetAreaResNeedToDownload(areaId);
            return DownloadFiles(files, onFinished);
        }
        public List<DownloadInfo> DownloadFiles(Dictionary<string, string> needDownloadFiles, Action<bool> onFinished)
        {
            var downloadSuccess = true;

            if (needDownloadFiles.Count > 0) // 去下载
            {
                var downloadedInTask = 0;
                foreach (var kv in needDownloadFiles)
                {
                    var fileName = kv.Key;
                    var fileMd5 = kv.Value;
                    if (_allTask.Find(t => t.fileMd5 == fileMd5) != null) continue;

                    downloadedInTask++;
                    var info = DownloadManager.Instance.DownloadInSeconds(fileName, fileMd5, (downloadinfo) =>
                    {
                        downloadedInTask--;

                        _allTask.Remove(downloadinfo);

                        if (downloadinfo.result == DownloadResult.Success)
                        {
                            VersionManager.Instance.RefreshRemoteToLocal(new List<string>(new[] { kv.Key }));
                        }
                        else // 超时，或者 重试3次后依然下载错误
                        {
                            if (downloadinfo.result != DownloadResult.ForceAbort) // 强制终止下载的不算
                            {
                                downloadSuccess = false;
                            }
                        }

                        if (downloadedInTask <= 0) //所有文件都处理完了
                        {
                            onFinished?.Invoke(downloadSuccess);
                            if (!downloadSuccess)
                            {
                                OnDownloadError(null);
                            }
                        }
                    });
                    _allTask.Add(info);
                    //DragonPlus.GameBIManager.SendGameEvent(BiEventMergeCooking.Types.GameEventType.GameEventDownloadStart, fileName, info.downloadSize.ToString());
                }

                if (downloadedInTask == 0) onFinished?.Invoke(downloadSuccess);
            }
            else //不需要更新任何文件
            {
                onFinished?.Invoke(downloadSuccess);
            }

            var taskList = _allTask.FindAll(info =>
            {
                //needDownloadFiles.Values.Contains(info.fileMd5
                foreach (var v in needDownloadFiles.Values)
                {
                    if (v.Contains(info.fileMd5)) return true;
                }
                return false;
            });

            return taskList;
        }

        public void OnDownloadError(BaseEvent e)
        {
            _allTask.Clear();
        }



















        public static string GetAreaBuildAtlasName(int areaId)
        {   
            return  $"Build{areaId}Atlas";
        }
        
        public static List<AssetGroup> GetResRequired(int areaId)
        {
            var worldConfig = DecorationConfigManager.Instance.WorldConfigs.Find(c => c.areaIds.Contains(areaId));
            if (worldConfig == null)
            {
                DebugUtil.LogError($"worldConfig == null , areaId = {areaId}");
                return null;
            }
            var worldId = worldConfig.id;

            var variantPostFix = ResourcesManager.Instance.IsSdVariant ? "Sd" : "Hd";

            var animator = $"Decoration/Animations.ab";
            var brush = $"Decoration/Worlds/World{worldId}/Brush.ab";
            var area = $"Decoration/Worlds/World{worldId}/Prefabs/Area.ab";
            var world = $"Decoration/Worlds/World{worldId}/Prefabs/Map.ab";
            var worldAtlas = $"Decoration/Worlds/World{worldId}/SpriteAtlas/World/{variantPostFix}.ab".ToLower();
            var areaAtlas =  $"Decoration/Worlds/World{worldId}/SpriteAtlas/Area{areaId}/{variantPostFix}.ab".ToLower();
            var video = $"Decoration/Worlds/World{worldId}/Prefabs/Building/{areaId}/video.ab".ToLower();
            var buildAtlas =  $"Decoration/Worlds/World{worldId}/SpriteAtlas/Build{areaId}/{variantPostFix}.ab".ToLower();
            
            var result = new List<AssetGroup>();
            if (worldId == 1)
            {
                result.Add(new AssetGroup("Decoration", animator.ToLower()));
                result.Add(new AssetGroup("Decoration", brush.ToLower()));
                result.Add(new AssetGroup("Decoration", area.ToLower()));
                result.Add(new AssetGroup("Decoration", worldAtlas.ToLower()));
                result.Add(new AssetGroup("Decoration", world.ToLower()));
                result.Add(new AssetGroup("Decoration", areaAtlas.ToLower()));
                result.Add(new AssetGroup("Decoration", buildAtlas.ToLower()));
            }
            else
            {
                result.Add(new AssetGroup("Decoration", area.ToLower()));
                result.Add(new AssetGroup("Decoration", world.ToLower()));
                result.Add(new AssetGroup("Decoration", areaAtlas.ToLower()));
                result.Add(new AssetGroup("Decoration", buildAtlas.ToLower()));
            }
            if(areaId == 106)
                result.Add(new AssetGroup("Building", video.ToLower()));
                
            foreach (var id in worldConfig.areaIds)
            {
                List<TableNodes> nodes = DecorationConfigManager.Instance.GetNodesByAreaID(id);
                if(nodes == null || nodes.Count == 0)
                    continue;

                if(id != areaId)
                    continue;
                
                foreach (var node in nodes)
                {
                    if (node.defaultItem > 0)
                    {
                        result.Add(new AssetGroup("Building", $"Decoration/Worlds/World{worldId}/Prefabs/Building/{id}/{node.defaultItem}.ab".ToLower()));
                        // if(AtlasConfigController.Instance.AtlasPathNodeList.Find(a=>a.AtlasName == node.defaultItem.ToString()) != null)
                        //     result.Add(new AssetGroup("BuildingAtlas", $"Decoration/Worlds/World{worldId}/SpriteAtlas/BuildingAtlas/{id}/{node.defaultItem}.ab".ToLower()));
                    }

                    if (node.itemList != null && node.itemList.Length >= 0 && node.itemList[0] > 0)
                    {
                        foreach (var itemId in node.itemList)
                        {
                            result.Add(new AssetGroup("Building", $"Decoration/Worlds/World{worldId}/Prefabs/Building/{id}/{itemId}.ab".ToLower()));
                            // if(AtlasConfigController.Instance.AtlasPathNodeList.Find(a=>a.AtlasName == itemId.ToString()) != null)
                            //     result.Add(new AssetGroup("BuildingAtlas", $"Decoration/Worlds/World{worldId}/SpriteAtlas/BuildingAtlas/{id}/{itemId}.ab".ToLower()));
                        }
                    }
                }
            }
            return result;
        }


        #region Building Storage
        public StorageWorld GetStorageWorld(int worldId)
        {
            var storage = GetStorage();
            return GetStorageWorld(storage.WorldMap, worldId);
        }

        public StorageWorld GetStorageWorld(StorageDictionary<int, StorageWorld> worldMap, int worldId)
        {
            StorageWorld world;
            if (!worldMap.TryGetValue(worldId, out world))
            {
                world = new StorageWorld();
                worldMap.Add(worldId, world);
            }
            return world;
        }

        public StorageArea GetStorageArea(StorageWorld world, int areaId)
        {
            StorageArea worldArea;
            if (!world.AreasData.TryGetValue(areaId, out worldArea))
            {
                worldArea = new StorageArea();
                world.AreasData.Add(areaId, worldArea);
            }
            return worldArea;
        }

        public StorageArea GetStorageArea(int worldId, int areaId)
        {
            return GetStorageArea(GetStorageWorld(worldId), areaId);
        }

        public StorageNode GetStorageNode(StorageStage stage, int nodeId)
        {
            if (!stage.NodesData.TryGetValue(nodeId, out var node))
            {
                var nodeConfig = DecorationConfigManager.Instance.GetNodeConfig(nodeId);
                if (nodeConfig != null)
                {
                    node = new StorageNode();
                    node.Id = nodeId;
                    node.CurrentItemId = nodeConfig.defaultItem;
                    stage.NodesData.Add(nodeId, node);
                }
                else
                {
                    DebugUtil.LogError("GetStorageNode Config Error: " + nodeId);
                }
            }
            return node;
        }

        public StorageNode GetStorageNode(int worldId, int areaId, int stageId, int nodeId)
        {
            return GetStorageNode(GetStorageStage(GetStorageArea(worldId, areaId), stageId), nodeId);
        }

        public StorageItem GetStorageItem(StorageNode nodeStorage, int itemId)
        {
            StorageItem build;
            if (!nodeStorage.ItemsData.TryGetValue(itemId, out build))
            {
                var itemConfig = DecorationConfigManager.Instance.GetItemConfig(itemId);
                if (itemConfig != null)
                {
                    build = new StorageItem();
                    nodeStorage.ItemsData.Add(itemId, build);
                }
                else
                {
                    DebugUtil.LogError("GetStorageItem Config Error: " + itemId);
                }
            }
            return build;
        }

        public StorageItem GetStorageItem(int worldId, int areaId, int stageId, int nodeId, int itemId)
        {
            return GetStorageItem(GetStorageNode(GetStorageStage(GetStorageArea(worldId, areaId), stageId), nodeId), itemId);
        }

        public StorageStage GetStorageStage(StorageArea nodeStorage, int stageId)
        {
            StorageStage stage;
            if (!nodeStorage.StagesData.TryGetValue(stageId, out stage))
            {
                var stageConfig = DecorationConfigManager.Instance.GetStageConfig(stageId);
                if (stageConfig != null)
                {
                    stage = new StorageStage();
                    nodeStorage.StagesData.Add(stageId, stage);
                }
                else
                {
                    DebugUtil.LogError("GetStorageItem Config Error: " + stageId);
                }
            }
            return stage;
        }
        public StorageStage GetStorageStage(int worldId, int areaId, int stageId)
        {
            return GetStorageStage(GetStorageArea(worldId, areaId), stageId);
        }


        /*     public StorageStory GetStorageStory(int storyId)
            {
                var dict = _storageGlobal.StoryModel.StoryDict;
                if (!dict.TryGetValue(storyId, out var story))
                {
                    story = new StorageStory();
                    dict.Add(storyId, story);
                }
                return story;
            }
        */
        public void SetFailedLevelCount(int levelId, int count)
        {
            //if (_storageCK.FailedLevelInfo.LevelId != levelId)
            //{
            //    _storageCK.FailedLevelInfo.PopedNotice = false;
            //}
            //_storageCK.FailedLevelInfo.LevelId = levelId;
            //_storageCK.FailedLevelInfo.FailedNumber = count;
        }

        public int GetFailedLevelCount(int levelId)
        {
            return 0;
            //return _storageCK.FailedLevelInfo.LevelId == levelId ? _storageCK.FailedLevelInfo.FailedNumber : 0;
        }

        public void SetFailedLevelNoticed(int levelId, bool noticed)
        {
            //_storageCK.FailedLevelInfo.LevelId = levelId;
            //_storageCK.FailedLevelInfo.PopedNotice = noticed;
        }

        public bool IsFailedLevelNoticed(int levelId)
        {
            return false;
            //return _storageCK.FailedLevelInfo.LevelId == levelId ? _storageCK.FailedLevelInfo.PopedNotice : false;
        }

        // 新手BI
        public bool IsSentGuideBI(int biEventType)
        {
            /* return _storageMain.BiEvents.ContainsKey(biEventType); */
            return false;
        }

        // public void SetSentGuideBI(int biEventType, bool sent)
        // {
        //     if (sent)
        //     {
        //         _storageGlobal.BiEvents[biEventType] = 1;
        //     }
        //     else if (_storageGlobal.BiEvents.ContainsKey(biEventType))
        //     {
        //         _storageGlobal.BiEvents.Remove(biEventType);
        //     }
        // }


        #endregion
    }

}