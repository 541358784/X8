using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DragonU3DSDK;

namespace DragonPlus.UI
{
    public enum ScrollType
    {
        Vertical,
        Horizontal
    }

    public enum VerticalType
    {
        TopToBottom,
        BottomToTop
    }

    public enum HorizontalType
    {
        LeftToRight,
        RigthToLeft
    }

    public class ReuseScrollView : MonoBehaviour
    {
        public GameObject prefab;
        public ScrollRect scrollRect;
        public RectTransform content;

        public ScrollType type = ScrollType.Vertical;
        public VerticalType verticalType = VerticalType.TopToBottom;
        public HorizontalType horizontalType = HorizontalType.LeftToRight;

        public float itemSize = 68;
        public int itemGenerate = 10;
        public int totalNumberItem = 100;

        private List<GameObject> loopItemList = new List<GameObject>();
        private GameObject[] arrayCurrent = null;
        private int cacheOld = -1;
        private bool isInit = false;

        void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            scrollRect.onValueChanged.AddListener(OnScrollChange);
        }

        public void Setup(int numberItem, GameObject itemPrefab)
        {
            prefab = itemPrefab;
            totalNumberItem = numberItem;
            if (totalNumberItem < 0)
            {
                totalNumberItem = 0;
            }

            Setup();
        }

        public void Setup()
        {
            if (prefab == null)
            {
                DebugUtil.LogError("No prefab/Gameobject Item linking");
                return;
            }

            if (type == ScrollType.Vertical)
            {
                int totalHeight = (int) (totalNumberItem * itemSize);
                content.SetHeight(totalHeight);
            }
            else
            {
                int totalWidth = (int) (totalNumberItem * itemSize);
                content.SetWidth(totalWidth);
            }

            arrayCurrent = new GameObject[totalNumberItem];

            for (int i = 0; i < itemGenerate; i++)
            {
                GameObject obj = null;
                if (!isInit)
                {
                    if (i < totalNumberItem)
                    {
                        obj = Instantiate(prefab) as GameObject;
                        //================特定逻辑。因为美术工程里缺少该脚本。==================//
                        // ReuseItemPlayer rItem = obj.AddComponent<ReuseItemPlayer>();
                        //===========================end=================================//
                        obj.name = "item_" + (i);
                        obj.transform.SetParent(content.transform, false);
                        obj.transform.localScale = Vector3.one;
                        loopItemList.Add(obj);
                        if (type == ScrollType.Vertical)
                        {
                            RectTransform rect = obj.GetComponent<RectTransform>();
                            if (rect != null)
                            {
                                Vector2 anchor = rect.pivot;
                                if (verticalType == VerticalType.BottomToTop)
                                {
                                    Vector2 min = rect.anchorMin;
                                    Vector2 max = rect.anchorMax;
                                    rect.anchorMin = new Vector2(min.x, 0);
                                    rect.anchorMax = new Vector2(max.x, 0);
                                    rect.pivot = new Vector2(anchor.x, 0);
                                }
                                else
                                {
                                    rect.pivot = new Vector2(anchor.x, 1);
                                }
                            }
                        }
                        else if (type == ScrollType.Horizontal)
                        {
                            RectTransform rect = obj.GetComponent<RectTransform>();
                            if (rect != null)
                            {
                                Vector2 anchor = rect.pivot;
                                if (horizontalType == HorizontalType.RigthToLeft)
                                {
                                    Vector2 min = rect.anchorMin;
                                    Vector2 max = rect.anchorMax;
                                    rect.anchorMin = new Vector2(1, min.y);
                                    rect.anchorMax = new Vector2(1, max.y);
                                    rect.pivot = new Vector2(1, anchor.y);
                                }
                                else
                                {
                                    rect.pivot = new Vector2(0, anchor.y);
                                }
                            }
                        }

                        Reload(obj, i);
                        arrayCurrent[i] = obj;
                    }
                }
                else
                {
                    if (i < totalNumberItem)
                    {
                        obj = loopItemList[i];
                        obj.SetActive(true);
                        Reload(obj, i);
                        arrayCurrent[i] = obj;
                    }
                    else
                    {
                        obj = loopItemList[i];
                        obj.SetActive(false);
                    }
                }
            }

            isInit = true;
        }

        private float GetContentSize()
        {
            return content.GetHeight();
        }

