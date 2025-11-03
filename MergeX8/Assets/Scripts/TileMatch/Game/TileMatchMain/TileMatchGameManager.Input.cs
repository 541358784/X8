using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK;
using TileMatch.Event;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TileMatch.Game
{
    public partial class TileMatchGameManager
    {
        private Block.Block _selectBlock = null;
        private Vector3 _touchPosition;
        void UpdateInput()
        {
            if(UIRoot.Instance.IsInputDisable())
                return;
            
            if (Input.GetMouseButtonDown(0))
            {
                RestSelectBlock();
                
                _selectBlock = GetTouchBlock(Input.mousePosition);
                if (_selectBlock == null)
                    return;
                
                if (IsTouchUI(Input.mousePosition))
                {
                    _selectBlock = null;
                    return;
                }
                
                _touchPosition = Input.mousePosition;
                _selectBlock.OnPointerEnter();
            }

            if (Input.GetMouseButtonUp(0))
            {
                if(_selectBlock == null)
                    return;
                
                var block = GetTouchBlock(Input.mousePosition);
                float distance = Mathf.Max(Mathf.Abs(Input.mousePosition.x - _touchPosition.x), Mathf.Abs(Input.mousePosition.y - _touchPosition.y));
                if (distance > 50 && block != _selectBlock)
                {
                    RestSelectBlock();
                    return;
                }
                
                if (!_selectBlock.CanRemove())
                {
                    RestSelectBlock();
                    return;
                }

                if (IsFullCollectBanner())
                {
                    RestSelectBlock();
                    return;
                }

                if (_isGameFail || _failedBlocks.Count > 0)
                {
                    RestSelectBlock();
                    return;
                }
                List<Block.Block> handleBlocks = null;
                handleBlocks = _selectBlock.PreparationPreprocess();

                if (handleBlocks == null || handleBlocks.Count == 0)
                {
                    _selectBlock = null;
                    return;
                }
                
                AudioManager.Instance.PlaySound(20+TileMatchRoot.AudioDistance);
               ExecutePreprocess(handleBlocks);
               PreparationPreprocess(handleBlocks);
               
                _selectBlock.OnPointerExit(true, () =>
                {
                    AddCollectBanner(handleBlocks);
                });

                
                var tempBlock = _selectBlock;
                TileMatchEventManager.Instance.SendEvent(GameEventConst.GameEvent_UpdatePropStatus);
                TileMatchEventManager.Instance.SendEvent(GameEventConst.GameEvent_TouchBlock, tempBlock);
                
                _selectBlock = null;
            }
        }

        private void RestSelectBlock()
        {
            if(_selectBlock == null)
                return;
            
            _selectBlock.OnPointerExit(false, null);
            _selectBlock = null;
        }
        
        private Block.Block GetTouchBlock(Vector3 mousePosition)
        {
            var block = GetTouchBlock(_sceneCamera, mousePosition, 1 << LayerMask.NameToLayer("TileMatch"));
            if (block != null)
                return block;

            return GetTouchBlock(TileMatchRoot.Instance._matchGuideCamera, mousePosition, 1 << LayerMask.NameToLayer("TileMatch_Guide"));
        }

        private Block.Block GetTouchBlock(Camera camera, Vector3 mousePosition, int layerMask)
        {
            if (camera == null)
                return null;
            
            if (!camera.gameObject.activeSelf)
                return null;
            
            var worldPoint = camera.ScreenToWorldPoint(mousePosition);
            var collider = Physics2D.OverlapPoint(new Vector2(worldPoint.x, worldPoint.y), layerMask);
            if (collider == null)
                return null;

            return GetBlock(collider);
        }

        private void InputDestroy()
        {
            
        }
        
        private void InputClean()
        {
            _selectBlock = null;
        }

        private bool IsTouchUI(Vector2 inputPosition)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = inputPosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            if (results.Count == 0)
                return false;
            
            if (results[0].gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;

            return false;
        }

    }
}