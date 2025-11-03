
    using System;
    using System.Collections.Generic;
    using Activity.TreasureMap;
    using DragonPlus;
    using UnityEngine;
    public class TreasureMapItem : MonoBehaviour
    {
        private List<MapChip> _chips = new List<MapChip>();
        private Animator _animator;
        private void Awake()
        {
            
        }
        
        public void Init(TreasureMapConfig config)
        {
            for (int i = 1; i <= config.ChipCount; i++)
            {
                var item=transform.Find((i).ToString());
                var chip=new MapChip();
                chip.Id = i;
                chip._animator = item.gameObject.GetComponent<Animator>();
                _chips.Add(chip);
            }

            _animator = GetComponent<Animator>();
            SetChipStatus();
        }

        public void SetChipStatus()
        {
            for (int i = 0; i < _chips.Count; i++)
            {
                bool isCollect = TreasureMapModel.Instance.IsCollectChip(_chips[i].Id);
                if (isCollect && TreasureMapModel.Instance.TreasureMap.NewChip!=_chips[i].Id)
                {
                    _chips[i]._animator.Play("Unlock");
                }
                else
                {
                    _chips[i]._animator.Play("Lock");

                }

              
            }
        }
        
        public void PlayUnlock(int chipId,Action cb)
        {
            StartCoroutine(CommonUtils.PlayAnimation(_chips[chipId - 1]._animator, "Unlocking", "", () =>
            {
                cb?.Invoke();
            }));
            
        }

        public void PlayFinish(Action cb)
        {
            for (int i = 0; i < _chips.Count; i++)
            {
                _chips[i]._animator.enabled = false;
            }
            AudioManager.Instance.PlaySoundById(147);

            StartCoroutine(CommonUtils.PlayAnimation(_animator, "Finish", "", () =>
            {
                cb?.Invoke();
            }));

        }
    }

    public class MapChip
    {
        public int Id;
        public Animator _animator;
    }
