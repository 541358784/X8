using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Haptics;
using Spine.Unity;
using Spine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Screw
{
    public static class ScrewUtility
    {
        public static long SecondsOfOneDay = 24 * 60 * 60;
        public static long MilliSecondsOfOneDay = 24 * 60 * 60 * 1000;

        /// <summary>
        /// 按照3位一组用逗号分割数字
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string GetCommaFormat(this long num)
        {
            return num.ToString("N0", CultureInfo.InvariantCulture);
        }

        public static void CloneProperties(this RectTransform to, RectTransform from)
        {
            to.pivot = from.pivot;
            to.anchorMax = from.anchorMax;
            to.anchorMin = from.anchorMin;
            to.offsetMax = from.offsetMax;
            to.offsetMin = from.offsetMin;
            to.sizeDelta = from.sizeDelta;
            to.anchoredPosition = from.anchoredPosition;
            to.anchoredPosition3D = from.anchoredPosition3D;
        }

        public static string GetCommaFormat(this int num)
        {
            return num.ToString("N0", CultureInfo.InvariantCulture);
        }

        public static T GetOrCreateComponent<T>( this GameObject owner) where T : Component
        {
            T targetT = owner.GetComponent<T>();
            if (targetT == null)
                targetT = owner.AddComponent<T>();

            return targetT;
        }

        /// <summary>
        /// 延迟N帧
        /// </summary>
        /// <param name="frameCount"></param>
        /// <param name="token"></param>
        public static async UniTask WaitNFrame(int frameCount, CancellationToken token = default(CancellationToken))
        {
            await UniTask.DelayFrame(frameCount, cancellationToken: token);
        }

        public static async void WaitNFrame(int frameCount, Action finishCallback,
            CancellationToken token = default(CancellationToken))
        {
            await UniTask.DelayFrame(frameCount, cancellationToken: token);
            finishCallback?.Invoke();
        }


        public static void PlayAnim(this Animator animator, string name)
        {
            int state = Animator.StringToHash(name);
            if (!animator || !animator.enabled || !animator.gameObject.activeSelf ||
                !animator.runtimeAnimatorController || state == 0 || !animator.HasState(0, state))
                return;
            animator.enabled = true;

            PlayAnimation(animator, name, () =>
            {
                if (animator != null && animator.gameObject != null)
                    animator.enabled = false;
            });
        }

        public static bool Contains(this int[] array, int value)
        {
            if (array == null || array.Length == 0)
                return false;

            foreach (var v in array)
            {
                if (v == value)
                    return true;
            }

            return false;
        }
        
        public static string GetTimeText(float leftTime)
        {
            var timeSpan = TimeSpan.FromSeconds(Math.Max(0, leftTime));

            return string.Format("{00:mm}:{00:ss}",timeSpan);
        }


        public static float GetAnimationTime(Animator animator, string playAnimName)
        {
            float length = 1.0f;
            AnimationClip[] clips = animator.runtimeAnimatorController?.animationClips;
            if (clips != null)
            {
                foreach (AnimationClip clip in clips)
                {
                    if (clip.name.Equals(playAnimName))
                    {
                        length = clip.length;
                        break;
                    }
                }
            }

            return length;
        }

        public static bool HasState(this Animator anim, string stateName, int layer = 0)
        {
            int stateID = Animator.StringToHash(stateName);
            return anim.HasState(layer, stateID);
        }

        public static float GetAnimationLength(Animator animator, string stateName)
        {
            if (animator == null || !animator.HasState(stateName))
                return 0;
            var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length == 0)
                return 0;
            return clipInfo[0].clip.length;
        }

        public static async void PlayAnimation(Animator animator, string stateName, Action finishCallback = null,
            CancellationToken token = default, bool forceCallback = false)
        {
             if (animator == null || !animator.HasState(stateName))
            {
                finishCallback?.Invoke();
                return;
            }

            if (finishCallback == null)
            {
                if (animator.HasState(stateName))
                {
                    animator.speed = 1;
                    animator.Play(stateName, -1, 0);
                }

                return;
            }

            if (!animator.gameObject.activeInHierarchy)
            {
                Debug.LogError("GameObject Is Inactive, Animation Playing Not Possible");

                finishCallback?.Invoke();
                return;
            }

            animator.speed = 1;
            animator.Play(stateName, -1, 0);

            await WaitNFrame(1);

            if (animator)
            {
                var clipInfo = animator.GetCurrentAnimatorClipInfo(0);

                try
                {
                    if (clipInfo.Length > 0)
                        await WaitSeconds(clipInfo[0].clip.length, false, token);
                    else
                    {
                        if (!token.IsCancellationRequested)
                            finishCallback?.Invoke();
                        return;
                    }

                    if (forceCallback)
                        if (!token.IsCancellationRequested)
                            finishCallback?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        /// <summary>
        /// 方便异步调用的等几秒
        /// </summary>
        /// <param name="timeToDelay"></param>
        /// <param name="realTime"></param>
        /// <param name="cancellationToken"></param>
        public static async UniTask WaitSeconds(float timeToDelay, bool realTime = false,
            CancellationToken cancellationToken = default)
        {
            if (timeToDelay <= 0)
                return;

            await UniTask.Delay((int)(timeToDelay * 1000), realTime, cancellationToken: cancellationToken);
        }

        public static IEnumerator DelayWork(float delayTime, Action action,
            CancellationToken cancellationToken = default)
        {
            if (action != null)
            {
                yield return new WaitForSeconds(delayTime);
                if (!cancellationToken.IsCancellationRequested)
                {
                    action();
                }
            }
        }

        public static bool IsTouchUGUI()
        {
            if (EventSystem.current == null) return false;
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        if (Input.touchCount > 0 ? EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) : EventSystem.current.IsPointerOverGameObject())
#else
            if (EventSystem.current.IsPointerOverGameObject())
#endif
                return true;
            else
                return false;
        }

        /// <summary>
        /// 方便异步调用的等几秒
        /// </summary>
        /// <param name="timeToDelay"></param>
        /// <param name="finishCallback"></param>
        /// <param name="realTime"></param>
        /// <param name="cancellationToken"></param>
        public static async UniTaskVoid WaitSeconds(float timeToDelay, Action finishCallback = null,
            CancellationToken cancellationToken = default, bool realTime = false)
        {
            if (timeToDelay <= 0)
            {
                finishCallback?.Invoke();
                return;
            }

            await UniTask.Delay((int)(timeToDelay * 1000), realTime, cancellationToken: cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
                finishCallback?.Invoke();
        }


        public static async UniTask PlayAnimationAsync(Animator animator, string stateName,
            CancellationToken cancellationToken, bool forceCallback = false)
        {
            UniTaskCompletionSource<bool> taskCompletionSource = new UniTaskCompletionSource<bool>();

            PlayAnimation(animator, stateName, () =>
            {
                if (!cancellationToken.IsCancellationRequested)
                    taskCompletionSource.TrySetResult(true);
            }, cancellationToken, forceCallback);
            await taskCompletionSource.Task;
        }

        public static bool IsLE_16_10()
        {
            float maxR = Mathf.Max(Screen.width, Screen.height);
            float minR = Mathf.Min(Screen.width, Screen.height);
            var ratio = (maxR / minR) < 1.595f;
            //DebugUtil.LogError($"IsPad {Screen.width} {Screen.height} {isiPad}");
            return ratio;
        }

        public static float GetScreenRatio()
        {
            float maxR = Mathf.Max(Screen.width, Screen.height);
            float minR = Mathf.Min(Screen.width, Screen.height);
            var ratio = maxR / minR;
            //DebugUtil.LogError($"IsPad {Screen.width} {Screen.height} {isiPad}");
            return ratio;
        }

        public static float GetAnimTime(Animator animator, string playAnimName)
        {
            float length = 1.0f;
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name.Equals(playAnimName))
                {
                    length = clip.length;
                    break;
                }
            }

            return length;
        }

        public static void ListRandom<T>(List<T> sources)
        {
            int index = 0;
            T temp;
            for (int i = 0; i < sources.Count; i++)
            {
                index = Random.Range(0, sources.Count - 1);
                if (index != i)
                {
                    temp = sources[i];
                    sources[i] = sources[index];
                    sources[index] = temp;
                }
            }
        }

        //向上取为3的倍数
        public static int UpToMultipleOf3(int value)
        {
            return (value / 3 + (value % 3 > 1 ? 1 : 0)) * 3;
        }

        public static long GetTimeStamp()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long) (DateTime.Now - startTime).TotalMilliseconds;
            return timeStamp;
        }

        /// <summary>
        /// 获取总天数
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static int GetTotalDays(ulong milliseconds)
        {
            TimeSpan s = TimeSpan.FromMilliseconds(milliseconds);
            return (int) s.TotalDays;
        }

        public static void IosAdapt(RectTransform rectTransform)
        {
            if (rectTransform == null) return;
            rectTransform.offsetMax = new Vector2(0, GetSafeAreaOffset());
        }

        public static float GetSafeAreaOffset()
        {
            int safeAreaOffset = (int) (Screen.height - Screen.safeArea.yMax);
            if (safeAreaOffset == 0) return 0.0f;

            safeAreaOffset += 30;
            return -safeAreaOffset;
        }

        // 随机奖励，总权重值为100, 权重值和没达到100就可能会没有奖励
        // 没有奖励时返回权重配置长度
        public static int GetRandomWeightResult(List<int> weight)
        {
            var randomInt = UnityEngine.Random.Range(0, 100);
            for (var i = 0; i < weight.Count; i++)
            {
                if (randomInt <= weight[i])
                {
                    return i;
                }
                else
                {
                    randomInt -= weight[i];
                }
            }

            return weight.Count - 1;
        }

        public static DateTime ConvertFromUnixTimestamp(ulong timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1);
            return origin.AddMilliseconds(timestamp);
        }
        public static string ConvertTimestampToYearAndMonth(ulong timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1);
            return origin.AddMilliseconds(timestamp).ToString("MM/yyyy");
        }

        public static long ConvertDateTimeToTimeStamp(DateTime date)
        {
            DateTime startTime = new DateTime(1970, 1, 1);
            long timeStamp = (long) (date - startTime).TotalMilliseconds;
            return timeStamp;
        }

        public static void TweenOpen(Transform t, Action callback = null)
        {
            Vector3 oldScale = t.localScale;
            if (oldScale == Vector3.zero)
            {
                oldScale = Vector3.one;
            }

            t.localScale = Vector3.zero;
            Sequence sequence = DOTween.Sequence();
            Tween tween1 = t.DOScale(oldScale * 1.1f, 0.16f).OnComplete(() => { callback?.Invoke(); });
            Tween tween2 = t.DOScale(oldScale * 1f, 0.08f);
            sequence.Append(tween1);
            sequence.Append(tween2);
        }

        public static void TweenClose(Transform t, Action callback)
        {
            Vector3 oldScale = t.localScale;
            if (oldScale == Vector3.zero)
            {
                oldScale = Vector3.one;
            }

            Sequence sequence = DOTween.Sequence();
            Tween tween1 = t.DOScale(1.1f * oldScale, 0.08f);
            Tween tween2 = t.DOScale(1f * oldScale, 0.08f).OnComplete(() => { callback(); });
            sequence.Append(tween1);
            sequence.Append(tween2);
        }

        /// <summary>
        /// usingFullFormat = false
        /// 小猪存钱罐和连赢活动的时间显示。
        /// 小于一天时：显示小时：分钟：秒（无单位）。
        /// 大于一天时：显示nd nh（有单位），n为个位数时，不显示前面的0（如3h而不是03h）。
        /// 计算小时数时，被舍去的分钟数大于等于30分钟记为当前时间数+1小时，小于30分钟时不进位。当小时数最终计算为0时，时间只显示nd。（3d 23h 30m 01s应显示为4d）
        /// </summary>
        /// <param name="l"></param>
        /// <param name="usingFullFormat"></param>
        /// <param name="shortFormatForLargeThanOneDay"></param>
        /// <returns></returns>
        public static String FormatLongToTimeStr(long l, bool usingFullFormat = true, bool shortFormatForLargeThanOneDay = true)
        {
            int hour = 0;
            int minute = 0;
            int second = 0;
            int day = 0;

            l = l < 0 ? 0 : l;
            second = (int) (l / 1000);

            if (second >= 60)
            {
                minute = second / 60;
                second = second % 60;
            }

            if (minute >= 60)
            {
                hour = minute / 60;
                minute = minute % 60;
            }

            if (hour >= 24)
            {
                day = hour / 24;
                hour = hour % 24;
            }

            var d = LocalizationManager.Instance.GetLocalizedString("&key.UI_common_time_d");
            var h = LocalizationManager.Instance.GetLocalizedString("&key.UI_common_time_h");
            var m = LocalizationManager.Instance.GetLocalizedString("&key.UI_common_time_m");
            var s = LocalizationManager.Instance.GetLocalizedString("&key.UI_common_time_s");

            if (usingFullFormat)
            {
                if (day > 0)
                {
                    if (shortFormatForLargeThanOneDay)
                    {
                        if(hour == 0 && minute > 30)
                            return $"{day}{d} 1{h}";
                        
                        return $"{day}{d} {hour}{h}";
                    }
                }
                return day > 0 ? $"{day}{d}{hour}{h}{minute}{m}" : $"{hour:D2}:{minute:D2}:{second:D2}";
            }
            else if (day > 0)
            {
                if (hour > 0)
                {
                    return $"{day}{d} {hour}{h}";
                }
                else if (minute >= 30)
                {
                    return $"{day}{d} 1{h}";
                }
                else
                {
                    return $"{day}{d}";
                }
            }
            else
            {
                if (hour >= 1)
                {
                    if (minute > 0)
                    {
                        return $"{hour}{h} {minute}{m}";
                    }
                    else
                    {
                        return $"{hour}{h}";
                    }
                }

                if (minute > 0)
                {
                    if (second > 0)
                    {
                        return $"{minute}{m} {second}{s}";
                    }
                    else
                    {
                        return $"{minute}{m}";
                    }
                }

                return $"{second}{s}";
            }
        }

        public static int GetStringLength(string str)
        {
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(str);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int) s[i] == 63)
                    tempLen += 2;
                else
                    tempLen += 1;
            }

            return tempLen;
        }


        public static bool IsMatchEmoji(string text, string pattern)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(text, pattern);
        }

        public static bool ContainsEmoji(string text)
        {
            bool containEmoji = false;

            // string pattern = @"\u00a9|\u00ae|[\u2000-\u3300]|\ud83c[\ud000-\udfff]|\ud83d[\ud000-\udfff]|\ud83e[\ud000-\udfff]";
            string pattern = @"1/(\ud83c[\udf00-\udfff])|(\ud83d[\udc00-\ude4f\ude80-\udeff])|[\u2600-\u2B55]/g";
            containEmoji = IsMatchEmoji(text, pattern);
            if (containEmoji)
            {
                return containEmoji;
            }

            return containEmoji;
        }


        public static string GetSubString(string srcStr, int start, int length)
        {
            int srcIndex = 0;
            int retLength = 0;
            List<byte> retBytes = new List<byte>();

            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            byte[] srcAsciiBytes = ascii.GetBytes(srcStr);

            var utf8 = new UTF8Encoding();
            byte[] srcUtf8Bytes = utf8.GetBytes(srcStr);

            for (int i = 0; i < srcAsciiBytes.Length; i++)
            {
                if ((int) srcAsciiBytes[i] == 63)
                {
                    // 汉字
                    if (srcIndex >= start)
                    {
                        retBytes.Add(srcUtf8Bytes[srcIndex]);
                        retBytes.Add(srcUtf8Bytes[srcIndex + 1]);
                        retBytes.Add(srcUtf8Bytes[srcIndex + 2]);
                        retLength += 2;
                    }

                    srcIndex += 3;
                }
                else
                {
                    if (srcIndex >= start)
                    {
                        retBytes.Add(srcUtf8Bytes[srcIndex]);
                        retLength += 1;
                    }

                    srcIndex += 1;
                }

                if (retLength >= length)
                {
                    break;
                }
            }

            byte[] bytesArray = retBytes.ToArray();
            string utf8String = utf8.GetString(bytesArray, 0, bytesArray.Length);
            return utf8String;
        }

        public static string ReplaceNewLineSpace(string srcStr)
        {
            return srcStr.Replace(" ", "\u00A0");
        }

        public static Component GetOrCreateComponent(GameObject go, System.Type type)
        {
            Component component = go.GetComponent(type);
            if (component == null)
            {
                component = go.AddComponent(type);
            }

            return component;
        }

        // 从[min, max) 中随机选择count个不重复的随机数字（不包含max)
        public static List<int> GenerateRandom(int count, int min, int max)
        {
            if (max <= min || count < 0 || (count > max - min && max - min > 0))
            {
                throw new ArgumentOutOfRangeException("Range " + min + " to " + max +
                                                      " (" + ((Int64) max - (Int64) min) + " values), or count " +
                                                      count +
                                                      " is illegal");
            }

            HashSet<int> candidates = new HashSet<int>();

            for (int top = max - count; top < max; top++)
            {
                UnityEngine.Random.InitState((int) System.DateTime.Now.Ticks);
                if (!candidates.Add(UnityEngine.Random.Range(min, top + 1)))
                {
                    candidates.Add(top);
                }
            }

            return candidates.ToList();
        }
        
        public static void Shuffle<T> (this System.Random rng, List<T> array)
        {
            int n = array.Count;
            while (n > 1) 
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
        public static void EnableVibrate(bool enabled)
        {
            if (enabled)
            {
                HapticsManager.Init();
            }
            else
            {
                HapticsManager.Release();
                HapticsManager.Active = false;
            }
        }

        public static void Vibrate(HapticTypes hapticTypes = HapticTypes.Medium)
        {
            try
            {
                if(SettingManager.Instance.ShakeClose)
                    return;
                
                HapticsManager.Haptics(hapticTypes);
            }
            catch (Exception e)
            {
            }
        }
        public static int RandomSelect(List<int> weights, int totalWeight)
        {
            var randomHit = UnityEngine.Random.Range(0, totalWeight);

            var currentWeight = 0;

            for (var i = 0; i < weights.Count; i++)
            {
                currentWeight += weights[i];

                if (currentWeight >= randomHit)
                {
                    return i;
                }
            }

            return 0;
        }


        public static void ShowTipAndAutoHide(Transform transform,CancellationToken cancellationToken = default
            , float secondsToHide = 3.0f, float scaleTime = 0.2f,
            bool clickAnyWhereToHide = true, bool clickChildDontHide = false,
            bool hideWithAnimation = false)
        {
            try
            {
                transform.localScale = Vector3.zero;
                transform.gameObject.SetActive(true);
                bool isHidingTip = false;

                transform.DOKill();
                transform.DOScale(new Vector3(1, 1, 1), scaleTime).OnComplete(() =>
                {
                    WaitSeconds(secondsToHide, () =>
                    {
                        if (transform != null && transform.gameObject.activeSelf && !isHidingTip)
                        {
                            isHidingTip = true;
                            transform.DOKill();
                            transform.DOScale(Vector3.zero, scaleTime * 0.5f).OnComplete(() =>
                            {
                                transform.gameObject.SetActive(false);
                            }).SetEase(Ease.InQuad);
                        }
                    }, cancellationToken).Forget();
                }).SetEase(Ease.OutBack);

                if (clickAnyWhereToHide)
                {
                    EventSystem.current.SetSelectedGameObject(transform.gameObject);

                    var selectEventCustomHandler = transform.gameObject.GetComponent<SelectEventCustomHandler>();

                    if (selectEventCustomHandler == null)
                    {
                        selectEventCustomHandler = transform.gameObject.AddComponent<SelectEventCustomHandler>();
                    }

                    selectEventCustomHandler.BindingDeselectedAction(async (BaseEventData baseEventData) =>
                    {
                        if (!isHidingTip)
                        {
                            isHidingTip = true;

                            await WaitNFrame(2, cancellationToken);

                            if (clickChildDontHide && EventSystem.current.currentSelectedGameObject != null
                                                   && EventSystem.current.currentSelectedGameObject.transform.IsChildOf(
                                                       transform))
                            {
                                EventSystem.current.SetSelectedGameObject(transform.gameObject);
                                isHidingTip = false;
                                return;
                            }

                            transform.DOKill();

                            if (hideWithAnimation)
                            {
                                transform.DOScale(Vector3.zero, scaleTime * 0.5f).OnComplete(() =>
                                {
                                    transform.gameObject.SetActive(false);
                                });
                            }
                            else
                            {
                                transform.gameObject.SetActive(false);
                            }
                        }
                    });
                }
            }
            catch (Exception e)
            {
            }
        }
        
        /// <summary>
        /// 检查RectTransform中是否包含另一个RectTransform
        /// </summary>
        public static bool InScreenRect( this RectTransform self, RectTransform rectTransform ) 
        {

            var rect1 = GetScreenRect( self );
            var rect2 = GetScreenRect( rectTransform );
            return rect1.Overlaps( rect2 );
        }
        
        /// <summary>
        /// 获取屏幕坐标Rect
        /// </summary>
        public static Rect GetScreenRect( this RectTransform self ) 
        {
            var rect = new Rect();
            var canvas = self.GetComponentInParent<Canvas>();
            var camera = canvas.worldCamera;
            if( camera != null )
            {
                var corners = new Vector3[4];
                self.GetWorldCorners( corners );
                rect.min = camera.WorldToScreenPoint( corners[0] );
                rect.max = camera.WorldToScreenPoint( corners[2] );
            }
            
            return rect;
        }
        
        public static async void WaitToPlayAni(Animator animator, string aniName)
        {
            if (animator)
            {
                await UniTask.Yield();
                if (animator)
                    animator.Play(aniName, -1, 0);
            }
        }
        public static void WaitToPlayAni(SkeletonGraphic sgp, string aniName, bool loop = true)
        {
            if (sgp)
            {
                sgp.AnimationState.ClearTracks();
                if (sgp)
                {
                    sgp.AnimationState.SetAnimation(0, aniName, loop);
                    sgp.AnimationState.Update(0);
                }
            }
        }
        
        public static bool IsPlayingAnim(Animator ani, string animationName)
        {
            if (ani)
            {
                return ani.GetCurrentAnimatorClipInfo(0)[0].clip.name == animationName;
            }
            return false;
        }

        public static bool IsPlayingSpineAnim(SkeletonGraphic sgp, string animationName)
        {
            if (sgp)
            {
                TrackEntry currentTrack = sgp.AnimationState.GetCurrent(0);
                return currentTrack != null && currentTrack.Animation.Name == animationName;
            }
            return false;
        }

        public static List<List<int>> GetAllEnum(int all, int need)
        {
            if (all < need || need <= 0)
                return null;

            if (all == need)
            {
                var option = new List<int>();
                for (var i = 1; i <= need; i++)
                {
                    option.Add(i);
                }

                return new List<List<int>>() {option};
            }

            if (need == 1)
            {
                var allResult = new List<List<int>>();
                for (var i = 0; i < all; i++)
                {
                    allResult.Add(new List<int>() {i + 1});
                }

                return allResult;
            }

            List<List<int>> result = new List<List<int>>();

            var subResult1 = GetAllEnum(all - 1, need - 1);
            if (subResult1 != null && subResult1.Count > 0)
            {
                for (var i = 0; i < subResult1.Count; i++)
                {
                    subResult1[i].Add(all);
                }

                result.AddRange(subResult1);
            }

            var subResult2 = GetAllEnum(all - 1, need);
            if (subResult2 != null && subResult2.Count > 0)
            {
                result.AddRange(subResult2);
            }

            return result;
        }

        public static string GetAnimationName(Animator animator)
        {
            if (animator && animator.gameObject.activeInHierarchy)
            {
                var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                if (clipInfo.Length == 0)
                    return string.Empty;
                return clipInfo[0].clip.name;
            }
            return  string.Empty;
        }
        
        
        public static void VibrateMatch()
        {
#if UNITY_ANDROID
            HapticsManager.Vibrate(10);
#else
            HapticsManager.Haptics(HapticTypes.Light);
#endif
        }
        
        
                /// <summary>
        /// 道具类型的时间显示格式统一处理
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static string FormatPropItemTime(long l)
        {
            int hour = 0;
            int minute = 0;
            int second = 0;
            int day = 0;

            l = l < 0 ? 0 : l;
            second = (int) (l / 1000);

            if (second >= 60)
            {
                minute = second / 60;
                second = second % 60;
            }

            if (minute >= 60)
            {
                hour = minute / 60;
                minute = minute % 60;
            }

            if (hour >= 24)
            {
                day = hour / 24;
                hour = hour % 24;
            }

            var d = LocalizationManager.Instance.GetLocalizedString("&key.UI_common_time_d");
            var h = LocalizationManager.Instance.GetLocalizedString("&key.UI_common_time_h");
            var m = LocalizationManager.Instance.GetLocalizedString("&key.UI_common_time_m");
            var s = LocalizationManager.Instance.GetLocalizedString("&key.UI_common_time_s");

            if (day > 0)
            {
                if (hour > 0)
                {
                    return $"{day}{d} {hour}{h}";
                }
                else if (minute >= 30)
                {
                    return $"{day}{d} 1{h}";
                }
                else
                {
                    return $"{day}{d}";
                }
            }
            else
            {
                if (hour >= 1)
                {
                    if (minute > 0)
                    {
                        return $"{hour}{h} {minute}{m}";
                    }
                    else
                    {
                        return $"{hour}{h}";
                    }
                }

                if (minute > 0)
                {
                    if (second > 0)
                    {
                        return $"{minute}{m} {second}{s}";
                    }
                    else
                    {
                        return $"{minute}{m}";
                    }
                }

                return $"{second}{s}";
            }
        }
    }
}