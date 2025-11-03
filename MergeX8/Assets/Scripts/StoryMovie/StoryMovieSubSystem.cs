using System;
using System.Collections.Generic;
using Decoration;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay.UI.Capybara;
using StoryMovie;
using UnityEngine;

public enum StoryMovieTrigger
{
    //首次进入游戏
    FirstEnterGame = 1,
    //剧情动画结束
    StoryEnd = 2,
    //剧情动画结束
    CGEnd = 3,
    //挂点装修结束
    FinishNode = 4,
    //挂点开始装修
    StartNode = 5,
    //主动触发
    Initiative = 6,
}

public class StoryMovieSubSystem : Manager<StoryMovieSubSystem>
{
    private bool _isPlayMovie = false;
    private List<ActionBase> _actions = new List<ActionBase>();
    private Action<bool> _onFinish = null;
    private Action<bool> _onStoryEnd;
    private TableStoryMovie _firstConfig;
    
    public bool IsShowing
    {
        get => _isPlayMovie;
    }
    
    private StorageStoryMovie _storageMovie;
    public StorageStoryMovie StorageMovie
    {
        get
        {
            if (_storageMovie == null)
                _storageMovie = StorageManager.Instance.GetStorage<StorageHome>().StoryMovieData;

            return _storageMovie;
        }
    }

    public bool Trigger(StoryMovieTrigger position, string param = null, Action<bool> onFinish = null, Action<bool> onStoryEnd = null)
    {
        if (_isPlayMovie)
        {
            onFinish?.Invoke(false);
            return false;
        }

        if (position == StoryMovieTrigger.FinishNode && CapybaraManager.Instance.IsOpenCapybara())
        {
            if (param == "801007" || param == "101017")
                param = "capybara" + param;
        }
        
        var config = GlobalConfigManager.Instance.GetTableStoryMovie((int)position, param);

        if (config == null)
        {
            onFinish?.Invoke(false);
            return false;
        }

        if (IsFinished(config.id))
        {
            onFinish?.Invoke(false);
            return false;
        }

        if (config.checkDecoNode)
        {
            for (int i = 0; i < config.triggerParam.Length; i++)
            {
                var node = DecoManager.Instance.FindNode(int.Parse(config.triggerParam[i]));
                if (node == null)
                    return false;
        
                if (!node.IsOwned)
                    return false;
            }
        }

        _onStoryEnd = onStoryEnd;
        _onFinish = onFinish;
        StartMovie(config);
        return true;
    }
    
    public bool IsFinished(int movieId)
    {
        var finishIds = StorageMovie.FinishedId;
        if (finishIds == null)
            return true;

        return finishIds.Contains(movieId);
    }

    private void FinishedMovie(int movieId)
    {
        if(IsFinished(movieId))
            return;
        
        StorageMovie.FinishedId.Add(movieId);
    }

    public void Update()
    {
        if(!_isPlayMovie)
            return;

        int stopNum = 0;
        _actions.ForEach(a=>
        {
            a.OnUpdate();
            if (a.IsStop())
                stopNum++;
        });

        if (stopNum < _actions.Count)
            return;
        
        StopMovie();
    }

    public void StopMovie()
    {
        if(!_isPlayMovie)
            return;
        
        if (_actions != null && _actions.Count > 0)
        {
            _actions.ForEach(a=>
            {
                a.OnStop();
                a.OnExit();
            });
        }

        if (_firstConfig != null)
        {
            if (_firstConfig.autoSave)
            {
                FinishedMovie(_firstConfig.id);
                if (_firstConfig.saveIds != null && _firstConfig.saveIds.Length > 0)
                {
                    foreach (var id in _firstConfig.saveIds)
                    {
                        FinishedMovie(id);
                    }
                }
            }
            var storyEnd = _onStoryEnd;
            if (_firstConfig.triggerPosition == (int)StoryMovieTrigger.FirstEnterGame ||
                !StorySubSystem.Instance.Trigger(StoryTrigger.StoryMovieEnd, _firstConfig.id.ToString(), storyEnd))
            {
                storyEnd?.Invoke(true);
            }
        }
        else
        {
            _onStoryEnd?.Invoke(false);
        }

        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.STORY_MOVIE_FINISH, _firstConfig.id.ToString());
        
        _onStoryEnd = null;
        UIRoot.Instance.EnableEventSystem = true;
        _firstConfig = null;
        _isPlayMovie = false;
        _onFinish?.Invoke(true);
        _onFinish = null;
        _actions.Clear();
    }

    private void StartMovie(TableStoryMovie config)
    {
        _firstConfig = config;
        _isPlayMovie = true;
        _actions.Clear();
        UIRoot.Instance.EnableEventSystem = false;
        
        while (config != null)
        {
            ActionBase actionBase = ActionFactory.CreateAction(config);
            if(actionBase != null)
                _actions.Add(actionBase);
            
            if(config.next_id <= 0)
                break;
            
            config = GlobalConfigManager.Instance.GetTableMovie(config.next_id);
        }
    }
    
    public void DEBUG_StartStory(int storyId)
    {
        var config = GlobalConfigManager.Instance.GetTableMovie(storyId);

        if (config != null)
            StartMovie(config);
    }
}