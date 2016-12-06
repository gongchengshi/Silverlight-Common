//////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SEL.Silverlight
{
    /// <summary>
    /// An updated version of the standard CollapsingGridSplitter control that includes a centered handle
    /// which allows complete collapsing and expanding of the appropriate grid column or row.
    /// </summary>
    [TemplatePart(Name = ElementHorizontalHandleName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = ElementVerticalHandleName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = ElementHorizontalTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ElementVerticalTemplateName, Type = typeof(FrameworkElement))]
    public class CollapsingGridSplitter : GridSplitter
    {
        /// <summary>
        /// Initializes a new instance of the CollapsingGridSplitter class,
        /// which inherits from System.Windows.Controls.CollapsingGridSplitter.
        /// </summary>
        public CollapsingGridSplitter()
        {
            // Set default values
            DefaultStyleKey = typeof(CollapsingGridSplitter);

            CollapseMode = GridSplitterCollapseMode.None;
            IsAnimated = true;
            LayoutUpdated += delegate { _gridCollapseDirection = GetCollapseDirection(); };

            // All CollapsingGridSplitter visual states are handled by the parent GridSplitter class.
        }

        #region Template

        private const string ElementHorizontalHandleName = "HorizontalGridSplitterHandle";
        private const string ElementVerticalHandleName = "VerticalGridSplitterHandle";
        private const string ElementHorizontalTemplateName = "HorizontalTemplate";
        private const string ElementVerticalTemplateName = "VerticalTemplate";
        private const string ElementGridsplitterBackground = "GridSplitterBackground";

        private ToggleButton _elementHorizontalGridSplitterButton;
        private ToggleButton _elementVerticalGridSplitterButton;
        private Rectangle _elementGridSplitterBackground;

        /// <summary>
        /// This method is called when the tempalte should be applied to the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _elementHorizontalGridSplitterButton = GetTemplateChild(ElementHorizontalHandleName) as ToggleButton;
            _elementVerticalGridSplitterButton = GetTemplateChild(ElementVerticalHandleName) as ToggleButton;
            _elementGridSplitterBackground = GetTemplateChild(ElementGridsplitterBackground) as Rectangle;

            // Wire up the Checked and Unchecked events of the HorizontalGridSplitterHandle.
            if (_elementHorizontalGridSplitterButton != null)
            {
                _elementHorizontalGridSplitterButton.Checked += GridSplitterButton_Checked;
                _elementHorizontalGridSplitterButton.Unchecked += GridSplitterButton_Unchecked;
            }

            // Wire up the Checked and Unchecked events of the VerticalGridSplitterHandle.
            if (_elementVerticalGridSplitterButton != null)
            {
                _elementVerticalGridSplitterButton.Checked += GridSplitterButton_Checked;
                _elementVerticalGridSplitterButton.Unchecked += GridSplitterButton_Unchecked;
            }

            // Set default direction since we don't have all the components layed out yet.
            _gridCollapseDirection = GridCollapseDirection.Auto;

            // Directely call these events so design-time view updates appropriately
            OnCollapseModeChanged(CollapseMode);
            OnIsCollapsedChanged(IsCollapsed);
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets a value that indicates the CollapseMode.
        /// </summary>
        public GridSplitterCollapseMode CollapseMode
        {
            get { return (GridSplitterCollapseMode)GetValue(CollapseModeProperty); }
            set { SetValue(CollapseModeProperty, value); }
        }
        /// <summary>
        /// Identifies the CollapseMode dependency property
        /// </summary>
        public static readonly DependencyProperty CollapseModeProperty =
            DependencyProperty.Register(
                "CollapseMode",
                typeof(GridSplitterCollapseMode),
                typeof(CollapsingGridSplitter),
                new PropertyMetadata(GridSplitterCollapseMode.None, OnCollapseModePropertyChanged));

        /// <summary>
        /// Gets or sets the style that customizes the appearance of the horizontal handle 
        /// that is used to expand and collapse the CollapsingGridSplitter.
        /// </summary>
        public Style HorizontalHandleStyle
        {
            get { return (Style)GetValue(HorizontalHandleStyleProperty); }
            set { SetValue(HorizontalHandleStyleProperty, value); }
        }
        /// <summary>
        /// Identifies the HorizontalHandleStyle dependency property
        /// </summary>
        public static readonly DependencyProperty HorizontalHandleStyleProperty =
            DependencyProperty.Register(
                "HorizontalHandleStyle",
                typeof(Style),
                typeof(CollapsingGridSplitter),
                null);

        ///<summary>
        /// Gets or sets the style that customizes the appearance of the vertical handle 
        /// that is used to expand and collapse the CollapsingGridSplitter.
        /// </summary>
        public Style VerticalHandleStyle
        {
            get { return (Style)GetValue(VerticalHandleStyleProperty); }
            set { SetValue(VerticalHandleStyleProperty, value); }
        }
        /// <summary>
        /// Identifies the VerticalHandleStyle dependency property
        /// </summary>
        public static readonly DependencyProperty VerticalHandleStyleProperty =
            DependencyProperty.Register(
                "VerticalHandleStyle",
                typeof(Style),
                typeof(CollapsingGridSplitter),
                null);

        /// <summary>
        /// Gets or sets a value that indicates if the collapse and
        /// expanding actions should be animated.
        /// </summary>
        public bool IsAnimated
        {
            get { return (bool)GetValue(IsAnimatedProperty); }
            set { SetValue(IsAnimatedProperty, value); }
        }
        /// <summary>
        /// Identifies the VerticalHandleStyle dependency property
        /// </summary>
        public static readonly DependencyProperty IsAnimatedProperty =
            DependencyProperty.Register(
                "IsAnimated",
                typeof(bool),
                typeof(CollapsingGridSplitter),
                null);

        /// <summary>
        /// Gets or sets a value that indicates if the target column is 
        /// currently collapsed.
        /// </summary>
        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }
        /// <summary>
        /// Identifies the IsCollapsed dependency property
        /// </summary>
        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register(
                "IsCollapsed",
                typeof(bool),
                typeof(CollapsingGridSplitter),
                new PropertyMetadata(OnIsCollapsedPropertyChanged));

        /// <summary>
        /// This is an optional field that can be assigned to and bound to
        /// when the right panel grid splitter style is used
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                "Header", 
                typeof(string), 
                typeof(CollapsingGridSplitter), 
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// This is an optional field that can be assigned to and bound to
        /// when the right panel grid splitter style is used
        /// </summary>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                "Content", 
                typeof(object), 
                typeof(CollapsingGridSplitter), 
                new PropertyMetadata(null));

        /// <summary>
        /// The IsCollapsed property porperty changed handler.
        /// </summary>
        /// <param name="d">CollapsingGridSplitter that changed IsCollapsed.</param>
        /// <param name="e">An instance of DependencyPropertyChangedEventArgs.</param>
        private static void OnIsCollapsedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {            
            (d as CollapsingGridSplitter).OnIsCollapsedChanged((bool)e.NewValue);
        }

        /// <summary>
        /// The CollapseModeProperty property changed handler.
        /// </summary>
        /// <param name="d">CollapsingGridSplitter that changed IsCollapsed.</param>
        /// <param name="e">An instance of DependencyPropertyChangedEventArgs.</param>
        private static void OnCollapseModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CollapsingGridSplitter).OnCollapseModeChanged((GridSplitterCollapseMode)e.NewValue);
        }

        #endregion

        #region Local Members

        private GridCollapseDirection _gridCollapseDirection = GridCollapseDirection.Auto;
        private GridLength _savedGridLength;
        private double _savedActualValue;
        private const double _animationTimeMillis = 200;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Handles the property change event of the IsCollapsed property.
        /// </summary>
        /// <param name="isCollapsed">The new value for the IsCollapsed property.</param>
        protected virtual void OnIsCollapsedChanged(bool isCollapsed)
        {
            // Determine if we are dealing with a vertical or horizontal splitter.
            if (_gridCollapseDirection == GridCollapseDirection.Rows)
            {
                if (_elementHorizontalGridSplitterButton != null)
                {
                    // Sets the target ToggleButton's IsChecked property equal
                    // to the provided isCollapsed property.
                    _elementHorizontalGridSplitterButton.IsChecked = isCollapsed;
                }
            }
            else
            {
                if (_elementVerticalGridSplitterButton != null)
                {
                    // Sets the target ToggleButton's IsChecked property equal
                    // to the provided isCollapsed property.
                    _elementVerticalGridSplitterButton.IsChecked = isCollapsed;
                }
            }
        }

        /// <summary>
        /// Handles the property change event of the CollapseMode property.
        /// </summary>
        /// <param name="collapseMode">The new value for the CollapseMode property.</param>
        protected virtual void OnCollapseModeChanged(GridSplitterCollapseMode collapseMode)
        {
            // Hide the handles if the CollapseMode is set to None.
            if (collapseMode == GridSplitterCollapseMode.None)
            {
                if (_elementHorizontalGridSplitterButton != null)
                {
                    _elementHorizontalGridSplitterButton.Visibility = Visibility.Collapsed;
                }
                if (_elementVerticalGridSplitterButton != null)
                {
                    _elementVerticalGridSplitterButton.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                // Ensure the handles are Visible.
                if (_elementHorizontalGridSplitterButton != null)
                {
                    _elementHorizontalGridSplitterButton.Visibility = Visibility.Visible;
                }
                if (_elementVerticalGridSplitterButton != null)
                {
                    _elementVerticalGridSplitterButton.Visibility = Visibility.Visible;
                }

                //TODO:  Add in error handling if current template does not include an existing ScaleTransform.

                // Rotate the direction that the handle is facing depending on the CollapseMode.
                if (collapseMode == GridSplitterCollapseMode.Previous)
                {
                    if (_elementHorizontalGridSplitterButton != null)
                    {
                        _elementHorizontalGridSplitterButton.RenderTransform.SetValue(ScaleTransform.ScaleYProperty, -1.0);
                    }
                    if (_elementVerticalGridSplitterButton != null)
                    {
                        _elementVerticalGridSplitterButton.RenderTransform.SetValue(ScaleTransform.ScaleXProperty, -1.0);
                    }
                }
                else if (collapseMode == GridSplitterCollapseMode.Next)
                {
                    if (_elementHorizontalGridSplitterButton != null)
                    {
                        _elementHorizontalGridSplitterButton.RenderTransform.SetValue(ScaleTransform.ScaleYProperty, 1.0);
                    }
                    if (_elementVerticalGridSplitterButton != null)
                    {
                        _elementVerticalGridSplitterButton.RenderTransform.SetValue(ScaleTransform.ScaleXProperty, 1.0);
                    }
                }

            }

        }

        #endregion

        #region Collapse and Expand Methods

        /// <summary>
        /// Collapses the target ColumnDefinition or RowDefinition.
        /// </summary>
        private void Collapse()
        {
            var parentGrid = Parent as Grid;

            if (_gridCollapseDirection == GridCollapseDirection.Rows)
            {
                // Get the index of the row containing the splitter
                var splitterIndex = (int)GetValue(Grid.RowProperty);

                // Determing the curent CollapseMode
                if (CollapseMode == GridSplitterCollapseMode.Next)
                {
                    // Save the next rows Height information
                    _savedGridLength = parentGrid.RowDefinitions[splitterIndex + 1].Height;
                    _savedActualValue = parentGrid.RowDefinitions[splitterIndex + 1].ActualHeight;

                    // Collapse the next row
                    if (IsAnimated)
                        AnimateCollapse(parentGrid.RowDefinitions[splitterIndex + 1]);
                    else
                        parentGrid.RowDefinitions[splitterIndex + 1].SetValue(RowDefinition.HeightProperty, new GridLength(0));
                }
                else
                {
                    // Save the previous row's Height information
                    _savedGridLength = parentGrid.RowDefinitions[splitterIndex - 1].Height;
                    _savedActualValue = parentGrid.RowDefinitions[splitterIndex - 1].ActualHeight;

                    // Collapse the previous row
                    if (IsAnimated)
                        AnimateCollapse(parentGrid.RowDefinitions[splitterIndex - 1]);
                    else
                        parentGrid.RowDefinitions[splitterIndex - 1].SetValue(RowDefinition.HeightProperty, new GridLength(0));
                }
            }
            else
            {
                // Get the index of the column containing the splitter
                var splitterIndex = (int)GetValue(Grid.ColumnProperty);

                // Determing the curent CollapseMode
                if (CollapseMode == GridSplitterCollapseMode.Next)
                {
                    // Save the next column's Width information
                    _savedGridLength = parentGrid.ColumnDefinitions[splitterIndex + 1].Width;
                    _savedActualValue = parentGrid.ColumnDefinitions[splitterIndex + 1].ActualWidth;

                    // Collapse the next column
                    if (IsAnimated)
                        AnimateCollapse(parentGrid.ColumnDefinitions[splitterIndex + 1]);
                    else
                        parentGrid.ColumnDefinitions[splitterIndex + 1].SetValue(ColumnDefinition.WidthProperty, new GridLength(0));
                }
                else
                {
                    // Save the previous column's Width information
                    _savedGridLength = parentGrid.ColumnDefinitions[splitterIndex - 1].Width;
                    _savedActualValue = parentGrid.ColumnDefinitions[splitterIndex - 1].ActualWidth;

                    // Collapse the previous column
                    if (IsAnimated)
                        AnimateCollapse(parentGrid.ColumnDefinitions[splitterIndex - 1]);
                    else
                        parentGrid.ColumnDefinitions[splitterIndex - 1].SetValue(ColumnDefinition.WidthProperty, new GridLength(0));
                }
            }
        }

        /// <summary>
        /// Expands the target ColumnDefinition or RowDefinition.
        /// </summary>
        private void Expand()
        {
            var parentGrid = Parent as Grid;

            if (_gridCollapseDirection == GridCollapseDirection.Rows)
            {
                // Get the index of the row containing the splitter
                var splitterIndex = (int)GetValue(Grid.RowProperty);

                // Determine the curent CollapseMode
                if (CollapseMode == GridSplitterCollapseMode.Next)
                {
                    // Expand the next row
                    if (IsAnimated)
                        AnimateExpand(parentGrid.RowDefinitions[splitterIndex + 1]);
                    else
                    {
                        parentGrid.RowDefinitions[splitterIndex + 1].SetValue(RowDefinition.HeightProperty, _savedGridLength);
                    }
                }
                else
                {
                    // Expand the previous row
                    if (IsAnimated)
                        AnimateExpand(parentGrid.RowDefinitions[splitterIndex - 1]);
                    else
                    {
                        parentGrid.RowDefinitions[splitterIndex - 1].SetValue(RowDefinition.HeightProperty, _savedGridLength);
                    }
                }
            }
            else
            {
                // Get the index of the column containing the splitter
                var splitterIndex = (int)GetValue(Grid.ColumnProperty);

                // Determine the curent CollapseMode
                if (CollapseMode == GridSplitterCollapseMode.Next)
                {
                    // Expand the next column
                    if (IsAnimated)
                        AnimateExpand(parentGrid.ColumnDefinitions[splitterIndex + 1]);
                    else
                    {
                        parentGrid.ColumnDefinitions[splitterIndex + 1].SetValue(ColumnDefinition.WidthProperty, _savedGridLength);
                    }
                }
                else
                {
                    // Expand the previous column
                    if (IsAnimated)
                        AnimateExpand(parentGrid.ColumnDefinitions[splitterIndex - 1]);
                    else
                    {
                        parentGrid.ColumnDefinitions[splitterIndex - 1].SetValue(ColumnDefinition.WidthProperty, _savedGridLength);
                    }
                }
            }
        }

        /// <summary>
        /// Determine the collapse direction based on the horizontal and vertical alignments
        /// </summary>
        private GridCollapseDirection GetCollapseDirection()
        {
            if (base.HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                return GridCollapseDirection.Columns;
            }
            if ((base.VerticalAlignment == VerticalAlignment.Stretch) && (base.ActualWidth <= base.ActualHeight))
            {
                return GridCollapseDirection.Columns;
            }
            return GridCollapseDirection.Rows;
        }

        #endregion

        #region Public Events

        // Define Collapsed and Expanded events
        public event EventHandler<EventArgs> Collapsed = delegate { };
        public event EventHandler<EventArgs> Expanded = delegate { };

        /// <summary>
        /// Raises the Collapsed event.
        /// </summary>
        /// <param name="e">Contains event arguments.</param>
        protected virtual void OnCollapsed(EventArgs e)
        {
            Collapsed(this, e);
        }

        /// <summary>
        /// Raises the Expanded event.
        /// </summary>
        /// <param name="e">Contains event arguments.</param>
        protected virtual void OnExpanded(EventArgs e)
        {
            Expanded(this, e);
        }

        #endregion

        #region Private Events

        /// <summary>
        /// Handles the Checked event of either the Vertical or Horizontal
        /// GridSplitterHandle ToggleButton.
        /// </summary>
        /// <param name="sender">An instance of the ToggleButton that fired the event.</param>
        /// <param name="e">Contains event arguments for the routed event that fired.</param>
        private void GridSplitterButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsCollapsed)
            {
                // In our case, Checked = Collapsed.  Which means we want everything
                // ready to be expanded.
                Collapse();

                IsCollapsed = true;

                // Toggle grid splitter and its surrounding grid splitters (dependent on collapse orientation)
                ToggleGridSplitters(false);

                // Raise the Collapsed event.
                OnCollapsed(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the Unchecked event of either the Vertical or Horizontal
        /// GridSplitterHandle ToggleButton.
        /// </summary>
        /// <param name="sender">An instance of the ToggleButton that fired the event.</param>
        /// <param name="e">Contains event arguments for the routed event that fired.</param>
        private void GridSplitterButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (IsCollapsed)
            {
                // In our case, Unchecked = Expanded.  Which means we want everything
                // ready to be collapsed.
                Expand();

                IsCollapsed = false;

                // Toggle grid splitter and its surrounding grid splitters (dependent on collapse orientation)
                ToggleGridSplitters(true);

                // Raise the Expanded event.
                OnExpanded(EventArgs.Empty);
            }
        }

        #endregion

        private void ToggleGridSplitters(bool collapsed)
        {
            // Quick patch (see comment below)...
            if (CollapseMode != GridSplitterCollapseMode.Next || _gridCollapseDirection != GridCollapseDirection.Rows)
            { return; }

            // Ideally check for direction and orientation and adjust
            // appropriately. For now, however, just deals with vertical
            // grid splitters collapsing in the Next setting

            var parentGrid = Parent as Grid;

            if (parentGrid == null)
                throw new Exception("CollapsingGridSplitter must reside inside a Grid.");

            var splitterIndex = (int)GetValue(Grid.RowProperty);

            // Check to make sure that, once expanded, the previous
            // grid area is also expanded.
            if (!collapsed)
            {
                SetDisable(false);
            }
            else if ((splitterIndex - 1) >= 0)
            {
                if (parentGrid.RowDefinitions[splitterIndex - 1].ActualHeight != 0)
                {
                    SetDisable(true);
                }
            }

            // Again, this just deals with vertical splitters set to Next collapse
            if ((splitterIndex + 2) < parentGrid.RowDefinitions.Count)
            {
                var neighbor = parentGrid.Children[splitterIndex + 2];
                // Set neighbor's hit test visible as current's hit test visible
                if (neighbor is CollapsingGridSplitter)
                {
                    ((CollapsingGridSplitter)neighbor).SetDisable(_elementGridSplitterBackground.IsHitTestVisible);
                }
            }

        }

        protected void SetDisable(bool value)
        {
            _elementGridSplitterBackground.IsHitTestVisible = value;
        }

        #region Collapse and Expand Animations

        #region Property for animating rows

        private RowDefinition _animatingRow;

        private static readonly DependencyProperty RowHeightAnimationProperty =
            DependencyProperty.Register(
                "RowHeightAnimation",
                typeof(double),
                typeof(CollapsingGridSplitter),
                new PropertyMetadata(RowHeightAnimationChanged));

        private double RowHeightAnimation
        {
            get { return (double)GetValue(RowHeightAnimationProperty); }
            set { SetValue(RowHeightAnimationProperty, value); }
        }

        private static void RowHeightAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CollapsingGridSplitter)._animatingRow.Height = new GridLength((double)e.NewValue);
        }

        #endregion

        #region Property for animating columns

        private ColumnDefinition _animatingColumn;

        private static readonly DependencyProperty ColWidthAnimationProperty =
            DependencyProperty.Register(
                "ColWidthAnimation",
                typeof(double),
                typeof(CollapsingGridSplitter),
                new PropertyMetadata(ColWidthAnimationChanged));

        private double ColWidthAnimation
        {
            get { return (double)GetValue(ColWidthAnimationProperty); }
            set { SetValue(ColWidthAnimationProperty, value); }
        }

        private static void ColWidthAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CollapsingGridSplitter)._animatingColumn.Width = new GridLength((double)e.NewValue);
        }

        #endregion

        /// <summary>
        /// Uses DoubleAnimation and a StoryBoard to animated the collapsing
        /// of the specificed ColumnDefinition or RowDefinition.
        /// </summary>
        /// <param name="definition">The RowDefinition or ColumnDefintition that will be collapsed.</param>
        private void AnimateCollapse(object definition)
        {
            double currentValue;

            // Setup the animation and StoryBoard
            var gridLengthAnimation = new DoubleAnimation() { Duration = new Duration(TimeSpan.FromMilliseconds(_animationTimeMillis)) };
            var sb = new Storyboard();

            // Add the animation to the StoryBoard
            sb.Children.Add(gridLengthAnimation);

            if (_gridCollapseDirection == GridCollapseDirection.Rows)
            {
                // Specify the target RowDefinition and property (Height) that will be altered by the animation.
                this._animatingRow = (RowDefinition)definition;
                Storyboard.SetTarget(gridLengthAnimation, this);
                Storyboard.SetTargetProperty(gridLengthAnimation, new PropertyPath("RowHeightAnimation"));

                currentValue = _animatingRow.ActualHeight;
            }
            else
            {
                // Specify the target ColumnDefinition and property (Width) that will be altered by the animation.
                this._animatingColumn = (ColumnDefinition)definition;
                Storyboard.SetTarget(gridLengthAnimation, this);
                Storyboard.SetTargetProperty(gridLengthAnimation, new PropertyPath("ColWidthAnimation"));

                currentValue = _animatingColumn.ActualWidth;
            }

            gridLengthAnimation.From = currentValue;
            gridLengthAnimation.To = 0;

            // Start the StoryBoard.
            sb.Begin();
        }

        /// <summary>
        /// Uses DoubleAnimation and a StoryBoard to animate the expansion
        /// of the specificed ColumnDefinition or RowDefinition.
        /// </summary>
        /// <param name="definition">The RowDefinition or ColumnDefintition that will be expanded.</param>
        private void AnimateExpand(object definition)
        {
            double currentValue;

            // Setup the animation and StoryBoard
            var gridLengthAnimation = new DoubleAnimation() { Duration = new Duration(TimeSpan.FromMilliseconds(_animationTimeMillis)) };
            var sb = new Storyboard();

            // Add the animation to the StoryBoard
            sb.Children.Add(gridLengthAnimation);

            if (_gridCollapseDirection == GridCollapseDirection.Rows)
            {
                // Specify the target RowDefinition and property (Height) that will be altered by the animation.
                this._animatingRow = (RowDefinition)definition;
                Storyboard.SetTarget(gridLengthAnimation, this);
                Storyboard.SetTargetProperty(gridLengthAnimation, new PropertyPath("RowHeightAnimation"));

                currentValue = _animatingRow.ActualHeight;
            }
            else
            {
                // Specify the target ColumnDefinition and property (Width) that will be altered by the animation.
                this._animatingColumn = (ColumnDefinition)definition;
                Storyboard.SetTarget(gridLengthAnimation, this);
                Storyboard.SetTargetProperty(gridLengthAnimation, new PropertyPath("ColWidthAnimation"));

                currentValue = _animatingColumn.ActualWidth;
            }
            gridLengthAnimation.From = currentValue;
            gridLengthAnimation.To = _savedActualValue;

            // Start the StoryBoard.
            sb.Begin();
        }

        #endregion

        /// <summary>
        /// An enumeration that specifies the direction the CollapsingGridSplitter will
        /// be collapased (Rows or Columns).
        /// </summary>
        internal enum GridCollapseDirection
        {
            Auto,
            Columns,
            Rows
        }
    }

    /// <summary>
    /// Specifies different collapse modes of a CollapsingGridSplitter.
    /// </summary>
    public enum GridSplitterCollapseMode
    {
        /// <summary>
        /// The CollapsingGridSplitter cannot be collapsed or expanded.
        /// </summary>
        None = 0,
        /// <summary>
        /// The column (or row) to the right (or below) the
        /// splitter's column, will be collapsed.
        /// </summary>
        Next = 1,
        /// <summary>
        /// The column (or row) to the left (or above) the
        /// splitter's column, will be collapsed.
        /// </summary>
        Previous = 2
    }
}
