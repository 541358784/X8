using System.Collections.Generic;
using System.Collections.Specialized;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.SDKEvents;
using Framework;
using Gameplay;
using Google.Protobuf;
using UnityEngine;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class CouponData
{
    public string title;
    public string desc;
    public Dictionary<int, int> Items;
}


public class DeepLinkModel
{
    private static DeepLinkModel instance = null;
    private static readonly object syslock = new object();

    private List<CouponData> CouponList;
    private string couponId;

    public static DeepLinkModel Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syslock)
                {
                    if (instance == null)
                    {
                        instance = new DeepLinkModel();
                    }
                }
            }

            return instance;
        }
    }

    private DeepLinkModel()
    {
        //DebugUtil.Log(CKer.admin, "In DeepLinkModel: init");
        CouponList = new List<CouponData>();
        couponId = "";
    }


    public void OnWorking(DeepLinkEvent deepLinkEvent)
    {
        //DebugUtil.Log(CKer.admin, "In DeepLinkModel: deepLinkEvent.route = {0}", deepLinkEvent.route);
        switch (deepLinkEvent.route)
        {
            case "coupon":
                var couponIdStr = deepLinkEvent.rawData.Get("couponId");

                //DebugUtil.Log(CKer.admin, "In DeepLinkModel: couponId = {0}", couponIdStr);

                if (!string.IsNullOrEmpty(couponIdStr))
                {
                    this.couponId = couponIdStr;
                    //DebugUtil.Log(CKer.admin, "In DeepLinkModel 22: couponId = {0}", couponIdStr);
                    //DebugUtil.Log(CKer.admin, "In DeepLinkModel 22: couponId = {0}", this.couponId);

                    // if (!MyMain.Game.IsInTinyGame())
                    //     TryClaimCoupon();
                }

                break;
        }

        PlayerPrefs.DeleteKey("DeepLinkRawQueryString");
    }

    public void TryClaimCoupon()
    {
        if (PlayerPrefs.HasKey("DeepLinkRawQueryString"))
        {
            string query = PlayerPrefs.GetString("DeepLinkRawQueryString");
            if (string.IsNullOrEmpty(query) && string.IsNullOrEmpty(this.couponId))
            {
                return;
            }

            if (string.IsNullOrEmpty(this.couponId))
            {
                NameValueCollection nvc = DragonU3DSDK.Utils.ParseQueryString(query);

                this.couponId = nvc.Get("couponId");
            }
        }

        //DebugUtil.Log(CKer.admin, "In DeepLinkModel TryClaimCoupon : couponId = {0}", this.couponId);
        if (string.IsNullOrEmpty(this.couponId))
        {
            return;
        }

        //DebugUtil.Log(CKer.admin, "In DeepLinkModel TryClaimCoupon : step = {0}", 1);
        if (!DragonU3DSDK.Account.AccountManager.Instance.HasLogin)
        {
            return;
        }

        //DebugUtil.Log(CKer.admin, "In DeepLinkModel TryClaimCoupon : step = {0}", 2);
        CClaimCoupon getClamCoupon = new CClaimCoupon
        {
            CouponId = couponId
        };
        APIManager.Instance.Send(getClamCoupon,
            (IMessage result) =>
            {
                var data = (SClaimCoupon) result;

                Dictionary<int, int> items = new Dictionary<int, int>();
                for (int index = 0; index < data.Coupon.Rewards.Count; index++)
                {
                    var reward = data.Coupon.Rewards[index];
                    // int rewardCfgIndex = MailModel.Instance.ParseRewardId(reward.RewardType);
                    var rewardId = int.Parse(reward.RewardType);
                    // if (rewardCfgIndex != -1)
                    {
                        int value = (int) reward.RewardValue;
                        items[rewardId] = value;
                    }
                }

                if (items.Count == 0) return;
                CouponList.Add(new CouponData
                {
                    Items = items,
                    title = data.Coupon.Title,
                    desc = data.Coupon.Message,
                });

                //获取成功后couponId置空
                this.couponId = "";

                PlayerPrefs.DeleteKey("DeepLinkRawQueryString");

                // if (!MyMain.Game.IsInTinyGame())
                //     GiveCoupon();
            },
            (ErrorCode errno, string errmsg, IMessage resp) =>
            {
                if (errno == ErrorCode.CouponAlreadyClaimedError || errno == ErrorCode.CouponExpireError ||
                    errno == ErrorCode.CouponNotExistsError)
                {
                    PlayerPrefs.DeleteKey("DeepLinkRawQueryString");
                }
                //DebugUtil.Log(CKer.admin, "deeplink error: " + errmsg);
            }
        );
    }

    public bool GiveCoupon(params object[] action)
    {
        //DebugUtil.Log(CKer.admin, "In DeepLinkModel TryClaimCoupon : step = {0}", 3);
        if (CouponList.Count <= 0)
        {
            return false;
        }

        //DebugUtil.Log(CKer.admin, "In DeepLinkModel TryClaimCoupon : step = {0}", 4);
        var coupon = CouponList[0];
        // UIManager.Instance.OpenCookingWindow("Common/Coupon", UIWindowType.Normal, action)
        //     .GetComponent<CouponController>()
        //     .SetData(
        //         coupon.Items,
        //         new DragonPlus.GameBIManager.ItemChangeReasonArgs
        //         {
        //             // reason = BiEventCooking.Types.ItemChangeReason.Coupon //TODO no this type
        //         },
        //         titleStr: coupon.title,
        //         descStr: coupon.desc
        //     );

        CouponList.Remove(coupon);

        return true;
    }
}