        private int GetCurrentIndex()
        {
            int index = -1;
            if (type == ScrollType.Vertical)
            {
                if (verticalType == VerticalType.TopToBottom)
                {
                    index = (int) (content.anchoredPosition.y / itemSize);
                }
                else
                {
                    index = (int) (-content.anchoredPosition.y / itemSize);
                }
            }
            else
            {
                if (horizontalType == HorizontalType.LeftToRight)
                {
                    index = (int) (-content.anchoredPosition.x / itemSize);
                }
                else
                {
                    index = (int) (content.anchoredPosition.x / itemSize);
                }
            }

            if (index < 0)
                index = 0;

            if (index > totalNumberItem - 1)
            {
                index = totalNumberItem - 1;
            }

            return index;
        }

        public void InternalReload()
        {
            int index = Mathf.Max(0, GetCurrentIndex());
            FixFastReload(index);
        }

        public void OnScrollChange(Vector2 vec)
        {
            if (arrayCurrent.Length < 1)
            {
                return;
            }

            int index = Mathf.Max(0, GetCurrentIndex());
            if (cacheOld != index)
            {
                cacheOld = index;
            }
            else
            {
                return;
            }

            if (!FixFastReload(index))
            {
                GameObject objIndex = arrayCurrent[index];
                if (objIndex == null)
                {
                    int next = index + itemGenerate;
                    if (next > totalNumberItem - 1)
                    {
                        return;
                    }
                    else
                    {
                        GameObject objNow = arrayCurrent[next];
                        if (objNow != null)
                        {
                            arrayCurrent[next] = objIndex;
                            arrayCurrent[index] = objNow;
                            Reload(arrayCurrent[index], index);
                        }
                    }
                }
                else
                {
                    if (index > 0)
                    {
                        GameObject obj = arrayCurrent[index - 1];
                        if (obj == null)
                        {
                            return;
                        }

                        int next = index - 1 + itemGenerate;
                        if (next > totalNumberItem - 1)
                        {
                            return;
                        }
                        else
                        {
                            GameObject objNow = arrayCurrent[next];
                            if (objNow == null)
                            {
                                arrayCurrent[next] = obj;
                                arrayCurrent[index - 1] = objNow;
                                Reload(arrayCurrent[next], next);
                            }
                        }
                    }
                }
            }
        }

        public bool FixFastReload(int index)
        {
            bool isNeedFix = false;

            int add = index + 1;
            for (int i = add; i < add + itemGenerate - 2; i++)
            {
                if (i < totalNumberItem)
                {
                    GameObject obj = arrayCurrent[i];
                    if (obj == null)
                    {
                        isNeedFix = true;
                        break;
                    }
                    else if (!obj.name.Equals("item_" + i))
                    {
                        isNeedFix = true;
                        break;
                    }
                }
            }

            if (isNeedFix)
            {
                for (int i = 0; i < totalNumberItem; i++)
                {
                    arrayCurrent[i] = null;
                }

                int start = index;
                if (start + itemGenerate > totalNumberItem)
                {
                    start = totalNumberItem - itemGenerate;
                }

                for (int i = 0; i < itemGenerate; i++)
                {
                    arrayCurrent[start + i] = loopItemList[i];
                    Reload(arrayCurrent[start + i], start + i);
                }

                return true;
            }

            return false;
        }

        protected virtual void Reload(GameObject obj, int indexReload)
        {
            obj.transform.name = "item_" + indexReload;
            int location = Mathf.Max(0, indexReload);
            Vector3 vec = Vector3.zero;
            vec.x = obj.transform.localPosition.x;
            vec.y = obj.transform.localPosition.y;
            vec = GetLocationAppear(vec, location);
            obj.transform.localPosition = vec;
            ReuseItemBase baseItem = obj.GetComponent<ReuseItemBase>();
            if (baseItem != null)
            {
                baseItem.Reload(this, indexReload);
            }
        }

        private Vector3 GetLocationAppear(Vector2 initVec, int location)
        {
            Vector3 vec = initVec;
            if (type == ScrollType.Vertical)
            {
                if (verticalType == VerticalType.TopToBottom)
                {
                    vec = new Vector3(vec.x, -itemSize * location, 0);
                }
                else
                {
                    vec = new Vector3(vec.x, itemSize * location, 0);
                }
            }
            else
            {
                if (horizontalType == HorizontalType.LeftToRight)
                {
                    vec = new Vector3(itemSize * location, vec.y, 0);
                }
                else
                {
                    vec = new Vector3(-itemSize * location, vec.y, 0);
                }
            }

            return vec;
        }
    }
}