// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace SEL.Silverlight.Behaviors
{
    public class MultiClickTrigger : EventTriggerBase<UIElement>
    {
        private readonly DispatcherTimer _timer;

        private int _numClicks;
        private Point _firstClickPosition;
        private object _sender;

        public MultiClickTrigger()
        {
            _timer = new DispatcherTimer
                         {
                             Interval = new TimeSpan(0, 0, 0, 0, 200)
                         };

            _timer.Tick += OnTimerTick;
        }

        protected override string GetEventName()
        {
            return "MultiClick";
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            (AssociatedObject as UIElement).MouseLeftButtonDown += OnMouseButtonDown;
        }

        protected override void OnDetaching()
        {
            (AssociatedObject as UIElement).MouseLeftButtonDown -= OnMouseButtonDown;

            if (_timer.IsEnabled)
                _timer.Stop();

            base.OnDetaching();
        }

        private void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(sender as UIElement);

            if (_numClicks == 0)
            {
                ++_numClicks;
                _firstClickPosition = position;
                _sender = sender;
                _timer.Start(); // If there is already a timer running this resets it.
            }
            else
            {
                if (Math.Abs(_firstClickPosition.X - position.X) < 4 &&
                    Math.Abs(_firstClickPosition.Y - position.Y) < 4)
                {
                    ++_numClicks;
                }
                else
                {
                    _numClicks = 1;
                    _firstClickPosition = position;
                    _sender = sender;
                    _timer.Start(); // If there is already a timer running this resets it.
                }
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();

            InvokeActions(Tuple.Create(_numClicks, _firstClickPosition, _sender));

            _numClicks = 0;
            _sender = null;
        }
    }
}