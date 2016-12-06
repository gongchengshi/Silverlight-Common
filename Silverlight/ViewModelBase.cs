// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using SEL.Silverlight;

namespace SEL
{
    [DataContract]
    public class ViewModelBase : ObservableObject
    {
        public ViewModelBase()
        {
            _messengerInstance = new Messenger();
            Dispatcher = new DispatcherWrapper(Deployment.Current.Dispatcher);
        }

        public readonly DispatcherWrapper Dispatcher;

        private static bool? _isInDesignMode;
        public readonly IMessenger _messengerInstance;

        public bool IsInDesignMode
        {
            get { return IsInDesignModeStatic; }
        }

        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
                    _isInDesignMode = DesignerProperties.IsInDesignTool;
                }

                return _isInDesignMode.Value;
            }
        }

        protected IMessenger MessengerInstance
        {
            get { return _messengerInstance ?? Messenger.Default; }
        }
    }
}