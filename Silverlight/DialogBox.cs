// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SEL.Silverlight
{
    public class DialogBox : ChildWindow
    {
        public DialogBox()
        {
            // Escape closes dialog box (same behavior as clicking on X)
            KeyDown += (s, e) =>
                           {
                               if (e.Key == Key.Escape)
                               {
                                   Close();
                               }
                           };
        }

        /// <summary>
        /// Deal with what I consider to be a bug in Silverlight.  While a ChildWindow
        /// is closing, if you call Close() again (a user presses enter repeatedly, for
        /// example), you can cause the entire UI to go into an unusable state (grayed
        /// out, like the ChildWindow is still open, but no ChildWindow).
        /// Apparently Silverlight leaves the window enabled while the close animation
        /// is running....
        /// </summary>
        public new void Close()
        {
            IsEnabled = false;
            base.Close();
        }

        /// <summary>
        /// Display a dialog box with the given message and a single "OK" button.
        /// </summary>
        public static DialogBox OkBox(string text)
        {
            var win = new DialogBox();
            var panel = new StackPanel {Orientation = Orientation.Vertical};
            win.Content = panel;
            panel.Children.Add(new TextBlock {Text = text});
            var ok = new Button
                         {
                             Content = "OK",
                             HorizontalAlignment = HorizontalAlignment.Center,
                             Width = 40,
                             Margin = new Thickness(0, 0, 10, 0),
                             Height = 20
                         };
            panel.Children.Add(ok);

            ok.Click += (s, e) => win.Close();

            return win;
        }

        /// <summary>
        /// Display a dialog box to request a single input value.
        /// </summary>
        /// <param name="text">Prompt to display.</param>
        /// <param name="initialValue">Initial value for the input.</param>
        /// <param name="okAction">Action to run when enter is pressed.</param>
        /// <returns>The child window.</returns>
        public static DialogBox InputBox(string text, string initialValue, Action<string> okAction)
        {
            var win = new DialogBox();
            var panel = new StackPanel {Orientation = Orientation.Horizontal};
            win.Content = panel;
            panel.Children.Add(new TextBlock {Text = text});
            var textBox = new TextBox
                              {Text = initialValue, Width = 140, SelectionStart = 0, SelectionLength = int.MaxValue};
            panel.Children.Add(textBox);
            win.KeyDown += (s, e) =>
                               {
                                   if (e.Key == Key.Enter)
                                   {
                                       okAction(textBox.Text);
                                       win.Close();
                                   }
                               };
            return win;
        }

        /// <summary>
        /// Create a dialog asking a Yes/No question.
        /// </summary>
        /// <param name="text">Text to prompt user with.</param>
        /// <param name="yesAction">Action to run when Yes clicked (default).</param>
        /// <param name="noAction">Optional action to run if No clicked.</param>
        /// <returns>The child window.</returns>
        public static DialogBox Confirm(string text, Action yesAction, Action noAction = null)
        {
            var win = new DialogBox();
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            win.Content = grid;
            var tb = new TextBlock {Text = text};
            grid.Children.Add(tb);
            Grid.SetColumnSpan(tb, 2);
            var yes = new Button
                          {
                              Content = "Yes",
                              HorizontalAlignment = HorizontalAlignment.Right,
                              Margin = new Thickness(0, 0, 10, 0),
                              Width = 30
                          };
            Grid.SetRow(yes, 1);
            grid.Children.Add(yes);
            yes.Click += (s, e) =>
                             {
                                 yesAction();
                                 win.Close();
                             };
            var no = new Button
                         {
                             Content = "No",
                             HorizontalAlignment = HorizontalAlignment.Left,
                             Margin = new Thickness(10, 0, 0, 0),
                             Width = 30
                         };
            grid.Children.Add(no);
            Grid.SetRow(no, 1);
            Grid.SetColumn(no, 1);
            no.Click += (s, e) =>
                            {
                                if (noAction != null)
                                    noAction.Invoke();
                                win.Close();
                            };

            return win;
        }
    }
}