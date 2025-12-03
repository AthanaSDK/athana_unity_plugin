using System;
using System.Collections.Generic;
using UnityEngine;
using Athana.Api;

#nullable enable
namespace Athana.Callbacks
{
    public static class AthanaCallbacks
    {

        private static Action<SdkCallback<AthanaInterface.AccountInfo?>> onCurrentUserResult;
        /// <summary>
        /// 获取当前登入账户信息的回调监听
        /// </summary>
        public static event Action<SdkCallback<AthanaInterface.AccountInfo?>> OnCurrentUserResult
        {
            add
            {
                onCurrentUserResult += value;
            }
            remove
            {
                onCurrentUserResult -= value;
            }
        }

        private static Action<SdkCallback<AthanaInterface.AccountInfo>> onRegistryUserResult;
        /// <summary>
        /// 注册平台用户的回调监听
        /// </summary>
        public static event Action<SdkCallback<AthanaInterface.AccountInfo>> OnRegistryUserResult
        {
            add
            {
                onRegistryUserResult += value;
            }
            remove
            {
                onRegistryUserResult -= value;
            }
        }

        private static Action<SdkCallback<AthanaInterface.AccountInfo>> onSignInResult;
        /// <summary>
        /// 登入的回调监听
        /// </summary>
        public static event Action<SdkCallback<AthanaInterface.AccountInfo>> OnSignInResult
        {
            add
            {
                onSignInResult += value;
            }
            remove
            {
                onSignInResult -= value;
            }
        }

        private static Action<SdkCallback<AthanaInterface.AccountInfo>> onSignInWithUIResult;
        /// <summary>
        /// 使用内置UI登入的回调监听
        /// </summary>
        public static event Action<SdkCallback<AthanaInterface.AccountInfo>> OnSignInWithUIResult
        {
            add
            {
                onSignInWithUIResult += value;
            }
            remove
            {
                onSignInWithUIResult -= value;
            }
        }

        private static Action<SdkCallback<object?>> onSignOutResult;
        /// <summary>
        /// 登出的回调监听
        /// </summary>
        public static event Action<SdkCallback<object?>> OnSignOutResult
        {
            add
            {
                onSignOutResult += value;
            }
            remove
            {
                onSignOutResult -= value;
            }
        }

        private static Action<SdkCallback<object?>> onAccountBindingResult;
        /// <summary>
        /// 绑定三方账号的回调监听
        /// </summary>
        public static event Action<SdkCallback<object?>> OnAccountBindingResult
        {
            add
            {
                onAccountBindingResult += value;
            }
            remove
            {
                onAccountBindingResult -= value;
            }
        }

        private static Action<SdkCallback<object?>> onAccountUnbindResult;
        /// <summary>
        /// 解绑三方账号的回调监听
        /// </summary>
        public static event Action<SdkCallback<object?>> OnAccountUnbindResult
        {
            add
            {
                onAccountUnbindResult += value;
            }
            remove
            {
                onAccountUnbindResult -= value;
            }
        }

        private static Action<SdkCallback<AthanaInterface.TriAccountBindMap>> onQueryAllAccountBindResult;
        /// <summary>
        /// 查询三方账号绑定信息的回调监听
        /// </summary>
        public static event Action<SdkCallback<AthanaInterface.TriAccountBindMap>> OnQueryAllAccountBindResult
        {
            add
            {
                onQueryAllAccountBindResult += value;
            }
            remove
            {
                onQueryAllAccountBindResult -= value;
            }
        }

        private static Action<SdkCallback<List<AthanaInterface.IapProduct>>> onQueryProductsResult;
        /// <summary>
        /// 查询商品信息的回调监听
        /// </summary>
        public static event Action<SdkCallback<List<AthanaInterface.IapProduct>>> OnQueryProductsResult
        {
            add
            {
                onQueryProductsResult += value;
            }
            remove
            {
                onQueryProductsResult -= value;
            }
        }

        private static Action<SdkCallback<object?>> onPurchaseResult;
        /// <summary>
        /// 购买的回调监听
        /// </summary>
        public static event Action<SdkCallback<object?>> OnPurchaseResult
        {
            add
            {
                onPurchaseResult += value;
            }
            remove
            {
                onPurchaseResult -= value;
            }
        }

