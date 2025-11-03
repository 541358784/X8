/*
 * @file ContactUsModel.js
 * 联系我们
 * @author lu
 */

using System;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Google.Protobuf;

namespace DragonPlus
{
    public class ContactUsModel : Manager<ContactUsModel>
    {
        public SListUserComplainMessage MessageList { get; set; }
        Action ResultCB { get; set; }
        Action ErrorCB { get; set; }

        // 发送的消息缓存,如果成功加到队列
        public UserComplainMessage TempSendMessage { get; set; }


        // 获得消息列表
        public void GetMessageList(Action resultCB, Action errorCB)
        {
            var cListUserComplainMessage = new CListUserComplainMessage();
            ResultCB = resultCB;
            ErrorCB = errorCB;

            // ---测试代码.开始---
            //MessageList = new SListUserComplainMessage();
            //MessageList.Messages.Add(new UserComplainMessage
            //{
            //    PlayerId = 123,
            //    Message = "yes or no",
            //    MessageType = UserComplainMessage.Types.MessageType.Complain,
            //    CreatedAt = (ulong)CommonUtils.GetTimeStamp(),
            //    Email = "aa@qq.com",
            //});
            //MessageList.Messages.Add(new UserComplainMessage
            //{
            //    PlayerId = 123,
            //    Message = "You`s haha",
            //    MessageType = UserComplainMessage.Types.MessageType.Reply,
            //    CreatedAt = (ulong)CommonUtils.GetTimeStamp(),
            //    Email = "aa@qq.com",
            //});
            //resultCB();
            // ---测试代码:结束---


            APIManager.Instance.Send<CListUserComplainMessage, SListUserComplainMessage>(cListUserComplainMessage,
                OnGetMessageListResult, OnGetMessageListError);
        }

        // 获得消息列表成功
        void OnGetMessageListResult(IMessage result)
        {
            MessageList = (SListUserComplainMessage) result;
            ResultCB();
        }

        // 获得消息列表失败
        void OnGetMessageListError(ErrorCode errno, string errmsg, IMessage resp)
        {
            ErrorCB();
            DebugUtil.LogError("ContactUs get messageList failed errno = {0} errmsg = {1}", errno, errmsg);
        }

        // 给客服发送消息
        public void SenMyMessage(string email, string message, Action resultCB, Action errorCB)
        {
            ResultCB = resultCB;
            ErrorCB = errorCB;

            var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();

            var cSendUserComplainMessage = new CSendUserComplainMessage
            {
                Message = new UserComplainMessage
                {
                    Message = message,
                    MessageType = UserComplainMessage.Types.MessageType.Complain,
                    PlayerId = StorageManager.Instance.GetStorage<StorageCommon>().PlayerId,
                    CreatedAt = (ulong) CommonUtils.GetTimeStamp(),
                    Email = email,
                    RevenueUsdCents = storageCommon.RevenueUSDCents,
                    DeviceType = DeviceHelper.GetDeviceType(),
                    DeviceModel = DeviceHelper.GetDeviceModel(),
                    DeviceMemory = DeviceHelper.GetTotalMemory().ToString(),
                    NetworkType = DeviceHelper.GetNetworkStatus().ToString(),
                    ResVersion = storageCommon.ResVersion,
                    NativeVersion = DragonNativeBridge.GetVersionCode().ToString(),
                    Platform = DeviceHelper.GetPlatform(),
                }
            };
            TempSendMessage = cSendUserComplainMessage.Message;
            APIManager.Instance.Send<CSendUserComplainMessage, SSendUserComplainMessage>(cSendUserComplainMessage,
                OnSendMessageResult, OnSendMessageError);
        }

        // 给客服发送消息成功
        void OnSendMessageResult(IMessage result)
        {
            ResultCB();
        }

        // 给客服发送消息失败
        void OnSendMessageError(ErrorCode errno, string errmsg, IMessage resp)
        {
            DebugUtil.LogError("ContactUs send message failed errno = {0} errmsg = {1}", errno, errmsg);
            ErrorCB();
        }

        public string GetSubscribeEmailAddress()
        {
            var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
            if (!string.IsNullOrEmpty(storageCommon.Email))
            {
                return storageCommon.Email;
            }

            if (!string.IsNullOrEmpty(storageCommon.FacebookEmail))
            {
                return storageCommon.FacebookEmail;
            }

            return "";
        }

        public void ForceInit()
        {
        }
    }
}