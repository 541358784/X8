using System;
using System.Collections.Generic;
using DG.Tweening;
using LayoutData;
using UnityEngine;

namespace TileMatch.Game
{
    public interface ILoadView
    {
        public void LoadView(Transform parent);
    }

    public interface IObstacleHandle
    {
        public void LoadObstacle(Transform parent);
        public void UnLoadObstacle();
        public void HideObstacle();
        public void ShowObstacle();
    }
    
    public interface IInitData
    {
        public void InitData(Layer.Layer layer, LayerBlock blockData);
    }

    public interface IShake
    {
        public void Shake();
        public Tweener ShakeTweener();
    }

    public interface IBlockAnim
    {
        public void BreakAnim(Action action = null);
    }
    
    public interface IDestroy
    {
        public void Destroy();
    }

    public interface IRemoveAnim
    {
        public void RemoveAnim(Action action);
    }
    
    public interface IOnPointerHandle
    {
        public void OnPointerEnter();
        public void OnPointerExit(bool isRemove, Action action);
    }

    public interface IBlockHandle
    {
        public void BeforeRemoveBlock(List<Block.Block> blocks, bool isRefresh = true);
        public void AfterRemoveBlock(List<Block.Block> blocks);
        public void BeforeRecoverBlock(List<Block.Block> blocks, Action action);
        public void AfterRecoverBlock(List<Block.Block> blocks);
        public void DisappearBlock(List<Block.Block> blocks, bool isMagic = false);
    }

    public interface IPlayMethod : IDestroy, IBlockHandle, IMagicHandle
    {
        public void Init(params object[] param);

        public void Start();
        public void Pause();
        public void Recover();
        public void Update();

        public void CleanRecord();
        public int GetTime();
        public int GetLeftTime();
        public void AddTime(int time);

        public void Hide();
        public void Show();

        public void StartShuffle();
        public void StopShuffle();
        public bool IsLock(Block.Block block);
    }

    public interface IShuffle
    {
        public void StartShuffle();

        public void StopShuffle();

        public bool CanShuffle();
        
        public void Shuffle(Block.Block block);

        public void ShuffleRefresh();
    }

    public interface ICollect
    {
        public bool IsCollect { get; set; }
    }

    public interface IMagicHandle
    {
        public void Magic_BeforeRemoveBlock(List<Block.Block> blocks);
        public void Magic_AfterRemoveBlock(List<Block.Block> blocks);
    }

    public interface IMagic
    {
        public void StartMagic();
    }

    public interface IGameCheck
    {
        public FailTypeEnum CheckFailure(List<Block.Block> blocks);
    }

    public interface IPreprocess
    {
        public List<Block.Block> PreparationPreprocess();
        public void ExecutePreprocess(List<Block.Block> blocks);
    }
}