using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Decoration;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

    public class SlotMachineSingleColumnWheel : SingleColumnWheel
    {
        private int ReelIndex;
        public SlotMachineSingleColumnWheel(int reelIndex,Transform inTransform, float wheelHeight, int inRowCount,
            IElementProvider inElementProvider, int inStartIndex, bool inIsHorizontal = false,
            Action<GameObject> inRecycleFunc = null)
            : base(inTransform,wheelHeight,inRowCount,inElementProvider,inStartIndex,inIsHorizontal,inRecycleFunc)
        {
            ReelIndex = reelIndex;
        }
        private List<float> paramList = new List<float>() {1,-2,-7,-7,-3};
        protected override float GetSlowStateByProcess(float process)
        {
            var x = process;
            float y = 0;
            for (int i = 0; i < paramList.Count; i++)
            {
                y += paramList[i] * (float) Math.Pow(x, i);
            }
            return y;
        }
        const float MaxElementScale = 1f;
        const float MinElementScale = 0.8f;
        private const float MaxMoveX = 0;
        private const float MinMoveX = -10;
        public override void ActionAfterUpdate()
        {
            base.ActionAfterUpdate();
            foreach (var container in _elementContainer)
            {
                var totalY = container.transform.localPosition.y + _rollTransform.localPosition.y;
                totalY -= stepSize/2;
                var absY = Math.Abs(totalY);
                var progress = absY / stepSize;
                var scale = MaxElementScale - (MaxElementScale - MinElementScale) * progress;
                container.transform.localScale = new Vector3(scale, scale, 1);
                var reelFixIndex = ReelIndex - 1;
                if (reelFixIndex == 0)
                {
                    var tempVec3 = container.transform.localPosition;
                    tempVec3.x = 0;
                    container.transform.localPosition = tempVec3;
                }
                else
                {
                    var localX = MaxMoveX - (MaxMoveX - MinMoveX) * progress;
                    var tempVec3 = container.transform.localPosition;
                    tempVec3.x = reelFixIndex>0?localX:-localX;
                    container.transform.localPosition = tempVec3;
                }
            }
        }
    }
    public class SlotMachineRoll:MonoBehaviour,IElementProvider
    {

        private bool enableClick = false;

        private SlotMachineSingleColumnWheel _wheel;

        private Transform[] _elementArray;
        private List<int> ElementIndexList => SlotMachineModel.Instance.GlobalConfig.ElementList;
        private List<int> wheelItems= new List<int>();
        private int spinResult;
        private SimpleRollUpdaterEasingConfig spinConfig;
        public void Init(int reelIndex,int result = -1)
        {
            spinConfig = new SimpleRollUpdaterEasingConfig()
            {
                spinSpeed = 25f,
                speedUpDuration = 0.5f,
                slowDownDuration = 0.5f,
                leastSpinDuration = 0,
                slowDownStepCount = 3,
                overShootAmount = 0.5f,
            };
            for (var i = 0; i <ElementIndexList.Count; i++)
            {
                GamePool.ObjectPoolManager.Instance.CreatePool(ElementAssetPath+ElementIndexList[i], 10,
                    GamePool.ObjectPoolDelegate.CreateGameItem);   
            }
            CreateWheelItems();
            var startIndex = 0;
            if (result > 0)
            {
                for (var i = 0; i < wheelItems.Count; i++)
                {
                    if (wheelItems[i] == result)
                    {
                        startIndex = i>0?i-1:wheelItems.Count-1;
                        break;
                    }
                }
            }
            _wheel = new SlotMachineSingleColumnWheel(reelIndex,transform, 300f, 2,
                this, startIndex,false,RecycleElement);
            _wheel.ForeUpdateElementContainer(_wheel.GetCurrentIndex());
        }

        public void CreateWheelItems()
        {
            wheelItems.Clear();
            var lastValue = 0;
            for (var n = 0; n < 1; n++)
            {
                var randomList = new List<int>(ElementIndexList);
                for (var i = 0; i < ElementIndexList.Count; i++)
                {
                    var tempRandomList = new List<int>(randomList);
                    tempRandomList.Remove(lastValue);
                    var randomNum = tempRandomList[Random.Range(0, tempRandomList.Count)];
                    randomList.Remove(randomNum);
                    wheelItems.Add(randomNum);
                    lastValue = randomNum;
                }
            }
        }
        public int GetReelMaxLength()
        {
            return wheelItems.Count;
        }

        private const string ElementAssetPath = "Prefabs/Activity/SlotMachine/Element_";
        public void RecycleElement(GameObject recycleObject)
        {
            var assetName = recycleObject.name;
            GamePool.ObjectPoolManager.Instance.DeSpawn(ElementAssetPath+assetName,recycleObject);
        }
        public GameObject GetElement(int index)
        {
            var element = GamePool.ObjectPoolManager.Instance.Spawn(ElementAssetPath+wheelItems[index]);
            element.name = wheelItems[index].ToString();
            element.transform.localPosition = Vector3.zero;
            return element;
        }

        public int GetElementMaxHeight()
        {
            return 1;
        }

        public int ComputeReelStopIndex(int currentIndex, int slowDownStepCount)
        {
            var stopIndex = (currentIndex - slowDownStepCount + wheelItems.Count) % wheelItems.Count;
            int totalStopIndex = -1;
            if (wheelItems[stopIndex] != spinResult)
            {
                for (var i = stopIndex-1; i >= 0; i--)
                {
                    if (wheelItems[i] == spinResult)
                    {
                        // totalStopIndex = i;
                        // var temp = wheelItems[i];
                        // wheelItems[i] = wheelItems[stopIndex];
                        // wheelItems[stopIndex] = temp;
                        (wheelItems[i], wheelItems[stopIndex]) = (wheelItems[stopIndex], wheelItems[i]);
                        totalStopIndex = stopIndex;
                        break;
                    }
                }

                if (totalStopIndex < 0)
                {
                    for (var i = wheelItems.Count - 1; i > stopIndex; i--)
                    {
                        if (wheelItems[i] == spinResult)
                        {
                            // totalStopIndex = i;
                            (wheelItems[i], wheelItems[stopIndex]) = (wheelItems[stopIndex], wheelItems[i]);
                            totalStopIndex = stopIndex;
                            break;
                        }
                    }
                }
            }
            else
            {
                totalStopIndex = stopIndex;
            }

            if (totalStopIndex < 0)
            {
                throw new Exception("找不到合适的stopIndex");
            }

            totalStopIndex = (totalStopIndex - 1 + wheelItems.Count) % wheelItems.Count;
            return totalStopIndex;
        }

        public int OnReelStopAtIndex(int currentIndex)
        {
            return currentIndex;
        }
        public void Update()
        {
            if (_wheel != null && transform.gameObject.activeInHierarchy)
            {
                _wheel.Update();
            }
        }

        public bool IsSpin = false;
        public async Task StartSpin(int inSpinResult,float spinTime)
        {
            if (IsSpin)
                return;
            spinResult = inSpinResult;
            IsSpin = true;
            var spinEndTask = new TaskCompletionSource<bool>();
            _wheel.StartSpinning(spinConfig, async () =>
            {
                await PerformSpinEnd();
                spinEndTask.SetResult(true);
            },_wheel.GetCurrentIndex(), null,null,true);
            await XUtility.WaitSeconds(spinTime);
            _wheel.OnSpinResultReceived();
            await spinEndTask.Task;
            IsSpin = false;
        }

        public async Task PerformSpinEnd()
        {
            spinResult = 0;
        }
    }