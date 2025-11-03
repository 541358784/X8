using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.JungleAdventure;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Activity.JungleAdventure.Controller
{
    class RewardItem
    {
        public Transform _root;
        public SpriteRenderer _spriteOpen;
        public SpriteRenderer _spriteClose;
        public BoxCollider2D _collider;
        public int _pathIndex;
        public int _stage;  // 添加stage标识
        public bool _isMiniReward;  // 标识是否是小奖励
        public TableJungleAdventureConfig _config;
        
        public void Init(Transform root, int stage, int index, bool isMiniReward = false)
        {
            _root = root;
            _stage = stage;
            _pathIndex = index;
            _isMiniReward = isMiniReward;
            _spriteOpen = _root.transform.Find("Open").GetComponent<SpriteRenderer>();
            _spriteClose = _root.transform.Find("Close").GetComponent<SpriteRenderer>();
            
            // 确保有Collider2D组件
            _collider = _root.gameObject.GetComponent<BoxCollider2D>();
            if (_collider == null)
            {
                _collider = _root.gameObject.AddComponent<BoxCollider2D>();
                _collider.size = Vector2.one*1.2f;
            }
        }
    }

    public partial class UIJungleAdventureMainController
    {
        private List<RewardItem> _rewardItems = new List<RewardItem>();
        private List<List<RewardItem>> _miniRewardItems = new List<List<RewardItem>>();
        private RewardItem _rewardItem;

        private void Awake_Reward()
        {
            for (int i = 1; i <= 12; i++)
            {
                RewardItem item = new RewardItem();
                item.Init(transform.Find($"Root/BGGroup/Reward/{i}"), i - 1, -1);
                _rewardItems.Add(item);
            }
        }

        private void InitMiniReward()
        {
            foreach (var config in JungleAdventureConfigManager.Instance.GetConfigs())
            {
                _miniRewardItems.Add(new List<RewardItem>());

                var points = GetPathPoint(config.Id);
                var length = GetPathLength(config.Id);
                int count = config.SmallRewardIds.Count;
                float step = length / (count);

                for (int i = 0; i < count; i++)
                {
                    float stepLength = step + step * i;
                    int index = 0;
                    var position = GetLocalPositionByLength(points, stepLength, ref index);

                    RewardItem rewardItem = new RewardItem();
                    var item = Instantiate(_rewardItems[0]._root, _rewardItems[0]._root.parent);
                    rewardItem.Init(item, config.Id, index, true);
                    item.transform.localPosition = position;
                    rewardItem._pathIndex = index;
                    
                    rewardItem._spriteOpen.sprite = UserData.GetResourceIcon(config.SmallRewardIds[i], UserData.ResourceSubType.Big);
                    rewardItem._spriteClose.gameObject.SetActive(false);
                    rewardItem._spriteOpen.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    
                    _miniRewardItems.Last().Add(rewardItem);
                }
            }
        }

        private void InitRewardStatus()
        {
            for (var i = 0; i < _rewardItems.Count; i++)
            {
                string key = GetFinalRewardKey(_rewardItems[i]._stage);

                bool isGet = JungleAdventureModel.Instance.JungleAdventure.GetRewardState.ContainsKey(key);
                _rewardItems[i]._spriteOpen.gameObject.SetActive(isGet);
                _rewardItems[i]._spriteClose.gameObject.SetActive(!isGet);
            }

            for (var i = 0; i < _miniRewardItems.Count; i++)
            {
                for (var j = 0; j < _miniRewardItems[i].Count; j++)
                {
                    string key = GetStageMiniRewardKey(i, j);

                    _miniRewardItems[i][j]._root.gameObject.SetActive(!JungleAdventureModel.Instance.JungleAdventure.GetRewardState.ContainsKey(key));
                }
            }
        }

        private void UpdateReward(int stage, int index, int pointCount)
        {
            if (index >= pointCount - 1)
            {
                var rewardItem = GetRewardItem(stage);

                SaveRewardStatus(stage, -1, rewardItem);
                
                string key = GetFinalRewardKey(stage);
                bool isGet = JungleAdventureModel.Instance.JungleAdventure.GetRewardState.ContainsKey(key);
                _rewardItems[stage]._spriteOpen.gameObject.SetActive(isGet);
                _rewardItems[stage]._spriteClose.gameObject.SetActive(!isGet);
            }

            UpdateMiniReward(stage, index);
        }

        private void UpdateMiniReward(int stage, int index)
        {
            var miniRewards = GetMiniRewardItems(stage);
            for (var i = miniRewards.Count - 1; i >= 0; i--)
            {
                if (index >= miniRewards[i]._pathIndex)
                    SaveRewardStatus(stage, i, miniRewards[i]);

                string key = GetStageMiniRewardKey(stage, i);
                
                if(!miniRewards[i]._root.gameObject.activeSelf)
                    continue;

                bool isShow = !JungleAdventureModel.Instance.JungleAdventure.GetRewardState.ContainsKey(key);
                if(isShow)
                    continue;

                miniRewards[i]._root.DOLocalMoveY(miniRewards[i]._root.localPosition.y+0.5f, 1f).SetEase(Ease.Linear);
                miniRewards[i]._spriteOpen.DOFade(0, 1.5f).OnComplete(() =>
                {
                    miniRewards[i]._root.gameObject.SetActive(isShow);
                }).SetEase(Ease.Linear);
            }
        }

        private RewardItem GetRewardItem(int stage)
        {
            if (stage >= _rewardItems.Count)
                return _rewardItems.Last();

            return _rewardItems[stage];
        }

        private List<RewardItem> GetMiniRewardItems(int stage)
        {
            if (stage >= _miniRewardItems.Count)
                return _miniRewardItems.Last();

            return _miniRewardItems[stage];
        }

        private string GetFinalRewardKey(int stage)
        {
            return "Final_" + stage;
        }

        private string GetStageMiniRewardKey(int stage, int index)
        {
            return "Mini_" + stage + "_" + index;
        }

        private void SaveRewardStatus(int stage, int index, RewardItem item)
        {
            string key = "";
            if (index < 0)
            {
                key = GetFinalRewardKey(stage);
            }
            else
            {
                key = GetStageMiniRewardKey(stage, index);
            }

            if (JungleAdventureModel.Instance.JungleAdventure.GetRewardState.ContainsKey(key))
                return;

            JungleAdventureModel.Instance.JungleAdventure.GetRewardState.Add(key, 1);
            var config = JungleAdventureConfigManager.Instance.GetConfigs().Find(a => a.Id == item._stage);
            if (config == null)
                return;
            
            if (item._isMiniReward)
            {
                int reward = config.SmallRewardIds[index];
                int count = config.SmallRewardNums[index];
                
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventJungleAdventureReward, config.Id.ToString(), "0", index.ToString());

                UserData.Instance.AddRes(reward, count, new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.JunbleAdventureGet});
                
                EventDispatcher.Instance.DispatchEvent(EventEnum.UserDataUpdate);
            }
            else
            {
                List<ResData> resDatas = new List<ResData>();
                for(int i = 0;i < config.RewardIds.Count; i++)
                {
                    ResData resData = new ResData(config.RewardIds[i], config.RewardNums[i]);
                    resDatas.Add(resData);
                    
                    if (!UserData.Instance.IsResource(resData.id))
                    {
                        var itemConfig = GameConfigManager.Instance.GetItemConfig(resData.id);
                        if (itemConfig != null)
                        {
                            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                            {
                                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonJungleAdventureGet,
                                itemAId = resData.id,
                                isChange = true,
                            });
                        }
                    }
                }
                CommonRewardManager.Instance.PopCommonReward(resDatas, CurrencyGroupManager.Instance.currencyController,true);
                
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventJungleAdventureReward, config.Id.ToString(), "1", "0");

            }
        }

        private RewardItem GetTouchItem(Vector3 position)
        {
            Vector2 clickScreenPosition = position;

            // 2. 将屏幕坐标转换为 RawImage 的 UV 坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rawImage.rectTransform, 
                clickScreenPosition, 
                UIRoot.Instance.mUICamera, 
                out Vector2 localPoint
            );

            // 将局部坐标归一化为 UV 坐标 [0, 1]
            Vector2 uv = Rect.PointToNormalized(_rawImage.rectTransform.rect, localPoint);

            // 3. 将 UV 坐标映射到 RenderTexture 的像素坐标
            Vector2 renderTexturePixel = new Vector2(
                uv.x * referenceWidth,
                uv.y * referenceHeight
            );

            // 4. 将 RenderTexture 的像素坐标映射到相机的视图坐标
            Vector3 worldPoint = _camera.ScreenToWorldPoint(
                new Vector3(renderTexturePixel.x, renderTexturePixel.y, 0)
            );
            
            var collider = Physics2D.OverlapPoint(new Vector2(worldPoint.x, worldPoint.y), 1 << LayerMask.NameToLayer("Zuma"));
            if (collider == null)
                return null;

            var item = _rewardItems.Find(a => a._collider == collider);
            if (item != null)
                return item;
            
            foreach (var miniRewardItem in _miniRewardItems)
            {
                var miniItem = miniRewardItem.Find(a => a._collider == collider);
                if (miniItem != null)
                    return miniItem;
            }

            return null;
        }

        private bool IsTouchUI(Vector3 inputPosition)
        {
            if (EventSystem.current == null)
                return false;
            
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = inputPosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            if (results.Count == 0)
                return false;

            if (results[0].gameObject == _dragArea.gameObject)
                return false;
            
            if (results[0].gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;

            return false;
        }
        
        private void UpdateRewardInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _rewardItem = null;
                
                if(IsTouchUI(Input.mousePosition))
                    return;
                
                _rewardItem = GetTouchItem(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_rewardItem == null)
                    return;

                if(IsTouchUI(Input.mousePosition))
                    return;
                
                var item = GetTouchItem(Input.mousePosition);
                if (_rewardItem != item || _rewardItem._isMiniReward)
                    return;

                if (_isDragging || _currentVelocity.magnitude > 1.5f)
                    return;

                UIManager.Instance.OpenWindow(UINameConst.UIPopupJungleAdventureReward, _rewardItem._stage);
                _rewardItem = null;
            }
        }
    }
}