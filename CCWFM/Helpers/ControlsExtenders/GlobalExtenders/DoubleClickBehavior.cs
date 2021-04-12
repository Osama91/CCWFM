using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace CCWFM.Helpers.ControlsExtenders.GlobalExtenders
{
    public class DoubleClickBehavior : Behavior<FrameworkElement>
    {
        #region Constants

        /// <summary>
        /// The threshold (in miliseconds) between clicks to be considered a double-click.  Windows default is 500; I'm a fast clicker.
        /// </summary>
        private const int ClickThresholdInMiliseconds = 200;

        #endregion Constants

        #region Properties [private]

        /// <summary>
        /// Holds the timestamp of the last click.
        /// </summary>
        private DateTime? LastClick { get; set; }

        /// <summary>
        /// Holds a reference to the instance of the last source object to generate a click.
        /// </summary>
        private object LastSource { get; set; }

        #endregion Properties [private]

        #region Events

        /// <summary>
        /// The event to be raised upon double-click.
        /// </summary>
        public event EventHandler<MouseButtonEventArgs> DoubleClick;

        #endregion Events

        #region Behavior Members [overridden]

        /// <summary>
        /// This is triggered when the behavior is attached to a FrameworkElement.  An event handler is attached to the
        /// FrameworkElement's MouseLeftButtonUp event.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
        }

        /// <summary>
        /// This is triggered when the behavior is detached from a FrameworkElement.  The event handler attached to the MouseLeftButtonUp
        /// event is removed.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonUp -= AssociatedObject_MouseLeftButtonUp;
        }

        #endregion Behavior Members [overridden]

        #region Event Handlers

        /// <summary>
        /// Occurs when the MouseLeftButtonDown event is triggered on the object associated to this behavior (this.AssociatedObject).
        /// </summary>
        /// <param name="sender">The object which is firing the MouseLeftButtonUp.  Note that this is not always the actual source of the event
        /// since events are bubbled; this is why we access e.OriginalSource</param>
        /// <param name="e">The MouseButtonEventArgs associated with the MouseLeftButtonDown event.</param>
        private void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (LastSource == null || !Equals(LastSource, e.OriginalSource))
            {
                LastSource = e.OriginalSource;
                LastClick = DateTime.Now;
            }
            else if ((DateTime.Now - LastClick.Value).Milliseconds <= ClickThresholdInMiliseconds)
            {
                LastClick = null;
                LastSource = null;
                if (DoubleClick != null)
                    DoubleClick(sender, e);
            }
            else
            {
                LastClick = null;
                LastSource = null;
            }
        }

        #endregion Event Handlers
    }
}