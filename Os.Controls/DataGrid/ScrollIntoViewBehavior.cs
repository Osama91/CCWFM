using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Os.Controls.DataGrid
{
    public class ScrollIntoViewBehavior : Behavior<System.Windows.Controls.DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

            
            if (sender is System.Windows.Controls.DataGrid)
            {
                var grid = (sender as System.Windows.Controls.DataGrid);
                if (grid.SelectedItem != null)
                {
                    grid.Dispatcher.BeginInvoke(delegate
                    {
                   
                        //grid.BeginEdit();
                        try
                        {
                            grid.UpdateLayout();
                            grid.ScrollIntoView(grid.SelectedItem, null);
                        }
                        catch (Exception)
                        {

                            
                        }
                     
                    });
                }
            }
            }
            catch (Exception)
            {


            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -=
                AssociatedObject_SelectionChanged;
        }
    }
}