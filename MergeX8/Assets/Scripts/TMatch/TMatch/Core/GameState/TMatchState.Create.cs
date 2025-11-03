using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus.Config.TMatch;
using UnityEngine;

namespace TMatch
{


    public class TMatchCreateStateParam : TMatchStateParam
    {
        public FsmParamTMatch fsmParamTMatch;
        public Dictionary<int, int> layoutResult;
    }

    public class TMatchCreateState : TMatchBaseState
    {
        TMatchStateType TMatchBaseState.Type => TMatchStateType.Create;

        public async Task Enter(TMatchStateParam param)
        {
            TMatchCreateStateParam stateParam = param as TMatchCreateStateParam;
            Debug.Log(
                $"[PlayInfo] LevelID : {stateParam.fsmParamTMatch.level}, LayoutID : {stateParam.fsmParamTMatch.layoutCfg.id}");
            foreach (var p in stateParam.layoutResult)
            {
                int cnt = p.Value;
                while (cnt-- > 0)
                {
                    TMatchItemSystem.Instance.Create(p.Key);
                }
            }

            List<TMatchBaseItem> items = TMatchItemSystem.Instance.Items;
            EventDispatcher.Instance.DispatchEvent(EventEnum.TMATCH_GAME_CREATE);
            bool needCorrectItemPosition = true;

            // 固定第一关物品位置
            if (stateParam.fsmParamTMatch.GameType == TMGameType.Normal)
            {
                if (stateParam.fsmParamTMatch.level == 1)
                {
                    if (items.Count == 9)
                    {
                        items[0].GameObject.transform.position = new Vector3(-1.810033f, 0.9712707f, 1.690079f);
                        items[0].GameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
                        items[1].GameObject.transform.position = new Vector3(0.0f, 0.9712707f, 1.690079f);
                        items[1].GameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
                        items[2].GameObject.transform.position = new Vector3(1.810033f, 0.9712707f, 1.690079f);
                        items[2].GameObject.transform.rotation = Quaternion.Euler(Vector3.zero);

                        items[3].GameObject.transform.position = new Vector3(-1.869996f, 0.8783228f, -0.5499966f);
                        items[3].GameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
                        items[4].GameObject.transform.position = new Vector3(0.0f, 0.8783228f, -0.5499966f);
                        items[4].GameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
                        items[5].GameObject.transform.position = new Vector3(1.869996f, 0.8783228f, -0.5499966f);
                        items[5].GameObject.transform.rotation = Quaternion.Euler(Vector3.zero);

                        items[6].GameObject.transform.position = new Vector3(-1.928859f, 0.6821452f, -2.669963f);
                        items[6].GameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
                        items[7].GameObject.transform.position = new Vector3(0.0f, 0.6821452f, -2.669963f);
                        items[7].GameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
                        items[8].GameObject.transform.position = new Vector3(1.928859f, 0.6821452f, -2.669963f);
                        items[8].GameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
                    }
                }   
            }

            if (needCorrectItemPosition) await CorrectItemPosition();
            EventDispatcher.Instance.DispatchEvent(new TMatchGameChangeStateEvent(TMatchStateType.Play,
                new TMatchPlayStateParam() {fsmParamTMatch = stateParam.fsmParamTMatch}));

            BoostGuide(stateParam);
        }

        private async void BoostGuide(TMatchCreateStateParam stateParam)
        {
            await Task.Yield();
            await Task.Yield();
            await Task.Yield();

            //引导使用扫把道具
            // if (stateParam.fsmParamTMatch.level == TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterUnlock2)
            // {
            //     if (!GuideSubSystem.Instance.IsFinished("GUIDE_112"))
            //     {
            //         List<TMatchBaseItem> untargetItems = new List<TMatchBaseItem>();
            //         for (int i = 0; i < TMatchItemSystem.Instance.Items.Count; i++)
            //         {
            //             TMatchBaseItem tempItem = TMatchItemSystem.Instance.Items[i];
            //             if(UIViewSystem.Instance.Get<UITMatchMainController>().Tasks.Find(x => x.itemCfg.Id == tempItem.Id) != null) continue;
            //             if(untargetItems.Find(x=>x.Id == tempItem.Id) != null) continue;
            //             untargetItems.Add(tempItem);
            //         }
            //         List<TMatchBaseItem> collectedItems = new List<TMatchBaseItem>();
            //         for (int i = 0; i < untargetItems.Count && i < 3; i++)
            //         {
            //             collectedItems.Add(untargetItems[i]);
            //         }
            //         TMatchCollectorSystem.Instance.Collect(collectedItems, true, false, 0.2f, Ease.OutQuad);
            //     }
            // }
        }

        public void Update(float deltaTime)
        {

        }

        public async Task Exit()
        {

        }

        private async Task CorrectItemPosition()
        {
            List<TMatchBaseItem> items = TMatchItemSystem.Instance.Items;
            for (int i = 0; i < items.Count; i++)
            {
                items[i].HideRender();
                items[i].DisbaleGravity();
            }

            Physics.autoSimulation = false;
            int cnt = 10;
            do
            {
                for (int i = 0; i < 10; i++)
                {
                    Physics.Simulate(0.016f);
                }

                await Task.Yield();
            } while ((--cnt) > 0);

            for (int i = 0; i < items.Count; i++)
            {
                items[i].ShowRender();
                items[i].EnableGravity();
            }

            Physics.autoSimulation = true;
        }
    }
}