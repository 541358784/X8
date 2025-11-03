using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using DragonPlus.Config.TMatch;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.PlayerProperties;
using DragonU3DSDK.Storage;
using Firebase.Crashlytics;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TMatch
{

    
    public static class CommonUtils
    {
        public static Dictionary<string, Color> _cachedColors = new Dictionary<string, Color>();

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

        public static void AddChildTo(Transform parent, Transform obj)
        {
            if (obj != null && parent != null)
            {
                obj.transform.SetParent(parent);
                obj.localPosition = Vector3.zero;
                obj.localRotation = Quaternion.identity;
                obj.localScale = Vector3.one;
            }
        }

        public static Color HexToColor(string hex)
        {
            if (!_cachedColors.ContainsKey(hex))
            {
                var color = hex.PadRight(6, '0').PadRight(8, 'F');

                float[] values = new float[4];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = Convert.ToInt32(color.Substring(i * 2, 2), 16) / 255f;
                }

                _cachedColors.Add(hex, new Color(values[0], values[1], values[2], values[3]));
            }

            return _cachedColors[hex];
        }

        public static long GetTimeStamp()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long) (DateTime.Now - startTime).TotalMilliseconds;
            return timeStamp;
        }

        public static DateTime ConvertFromUnixTimestamp(ulong timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1);
            return origin.AddMilliseconds(timestamp);
        }

        public static long ConvertDateTimeToTimeStamp(DateTime date)
        {
            DateTime startTime = new DateTime(1970, 1, 1);
            long timeStamp = (long) (date - startTime).TotalMilliseconds;
            return timeStamp;
        }

        public static IEnumerator DelayWork(float delayTime, Action action)
        {
            if (action != null)
            {
                yield return new WaitForSeconds(delayTime);
                action?.Invoke();
            }
        }

        public static Tween ScoreTo(float startTime, float endTime, float timer, Action<float> onUpdate = null,
            Action onFinish = null)
        {
            float v = startTime;
            var tween = DOTween.To(x => v = x, startTime, endTime, timer).SetEase(Ease.Linear)
                .OnUpdate(() => { onUpdate?.Invoke(v); }).OnComplete(() => { onFinish?.Invoke(); });
            tween.SetAutoKill();
            return tween;
        }

        public static Tween ScoreTo(int startTime, int endTime, float timer, Action<int> onUpdate = null,
            Action onFinish = null)
        {
            int v = startTime;
            var tween = DOTween.To(x => v = Mathf.FloorToInt(x), startTime, endTime, timer).SetEase(Ease.Linear)
                .OnUpdate(() => { onUpdate?.Invoke(v); }).OnComplete(() => { onFinish?.Invoke(); });
            tween.SetAutoKill();
            return tween;
        }

        public static IEnumerator DelayCall(float delay, Action callback = null)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

        public static Tween DelayedCall(float delay, TweenCallback callback, bool ignoreTimeScale = true)
        {
            return DOVirtual.DelayedCall(delay, () =>
            {
                try
                {
                    callback?.Invoke();
                }
                catch (Exception e)
                {
                    DebugUtil.LogError(string.Format("DelayedCall Exception: {0}", e));
                }
            }, ignoreTimeScale);
        }


        /// <summary>
        /// 验证EMail是否合法
        /// </summary>
        /// <param name="email">要验证的Email</param>
        public static bool IsEmail(string email)
        {
            //如果为空，认为验证不合格
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            if (email.Length > 320) return false;
            if (!email.Contains("@") || !email.Contains(".")) return false;
            var atIndex = email.IndexOf("@");
            var pointIndex = email.IndexOf(".");
            if (atIndex < 1) return false;
            //if (atIndex > pointIndex) return false;
            var frontChars = email.Substring(0, atIndex);
            var endChars = email.Substring(atIndex + 1, email.Length - atIndex - 1);
            if (frontChars.Length > 64 || endChars.Length > 255) return false;
            //清除要验证字符串中的空格
            email = email.Trim();
            //模式字符串
            string pattern = @"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";
            //验证
            return new System.Text.RegularExpressions.Regex(pattern).IsMatch(email);
        }

        public static float GetAnimTime(Animator animator, string playAnimName)
        {
            float length = 0f;
            var controller = animator.runtimeAnimatorController;
            if (controller != null)
            {
                foreach (AnimationClip clip in controller.animationClips)
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

        public static IEnumerator PlayAnimation(Animator animator, string playAnimName, string endAnimName,
            Action action)
        {
            if (animator != null)
            {
                animator.Play(playAnimName);
                if (!string.IsNullOrEmpty(endAnimName) || action != null)
                {
                    float time = GetAnimTime(animator, playAnimName);
                    yield return new WaitForSeconds(time);
                    if (!string.IsNullOrEmpty(endAnimName))
                    {
                        animator.Play(endAnimName);
                    }

                    action?.Invoke();
                }
                else
                {
                    action?.Invoke();
                }
            }
            else
            {
                action?.Invoke();
            }
        }

        // 删除一个节点的全部子节点
        public static void DestroyAllChildren(Transform rootTransform)
        {
            rootTransform?.gameObject.RemoveAllChildren();
        }

        public static Vector2 ScreenToCanvasPos(RectTransform parent, Vector3 screenPos)
        {
            Vector2 position = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPos, UIRoot.Instance.mUICamera,
                out position);
            return position;
        }

        public static Vector2 WorldToCanvasPos(RectTransform parent, Vector3 world)
        {
            //world to screen
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(UIRoot.Instance.mUICamera, world);
            //screen to ui
            Vector2 position = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPos, UIRoot.Instance.mUICamera,
                out position);
            return position;
        }

        // 统一的发奖入口
        // public static void AddReward(ResourceId rewardId, uint value, DragonPlus.GameBIManager.ItemChangeReasonArgs BI,
        //     bool dispatchEvent = true)
        // {
        //     var config = ItemModel.Instance.GetConfigById((int) rewardId);
        //     if (config != null)
        //     {
        //         AddReward(config, value, BI, dispatchEvent);
        //     }
        // }

        // 统一的发奖入口
        // static void AddReward(ItemConfig config, uint value, DragonPlus.GameBIManager.ItemChangeReasonArgs BI, bool dispatchEvent = true)
        // {
        //     switch ((ResourceId) config.id)
        //     {
        //         case ResourceId.Gem:
        //             CurrencyModel.Instance.AddRes(ResourceId.Gem, (int) value, BI, true);
        //             break;
        //         case ResourceId.RedGem:
        //             CurrencyModel.Instance.AddRes(ResourceId.RedGem, (int) value, BI, true);
        //             break;
        //         case ResourceId.Coin:
        //             CurrencyModel.Instance.AddRes(ResourceId.Coin, (int) value, BI);
        //             break;
        //         // case ResourceId.BattlePassEventCoin:
        //         //     UserDataMoudule.AddRes(ResourceId.BattlePassEventCoin, (int)value, BI);
        //         //     break;
        //         // case ResourceId.BattlePassEventCoin_2:
        //         //     UserDataMoudule.AddRes(ResourceId.BattlePassEventCoin_2, (int)value, BI);
        //         //     break;
        //         // case ResourceId.BattlePassEventCoin_3:
        //         //     UserDataMoudule.AddRes(ResourceId.BattlePassEventCoin_3, (int)value, BI);
        //         //     break;
        //         // case ResourceId.BattlePassEventCoin_4:
        //         //     UserDataMoudule.AddRes(ResourceId.BattlePassEventCoin_4, (int)value, BI);
        //         //     break;
        //         // case ResourceId.BattlePassEventCoin_5:
        //         //     UserDataMoudule.AddRes(ResourceId.BattlePassEventCoin_5, (int)value, BI);
        //         // break;
        //         case ResourceId.Energy:
        //             //TODO 每日有个最大领取次数
        //             EnergyModel.Instance.AddEnergy((int) value, BI);
        //             break;
        //         // case ResourceId.SpringDayTicket:
        //         //     UserDataMoudule.AddRes(ResourceId.SpringDayTicket, (int)value, BI);
        //         //     break;
        //         // case ResourceId.SpringDayTicket_Infinity:
        //         //     ActivityTicketModel.Instance.SetTicketUnlimitedTime(value * 1000, BI);
        //         //     break;
        //         // case ResourceId.SummerDayTicket:
        //         //     UserDataMoudule.AddRes(ResourceId.SummerDayTicket, (int)value, BI);
        //         //     break;
        //         // case ResourceId.SummerDayTicket_Infinity:
        //         //     ActivityTicketModel.Instance.SetTicketUnlimitedTime(value * 1000, BI);
        //         //     break;
        //         // case ResourceId.SummerDayCoin:
        //         //     UserDataMoudule.AddRes(ResourceId.SummerDayCoin, (int)value, BI);
        //         //     break;
        //         // case ResourceId.HalloweenCoin:
        //         //     UserDataMoudule.AddRes(ResourceId.HalloweenCoin, (int)value, BI);
        //         //     break;
        //         case ResourceId.Energy_Infinity:
        //             EnergyModel.Instance.SetEnergyUnlimitedTime(value * 1000, BI);
        //             break;
        //         case ResourceId.Star:
        //             CurrencyModel.Instance.AddRes(ResourceId.Star, (int) value, BI);
        //             break;
        //         case ResourceId.NewKey:
        //             CurrencyModel.Instance.AddRes(ResourceId.NewKey, (int) value, BI);
        //             break;
        //         case ResourceId.GachaCoin:
        //             CurrencyModel.Instance.AddRes(ResourceId.GachaCoin, (int) value, BI);
        //             break;
        //         // case ResourceId.RouletteDrawDiamondCoin:
        //         //     UserDataMoudule.AddRes(ResourceId.RouletteDrawDiamondCoin, (int)value, BI);
        //         //     break;
        //         // case ResourceId.RouletteDrawBoostCoin:
        //         //     UserDataMoudule.AddRes(ResourceId.RouletteDrawBoostCoin, (int)value, BI);
        //         //     break;
        //         default:
        //             DebugUtil.LogError($"ResourceId = {((ResourceId) config.id).ToString()}加资源失败");
        //             break;
        //     }
        //
        //     if (dispatchEvent)
        //     {
        //         EventDispatcher.Instance.DispatchEvent(new ResChangeEvent((ResourceId) config.id, ResHostUI.None));
        //     }
        // }

        // 当前奖品个数
        public static long GetRewardById(ResourceId rewardId)
        {
            long result = 0;

            var config = ItemModel.Instance.GetConfigById((int) rewardId);
            if (config == null)
            {
                return result;
            }

            switch ((ResourceId) config.id)
            {
                // case ResourceId.Gem:
                //     result = CurrencyModel.Instance.GetRes(ResourceId.Gem);
                //     break;
                // case ResourceId.RedGem:
                //     result = CurrencyModel.Instance.GetRes(ResourceId.RedGem);
                //     break;
                // case ResourceId.Coin:
                //     result = CurrencyModel.Instance.GetRes(ResourceId.Coin);
                //     break;
                // case ResourceId.BattlePassEventCoin:
                //     result = UserDataMoudule.GetRes(ResourceId.BattlePassEventCoin);
                //     break;
                // case ResourceId.BattlePassEventCoin_2:
                //     result = UserDataMoudule.GetRes(ResourceId.BattlePassEventCoin_2);
                //     break;
                // case ResourceId.BattlePassEventCoin_3:
                //     result = UserDataMoudule.GetRes(ResourceId.BattlePassEventCoin_3);
                //     break;
                // case ResourceId.BattlePassEventCoin_4:
                //     result = UserDataMoudule.GetRes(ResourceId.BattlePassEventCoin_4);
                //     break;
                // case ResourceId.BattlePassEventCoin_5:
                //     result = UserDataMoudule.GetRes(ResourceId.BattlePassEventCoin_5);
                //     break;
                // case ResourceId.Energy:
                //     //TODO 每日有个最大领取次数
                //     result = EnergyModel.Instance.EnergyNumber();
                //     break;
                // case ResourceId.Energy_Infinity:
                //     result = EnergyModel.Instance.EnergyUnlimitedLeftTime();
                //     break;
                // case ResourceId.Star:
                //     result = CurrencyModel.Instance.GetRes(ResourceId.Star);
                //     break;
                // case ResourceId.NewKey:
                //     result = CurrencyModel.Instance.GetRes(ResourceId.NewKey);
                //     break;
            }

            return result;
        }

        // 判断宽屏设备
        public static bool IsWideScreenDevice()
        {
            return ((float) Screen.width / Screen.height <= 1.5f);
        }

        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            if (second == null || first == null) return;
            if (second.Count == 0) return;

            foreach (var item in second)
            {
                if (!first.ContainsKey(item.Key))
                {
                    first.Add(item.Key, item.Value);
                }
            }
        }

        public static string FirstCharToUpper(string str)
        {
            char[] a = str.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        // 把配置里的奖励转换为字典 
        public static Dictionary<ResourceId, int> RewardStringToData(string rewardString, char splitChar1 = '#',
            char splitChar2 = ',')
        {
            var rewards = rewardString.Split(splitChar1);

            var items = new Dictionary<ResourceId, int>();
            for (int index = 0; index < rewards.Length; index++)
            {
                var reward = rewards[index].Split(splitChar2);
                var rewardId = (ResourceId) int.Parse(reward[0]);

                if (rewardId != ResourceId.None)
                    if (reward.Length == 2)
                        items[rewardId] = int.Parse(reward[1]);
                    else if (reward.Length == 3)
                    {
                        int value = Mathf.Min(int.Parse(reward[1]), int.Parse(reward[2]));
                        items[rewardId] = value;
                    }
            }

            return items;
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
        /// <returns></returns>
        public static String FormatLongToTimeStr(long l, bool usingFullFormat = false)
        {
            // String str = "";
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
            if (usingFullFormat)
            {
                return day > 0 ? $"{day}{d} {hour:D2}:{minute:D2}" : $"{hour:D2}:{minute:D2}:{second:D2}";
            }
            else if (day > 0)
            {
                var h = LocalizationManager.Instance.GetLocalizedString("&key.UI_common_time_h");
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
                return $"{hour:D2}:{minute:D2}:{second:D2}";
            }
        }

        public static bool IsLE_16_10()
        {
            float maxR = Mathf.Max(Screen.width, Screen.height);
            float minR = Mathf.Min(Screen.width, Screen.height);
            var ratio = (maxR / minR) <= 1.605f;
            //DebugUtil.LogError($"IsPad {Screen.width} {Screen.height} {isiPad}");
            return ratio;
        }

        public static Sprite GetRewardImage(string imgName, bool useOld = false)
        {
            Sprite sp = ResourcesManager.Instance.GetSpriteVariant(AtlasName.UIIconAllAtlas, imgName);
            return sp;
        }

        public static int StringToInt(string x)
        {
            try
            {
                return Convert.ToInt32(x);
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e.ToString());
                return 0;
            }
        }

        /// <summary>
        /// "1,2,3"字符串转int list
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<int> StringToIntList(string str)
        {
            var result = new List<int>();
            try
            {
                string[] arr = str.Split(',');
                if (arr != null)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        result.Add(StringToInt(arr[i]));
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }

            return result;
        }

        public static T GetOrCreateComponent<T>(GameObject owner) where T : MonoBehaviour
        {
            T targetT = owner.GetComponent<T>();
            if (targetT == null)
                targetT = owner.AddComponent<T>();

            return targetT;
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

        public static void AddChild(Transform parent, Transform obj, bool resetState = true)
        {
            if (obj != null && parent != null)
            {
                obj.transform.SetParent(parent, false);
                if (resetState)
                {
                    obj.localPosition = Vector3.zero;
                    obj.localRotation = Quaternion.identity;
                    obj.localScale = Vector3.one;
                }
            }
        }

        public static bool IsTouchUGUI()
        {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        if (Input.touchCount > 0 ? EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) : EventSystem.current.IsPointerOverGameObject())
#else
            if (EventSystem.current.IsPointerOverGameObject())
#endif
                return true;
            else
                return false;
        }

        public static Sprite LoadDecoItemIconSprite(int worldId, int areaId, string iconName)
        {
            if (string.IsNullOrEmpty(iconName))
            {
                return null;
            }
            else
            {
                // var iconPath = $"Textures/UI/BuildingIcons/{areaId}/{iconName}";
                var iconPath = $"{Decoration.Utils.PathTextureIcon(worldId, areaId)}/{iconName}";
                var icon = ResourcesManager.Instance.LoadResource<Sprite>(iconPath, addToCache: false);

                return icon;
            }
        }

        public static void DicSafeSet<T1, T2>(Dictionary<T1, T2> dic, T1 key, T2 value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }

        public static int GetVersionCode()
        {
            var currentVersionStr = AssetConfigController.Instance.VersionCode;
#if UNITY_ANDROID
            currentVersionStr = AssetConfigController.Instance.VersionCode;
#else
        currentVersionStr = AssetConfigController.Instance.IOSVersionCode;
#endif
            var currentVersion = int.Parse(currentVersionStr);

            return currentVersion;
        }

        public static float GetAnimEventTime(Animator animator, string clipName, string eventName)
        {
            var clips = animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                if (clip.name.Equals(clipName))
                {
                    if (clip.events[0].functionName.Equals(eventName))
                    {
                        var time = clip.events[0].time;
                        return time;
                    }
                }
            }

            return 0f;
        }

        public static string GetFormatTimeString(long seconds)
        {
            return Utils.GetTimeString("%hh:%mm:%ss", Mathf.FloorToInt(seconds));
        }

        /// <summary>
        /// 获取指定图集中的Sprite
        /// </summary>
        /// <param name="atlasName"></param>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public static Sprite GetSprite(string atlasName, string spriteName)
        {
            return ResourcesManager.Instance.GetSprite(atlasName, spriteName);
        }

        /// <summary>
        /// 获取通用图集中的Sprite
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public static Sprite GetCommonSprite(string spriteName)
        {
            return ResourcesManager.Instance.GetSprite(HospitalConst.CommonUIAtlas, spriteName);
        }

        /// <summary>
        /// 获取通用图集中的Sprite
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public static Sprite GetMapCommonSprite(string spriteName)
        {
            return ResourcesManager.Instance.GetSprite(HospitalConst.CommonUIMapAtlas, spriteName);
        }

        /// <summary>
        /// 获取公会图集里的徽章
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        // public static Sprite GetGuildBadgeSprite(int id)
        // {
        //     var config = GuildConfigManager.Instance.BadgeList.Find(item => item.Id == id);
        //     if (config != null)
        //     {
        //         var sprite = GetGuildSprite(config.Icon);
        //         if (sprite != null)
        //             return sprite;
        //     }
        //
        //     return GetGuildSprite("Default");
        // }

        /// <summary>
        /// 获取公会图集里的图片
        /// </summary>
        /// <param name="spriteName">图片名</param>
        /// <returns></returns>
        public static Sprite GetGuildSprite(string spriteName)
        {
            return ResourcesManager.Instance.GetSprite(HospitalConst.GuildAtlas, spriteName);
        }

        /// <summary>
        /// 获取成就图标
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        // public static Sprite GetAchievementIcon(int id)
        // {
        //     var config = CollectConfigManager.Instance.AchievmentsList.Find(item => item.Id == id);
        //     return config == null ? null : GetAchievementIcon(config.RewardIcon);
        // }

        /// <summary>
        /// 获取成就图标
        /// </summary>
        /// <param name="spriteName">Sprite名字</param>
        /// <returns></returns>
        public static Sprite GetAchievementIcon(string spriteName)
        {
            return ResourcesManager.Instance.GetSprite(HospitalConst.Achievement, spriteName);
        }

        /// <summary>
        /// 获取头像图集中的Sprite
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        private static Sprite GetPortraitSprite(string spriteName)
        {
            return ResourcesManager.Instance.GetSprite(HospitalConst.PortraitAtlas, spriteName) ??
                   ResourcesManager.Instance.GetSprite(HospitalConst.PortraitAtlas, "Default");
        }

        /// <summary>
        /// 获取时装图集里的
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public static Sprite GetAvatarSprite(string spriteName)
        {
            return ResourcesManager.Instance.GetSprite(HospitalConst.AvatarAtlas, spriteName);
        }

        /// <summary>
        /// 获取宠物图标
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        private static Sprite GetPetSprite(string spriteName)
        {
            return ResourcesManager.Instance.GetSprite(HospitalConst.PetAtlas, spriteName);
        }

        /// <summary>
        /// 获取宠物图标
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        private static Sprite GetTMatchSprite(string spriteName)
        {
            return ResourcesManager.Instance.GetSpriteVariant(HospitalConst.TMatchAtlas, spriteName);
        }

        /// <summary>
        /// 获取物品图片
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="iconType"></param>
        /// <returns></returns>
        public static Sprite GetItemSprite(int itemId, ResourceIconType iconType = ResourceIconType.Normal)
        {
            var cfg = ItemModel.Instance.GetConfigById(itemId);
            if (cfg == null) return null;

            Func<string, Sprite> getSprite;

            if (itemId >= 100000 && itemId <= 101000)
            {
                getSprite = GetTMatchSprite;
            }
            else
            {
                switch ((ItemType) cfg.type)
                {
                    // case ItemType.Avatar:
                    //     getSprite = GetAvatarSprite;
                    //     break;
                    // case ItemType.Portrait:
                    // case ItemType.PortraitFrame:
                    //     getSprite = GetPortraitSprite;
                    //     break;
                    // case ItemType.Pet:
                    //     getSprite = GetPetSprite;
                    //     break;
                    case ItemType.None:
                    // case ItemType.Currency:
                    // case ItemType.Energy:
                    // case ItemType.Booster:
                    // case ItemType.EventToken:
                    // case ItemType.AddDuration:
                    // case ItemType.AddMaxLimit:
                    default:
                        getSprite = GetCommonSprite;
                        break;
                }
            }

            return iconType switch
            {
                ResourceIconType.Normal => getSprite(cfg.pic_res),
                ResourceIconType.Big => getSprite(cfg.pic_res_big),
                ResourceIconType.Special => getSprite(cfg.pic_res_special),
                _ => getSprite(cfg.pic_res)
            };
        }

        /// <summary>
        /// 获取道具数量描述
        /// </summary>
        /// <param name="itemId">道具Id</param>
        /// <param name="itemCount">道具数量</param>
        /// <returns></returns>
        public static string GetItemText(int itemId, int itemCount)
        {
            var cfg = ItemModel.Instance.GetConfigById(itemId);
            if (cfg == null)
                return $"x{itemCount}";

            switch ((ItemType) cfg.type)
            {
                // case ItemType.AddDuration: //增加持续时间
                //     int time = cfg.param.Length <= 0 ? 0 : cfg.param[1] * itemCount;
                //     return GetLimitTimeNumText(time);
                // case ItemType.AddMaxLimit: //增加上限个数
                //     int count = cfg.param.Length <= 0 ? 0 : cfg.param[1] * itemCount;
                //     return $"+{count * itemCount}";
                case ItemType.TMEnergyInfinity:
                case ItemType.TMLightingInfinity:
                case ItemType.TMClockInfinity:
                case ItemType.TMWeeklyChallengeBuff:
                    return GetLimitTimeNumText(itemCount * cfg.infiniityTime);
                default:
                    return $"x{itemCount}";
            }
        }

        /// <summary>
        /// 获取限时时间显示描述
        /// </summary>
        /// <param name="num">限时时间</param>
        /// <returns></returns>
        public static string GetLimitTimeNumText(int num)
        {
            if (num < 60) return num + "s";

            if (num < 3600)
            {
                var min = num / 60;
                return min.ToString("0.##") + LocalizationManager.Instance.GetLocalizedString("&key.UI_common_time_m");
            }

            var h = num / 3600;
            return h.ToString("0.##") + LocalizationManager.Instance.GetLocalizedString("&key.UI_common_time_h");
        }

        private static Vector3 Bezier(Vector3 startPos, Vector3 controPos, Vector3 endPos, float t)
        {
            return (1 - t) * (1 - t) * startPos + 2 * t * (1 - t) * controPos + t * t * endPos;
        }

        public static Vector3[] Bezier2Path(Vector3 startPos, Vector3 controlPos, Vector3 endPos, int count)
        {
            Vector3[] path = new Vector3[count];
            for (int i = 1; i <= count; i++)
            {
                float t = i / (float) count;
                path[i - 1] = Bezier(startPos, controlPos, endPos, t);
            }

            return path;
        }

        /// <summary>
        /// 飞行物体
        /// </summary>
        /// <param name="flyObj">需要飞行的物体</param>
        /// <param name="destPos">目标点，世界坐标</param>
        /// <param name="isBezier">是否用贝塞尔</param>
        /// <param name="onFinish">完成事件</param>
        public static Tweener FlyObj(Transform flyObj, Vector3 destPos, float timer = 0.5f, Vector2? controlPos = null,
            bool isBezier = true, Action onFinish = null)
        {
            var vDis = destPos - flyObj.position;
            controlPos = controlPos == null
                ? new Vector2(flyObj.position.x + vDis.x * UnityEngine.Random.Range(-0.1f, 0.1f),
                    flyObj.position.y + vDis.y * UnityEngine.Random.Range(0.1f, 0.1f))
                : controlPos;
            var pos = Bezier2Path(flyObj.position, controlPos.Value, destPos, 10);
            Tweener tweener = flyObj.DOPath(pos, timer, isBezier ? PathType.CatmullRom : PathType.Linear)
                .SetEase(Ease.Linear).OnComplete(() => { onFinish?.Invoke(); });
            return tweener;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static object GetProperty<T>(T target, string propertyName)
        {
            System.Type type = target.GetType();
            System.Reflection.PropertyInfo property = type.GetProperty(propertyName);
            if (property == null) return null;
            return property.GetValue(target);
        }

        /// <summary>
        /// 获取字段值
        /// </summary>
        /// <param name="target"></param>
        /// <param name="fieldName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static object GetField<T>(T target, string fieldName)
        {
            System.Type type = target.GetType();
            System.Reflection.FieldInfo property = type.GetField(fieldName);
            if (property == null) return null;
            return property.GetValue(target);
        }

        /// <summary>
        /// 获取执行方法值
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static object GetMethod<T>(T target, string methodName, params object[] args)
        {
            System.Type type = target.GetType();
            System.Reflection.MethodInfo method = type.GetMethod(methodName);
            if (method == null) return null;
            return method.Invoke(target, args);
        }

        /// <summary>
        /// 是否跨天
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static bool IsDayAfter(ulong time)
        {
            var cur = APIManager.Instance.GetServerTime();
            if (cur < time) return false;
            var dateTime = new DateTime(1970, 1, 1).AddMilliseconds(time);
            var curDateTime = new DateTime(1970, 1, 1).AddMilliseconds(cur);
            return dateTime.Year != curDateTime.Year || dateTime.Month != curDateTime.Month ||
                   dateTime.Day != curDateTime.Day;
        }

        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetServerTime()
        {
            return new DateTime(1970, 1, 1).AddMilliseconds(APIManager.Instance.GetServerTime());
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static ulong GetTimeStamp(DateTime dateTime)
        {
            return (ulong) (dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        /// <summary>
        /// 格式化服务器时间
        /// </summary>
        /// <param name="format">格式</param>
        /// <returns></returns>
        public static string FormatServerTime(string format)
        {
            return GetServerTime().ToString(format);
        }

        /// <summary>
        /// 捕获错误日志
        /// </summary>
        /// <param name="log">日志</param>
        public static void CatchErrorLog(string log)
        {
            try
            {
                DebugUtil.LogError(log);
                Crashlytics.LogException(new Exception(log));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static int RandomWeightIndex(List<int> weights)
        {
            if (weights.Count == 0)
                return -1;
            var v = UnityEngine.Random.Range(0, weights.Sum());
            var w = 0;
            for (var i = 0; i < weights.Count; ++i)
            {
                if (weights[i] == 0)
                    continue;

                w += weights[i];
                if (v < w)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// 增加物品（带上线换算，带反馈数据）
        /// </summary>
        /// <param name="lstIds">物品Ids</param>
        /// <param name="lstCounts">物品Counts</param>
        /// <param name="args">Bi</param>
        /// <param name="feedbackList">反馈列表</param>
        /// <param name="feedbackConverter">反馈列表转换器</param>
        /// <typeparam name="T">反馈类型</typeparam>
        public static void AddItemsWithExchangeable<T>(List<int> lstIds, List<int> lstCounts,
            DragonPlus.GameBIManager.ItemChangeReasonArgs args, List<T> feedbackList, CommonIFeedbackItemConverter<T> feedbackConverter)
        {
            for (var index = 0; index < lstIds.Count; index++)
            {
                var id = lstIds[index];
                var count = lstCounts[index];

                var itemCfg = ItemModel.Instance.GetConfigById(id);
                if (null == itemCfg)
                {
                    DebugUtil.LogError("itemCfg is Null " + id);
                    continue;
                }

                //判定是否存在持有上限，如果存在则根据汇率拆分为其他道具
                InnerAddItemsWithExchangeable(id, count, args, feedbackList, feedbackConverter);
            }
        }


        private static void InnerAddItemsWithExchangeable<T>(int id, int count,
            DragonPlus.GameBIManager.ItemChangeReasonArgs? args, List<T> feedbackList, CommonIFeedbackItemConverter<T> feedbackConverter)
        {
            ItemModel.Instance.Add(id, count, args, true);
            Debug.Assert(!((feedbackList != null) ^ (feedbackConverter != null)));
            if (feedbackList != null)
            {
                T feedbackItem = default(T);
                for (int i = 0; i < feedbackList.Count; i++)
                {
                    if (!feedbackConverter.IsMapping(id, feedbackList[i])) continue;
                    feedbackItem = feedbackList[i];
                    break;
                }

                if (feedbackItem != null)
                {
                    feedbackConverter.ConvertAddTo(feedbackItem, count);
                }
                else
                {
                    feedbackList.Add(feedbackConverter.ConvertCreate(id, count));
                }
            }
        }

        public static int RandomOneIndexByWeight<T>(this IEnumerable<T> data, Func<T, int> getWeight)
        {
            int ret = 0;
            var totalWeight = 0;
            foreach (var item in data)
            {
                totalWeight += getWeight(item);
            }

            var randomWeight = UnityEngine.Random.Range(1, totalWeight + 1);
            var curWeight = 0;
            int index = 0;
            foreach (var item in data)
            {
                if (getWeight(item) == 0) continue;
                curWeight += getWeight(item);
                if (curWeight >= randomWeight)
                {
                    ret = index;
                    break;
                }

                index += 1;
            }

            return ret;
        }

        public static void BindDrag(UnityEngine.GameObject go, UnityAction<BaseEventData> action)
        {
            if (go == null) DebugUtil.LogError($"绑定主体为 null");
            var trigger = go.GetOrCreateComponent<EventTrigger>();
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.RemoveAllListeners();
            entry.callback.AddListener(action);
            trigger.triggers.Add(entry);
        }

        // public static bool IsLocalConfigValid(string timeout, int CdTime)
        // {
        //     if (string.IsNullOrEmpty(timeout))
        //     {
        //         return false;
        //     }
        //
        //     long expire = 0;
        //
        //     try
        //     {
        //         expire = long.Parse(timeout);
        //     }
        //     catch (Exception)
        //     {
        //         return false;
        //     }
        //
        //     if (expire + TMatchShopConfigManager.Instance.GlobalConfig.SeverActivityDataFetchCD > Utils.TotalSeconds())
        //     {
        //         return true;
        //     }
        //
        //     return false;
        // }

        public static void AdaptArialFont(Text text)
        {
#if UNITY_EDITOR
            text.font = Resources.Load<Font>("Fonts/CustomArial");
#elif UNITY_IOS
        if (DeviceHelper.s_mainOSVersion == 0 || DeviceHelper.s_mainOSVersion >= 13)
        {
            text.font = Resources.Load<Font>("Fonts/CustomArial");
        }
#endif
        }

        public static void AdaptArialFont(InputField inputField)
        {
            AdaptArialFont(inputField.textComponent);
            var placeholderText = inputField.placeholder as Text;
            if (placeholderText)
            {
                AdaptArialFont(placeholderText);
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

                if (second > 0)
                {
                    return $"{minute}{m} {second}{s}";
                }
                else
                {
                    return $"{minute}{m}";
                }
            }
        }

        public static string ReplaceNewLineSpace(string srcStr)
        {
            return srcStr.Replace(" ", "\u00A0");
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

        /// <summary>
        /// 获取大洲信息
        /// </summary>
        /// <returns></returns>
        // public static CountryToContinent GetContinent(string country = null)
        // {
        //     try
        //     {
        //         country ??= StorageManager.Instance.GetStorage<StorageCommon>().Country;
        //         return GameConfigManager.Instance.CountryToContinentList.Find(item => item.Country == country);
        //     }
        //     catch (Exception e)
        //     {
        //         DebugUtil.LogError(e.Message);
        //     }
        //
        //     return null;
        // }

        /// <summary>
        /// 获取大洲列表
        /// </summary>
        /// <returns></returns>
        // public static List<CountryToContinent> GetContinentList()
        // {
        //     var list = new List<CountryToContinent>();
        //
        //     foreach (var element in GameConfigManager.Instance.CountryToContinentList)
        //     {
        //         if (list.Find(item => item.Continent == element.Continent) != null)
        //             continue;
        //
        //         list.Add(element);
        //     }
        //
        //     return list;
        // }

        /// <summary>
        /// 获取大洲的文本Key
        /// </summary>
        /// <param name="continent">大洲</param>
        /// <returns></returns>
        // public static string GetContinentString(string continent)
        // {
        //     return GameConfigManager.Instance.CountryToContinentList.Find(item => item.Continent == continent)
        //         ?.ContinentString;
        // }

        /// <summary>
        /// 设置宝箱状态
        /// </summary>
        /// <param name="box">宝箱</param>
        /// <param name="type">类型</param>
        public static void SetGuildBoxStatus(Transform box, int index)
        {
            for (var i = 0; i < box.childCount; i++)
            {
                var go = box.GetChild(i).gameObject;
                go.SetActive(go.name == "Icon" + (index + 1));
            }
        }

        public static int GetValue<T>(Dictionary<T, int> dict, T key)
        {
            int value;
            dict.TryGetValue(key, out value);
            return value;
        }

        public static void AddValue<T>(Dictionary<T, int> dict, T key, int value)
        {
            int oldValue;
            dict.TryGetValue(key, out oldValue);
            dict[key] = oldValue + value;
        }

        public static void AddEventTrigger(GameObject go, EventTriggerType triggerType,
            UnityAction<BaseEventData> action)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = triggerType;
            entry.callback.AddListener(action);
            go.GetOrCreateComponent<EventTrigger>().triggers.Add(entry);
        }

        public static void Swap<T>(T[] array, int a, int b)
        {
            T temp = array[a];
            array[a] = array[b];
            array[b] = temp;
        }

        public static void Swap<T>(T[] array, ref int a, ref int b)
        {
            Swap(array, a, b);
            a ^= b;
            b ^= a;
            a ^= b;
        }

        public static float GetScreenRatio()
        {
            float maxR = Mathf.Max(Screen.width, Screen.height);
            float minR = Mathf.Min(Screen.width, Screen.height);
            var ratio = maxR / minR;
            //DebugUtil.LogError($"IsPad {Screen.width} {Screen.height} {isiPad}");
            return ratio;
        }

        public static void NotchAdapte(Transform transform)
        {
            if (transform == null)
                return;

            NotchAdapte(transform as RectTransform);
        }

        public static void NotchAdapte(RectTransform rectTransform)
        {
            if (rectTransform == null)
                return;

            int safeAreaOffset = (int) (Screen.height - Screen.safeArea.yMax);
            if (safeAreaOffset == 0)
                return;

            safeAreaOffset = safeAreaOffset / 2;
            float scaleRatio = 1.0f * 1366 / Screen.height;
            safeAreaOffset = (int) (safeAreaOffset * scaleRatio);
            safeAreaOffset += 15;

            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x,
                rectTransform.anchoredPosition.y - safeAreaOffset);
        }

        public static void AddClickListener(Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
        }

        public static void SetShieldButTime(GameObject gameObject, float time = 0f)
        {
            if (gameObject == null)
                return;

            ShieldButtonOnClick shield = gameObject.GetComponent<ShieldButtonOnClick>();
            if (shield == null)
                return;

            shield.shieldButTime = time;
        }

        public static int RandomIndexByWeight(List<int> weights)
        {
            int sum = weights.Sum();
            int random = Random.Range(1, sum + 1);
            int cur_total = 0;
            for (int i = 0; i < weights.Count; i++)
            {
                cur_total += weights[i];
                if (random <= cur_total)
                    return i;
            }

            return -1;
        }

        public static ulong ProcessServerTimeValidate(ulong serverTime, string from)
        {
            // 2020-01-01 00:00:00 - 2040-01-01 00:00:00 之前的时间视为无效
            if (serverTime < 1577808000000 || serverTime > 2208960000000)
            {
                // DebugUtil.LogError($"[recall] serverTime({serverTime}) from ({from}) is not valid.");
                return 0;
            }

            return serverTime;
        }

        private static ulong heartBeatTime = 0;

        public static ulong GetLastSyncServerTime()
        {
            var t = APIManager.Instance.GetLastSyncServerTime();
            if (t == 0)
            {
                // 若还未与服务器同步过时间，则最多在30秒内立即同步一次
                var timeNow = DeviceHelper.CurrentTimeMillis();
                if (timeNow - heartBeatTime > 30 * 1000)
                {
                    APIManager.Instance.HeartBeat();
                    heartBeatTime = timeNow;
                }
            }

            return ProcessServerTimeValidate(t, "LastSyncServerTime");
        }

        public static ulong GetLastLoginTime()
        {
            var timeLastLogin = PlayerPropertiesManager.Instance.PlayerProperties.LastLoginDate;
            return ProcessServerTimeValidate(timeLastLogin, "LastLoginTime");
        }

        public static void SetShieldButUnEnable(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            ShieldButtonOnClick shield = gameObject.GetComponent<ShieldButtonOnClick>();
            if (shield == null)
                return;

            shield.enabled = false;
            shield.isUse = false;
        }

        public static List<T2> CloneWithJson<T1, T2>(List<T1> src)
        {
            return JsonConvert.DeserializeObject<List<T2>>(JsonConvert.SerializeObject(src));
        }

        public static void PopExitGame(Action onCancel = null)
        {
            FrameWorkUINotice.Open(new UINoticeData
            {
                DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_quit_game_tips_text"),
                OKCallback = () =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    NotificationManager.Instance.RegistLocalNotifications();
                UnityEngine.Application.Quit();
#endif
                },
                OKButtonText = LocalizationManager.Instance.GetLocalizedString("&key.UI_button_yes"),
                CancelButtonText = LocalizationManager.Instance.GetLocalizedString("&key.UI_button_no"),
                HasCancelButton = true,
                CancelCallback = () => onCancel?.Invoke(),
            });
        }

        public static float GetSafeAreaOffset()
        {
            int safeAreaOffset = (int) (Screen.height - Screen.safeArea.yMax);
            if (safeAreaOffset == 0) return 0.0f;

            safeAreaOffset = safeAreaOffset / 2;
            float scaleRatio = UIRoot.Instance.mRootCanvas.GetComponent<CanvasScaler>().referenceResolution.y /
                               Screen.height;
            safeAreaOffset = (int) (safeAreaOffset * scaleRatio);
            safeAreaOffset += 30;
            return -safeAreaOffset;
        }

        public static DragonPlus.Config.TMatch.TMatchConfigManager.ItemColor GetItemColor(
            this DragonPlus.Config.TMatch.Item item)
        {
            if (item.isRed == 1) return DragonPlus.Config.TMatch.TMatchConfigManager.ItemColor.Red;
            else if (item.isPurple == 1) return DragonPlus.Config.TMatch.TMatchConfigManager.ItemColor.Purple;
            else if (item.isWhite == 1) return DragonPlus.Config.TMatch.TMatchConfigManager.ItemColor.White;
            else if (item.isOrange == 1) return DragonPlus.Config.TMatch.TMatchConfigManager.ItemColor.Orange;
            else if (item.isYellow == 1) return DragonPlus.Config.TMatch.TMatchConfigManager.ItemColor.Yellow;
            else if (item.isGreen == 1) return DragonPlus.Config.TMatch.TMatchConfigManager.ItemColor.Green;
            else if (item.isBrown == 1) return DragonPlus.Config.TMatch.TMatchConfigManager.ItemColor.Brown;
            else if (item.isBlue == 1) return DragonPlus.Config.TMatch.TMatchConfigManager.ItemColor.Blue;
            else if (item.isBlack == 1) return DragonPlus.Config.TMatch.TMatchConfigManager.ItemColor.Black;
            else if (item.isPink == 1) return DragonPlus.Config.TMatch.TMatchConfigManager.ItemColor.Pink;
            return DragonPlus.Config.TMatch.TMatchConfigManager.ItemColor.None;
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

        public static string[] TMatchSpriteEndNameByDifficulty = {"_diffnormal", "_diffhard", "_diffsuperhard"};

        public static void TMatchRefreshImageByDifficulty(Transform transform, TMatchDifficulty difficulty)
        {
            Image[] images = transform.GetComponentsInChildren<Image>(true);
            foreach (var p in images)
            {
                if (p.transform.name.EndsWith("_ReplaceDiff") && p.sprite != null)
                {
                    string spriteName = p.sprite.name;
                    foreach (var end in TMatchSpriteEndNameByDifficulty)
                    {
                        int index = spriteName.LastIndexOf(end);
                        if (index > 0)
                        {
                            spriteName = spriteName.Substring(0, index);
                            break;
                        }
                    }

                    spriteName += TMatchSpriteEndNameByDifficulty[(int) difficulty - 1];
                    p.sprite = ResourcesManager.Instance.GetSpriteVariant(HospitalConst.TMatchAtlas, spriteName);
                }
            }
        }

        public static ResourceId GetResouceId(this ItemConfig item)
        {
            if (item.id > 100000 && item.id <= 100015)
                return (ResourceId) item.id;

            return ResourceId.None;
        }

        public static ItemType GetItemType(this ItemConfig item)
        {
            if (item.type > 1000 && item.type <= 1015)
                return (ItemType) item.type;

            return ItemType.None;
        }

        public static ItemInfinityIconType GetItemInfinityIconType(this ItemConfig item)
        {
            return (ItemInfinityIconType) item.infinityIcon;
        }

        public static IAPShopType GetIAPShopType(this Shop shop)
        {
            return (IAPShopType) TMatchConfigManager.Instance.ShopConfigList.Find(item => item.id == shop.id).shopType;
        }

        public static int GetShopBgType(this Shop shop)
        {
            return TMatchConfigManager.Instance.ShopConfigList.Find(item => item.id == shop.id).bgType;
        }

        public static List<int> GetItemIds(this Shop shop)
        {
            return TMatchConfigManager.Instance.ShopConfigList.Find(item => item.id == shop.id).itemId.ToList();
        }

        public static List<int> GetItemCounts(this Shop shop)
        {
            var item = TMatchConfigManager.Instance.ShopConfigList.Find(item => item.id == shop.id);
            if (item.itemCnt == null || item.itemId==null)
                return null;
            return item.itemCnt.ToList();
        }

        public static class GlobalDefines
        {
            public static readonly float DelayCloseDuration = 1;
            public static readonly int OneDayMillSecond = 86400000;
            public static readonly int OneHourMillSecond = 3600000;
        }

        public static string GetUnlimiteLeftTimeString(long leftTime)
        {
            var leftTimeStr = "";
            if (leftTime < GlobalDefines.OneHourMillSecond)
            {
                leftTimeStr = DragonU3DSDK.Utils.GetTimeString("%mm:%ss", (int) (leftTime * 0.001f));
            }
            else if (leftTime >= GlobalDefines.OneHourMillSecond && leftTime < GlobalDefines.OneDayMillSecond)
            {
                var hour = (int) Mathf.Round(leftTime / (GlobalDefines.OneHourMillSecond * 1.0f));
                var h = LocalizationManager.Instance.GetLocalizedString("&key.UI_common_time_h");
                leftTimeStr = hour + h;
            }
            else
            {
                var day = (int) Mathf.Round(leftTime / (GlobalDefines.OneDayMillSecond * 1.0f));
                var d = LocalizationManager.Instance.GetLocalizedString("&key.UI_common_time_d");
                leftTimeStr = day + d;
            }

            return leftTimeStr;
        }

        // 随机奖励，总权重值为100, 权重值和没达到100就可能会没有奖励
        // 没有奖励时返回权重配置长度
        public static int GetRandomWeightResult(List<int> weight)
        {
            var randomInt = UnityEngine.Random.Range(0, 100);
            for (var i = 0; i < weight.Count; i++)
            {
                if (randomInt < weight[i])
                {
                    return i;
                }
                else
                {
                    randomInt -= weight[i];
                }
            }

            return weight.Count;
        }

        
    }
}