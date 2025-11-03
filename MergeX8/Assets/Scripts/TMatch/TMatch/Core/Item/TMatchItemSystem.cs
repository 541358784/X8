using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace TMatch
{


    public class TMatchItemSystem : GlobalSystem<TMatchItemSystem>, IInitable, ILateUpdatable
    {
        private List<TMatchBaseItem> items = new List<TMatchBaseItem>();
        private List<TMatchBaseItem> collectItems = new List<TMatchBaseItem>();

        public List<TMatchBaseItem> Items => items;

        public float keepPositionDeltaTime;

        public void Init()
        {

        }

        public void Release()
        {
            foreach (var p in items)
            {
                p.UnLoad();
                p.Destory();
            }

            items.Clear();
            foreach (var p in collectItems)
            {
                p.UnLoad();
                p.Destory();
            }

            collectItems.Clear();
        }

        public TMatchBaseItem Create(int itemId, int layout = 0)
        {
            TMatchBaseItem item = new TMatchBaseItem(itemId);
            item.Load();
            item.RandomPos(TMatchEnvSystem.Instance.SceneRandomPosMin, TMatchEnvSystem.Instance.SceneRandomPosMax,
                layout);
            item.OperState = TMatchBaseItem.OperStateType.Scene;
            items.Add(item);
            return item;
        }

        public void LateUpdate(float deltaTime)
        {
            keepPositionDeltaTime += deltaTime;
            if (keepPositionDeltaTime < 3.0f) return;
            keepPositionDeltaTime = 0.0f;

            foreach (var p in items) p.KeepPositionInSceneBound();
        }

        public TMatchBaseItem FindItem(Rigidbody rigidbody)
        {
            foreach (var p in items)
            {
                if (p.GameObject.GetComponent<Rigidbody>() == rigidbody)
                {
                    return p;
                }
            }

            return null;
        }

        public void CollectItem(TMatchBaseItem item)
        {
            item.GameObject.SetActive(false);
            collectItems.Add(item);
            items.Remove(item);
        }

        public void Explode(Vector3 position)
        {
            foreach (var p in items)
            {
                p.Explode(position, 450.0f, 1.0f);
            }
        }
    }
}