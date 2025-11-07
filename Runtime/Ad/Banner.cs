using System;
using Athana.Api;

namespace Athana
{
    public static class Banner
    {
        internal static Action<AthanaInterface.ProxyAd> onLoadedEvent;
        public static event Action<AthanaInterface.ProxyAd> OnLoadedEvent
        {
            add
            {
                onLoadedEvent += value;
            }
            remove
            {
                onLoadedEvent -= value;
            }
        }

        internal static Action<AthanaInterface.ProxyAd, AthanaInterface.AdError?> onLoadFailedEvent;
        public static event Action<AthanaInterface.ProxyAd, AthanaInterface.AdError?> OnLoadFailedEvent
        {
            add
            {
                onLoadFailedEvent += value;
            }
            remove
            {
                onLoadFailedEvent -= value;
            }
        }

        internal static Action<AthanaInterface.ProxyAd> onDisplayedEvent;
        public static event Action<AthanaInterface.ProxyAd> OnDisplayedEvent
        {
            add
            {
                onDisplayedEvent += value;
            }
            remove
            {
                onDisplayedEvent -= value;
            }
        }

        internal static Action<AthanaInterface.ProxyAd, AthanaInterface.AdError?> onDisplayFailedEvent;
        public static event Action<AthanaInterface.ProxyAd, AthanaInterface.AdError?> OnDisplayFailedEvent
        {
            add
            {
                onDisplayFailedEvent += value;
            }
            remove
            {
                onDisplayFailedEvent -= value;
            }
        }

        internal static Action<AthanaInterface.ProxyAd> onClickEvent;
        public static event Action<AthanaInterface.ProxyAd> OnClickEvent
        {
            add
            {
                onClickEvent += value;
            }
            remove
            {
                onClickEvent -= value;
            }
        }

        internal static Action<AthanaInterface.ProxyAd> onClosedEvent;
        public static event Action<AthanaInterface.ProxyAd> OnClosedEvent
        {
            add
            {
                onClosedEvent += value;
            }
            remove
            {
                onClosedEvent -= value;
            }
        }
    }
}