        private static Action<SdkCallback<List<AthanaInterface.IapPurchase>>> onQueryPurchaseHistoryResult;
        /// <summary>
        /// 查询购买历史记录的回调监听
        /// </summary>
        public static event Action<SdkCallback<List<AthanaInterface.IapPurchase>>> OnQueryPurchaseHistoryResult
        {
            add
            {
                onQueryPurchaseHistoryResult += value;
            }
            remove
            {
                onQueryPurchaseHistoryResult -= value;
            }
        }

        private static Action<SdkCallback<object?>> onVerifyOrderResult;
        /// <summary>
        /// 验证订单的回调监听
        /// </summary>
        public static event Action<SdkCallback<object?>> OnVerifyOrderResult
        {
            add
            {
                onVerifyOrderResult += value;
            }
            remove
            {
                onVerifyOrderResult -= value;
            }
        }

        private static Action<SdkCallback<object?>> onRequestReviewResult;
        /// <summary>
        /// 应用内评价调用结果的回调监听
        /// </summary>
        public static event Action<SdkCallback<object?>> OnRequestReviewResult
        {
            add
            {
                onRequestReviewResult += value;
            }
            remove
            {
                onRequestReviewResult -= value;
            }
        }

        private static Action<SdkCallback<object?>> onUpdateUserInfoResult;
        /// <summary>
        /// 补充用户信息调用结果的回调监听
        /// </summary>
        public static event Action<SdkCallback<object?>> OnUpdateUserInfoResult
        {
            add
            {
                onUpdateUserInfoResult += value;
            }
            remove
            {
                onUpdateUserInfoResult -= value;
            }
        }

        [Serializable]
        private class SdkResult
        {
            public string functionName;
            public string? data;
            public string? message;
            public string? error;
        }

        public class SdkCallback<T>
        {
            public T? data { get; private set; }
            public AthanaInterface.SdkError? error { get; private set; }

            public SdkCallback(T? data, AthanaInterface.SdkError? error = null)
            {
                this.data = data;
                this.error = error;
            }

            public bool isSuccess()
            {
                return error == null;
            }
        }

        // 事件分发
        public static void DispatchEvent(string json)
        {
            AthanaLogger.D("DispatchEvent: RAW DATA(" + json + ")");

            var result = JsonUtility.FromJson<SdkResult>(json);
            if (result == null)
            {
                AthanaLogger.D("Event data decode failed");
                return;
            }
            var funcName = result.functionName;
            string? dataJson = result.data;
            string? message = result.message;
            string? errorJson = result.error;

            if (funcName == "onCurrentUserResult")
            {
                invoke(dataJson, errorJson, funcName, onCurrentUserResult);
            }
            else if (funcName == "onRegistryUserResult")
            {
                invoke(dataJson, errorJson, funcName, onRegistryUserResult);
            }
            else if (funcName == "onSignInResult")
            {
                invoke(dataJson, errorJson, funcName, onSignInResult);
            }
            else if (funcName == "onSignInWithUIResult")
            {
                invoke(dataJson, errorJson, funcName, onSignInWithUIResult);
            }
            else if (funcName == "onSignOutResult")
            {
                invokeNoResultObj(errorJson, funcName, onSignOutResult);
            }
            else if (funcName == "onAccountBindingResult")
            {
                invokeNoResultObj(errorJson, funcName, onAccountBindingResult);
            }
            else if (funcName == "onAccountUnbindResult")
            {
                invokeNoResultObj(errorJson, funcName, onAccountUnbindResult);
            }
            else if (funcName == "onQueryAllAccountBindResult")
            {
                invoke(dataJson, errorJson, funcName, onQueryAllAccountBindResult);
            }
            else if (funcName == "onQueryProductsResult")
            {
                invokeFromList(dataJson, errorJson, funcName, onQueryProductsResult);
            }
            else if (funcName == "onPurchaseResult")
            {
                invokeNoResultObj(errorJson, funcName, onPurchaseResult);
            }
            else if (funcName == "onQueryPurchaseHistoryResult")
            {
                invokeFromList(dataJson, errorJson, funcName, onQueryPurchaseHistoryResult);
            }
            else if (funcName == "onVerifyOrderResult")
            {
                invokeNoResultObj(errorJson, funcName, onVerifyOrderResult);
            }
            else if (funcName == "onRequestReviewResult")
            {
                invokeNoResultObj(errorJson, funcName, onRequestReviewResult);
            }
            else if (funcName == "onUpdateUserInfoResult")
            {
                invokeNoResultObj(errorJson, funcName, onUpdateUserInfoResult);
            }
            else
            {
                AthanaLogger.D("Function not found on this event result");
            }
        }

