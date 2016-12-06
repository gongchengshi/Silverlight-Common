///////////////////////////////////////////////////////////////////////////////
//  COPYRIGHT (c) 2011 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Interactivity;

namespace SEL.Silverlight.Behaviors
{
    public class MouseOverShowBehavior : Behavior<Panel>
    {
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", 
            typeof(UIElement), typeof(MouseOverShowBehavior), new PropertyMetadata(null));

        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            Content.Visibility = Visibility.Collapsed;
            AssociatedObject.Children.Add(Content);
            
            AssociatedObject.MouseEnter += new MouseEventHandler(OnMouseEnter);
            AssociatedObject.MouseLeave += new MouseEventHandler(OnMouseLeave);

            _storyboard = new Storyboard();
            _opacityAnimation = new DoubleAnimation() { Duration = new TimeSpan(0, 0, 0, 0, 300), From = 0.0, To = 1.0 };
            _storyboard.Children.Add(_opacityAnimation);
            Storyboard.SetTarget(_opacityAnimation, Content);
            Storyboard.SetTargetProperty(_opacityAnimation, new PropertyPath("Opacity"));
            _storyboard.Completed += (s, e) =>
            {
                if (Content.Opacity == 0.0)
                {
                    Content.Visibility = Visibility.Collapsed;
                }
            };
        }
        protected override void OnDetaching()
        {
            AssociatedObject.MouseEnter -= new MouseEventHandler(OnMouseEnter);
            AssociatedObject.MouseLeave -= new MouseEventHandler(OnMouseLeave);

            base.OnDetaching();
        }

        private Storyboard _storyboard;
        private DoubleAnimation _opacityAnimation;

        void OnMouseEnter(object sender, MouseEventArgs e)
        {
            Content.Visibility = Visibility.Visible;
            _opacityAnimation.From = 0.0;
            _opacityAnimation.To = 1.0;
            _storyboard.Begin();
        }

        void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _opacityAnimation.From = 1.0;
            _opacityAnimation.To = 0.0;
            _storyboard.Begin();
        }
    }


    public class FadeOnDataContextChanged : Behavior<FrameworkElement>
    {
        public TimeSpan Duration 
        { 
            get { return (TimeSpan)GetValue(DurationProperty); } 
            set { SetValue(DurationProperty, value); } 
        }
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", 
            typeof(TimeSpan), typeof(FadeOnDataContextChanged), new PropertyMetadata(new TimeSpan(0, 0, 0, 1)));

        public object DataContext 
        { 
            get { return GetValue(DataContextProperty); } 
            set { SetValue(DataContextProperty, value); } 
        }
        public static readonly DependencyProperty DataContextProperty = DependencyProperty.Register("Duration", 
            typeof(object), typeof(FadeOnDataContextChanged),
            new PropertyMetadata(null, (o,e) => ((FadeOnDataContextChanged)o).DataContextChanged()));
        

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.DataContext = DataContext;
            
            _storyboard = new Storyboard();
            _opacityAnimation = new DoubleAnimation();
            _storyboard.Children.Add(_opacityAnimation);
            Storyboard.SetTarget(_opacityAnimation, AssociatedObject);
            Storyboard.SetTargetProperty(_opacityAnimation, new PropertyPath("Opacity"));
            _storyboard.Completed += (s, e) =>
                {
                    if (! Object.ReferenceEquals(DataContext, AssociatedObject.DataContext))
                    {
                        AssociatedObject.DataContext = DataContext;
                        _opacityAnimation.From = AssociatedObject.Opacity;                
                        _opacityAnimation.To = 1.0;
                        _opacityAnimation.Duration = new TimeSpan(Duration.Ticks * 4 / 5);
                        _storyboard.Begin();
                    }
                };
        }

        private Storyboard _storyboard;
        private DoubleAnimation _opacityAnimation;

        void DataContextChanged()
        {
            if (_storyboard != null)
            {
                _opacityAnimation.From = AssociatedObject.Opacity;
                _opacityAnimation.To = 0.1;
                _opacityAnimation.Duration = new TimeSpan(Duration.Ticks / 5);
                _storyboard.Begin();
            }
        }
    }

}
