// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Threading;

namespace SEL.Silverlight
{
    public class DispatcherWrapper
    {
        public DispatcherWrapper(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        private readonly Dispatcher _dispatcher;

        public void InvokeAsync(Action action)
        {
            _dispatcher.BeginInvoke(action);
        }
    }

    public static class Dispatching
    {
        /// <summary>
        /// Sleep function that uses a dispatcher timer to not hold up the cpu
        /// </summary>
        public static void Sleep(TimeSpan timeout, Action callback)
        {
            var timer = new DispatcherTimer {Interval = timeout};
            timer.Tick += (s, e) =>
                              {
                                  timer.Stop();
                                  callback();
                              };
            timer.Start();
        }
    }
}