        public static void SendAdLoadedEvent(AthanaInterface.ProxyAd ad)
        {
            Action<AthanaInterface.ProxyAd>? evt;
            switch (ad.getAdType())
            {
                case AthanaInterface.AdType.Rewarded:
                    evt = RewardedAd.onLoadedEvent;
                    break;
                case AthanaInterface.AdType.Banner:
                    evt = Banner.onLoadedEvent;
                    break;
                case AthanaInterface.AdType.Interstitial:
                    evt = InterstitialAd.onLoadedEvent;
                    break;
                case AthanaInterface.AdType.AppOpen:
                    evt = AppOpenAd.onLoadedEvent;
                    break;
                default:
                    evt = null;
                    break;
            }
            if (evt == null)
            {
                return;
            }
            InvokeEvent(evt, ad, "onAdLoadedEvent");
        }

        public static void SendAdLoadFailedEvent(AthanaInterface.ProxyAd ad, AthanaInterface.AdError? error)
        {
            Action<AthanaInterface.ProxyAd, AthanaInterface.AdError?>? evt;
            switch (ad.getAdType())
            {
                case AthanaInterface.AdType.Rewarded:
                    evt = RewardedAd.onLoadFailedEvent;
                    break;
                case AthanaInterface.AdType.Banner:
                    evt = Banner.onLoadFailedEvent;
                    break;
                case AthanaInterface.AdType.Interstitial:
                    evt = InterstitialAd.onLoadFailedEvent;
                    break;
                case AthanaInterface.AdType.AppOpen:
                    evt = AppOpenAd.onLoadFailedEvent;
                    break;
                default:
                    evt = null;
                    break;
            }
            if (evt == null)
            {
                return;
            }
            InvokeEvent(evt, ad, error, "onAdLoadFailedEvent");
        }

        public static void SendAdDisplayedEvent(AthanaInterface.ProxyAd ad)
        {
            Action<AthanaInterface.ProxyAd>? evt;
            switch (ad.getAdType())
            {
                case AthanaInterface.AdType.Rewarded:
                    evt = RewardedAd.onDisplayedEvent;
                    break;
                case AthanaInterface.AdType.Banner:
                    evt = Banner.onDisplayedEvent;
                    break;
                case AthanaInterface.AdType.Interstitial:
                    evt = InterstitialAd.onDisplayedEvent;
                    break;
                case AthanaInterface.AdType.AppOpen:
                    evt = AppOpenAd.onDisplayedEvent;
                    break;
                default:
                    evt = null;
                    break;
            }
            if (evt == null)
            {
                return;
            }
            InvokeEvent(evt, ad, "onAdDisplayedEvent");
        }

        public static void SendAdDisplayFailedEvent(AthanaInterface.ProxyAd ad, AthanaInterface.AdError? error)
        {
            Action<AthanaInterface.ProxyAd, AthanaInterface.AdError?>? evt;
            switch (ad.getAdType())
            {
                case AthanaInterface.AdType.Rewarded:
                    evt = RewardedAd.onDisplayFailedEvent;
                    break;
                case AthanaInterface.AdType.Banner:
                    evt = Banner.onDisplayFailedEvent;
                    break;
                case AthanaInterface.AdType.Interstitial:
                    evt = InterstitialAd.onDisplayFailedEvent;
                    break;
                case AthanaInterface.AdType.AppOpen:
                    evt = AppOpenAd.onDisplayFailedEvent;
                    break;
                default:
                    evt = null;
                    break;
            }
            if (evt == null)
            {
                return;
            }
            InvokeEvent(evt, ad, error, "onAdDisplayFailedEvent");
        }

        public static void SendAdRewardedEvent(AthanaInterface.ProxyAd ad)
        {
            Action<AthanaInterface.ProxyAd> evt = RewardedAd.onRewardedEvent;
            InvokeEvent(evt, ad, "onAdRewardedEvent");
        }

        public static void SendAdClickEvent(AthanaInterface.ProxyAd ad)
        {
            Action<AthanaInterface.ProxyAd>? evt;
            switch (ad.getAdType())
            {
                case AthanaInterface.AdType.Rewarded:
                    evt = RewardedAd.onClickEvent;
                    break;
                case AthanaInterface.AdType.Banner:
                    evt = Banner.onClickEvent;
                    break;
                case AthanaInterface.AdType.Interstitial:
                    evt = InterstitialAd.onClickEvent;
                    break;
                case AthanaInterface.AdType.AppOpen:
                    evt = AppOpenAd.onClickEvent;
                    break;
                default:
                    evt = null;
                    break;
            }
            if (evt == null)
            {
                return;
            }
            InvokeEvent(evt, ad, "onAdClickEvent");
        }

