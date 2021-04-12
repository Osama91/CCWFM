using System;
using System.Windows;
using System.Windows.Controls;
using CCWFM.ViewModel.SMLViewModels;

namespace CCWFM.Views.StylePages
{
    public partial class SMLRowEditor
    {

        public event EventHandler Submit;

        protected virtual void OnSubmit()
        {
            var handler = Submit;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public SMLRowEditor()
        {
            InitializeComponent();
            
        }
        public void InitiateCustomeEvents()
        {
            var seasonalMasterListViewModel = LayoutRoot.DataContext as SmlRowEditorViewModel;
            if (seasonalMasterListViewModel == null) return;
                seasonalMasterListViewModel.StylesPupulated +=
                    (s, e) => styleCodeTextBox.PopulateComplete();
            seasonalMasterListViewModel.Colorspopulated +=
                    (s, e) => colorCodeTextBox.PopulateComplete();
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            OnSubmit();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void StyleCodeTextBox_OnPopulating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
        }

        private void ColorCodeTextBox_OnPopulating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}

