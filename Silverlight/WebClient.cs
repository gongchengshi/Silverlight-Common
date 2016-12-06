// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Threading;

namespace SEL.Silverlight
{
    public abstract class WebClient
    {
        public static void DefaultErrorHandler(Exception ex)
        {
#if DEBUG
            // This is where web service exceptions are handled
            Debug.WriteLine(ex);
            Debugger.Break();
#endif
            Debug.Assert(false, ex.Message);
        }

        private readonly Dispatcher _dispatcher = Deployment.Current.Dispatcher;
        private readonly Action<Exception> _errorHandler = DefaultErrorHandler;

        public void HandleEnd<TCallback>(Action<TCallback> callback, Func<IAsyncResult, TCallback> op, IAsyncResult a)
        {
            try
            {
                _dispatcher.BeginInvoke(callback, op(a));
            }
            catch (Exception ex)
            {
                _errorHandler(ex);
                throw;
            }
        }

        /// <summary>
        /// Used for web service methods that have no return value.
        /// </summary>
        public void HandleEnd(Action callback, Action<IAsyncResult> op, IAsyncResult a)
        {
            try
            {
                op(a);
                _dispatcher.BeginInvoke(callback);
            }
            catch (Exception ex)
            {
                _errorHandler(ex);
                throw;
            }
        }
    }

    public class WebClient<T> : WebClient
    {
        public WebClient(string webServiceHostPath)
        {
            var httpTransport = (Application.Current.Host.Source.Scheme == Uri.UriSchemeHttps)
                                    ? new HttpsTransportBindingElement()
                                    : new HttpTransportBindingElement();

            httpTransport.MaxBufferSize = 2147483647;
            httpTransport.MaxReceivedMessageSize = 2147483647;
            
            var binding = new CustomBinding(new BinaryMessageEncodingBindingElement(), httpTransport);

#if DEBUG
            binding.OpenTimeout = TimeSpan.FromMinutes(10);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            binding.SendTimeout = TimeSpan.FromMinutes(10);
#endif

            var uri = new Uri(Application.Current.Host.Source, webServiceHostPath).AbsolutePath;
            var channelFactory = new ChannelFactory<T>(binding, new EndpointAddress(uri));

            _webService = channelFactory.CreateChannel();
        }

        protected T _webService;
    }

    /// <summary>
    /// Used for web services that have a PolledDuplex callback contract
    /// </summary>
    public class WebClient<TWebService, TCallbackContract> : WebClient where TCallbackContract : new()
    {
        public WebClient(string webServiceHostPath)
        {
            _callbackContract = new TCallbackContract();

            var securityMode = (Application.Current.Host.Source.Scheme == Uri.UriSchemeHttps)
                                   ? PollingDuplexHttpSecurityMode.Transport
                                   : PollingDuplexHttpSecurityMode.None;

            var binding = new PollingDuplexHttpBinding(securityMode, PollingDuplexMode.MultipleMessagesPerPoll);

            binding.MaxBufferSize = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;

#if DEBUG
            binding.OpenTimeout = TimeSpan.FromMinutes(10);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            binding.SendTimeout = TimeSpan.FromMinutes(10);
#endif

            var uri = new Uri(Application.Current.Host.Source, webServiceHostPath).AbsolutePath;

            var instanceContext = new InstanceContext(_callbackContract);
            var channelFactory = new DuplexChannelFactory<TWebService>(instanceContext, binding,
                                                                       new EndpointAddress(uri));

            _webService = channelFactory.CreateChannel();
        }

        protected TWebService _webService;
        protected TCallbackContract _callbackContract;
    }
}