        public static void SendAdClosedEvent(AthanaInterface.ProxyAd ad)
        {
            Action<AthanaInterface.ProxyAd>? evt;
            switch (ad.getAdType())
            {
                case AthanaInterface.AdType.Rewarded:
                    evt = RewardedAd.onClosedEvent;
                    break;
                case AthanaInterface.AdType.Banner:
                    evt = Banner.onClosedEvent;
                    break;
                case AthanaInterface.AdType.Interstitial:
                    evt = InterstitialAd.onClosedEvent;
                    break;
                case AthanaInterface.AdType.AppOpen:
                    evt = AppOpenAd.onClosedEvent;
                    break;
                default:
                    evt = null;
                    break;
            }
            if (evt == null)
            {
                return;
            }
            InvokeEvent(evt, ad, "onAdClosedEvent");
        }


        private static void invokeFromList<T>(string? dataJson, string? errorJson, string funcName, Action<SdkCallback<List<T>>> act)
        {
            if (errorJson != null)
            {
                // 调用异常
                AthanaInterface.SdkError error = JsonUtility.FromJson<AthanaInterface.SdkError>(errorJson);
                List<T>? data = null;
                var paramData = new SdkCallback<List<T>>(data, error);
                InvokeEvent(act, paramData, funcName);
            }
            else
            {
                var wrapper = JsonUtility.FromJson<ListWrapper<T>>(dataJson);
                var paramData = new SdkCallback<List<T>>(wrapper.items);
                InvokeEvent(act, paramData, funcName);
            }
        }

        private static void invoke<T>(string? dataJson, string? errorJson, string funcName, Action<SdkCallback<T>> act)
        {
            if (errorJson != null)
            {
                // 调用异常
                AthanaInterface.SdkError error = JsonUtility.FromJson<AthanaInterface.SdkError>(errorJson);
                T? data = default;
                var paramData = new SdkCallback<T>(data, error);
                InvokeEvent(act, paramData, funcName);
            }
            else
            {
                T data = JsonUtility.FromJson<T>(dataJson);
                var paramData = new SdkCallback<T>(data);
                InvokeEvent(act, paramData, funcName);
            }
        }

        private static void invokeNoResultObj(string? errorJson, string funcName, Action<SdkCallback<object?>> act)
        {
            if (errorJson != null)
            {
                // 调用异常
                AthanaInterface.SdkError error = JsonUtility.FromJson<AthanaInterface.SdkError>(errorJson);
                var paramData = new SdkCallback<object?>(null, error);
                InvokeEvent(act, paramData, funcName);
            }
            else
            {
                // 调用成功
                var paramData = new SdkCallback<object?>(null);
                InvokeEvent(act, paramData, funcName);
            }
        }

        [Serializable]
        private class ListWrapper<T>
        {
            public List<T> items;
        }

        private static void InvokeEvent(Action evt, string eventName)
        {
            if (!CanInvokeEvent(evt)) return;

            try
            {
                evt();
            }
            catch (Exception exception)
            {
                AthanaLogger.E("Failed to invoke(" + eventName + ")");
                AthanaLogger.LogException(exception);
            }
        }

        private static void InvokeEvent<T>(Action<T> evt, T param, string eventName)
        {
            if (!CanInvokeEvent(evt)) return;

            try
            {
                evt(param);
            }
            catch (Exception exception)
            {
                AthanaLogger.E("Failed to invoke(" + eventName + ")");
                AthanaLogger.LogException(exception);
            }
        }

        private static void InvokeEvent<T, E>(Action<T, E> evt, T param1, E param2, string eventName)
        {
            if (!CanInvokeEvent(evt)) return;

            try
            {
                evt(param1, param2);
            }
            catch (Exception exception)
            {
                AthanaLogger.E("Failed to invoke(" + eventName + ")");
                AthanaLogger.LogException(exception);
            }
        }

        private static bool CanInvokeEvent(Delegate evt)
        {
            if (evt == null) return false;

            // Check that publisher is not over-subscribing
            if (evt.GetInvocationList().Length > 5)
            {
                AthanaLogger.D("Ads Event (" + evt + ") has over 5 subscribers.");
            }

            return true;
        }

    }
}