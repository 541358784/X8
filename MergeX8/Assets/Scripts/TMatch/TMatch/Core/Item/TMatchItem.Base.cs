using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Asset;
using Framework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TMatch
{


    public class TMatchBaseItem
    {
        public class DoTweenData
        {
            public Vector3 pos;
            public Vector3 rot;
            public Vector3 scale;
            public float duration;
            public Action finish;
            public Ease ease;

            public int loop;
            public LoopType loopType;
        }

        public enum OperStateType
        {
            Scene, //场景中
            Exploed, //爆炸
            SceneToCollectore, //场景到收集栏
            Collectore, //收集栏
            Triple, //消除
        }

        protected int id;
        protected Item cfg;
        protected GameObject obj;
        protected OperStateType operState;

        private TweenerCore<Vector3, Vector3, VectorOptions> moveLoopTween;
        private Queue<DoTweenData> moveTweenDataQueue = new Queue<DoTweenData>();
        private TweenerCore<Vector3, Vector3, VectorOptions> curMoveTween;

        private Queue<DoTweenData> scaleTweenDataQueue = new Queue<DoTweenData>();
        private TweenerCore<Vector3, Vector3, VectorOptions> curScaleTween;

        private Queue<DoTweenData> rotTweenDataQueue = new Queue<DoTweenData>();
        private TweenerCore<Quaternion, Vector3, QuaternionOptions> curRotTween;

        private TweenerCore<Vector3, Vector3, VectorOptions> pickScaleTween;

        public int Id => id;
        public Item Cfg => cfg;
        public GameObject GameObject => obj;

        public OperStateType OperState
        {
            get { return operState; }
            set { operState = value; }
        }

        public TMatchBaseItem(int id)
        {
            this.id = id;
            this.cfg = TMatchConfigManager.Instance.GetItem(id);

            //check
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (cfg == null) Debug.LogError($"TMatch Item {id} cfg is null!");
            if (!string.IsNullOrEmpty(cfg.layRot))
            {
                string[] temp = cfg.layRot.Split(',');
                if (temp.Length != 3) Debug.LogError($"TMatch Item {id} layRot error!");
            }

            if (!string.IsNullOrEmpty(cfg.scalingRatio))
            {
                string[] temp = cfg.scalingRatio.Split(',');
                if (temp.Length != 3) Debug.LogError($"TMatch Item {id} scalingRatio error!");
            }
#endif
        }

        public void Destory()
        {
            if (obj != null)
            {
                GameObject.Destroy(obj);
                obj = null;
            }
        }

        public void Load()
        {
            GameObject prefab =
                ResourcesManager.Instance.LoadResource<GameObject>($"TMatch/TMatch/Item/Prefabs/{cfg.prefabName}");
            if (prefab == null) Debug.LogError($"can not find : {cfg.prefabName} asset.");
            obj = GameObject.Instantiate(prefab, TMatchEnvSystem.Instance.ItemRoot.transform);
            obj.GetComponent<Renderer>().material.SetFloat("_Saturation", 0.92f);
            obj.GetComponent<Renderer>().material.SetFloat("_Contrast", 1.12f);
            obj.transform.localScale = GetRealScale(1.0f);

            //effect
            if (!string.IsNullOrEmpty(cfg.effectName))
            {
                GameObject vfxPrefab =
                    ResourcesManager.Instance.LoadResource<GameObject>($"TMatch/Particle/Prefabs/{cfg.effectName}",
                        addToCache: true);
                if (vfxPrefab == null) Debug.LogError($"can not find : {cfg.effectName} asset.");
                GameObject.Instantiate(vfxPrefab, GameObject.transform);
            }
        }

        public void RandomPos(Vector3 min, Vector3 max, int layer = 0)
        {
            int layerValue = layer == 0 ? cfg.layer : layer;
            float slice = (max.y - min.y) / 7.0f;
            float minX = min.x;
            float maxX = max.x;
            float minY = min.y;
            float maxY = min.y;
            float minZ = min.z;
            float maxZ = max.z;
            if (layerValue == 1) //切片1
            {
                maxY += slice;
            }
            else if (layerValue == 2) //[切片2,切片3]
            {
                minY += slice;
                maxY += slice * 3;
            }
            else if (layerValue == 3) //[切片1,切片4]
            {
                maxY += slice * 4;
            }
            else if (layerValue == 4) //[切片7]
            {
                //一般都是在start后1、2秒才创建
                //不要太靠边界
                float tempX = Random.Range(minX + 1.0f, maxX - 1.0f);
                float tempZ = Random.Range(minZ + 1.0f, maxZ - 1.0f);
                float tempMaxY = minY;
                bool find = false;
                Vector2[] testPos =
                {
                    new Vector2(tempX - 0.1f, tempZ - 0.1f),
                    new Vector2(tempX + 0.1f, tempZ - 0.1f),
                    new Vector2(tempX + 0.1f, tempZ + 0.1f),
                    new Vector2(tempX - 0.1f, tempZ + 0.1f),
                    new Vector2(tempX, tempZ),
                };
                foreach (var p in testPos)
                {
                    Ray ray = new Ray(new Vector3(p.x, max.y, p.y), new Vector3(0.0f, -1.0f, 0.0f));
                    int layerMask = 1 << LayerMask.NameToLayer("TMatchItem");
                    RaycastHit[] hits = Physics.RaycastAll(ray, 100, layerMask);

                    foreach (var hit in hits)
                    {
                        if (hit.transform.position.y > tempMaxY)
                        {
                            tempMaxY = hit.transform.position.y;
                            find = true;
                        }
                    }
                }

                if (find)
                {
                    minY = tempMaxY + 0.5f;
                    maxY = tempMaxY + 0.5f;
                    minY = Mathf.Clamp(minY, min.y, max.y);
                    maxY = Mathf.Clamp(maxY, min.y, max.y);

                    minX = tempX;
                    maxX = tempX;

                    minZ = tempZ;
                    maxZ = tempZ;
                }
                else
                {
                    minY += slice * 6;
                    maxY += slice * 7;
                }
            }
            else if (layerValue == 999) //[切片7] 用于修正位置
            {
                minY += slice * 6;
                maxY += slice * 7;
            }

            GameObject.transform.position = new Vector3(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY),
                Random.Range(minZ, maxZ));
        }

        public void KeepPositionInSceneBound()
        {
            if (operState == OperStateType.Scene)
            {
                //卡在收集栏处
                /*
                if (GameObject.transform.position.x >= TMatchEnvSystem.Instance.SceneBoundMin.x && GameObject.transform.position.x <= TMatchEnvSystem.Instance.SceneBoundMax.x &&
                    GameObject.transform.position.y >= TMatchEnvSystem.Instance.SceneBoundMin.y && GameObject.transform.position.y <= TMatchEnvSystem.Instance.SceneBoundMax.y &&
                    GameObject.transform.position.z < TMatchEnvSystem.Instance.SceneBoundMin.z && GameObject.transform.position.z > TMatchEnvSystem.Instance.SceneBoundMin.z - 2)
                {
                    DisableCollider();
                    Retract(7);
                    Debug.LogError($"Item Position in collector, force reset to SceneBound.");
                    return;
                }
                */

                if (GameObject.transform.position.x < TMatchEnvSystem.Instance.SceneBoundMin.x ||
                    GameObject.transform.position.x > TMatchEnvSystem.Instance.SceneBoundMax.x ||
                    GameObject.transform.position.y < TMatchEnvSystem.Instance.SceneBoundMin.y ||
                    GameObject.transform.position.y > TMatchEnvSystem.Instance.SceneBoundMax.y ||
                    GameObject.transform.position.z < TMatchEnvSystem.Instance.SceneBoundMin.z ||
                    GameObject.transform.position.z > TMatchEnvSystem.Instance.SceneBoundMax.z)
                {
                    RandomPos(TMatchEnvSystem.Instance.SceneRandomPosMin, TMatchEnvSystem.Instance.SceneRandomPosMax,
                        999);
                    Debug.LogError($"Item Position not in SceneBound, force reset to SceneBound.");
                }
            }
        }

        public Vector3 GetRealScale(float expectScale)
        {
            Vector3 defaultScale = Vector3.one;
            if (!string.IsNullOrEmpty(cfg.scalingRatio))
            {
                string[] temp = cfg.scalingRatio.Split(',');
                defaultScale = new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
            }

            return defaultScale * expectScale;
        }

        public void UnLoad()
        {
            GameObject.Destroy(obj);
        }

        public void ShowRender()
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var p in renderers) p.enabled = true;
        }

        public void HideRender()
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var p in renderers) p.enabled = false;
        }

        public void SleepPhysic()
        {
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            rigidbody.Sleep();
        }

        public void WakeUpPhysic()
        {
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            rigidbody.WakeUp();
        }

        public void EnableGravity()
        {
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            rigidbody.useGravity = true;
        }

        public void DisbaleGravity()
        {
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
        }

        public void DisableCollider()
        {
            obj.transform.GetComponent<Collider>().enabled = false;
        }

        public void EnableCollider()
        {
            obj.transform.GetComponent<Collider>().enabled = true;
        }

        public void Pick()
        {
            pickScaleTween?.Kill();
            pickScaleTween = obj.transform.DOScale(GetRealScale(1.1f), 0.2f);
            obj.GetComponent<Renderer>().material.shader = Shader.Find("Custom/TMatch/DiffuseOutline");
            obj.GetComponent<Renderer>().material.SetColor("_OutlineColor",
                new Color(255.0f / 255.0f, 255.0f / 255.0f, 84.0f / 255.0f, 1.0f));
            obj.GetComponent<Renderer>().material.SetFloat("_OutlineSize", 0.05f);
            obj.GetComponent<Renderer>().material.SetFloat("_Saturation", 0.92f);
            obj.GetComponent<Renderer>().material.SetFloat("_Contrast", 1.12f);
        }

        public void UnPick()
        {
            pickScaleTween?.Kill();
            pickScaleTween = null;
            obj.transform.localScale = GetRealScale(1.0f);
            obj.GetComponent<Renderer>().material.shader = Shader.Find("Custom/TMatch/Diffuse");
            obj.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            obj.GetComponent<Renderer>().material.SetFloat("_Saturation", 0.92f);
            obj.GetComponent<Renderer>().material.SetFloat("_Contrast", 1.12f);
        }

        public void Hit()
        {
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            rigidbody.AddForce(Vector3.up * 500.0f, ForceMode.Acceleration);
        }

        public void Lightinged(Action finish)
        {
            obj.GetComponent<Renderer>().material.SetColor("_Color",
                new Color(128.0f / 255.0f, 128.0f / 255.0f, 128.0f / 255.0f, 1.0f));
            DOScale(GetRealScale(0.75f), 0.2f, finish, Ease.InBack);
        }

        public void Explode(Vector3 position, float power, float radius)
        {
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            rigidbody.AddExplosionForce(power, position, radius);
        }

        public async void Retract(int collectorIndex)
        {
            moveLoopTween?.Kill();
            moveLoopTween = null;
            curMoveTween?.Kill();
            curMoveTween = null;
            moveTweenDataQueue.Clear();
            curRotTween?.Kill();
            curRotTween = null;
            scaleTweenDataQueue.Clear();
            curScaleTween?.Kill();
            curScaleTween = null;
            rotTweenDataQueue.Clear();

            EnableGravity();
            WakeUpPhysic();
            DOScale(GetRealScale(1.0f), 0.2f, null);
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            Vector2[] rang = new Vector2[8]
            {
                new Vector2(0, 1),
                new Vector2(-0.5f, 1),
                new Vector2(-1, 1),
                new Vector2(-1, 1),
                new Vector2(-1, 1),
                new Vector2(-1, 0.5f),
                new Vector2(-1, 0),
                new Vector2(0, 0)
            };
            rigidbody.AddForce(
                new Vector3(Random.Range(rang[collectorIndex].x, rang[collectorIndex].y), 2, 0.25f).normalized *
                Random.Range(500, 800));
            rigidbody.AddTorque(new Vector3(0, 1, 1).normalized * 250);
            await Task.Delay(200);
            EnableCollider();
        }

        public void Shuffle()
        {
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            rigidbody.AddForce(
                new Vector3(Random.Range(-0.85f, 0.85f), Random.Range(0.0f, 1.0f), Random.Range(-0.85f, 0.85f))
                    .normalized * Random.Range(1000, 1250));
            rigidbody.AddTorque(new Vector3(Random.Range(-1, 1), 1, Random.Range(-1, 1)).normalized * 250);
        }

        public Vector3 GetLayRot()
        {
            if (string.IsNullOrEmpty(cfg.layRot))
            {
                return Vector3.zero;
            }
            else
            {
                string[] temp = cfg.layRot.Split(',');
                Vector3 rot = new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
                Quaternion oldRot = GameObject.transform.rotation;
                GameObject.transform.rotation = Quaternion.Euler(rot);
                GameObject.transform.rotation =
                    CameraManager.MainCamera.transform.rotation * GameObject.transform.rotation;
                rot = GameObject.transform.rotation.eulerAngles;
                GameObject.transform.rotation = oldRot;
                return rot;
            }
        }

        public void DOMove(Vector3 pos, float duration, Action finish, Ease ease = Ease.Linear, int loop = 0,
            LoopType loopType = LoopType.Yoyo)
        {
            if (moveTweenDataQueue.Count == 0)
            {
                OnMoveQueueBegin();
            }

            DoTweenData doTweenData = new DoTweenData()
            {
                pos = pos,
                duration = duration,
                finish = finish,
                ease = ease,
                loop = loop,
                loopType = loopType
            };
            moveTweenDataQueue.Enqueue(doTweenData);
            DOMoveQueue();
        }

        private void DOMoveQueue()
        {
            if (moveTweenDataQueue.Count == 0)
            {
                OnMoveQueueFinish();
                return;
            }

            if (curMoveTween != null)
            {
                /*
                if (curMoveTween.Loops() == -1)
                {
                    curMoveTween.Kill();
                    curMoveTween = null;
                }
                else
                */
                {
                    return;
                }
            }

            DoTweenData doTweenData = moveTweenDataQueue.Peek();
            curMoveTween = obj.transform.DOMove(doTweenData.pos, doTweenData.duration).SetEase(doTweenData.ease);
            if (doTweenData.loop != 0)
            {
                Debug.LogError("do not add loop tween!");
                //curMoveTween.SetLoops(doTweenData.loop, doTweenData.loopType);
                //moveTweenDataQueue.Dequeue();
            }
            else
            {
                curMoveTween.onComplete += () =>
                {
                    curMoveTween = null;
                    moveTweenDataQueue.Dequeue();
                    doTweenData.finish?.Invoke();
                    DOMoveQueue();
                };
            }
        }

        private void OnMoveQueueBegin()
        {
            moveLoopTween?.Kill();
            moveLoopTween = null;
        }

        private void OnMoveQueueFinish()
        {
            if (operState == OperStateType.Collectore)
            {
                GameObject.transform.position =
                    TMatchEnvSystem.Instance.CollectorPos[TMatchCollectorSystem.Instance.GetItemIndex(this)];
                moveLoopTween = obj.transform
                    .DOMove(
                        TMatchEnvSystem.Instance.CollectorPos[TMatchCollectorSystem.Instance.GetItemIndex(this)] +
                        new Vector3(0.0f, -0.2f, 0.0f), 1.0f).SetEase(Ease.Linear);
                moveLoopTween.SetLoops(-1, LoopType.Yoyo);
            }
        }

        public void DOScale(Vector3 scale, float duration, Action finish, Ease ease = Ease.Linear)
        {
            DoTweenData doTweenData = new DoTweenData()
            {
                scale = scale,
                duration = duration,
                finish = finish,
                ease = ease
            };
            scaleTweenDataQueue.Enqueue(doTweenData);
            DOScaleQueue();
        }

        private void DOScaleQueue()
        {
            if (scaleTweenDataQueue.Count == 0) return;
            if (curScaleTween != null) return;
            DoTweenData doTweenData = scaleTweenDataQueue.Peek();
            curScaleTween = obj.transform.DOScale(doTweenData.scale, doTweenData.duration).SetEase(doTweenData.ease);
            curScaleTween.onComplete += () =>
            {
                curScaleTween = null;
                scaleTweenDataQueue.Dequeue();
                doTweenData.finish?.Invoke();
                DOScaleQueue();
            };
        }

        public void DORotate(Vector3 rot, float duration, Action finish, Ease ease = Ease.Linear)
        {
            DoTweenData doTweenData = new DoTweenData()
            {
                rot = rot,
                duration = duration,
                finish = finish,
                ease = ease
            };
            rotTweenDataQueue.Enqueue(doTweenData);
            DORotQueue();
        }

        private void DORotQueue()
        {
            if (rotTweenDataQueue.Count == 0) return;
            if (curRotTween != null) return;
            DoTweenData doTweenData = rotTweenDataQueue.Peek();
            curRotTween = obj.transform.DORotate(doTweenData.rot, doTweenData.duration).SetEase(doTweenData.ease);
            curRotTween.onComplete += () =>
            {
                curRotTween = null;
                rotTweenDataQueue.Dequeue();
                doTweenData.finish?.Invoke();
                DOScaleQueue();
            };
        }

        public Vector3 ToUIPosition()
        {
            return UIRoot.Instance.mUICamera.ScreenToWorldPoint(
                CameraManager.MainCamera.WorldToScreenPoint(obj.transform.position));
        }
    }
}